﻿//******************************************************************************************************
//  FunctionParsing.cs - Gbtc
//
//  Copyright © 2023, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
//  file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  08/23/2023 - Timothy Liakh
//       Generated original version of source code.
//
//******************************************************************************************************

using GrafanaAdapters.DataSourceValueTypes;
using GrafanaAdapters.DataSourceValueTypes.BuiltIn;
using GrafanaAdapters.Functions.BuiltIn;
using GrafanaAdapters.Metadata;
using GrafanaAdapters.Model.Common;
using Gemstone.Diagnostics;
using Gemstone.IO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Gemstone.StringExtensions;
using Gemstone.TimeSpanExtensions;
using Gemstone.TypeExtensions;

namespace GrafanaAdapters.Functions;

internal static class FunctionParsing
{
    private static IGrafanaFunction[] s_grafanaFunctions;
    private static readonly object s_grafanaFunctionsLock = new();
    private static readonly LogPublisher s_log = Logger.CreatePublisher(typeof(FunctionParsing), MessageClass.Component);

    // Calls to this expensive match operation should be temporally cached by expression
    public static ParsedGrafanaFunction<T>[] MatchFunctions<T>(string expression, QueryParameters queryParameters) where T : struct, IDataSourceValueType<T>
    {
        Regex functionsRegex = DataSourceValueTypeCache<T>.FunctionsRegex;
        Dictionary<string, IGrafanaFunction<T>> functionMap = DataSourceValueTypeCache<T>.FunctionMap;

        // Match all top-level functions in expression
        MatchCollection matches = functionsRegex.Matches(expression);

        List<ParsedGrafanaFunction<T>> parsedGrafanaFunctions = [];

        foreach (Match match in matches)
        {
            // Get the matched groups from the regex
            GroupCollection groups = match.Groups;

            // Lookup function by user provided name or alias
            if (!functionMap.TryGetValue(groups["Function"].Value, out IGrafanaFunction<T> function))
            {
            #if DEBUG
                Debug.Fail($"Unexpected failure to find function '{groups["Function"].Value}'.");
            #else
                continue;
            #endif
            }

            // Check if the function has a group operation prefix, e.g., slice or set
            Enum.TryParse(groups["GroupOp"].Value, true, out GroupOperations groupOperation);

            // Verify that the function allows the requested group operation - this will
            // throw an exception if the function does not support the requested operation
            groupOperation = function.CheckAllowedGroupOperation(groupOperation);

            // Get the function expression from the regex representing the parameters passed to the function
            string functionExpression = groups["Expression"].Value;

            // Any slice operation on a function that returns the same result matrix whether processed horizontally, i.e.,
            // series-by-series, or vertically, i.e., slice-by-slice, is equivalent to its non-slice operation when using
            // the 'Interval' function over the same expression. For example, the following queries are equivalent:
            //
            //     SliceShift(0.02, 1, FILTER TOP 10 ActiveMeasurements WHERE SignalType='FREQ')
            //     Shift(1, Interval(0.02, FILTER TOP 10 ActiveMeasurements WHERE SignalType='FREQ'))
            //
            //     SliceRound(0.0333, 3, ACME-STAR:FREQ; ACME-PLUS:FREQ)
            //     Round(3, Interval(0.0333, ACME-STAR:FREQ; ACME-PLUS:FREQ))
            //
            // As an optimization in this scenario, replace the slice operation with the equivalent interval operation.
            // Because of this automatic optimization, slice operations meeting these criteria are also hidden from the
            // 'PublishedGroupOperations' function property, see 'GetGrafanaFunctions' method below.
            if (groupOperation == GroupOperations.Slice && function.ReturnType == ReturnType.Series && function.IsSliceSeriesEquivalent)
            {
                // Parse the function parameters from expression
                (string[] parsedParameters, string queryExpression) = function.ParseParameters(queryParameters, functionExpression, groupOperation);

                // First parameter in a slice operation is the tolerance parameter
                string tolerance = parsedParameters[0];

                // Remaining parameters represent normal non-slice function parameters
                string[] parameters = parsedParameters.Skip(1).ToArray();

                // Rearrange expression such that parameters are passed to non-slice version of function and tolerance is
                // passed to 'Interval' function. Since functions regex only matches top-level functions, it is safe to adjust
                // the expression in this manner as it is just now being parsed and query expression will be parsed later
                functionExpression = $"{string.Join(", ", parameters)}{(parameters.Length > 0 ? ", " : "")}{nameof(Interval<T>)}({tolerance}, {queryExpression})";
                groupOperation = GroupOperations.None;
            }

            parsedGrafanaFunctions.Add(new ParsedGrafanaFunction<T>
            {
                Function = function,
                GroupOperation = groupOperation,
                Expression = functionExpression,
                MatchedValue = match.Value
            });
        }

        return [.. parsedGrafanaFunctions];
    }

    // Gets all the available Grafana functions.
    public static IGrafanaFunction[] GetGrafanaFunctions()
    {
        // Caching default grafana functions so expensive assembly load with type inspections
        // and reflection-based instance creation of types are only done once. If dynamic
        // reload is needed at runtime, call ReloadGrafanaFunctions() method.
        IGrafanaFunction[] grafanaFunctions = Interlocked.CompareExchange(ref s_grafanaFunctions, null, null);

        if (grafanaFunctions is not null)
            return grafanaFunctions;

        // If many external calls, e.g., web requests, are made to this function at the same time,
        // there will be an initial pause while the first thread loads the Grafana functions
        lock (s_grafanaFunctionsLock)
        {
            // Check if another thread already created the Grafana functions
            if (s_grafanaFunctions is not null)
                return s_grafanaFunctions;

            const string EventName = $"{nameof(FunctionParsing)} {nameof(IGrafanaFunction)} Type Load";

            try
            {
                s_log.Publish(MessageLevel.Info, EventName, $"Starting load for {nameof(IGrafanaFunction)} types...");
                long startTime = DateTime.UtcNow.Ticks;

                string grafanaFunctionsPath = FilePath.GetAbsolutePath("").EnsureEnd(Path.DirectorySeparatorChar);
                List<Type> implementationTypes = typeof(IGrafanaFunction).LoadImplementations(grafanaFunctionsPath, true, false);
                List<IGrafanaFunction> functions = [];

                foreach (Type type in implementationTypes.Where(type => type.GetConstructor(Type.EmptyTypes) is not null))
                {
                    try
                    {
                        (Type functionType, bool builtIn) = checkNestedType(type);

                        if (functionType is null)
                            continue;

                        IGrafanaFunction function = (IGrafanaFunction)Activator.CreateInstance(functionType);

                        // Set function category to built-in when function is in the BuiltIn namespace (this is a non-overridable property)
                        functionType.GetProperty(nameof(IGrafanaFunction.Category))?.SetValue(function, builtIn ? Category.BuiltIn : Category.Custom);

                        // If function returns slice-series equivalent results, remove slice from published group operations
                        if (function.ReturnType == ReturnType.Series && function.IsSliceSeriesEquivalent)
                        {
                            PropertyInfo publishedGroupOperations = functionType.GetProperty(nameof(IGrafanaFunction.PublishedGroupOperations));

                            // Attempt to update 'GrafanaFunctionBase<T>.PublishedGroupOperations' internal set property to hide 'Slice',
                            // note that derived classes may have overridden this property with no available set property, so check this:
                            if (publishedGroupOperations?.GetSetMethod(true) is not null)
                                publishedGroupOperations.SetValue(function, function.PublishedGroupOperations & ~GroupOperations.Slice);
                        }

                        functions.Add(function);
                    }
                    catch (Exception ex)
                    {
                        s_log.Publish(MessageLevel.Error, EventName, $"Failed while loading {nameof(IGrafanaFunction)} type '{type.FullName}': {ex.Message}", exception: ex);
                    }
                }

                Interlocked.Exchange(ref s_grafanaFunctions, [.. functions]);

                string elapsedTime = new TimeSpan(DateTime.UtcNow.Ticks - startTime).ToElapsedTimeString(3);
                s_log.Publish(MessageLevel.Info, EventName, $"Completed loading {nameof(IGrafanaFunction)} types: loaded {s_grafanaFunctions.Length:N0} types in {elapsedTime}.");
            }
            catch (Exception ex)
            {
                s_log.Publish(MessageLevel.Error, EventName, $"Failed while loading {nameof(IGrafanaFunction)} types: {ex.Message}", exception: ex);
            }

            return s_grafanaFunctions;
        }

        // Check for Grafana functions nested within abstract base class definition - 'BuiltIn' pattern
        static (Type functionType, bool builtIn) checkNestedType(Type type)
        {
            const string BuiltInNamespace = $"{nameof(GrafanaAdapters)}.{nameof(Functions)}.{nameof(BuiltIn)}";

            if (!type.ContainsGenericParameters)
                return (type, false);

            if (!type.IsNested || !type.DeclaringType!.IsGenericType)
                return (type, false);

            // Must contain at least one generic argument because IsGenericType is true
            Type[] constraints = type.DeclaringType.GetGenericArguments()[0].GetGenericParameterConstraints();

            // Look for any constraint based on IDataSourceValue, if found, assign a specific
            // type (any is fine) to generic parent class so nested type can be constructed
            return constraints.Any(constraint => constraint.GetInterfaces().Any(interfaceType => interfaceType == typeof(IDataSourceValueType))) ?
                (type.MakeGenericType(typeof(MeasurementValue)), type.Namespace?.Equals(BuiltInNamespace) ?? false) :
                (null, false);
        }
    }

    // Gets the Grafana functions for a specific data source value type (by its index).
    public static IEnumerable<IGrafanaFunction> GetGrafanaFunctions(int dataTypeIndex)
    {
        return GetGrafanaFunctions().Where(function => function.DataTypeIndex == dataTypeIndex);
    }

    // Reloads the Grafana functions.
    public static void ReloadGrafanaFunctions()
    {
        lock (s_grafanaFunctionsLock)
        {
            Interlocked.Exchange(ref s_grafanaFunctions, null);
            TargetCaches.ResetAll();
        }

        // Reinitializing data source caches will reload all grafana functions per data source value type
        DataSourceValueTypeCache.ReinitializeAll();
    }

    // Handle series rename operations for Grafana functions - this is a special case for handling the Label function
    public static async IAsyncEnumerable<DataSourceValueGroup<T>> RenameSeries<T>(this IAsyncEnumerable<DataSourceValueGroup<T>> dataset, QueryParameters queryParameters, DataSet metadata, string labelExpression, [EnumeratorCancellation] CancellationToken cancellationToken) where T : struct, IDataSourceValueType<T>
    {
        if (labelExpression.StartsWith("\"") || labelExpression.StartsWith("'"))
            labelExpression = labelExpression.Substring(1, labelExpression.Length - 2);

        HashSet<string> uniqueLabelSet = new(StringComparer.OrdinalIgnoreCase);
        int index = 1;

        await foreach (DataSourceValueGroup<T> valueGroup in dataset.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            string rootTarget = valueGroup.RootTarget;

            string seriesLabel = TargetCache<string>.GetOrAdd($"{default(T).DataTypeIndex}:{labelExpression}@{rootTarget}", () =>
            {
                string derivedLabel = ParseLabel(labelExpression, metadata, rootTarget);
               
                if (derivedLabel.Equals(labelExpression, StringComparison.Ordinal))
                    derivedLabel = $"{labelExpression}{(index > 1 ? $" {index}" : "")}";

                index++;
                return derivedLabel;
            });

            // Verify that series label is unique
            while (uniqueLabelSet.Contains(seriesLabel))
            {
                Match match = s_uniqueSeriesRegex.Match(seriesLabel);

                if (match.Success)
                {
                    int count = int.Parse(match.Result("${Count}")) + 1;
                    seriesLabel = $"{match.Result("${Label}")} {count}";
                }
                else
                {
                    seriesLabel = $"{seriesLabel} 1";
                }
            }

            uniqueLabelSet.Add(seriesLabel);

            yield return new DataSourceValueGroup<T>
            {
                Target = seriesLabel,
                RootTarget = valueGroup.RootTarget,
                SourceTarget = queryParameters.SourceTarget,
                Source = valueGroup.Source,
                DropEmptySeries = queryParameters.DropEmptySeries,
                RefID = queryParameters.SourceTarget.refID,
                MetadataMap = valueGroup.MetadataMap
            };
        }
    }

    private static void LoadFieldSubstitutions(DataSet metadata, Dictionary<string, string> substitutions, string target, string tableName, bool usePrefix)
    {
        DataRow record = target.RecordFromTag(metadata, tableName);
        string prefix = usePrefix ? $"{tableName}." : "";

        if (record is null)
        {
            // Apply empty field substitutions when point tag metadata is not found
            foreach (string fieldName in metadata.Tables[tableName].Columns.Cast<DataColumn>().Select(column => column.ColumnName))
            {
                string columnName = $"{prefix}{fieldName}";

                if (columnName.Equals("PointTag", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (!substitutions.ContainsKey(columnName))
                    substitutions.Add(columnName, "");
            }

            if (usePrefix)
                return;

            if (substitutions.TryGetValue("PointTag", out string substitution))
                substitutions["PointTag"] = $"{substitution}, {target}";
            else
                substitutions.Add("PointTag", target);
        }
        else
        {
            foreach (string fieldName in record.Table.Columns.Cast<DataColumn>().Select(column => column.ColumnName))
            {
                string columnName = $"{prefix}{fieldName}";
                string columnValue = record[fieldName].ToString();

                if (substitutions.TryGetValue(columnName, out string substitution))
                {
                    // Pattern for multiple field substitutions is: {field}, {field}, {field}, ...
                    // This handles the case where multiple tags exist in the single root target
                    if (!string.IsNullOrWhiteSpace(columnValue))
                        substitutions[columnName] = string.IsNullOrWhiteSpace(substitution) ? columnValue : $"{substitution}, {columnValue}";
                }
                else
                {
                    substitutions.Add(columnName, columnValue);
                }
            }
        }
    }

    public static string ParseLabel(string labelExpression, DataSet metadata, string rootTarget = "")
    {
        // If label expression does not contain any substitutions, just return the expression
        if (labelExpression.IndexOf('{') < 0)
            return labelExpression;

        Dictionary<string, string> substitutions = new(StringComparer.OrdinalIgnoreCase);
        Regex fieldExpression = new(@"\{(?<Field>[^}]+)\}", RegexOptions.Compiled);

        // Handle substitutions for each tag defined in the rootTarget
        foreach (string item in rootTarget.Split(';'))
        {
            // Load {alias} substitutions - alias values are for tags that have been assigned an alias, e.g.:
            // for 'x=TAGNAME', the alias would be 'x' and the target would be 'TAGNAME'
            string target = item.SplitAlias(out string alias);

            if (substitutions.TryGetValue("alias", out string substitution))
            {
                // Pattern for multiple alias substitutions is: {alias}, {alias}, {alias}, ...
                // This handles the case where multiple tags exist in the single root target
                // and each one has am assigned alias
                if (!string.IsNullOrWhiteSpace(alias))
                    substitutions["alias"] = string.IsNullOrWhiteSpace(substitution) ? alias : $"{substitution}, {alias}";
            }
            else
            {
                substitutions.Add("alias", alias ?? "");
            }

            // Check all substitution fields for table name specifications (ActiveMeasurements assumed)
            HashSet<string> tableNames = new([MeasurementValue.MetadataTableName], StringComparer.OrdinalIgnoreCase);
            MatchCollection fields = fieldExpression.Matches(labelExpression);

            foreach (Match match in fields)
            {
                string field = match.Result("${Field}");

                // Check if specified field substitution has a table name prefix
                string[] components = field.Split('.');

                if (components.Length == 2)
                    tableNames.Add(components[0]);
            }

            // Load field substitutions for each table name from metadata
            foreach (string tableName in tableNames)
            {
                // ActiveMeasurements view fields are added as non-prefixed field name substitutions
                if (tableName.Equals(MeasurementValue.MetadataTableName, StringComparison.OrdinalIgnoreCase))
                    LoadFieldSubstitutions(metadata, substitutions, target, tableName, false);

                // All other table fields are added with table name as the prefix {table.field}
                LoadFieldSubstitutions(metadata, substitutions, target, tableName, true);
            }
        }

        string derivedLabel = labelExpression;

        foreach (KeyValuePair<string, string> substitution in substitutions)
            derivedLabel = derivedLabel.ReplaceCaseInsensitive($"{{{substitution.Key}}}", substitution.Value);

        return derivedLabel;
    }

    public static string ParseLabel(string labelExpression, DataSet metadata, string rootTarget = "")
    {
        // If label expression does not contain any substitutions, just return the expression
        if (labelExpression.IndexOf('{') < 0)
            return labelExpression;

        Dictionary<string, string> substitutions = new(StringComparer.OrdinalIgnoreCase);
        Regex fieldExpression = new(@"\{(?<Field>[^}]+)\}", RegexOptions.Compiled);

        // Handle substitutions for each tag defined in the rootTarget
        foreach (string item in rootTarget.Split(';'))
        {
            // Load {alias} substitutions - alias values are for tags that have been assigned an alias, e.g.:
            // for 'x=TAGNAME', the alias would be 'x' and the target would be 'TAGNAME'
            string target = item.SplitAlias(out string alias);

            if (substitutions.TryGetValue("alias", out string substitution))
            {
                // Pattern for multiple alias substitutions is: {alias}, {alias}, {alias}, ...
                // This handles the case where multiple tags exist in the single root target
                // and each one has am assigned alias
                if (!string.IsNullOrWhiteSpace(alias))
                    substitutions["alias"] = string.IsNullOrWhiteSpace(substitution) ? alias : $"{substitution}, {alias}";
            }
            else
            {
                substitutions.Add("alias", alias ?? "");
            }

            // Check all substitution fields for table name specifications (ActiveMeasurements assumed)
            HashSet<string> tableNames = new([MeasurementValue.MetadataTableName], StringComparer.OrdinalIgnoreCase);
            MatchCollection fields = fieldExpression.Matches(labelExpression);

            foreach (Match match in fields)
            {
                string field = match.Result("${Field}");

                // Check if specified field substitution has a table name prefix
                string[] components = field.Split('.');

                if (components.Length == 2)
                    tableNames.Add(components[0]);
            }

            // Load field substitutions for each table name from metadata
            foreach (string tableName in tableNames)
            {
                // ActiveMeasurements view fields are added as non-prefixed field name substitutions
                if (tableName.Equals(MeasurementValue.MetadataTableName, StringComparison.OrdinalIgnoreCase))
                    LoadFieldSubstitutions(metadata, substitutions, target, tableName, false);

                // All other table fields are added with table name as the prefix {table.field}
                LoadFieldSubstitutions(metadata, substitutions, target, tableName, true);
            }
        }

        string derivedLabel = labelExpression;

        foreach (KeyValuePair<string, string> substitution in substitutions)
            derivedLabel = derivedLabel.ReplaceCaseInsensitive($"{{{substitution.Key}}}", substitution.Value);

        return derivedLabel;
    }

    private static readonly Regex s_uniqueSeriesRegex = new(@"(?<Label>.+) (?<Count>\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
}