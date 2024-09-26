﻿//******************************************************************************************************
//  ParameterParsing.cs - Gbtc
//
//  Copyright © 2024, Grid Protection Alliance.  All Rights Reserved.
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
//  01/01/2024 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************
// ReSharper disable PossibleMultipleEnumeration

using GrafanaAdapters.DataSourceValueTypes;
using GrafanaAdapters.Functions.BuiltIn;
using GrafanaAdapters.Metadata;
using Gemstone;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gemstone.CharExtensions;
using Gemstone.StringExtensions;
using Gemstone.TypeExtensions;

namespace GrafanaAdapters.Functions;

internal static class ParameterParsing
{
    // Gets formatted parameters to display for a target function.
    public static string FormatParameters<T>(this IGrafanaFunction<T> function, string[] parsedParameters) where T : struct, IDataSourceValueType<T>
    {
        int visibleCount = parsedParameters.Length - function.InternalParameterCount;
        IEnumerable<string> parameters = parsedParameters.Take(visibleCount);
        return $"{string.Join(", ", parameters)}{(visibleCount > 0 ? ", " : string.Empty)}";
    }

    /// <summary>
    /// Parses function parameters from a given expression as an array of strings.
    /// </summary>
    /// <param name="function">Target function.</param>
    /// <param name="queryParameters">Query parameters.</param>
    /// <param name="queryExpression">Expression to parse.</param>
    /// <param name="groupOperation">Group operation.</param>
    /// <returns>
    /// A tuple of parsed parameters and any remaining query expression.
    /// Remaining query expression is typically the filter expression.
    /// </returns>
    /// <exception cref="SyntaxErrorException">Expected parameters did not match those received.</exception>
    public static (string[] parsedParameters, string queryExpression) ParseParameters(this IGrafanaFunction function, QueryParameters queryParameters, string queryExpression, GroupOperations groupOperation)
    {
        return TargetCache<(string[], string)>.GetOrAdd($"{function.Name}:{queryExpression}", () =>
        {
            // Check if function defines any custom parameter parsing
            (List<string> parsedParameters, string updatedQueryExpression) = function.ParseParameters(queryParameters, queryExpression);

            if (parsedParameters is not null)
                return (parsedParameters.Select(parameter => parameter.Trim()).ToArray(), updatedQueryExpression);

            parsedParameters = [];

            int requiredParameters = function.RequiredParameterCount;

            // Any slice operation adds one required parameter for time tolerance
            if (groupOperation == GroupOperations.Slice)
                requiredParameters++;

            // Extract any required function parameters - this does not include the filter expression
            if (requiredParameters > 0)
            {
                int index = 0;

                for (int i = 0; i < requiredParameters && index > -1; i++)
                    index = queryExpression.IndexOf(',', index + 1);

                if (index > -1)
                    parsedParameters.AddRange(queryExpression.Substring(0, index).Split(','));

                if (parsedParameters.Count == requiredParameters)
                {
                    // Separate any remaining query expression from required parameters,
                    // this could be optional parameters or the filter expression
                    queryExpression = queryExpression.Substring(index + 1).Trim();
                }
                else
                {
                    // Offset counts for filter expression in error message not included in required parameters for better user feedback
                    throw new SyntaxErrorException($"Expected {requiredParameters + 1} parameters, received {parsedParameters.Count + 1} in: {function.Name}({queryExpression})");
                }
            }

            // Extract any provided optional function parameters
            int optionalParameters = function.OptionalParameterCount;

            if (optionalParameters > 0)
            {
                int index = queryExpression.IndexOf(',');

                if (index > -1 && !hasSubExpression(queryExpression.Substring(0, index)))
                {
                    int lastIndex = index;

                    for (int i = 1; i < optionalParameters && index > -1; i++)
                    {
                        index = queryExpression.IndexOf(',', index + 1);

                        if (index > -1 && hasSubExpression(queryExpression.Substring(lastIndex + 1, index - lastIndex - 1).Trim()))
                        {
                            index = lastIndex;
                            break;
                        }

                        lastIndex = index;
                    }

                    if (index > -1)
                    {
                        parsedParameters.AddRange(queryExpression.Substring(0, index).Split(','));
                        queryExpression = queryExpression.Substring(index + 1).Trim();
                    }
                }
            }

            return (parsedParameters.Select(parameter => parameter.Trim()).ToArray(), queryExpression);
        });

        static bool hasSubExpression(string target) =>
            target.StartsWith("FILTER", StringComparison.OrdinalIgnoreCase) || target.Contains("(");
    }

    /// <summary>
    /// Generates a typed list of value mutable parameters from parsed parameters.
    /// </summary>
    /// <typeparam name="TDataSourceValue">The type of the data source value.</typeparam>
    /// <param name="function">Target function.</param>
    /// <param name="parsedParameters">Parsed parameters.</param>
    /// <param name="dataSourceValues">Data source values.</param>
    /// <param name="rootTarget">Root target.</param>
    /// <param name="metadata">Metadata.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of value mutable parameters from parsed parameters.</returns>
    /// <remarks>
    /// In case user has requested metadata as a parameter, pass in the root target
    /// which has a higher chance of being resolved for associated metadata.
    /// </remarks>
    public static async ValueTask<Parameters> GenerateParametersAsync<TDataSourceValue>(this IGrafanaFunction<TDataSourceValue> function, string[] parsedParameters, IAsyncEnumerable<TDataSourceValue> dataSourceValues, string rootTarget, DataSet metadata, CancellationToken cancellationToken) where TDataSourceValue : struct, IDataSourceValueType<TDataSourceValue>
    {
        // Generate a list of value mutable parameters
        Parameters parameters = function.ParameterDefinitions.CreateParameters();
        int index = 0;

        Debug.Assert(parameters.Count == function.RequiredParameterCount + function.OptionalParameterCount + 1, $"Expected {function.RequiredParameterCount + function.OptionalParameterCount + 1} parameters, received {parameters.Count} in: {function.Name}({string.Join(",", parsedParameters)})");
        Debug.Assert(parsedParameters.Length <= parameters.Count - 1, $"Expected {parameters.Count - 1} parameters, received {parsedParameters.Length} in: {function.Name}({string.Join(",", parsedParameters)})");
        Debug.Assert(parsedParameters.Length == 0 || parsedParameters.All(parameter => !string.IsNullOrWhiteSpace(parameter)) || function.Name.Equals(nameof(Evaluate<TDataSourceValue>)), $"Expected all parameters to be non-empty in: {function.Name}({string.Join(",", parsedParameters)})");

        parameters.ParsedCount = parsedParameters.Length;

        for (int i = 0; i < parameters.Count; i++)
        {
            IMutableParameter parameter = parameters[i];

            // Data -- last parameter is always a data source value
            if (i == parameters.Count - 1)
            {
                Debug.Assert(parameter is IParameter<IAsyncEnumerable<IDataSourceValueType>>, $"Last parameter is not a data source value of type '{typeof(IAsyncEnumerable<IDataSourceValueType>).Name}'.");

                // Replace last parameter with data source type specific parameter with associated values
                parameters[i] = new Parameter<IAsyncEnumerable<TDataSourceValue>>(ParameterDefinitions<TDataSourceValue>.DataSourceValues)
                {
                    Value = dataSourceValues
                };

                break;
            }

            // Parameter
            if (index < parsedParameters.Length)
                await parameter.ConvertParsedValueAsync(parsedParameters[index++].Trim(), rootTarget, dataSourceValues, metadata, cancellationToken).ConfigureAwait(false);

        #if DEBUG
            // Required parameters were already validated in ParseParameters - this is a sanity check
            else if (parameter.Required)
                Debug.Fail($"Expected {function.RequiredParameterCount} parameters, received {index} in: {function.Name}({string.Join(",", parsedParameters)})");
        #endif
        }

        return parameters;
    }

    /// <summary>
    /// Converts parsed value to the mutable parameter type for a given data source value type.
    /// </summary>
    /// <typeparam name="TDataSourceValue">The type of the data source value.</typeparam>
    /// <param name="parameter">Mutable parameter to hold the converted data.</param>
    /// <param name="value">Parsed value to convert.</param>
    /// <param name="target">Associated target.</param>
    /// <param name="dataSourceValues">Data source values.</param>
    /// <param name="metadata">Source metadata.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <remarks>
    /// This function is used to convert the parsed value to the parameter type.
    /// If the type of value provided and expected match, then it directly converts.
    /// If the types do not match, then it first searches through the provided metadata.
    /// If nothing is found, it looks through ActiveMeasurements for it.
    /// Finally, if none of the above work it throws an error.
    /// </remarks>
    public static async ValueTask ConvertParsedValueAsync<TDataSourceValue>(this IMutableParameter parameter, string value, string target, IAsyncEnumerable<TDataSourceValue> dataSourceValues, DataSet metadata, CancellationToken cancellationToken) where TDataSourceValue : struct, IDataSourceValueType<TDataSourceValue>
    {
        // No value specified
        if (string.IsNullOrWhiteSpace(value))
        {
            // Required -> error
            if (parameter.Required)
                throw new SyntaxErrorException($"Required '{parameter.Name}' parameter of type '{parameter.Type.Name}' is missing.");

            // Not required -> default
            parameter.Value = parameter.Default;
            return;
        }

        // Check if metadata is requested
        if (value.StartsWith("{") && value.EndsWith("}"))
        {
            value = value.Substring(1, value.Length - 2);

            if (target is null)
            {
                // When no target is specified, attempt to use the target from first data source value
                TDataSourceValue firstValue = await dataSourceValues.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
                target = firstValue.Target;
            }

            if (!string.IsNullOrWhiteSpace(target))
            {
                (string value, bool success) lookup = LookupMetadata<TDataSourceValue>(value, target, metadata);

                if (lookup.success)
                    value = lookup.value;
            }
        }

        object result = null;

        // Check if parameter has a custom parse operation
        if (parameter.Parse is not null)
        {
            (result, bool success) = parameter.Parse(value);

            // If custom parsing succeeds, return result; otherwise, continue with default parsing
            if (success)
            {
                parameter.Value = result;
                return;
            }
        }

        // String type
        if (parameter.Type == typeof(string))
        {
            parameter.Value = value;
            return;
        }

        // Boolean type - use custom boolean parsing extension
        if (parameter.Type == typeof(bool))
        {
            parameter.Value = value.ParseBoolean();
            return;
        }

        // Attempt to convert to the parameter type
        if (value[0].IsNumeric())
        {
            try
            {
                // Convert.ChangeType is faster for numeric types
                result = Convert.ChangeType(value, parameter.Type);
            }
            catch
            {
                result = null;
            }
        }

        // ConvertToType uses a TypeConverter which works for most types, including enums,
        // note that this function returns null if conversion fails
        result ??= value.ConvertToType(parameter.Type);

        // If conversion fails, check if value is a named target
        if (result is null && char.IsLetter(value[0]))
        {
            object defaultValue = default;
            bool hasDefaultValue = false;
            string[] targets = null;

            // Named target parameters can optionally specify multiple fall-back series and one final
            // default constant value each separated by a semicolon to use when the named target series
            // is not available, e.g.: Top(T1;T2;5, T1;T2;T3)
            if (value.IndexOf(';') > -1)
            {
                string[] parts = value.Split(';');

                if (parts.Length >= 2)
                {
                    targets = new string[parts.Length - 1];
                    Array.Copy(parts, 0, targets, 0, targets.Length);
                    defaultValue = parts[parts.Length - 1].ConvertToType(parameter.Type);
                    hasDefaultValue = true;
                }
            }

            targets ??= [value];

            foreach (string targetName in targets)
            {
                // Attempt to find named target parameter values in data source values. In this implementation, where parameters are
                // pre-parsed for function calls, the named target parameters only return the first value in the series. This will be
                // more useful in slice operations where the first value is the only one in the slice. In non-slice operations, the
                // first value in the series for the named parameter will be used for each function call over all series operations.
                // For example, for the function 'Shift(T1;0, T1;T2)', the first parameter, 'T1;0', means the first T1 series value
                // (when there is one) will be used as the function parameter value for every 'Shift' function call over each series
                // value in both the T1 and T2 targets. This may be OK in some scenarios, but for others it is recommended that the
                // user consider using slice operations when possible.
                TDataSourceValue sourceResult = await dataSourceValues.FirstOrDefaultAsync(dataSourceValue =>
                    dataSourceValue.Target.Equals(targetName, StringComparison.OrdinalIgnoreCase), cancellationToken).ConfigureAwait(false);

                // Data source value types are structs and cannot be null so an empty target means lookup failed
                if (string.IsNullOrEmpty(sourceResult.Target))
                    continue;

                // Get target value from time-series value array
                double seriesValue = sourceResult.TimeSeriesValue[default(TDataSourceValue).ValueIndex];

                result = parameter.Type.IsNumeric() ?
                    Convert.ChangeType(seriesValue, parameter.Type) :
                    seriesValue.ToString(CultureInfo.InvariantCulture).ConvertToType(parameter.Type);

                break;
            }

            if (result is null && hasDefaultValue)
                result = defaultValue;
        }

        parameter.Value = result ?? parameter.Default;
    }

    private static (string value, bool success) LookupMetadata<TDataSourceValue>(string value, string target, DataSet metadata) where TDataSourceValue : struct, IDataSourceValueType
    {
        // Lookup target in metadata
        (string tableName, string fieldName) = value.ParseAsTableAndField<TDataSourceValue>();
        DataRow record = metadata.Lookup<TDataSourceValue>(tableName, target);

        if (record is null || !record.Table.Columns.Contains(fieldName))
            return (default, false);

        string fieldValue = record[fieldName]?.ToString();

        return string.IsNullOrEmpty(fieldValue) ?
            (default, false) :
            (fieldValue, true);
    }
}