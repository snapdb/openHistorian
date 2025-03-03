﻿//******************************************************************************************************
//  GrafanaDataSourceBase_ApiControllerOperations.cs - Gbtc
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
//  01/14/2024 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using GrafanaAdapters.Annotations;
using GrafanaAdapters.DataSourceValueTypes;
using GrafanaAdapters.DataSourceValueTypes.BuiltIn;
using GrafanaAdapters.Functions;
using GrafanaAdapters.Metadata;
using GrafanaAdapters.Model.Annotations;
using GrafanaAdapters.Model.Common;
using GrafanaAdapters.Model.Database;
using GrafanaAdapters.Model.Functions;
using GrafanaAdapters.Model.Metadata;
using Gemstone;
using Gemstone.Data;
using Gemstone.Data.Model;
using Gemstone.Timeseries.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Gemstone.Configuration;
using Gemstone.StringExtensions;
using Gemstone.TypeExtensions;
using static GrafanaAdapters.Functions.Common;

namespace GrafanaAdapters;

// ApiController specific Grafana functionality is defined here, see 'Documentation/WebAPIInterfaces.docx' for details
partial class GrafanaDataSourceBase
{
    /// <summary>
    /// Gets the data source value types, i.e., any type that has implemented <see cref="IDataSourceValueType"/>,
    /// that have been loaded into the application domain.
    /// </summary>
    public virtual IEnumerable<DataSourceValueType> GetValueTypes()
    {
        return DataSourceValueTypeCache.DefaultInstances.Select((value, index) => new DataSourceValueType
        {
            name = value.GetType().Name,
            index = index,
            timeSeriesDefinition = string.Join(", ", value.TimeSeriesValueDefinition),
            metadataTableName = value.MetadataTableName
        });
    }

    /// <summary>
    /// Gets the table names that, at a minimum, contain all the fields that the value type has defined as required,
    /// see <see cref="IDataSourceValueType.RequiredMetadataFieldNames"/> when <see cref="SearchRequest.dataTypeIndex"/>
    /// is a valid index in the data source value cache. When <see cref="SearchRequest.dataTypeIndex"/> is -1, all
    /// table names are returned.
    /// </summary>
    /// <param name="request">Search request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public virtual Task<IEnumerable<string>> GetValueTypeTables(SearchRequest request, CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            return TargetCache<IEnumerable<string>>.GetOrAdd($"{request.dataTypeIndex}", () =>
            {
                int dataTypeIndex = request.dataTypeIndex;
                IDataSourceValueType dataSourceValueType = DataSourceValueTypeCache.GetDefaultInstance(dataTypeIndex == -1 ? MeasurementValue.TypeIndex : dataTypeIndex);
                DataSet metadata = Metadata.GetAugmentedDataSet(dataSourceValueType);

                // Provided unrestricted metadata table names if data type index is -1
                return dataTypeIndex > -1 ?
                    metadata.Tables.Cast<DataTable>().Where(table => dataSourceValueType.MetadataTableIsValid(metadata, table.TableName)).Select(table => table.TableName) :
                    metadata.Tables.Cast<DataTable>().Select(table => table.TableName);
            });
        },
        cancellationToken);
    }

    /// <summary>
    /// Gets the field names for a given table when <see cref="SearchRequest.dataTypeIndex"/> is a valid index in the data
    /// source value cache and selected table name contains all the fields that the value type has defined as required, see
    /// <see cref="IDataSourceValueType.RequiredMetadataFieldNames"/> . When <see cref="SearchRequest.dataTypeIndex"/> is -1,
    /// fields for any valid metadata table name are returned.
    /// </summary>
    /// <param name="request">Search request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public virtual Task<IEnumerable<FieldDescription>> GetValueTypeTableFields(SearchRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request?.expression))
            return Task.FromResult(Enumerable.Empty<FieldDescription>());

        return Task.Run(() =>
        {
            return TargetCache<IEnumerable<FieldDescription>>.GetOrAdd($"{request.dataTypeIndex}:{request.expression}", () =>
            {
                int dataTypeIndex = request.dataTypeIndex;
                IDataSourceValueType dataSourceValueType = DataSourceValueTypeCache.GetDefaultInstance(dataTypeIndex == -1 ? MeasurementValue.TypeIndex : dataTypeIndex);
                DataSet metadata = Metadata.GetAugmentedDataSet(dataSourceValueType);
                string tableName = request.expression.Trim();

                // Provided unrestricted metadata table field names if data type index is -1
                if (dataTypeIndex > -1 && !dataSourceValueType.MetadataTableIsValid(metadata, tableName))
                    return Enumerable.Empty<FieldDescription>();

                return metadata.Tables[tableName].Columns.Cast<DataColumn>().Select(column => new FieldDescription
                {
                    name = column.ColumnName,
                    type = column.DataType.GetReflectedTypeName(false),
                    required = dataSourceValueType.RequiredMetadataFieldNames.Contains(column.ColumnName, StringComparer.OrdinalIgnoreCase)
                });
            });
        },
        cancellationToken);
    }

    /// <summary>
    /// Gets the functions that are available for a given data source value type.
    /// </summary>
    /// <param name="request">Search request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public virtual Task<IEnumerable<FunctionDescription>> GetValueTypeFunctions(SearchRequest request, CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            return TargetCache<IEnumerable<FunctionDescription>>.GetOrAdd($"{request.dataTypeIndex}:{request.expression}", () =>
            {
                // Parse out any requested group operation filter from search expression
                GroupOperations filteredGroupOperations = Enum.TryParse(request.expression.Trim(), out GroupOperations groupOperations) ?
                    groupOperations :
                    DefaultGroupOperations;

                // Assume no group operation is desired if filtered operations are undefined
                if (filteredGroupOperations == GroupOperations.Undefined)
                    filteredGroupOperations = GroupOperations.None;

                return FunctionParsing.GetGrafanaFunctions(request.dataTypeIndex).SelectMany(function =>
                {
                    GroupOperations publishedGroupOperations = function.PublishedGroupOperations;
                    GroupOperations allowedGroupOperations = function.AllowedGroupOperations;
                    GroupOperations requestedGroupOperations = filteredGroupOperations; // Value copied as it can be modified
                    GroupOperations? groupOperationNameOverride = null;

                    if (publishedGroupOperations == GroupOperations.Undefined)
                        publishedGroupOperations = GroupOperations.None;

                    if (allowedGroupOperations == GroupOperations.Undefined)
                        allowedGroupOperations = GroupOperations.None;

                    // Check for group operation exceptions where published group operation is not a subset of
                    // allowed group operations. For example, this accommodates cases like the "Evaluate" function
                    // which is forced to be a "Slice" operation, but only publishes "None". Additionally, only 
                    // apply this override if the requested group operation is a subset of either the allowed or
                    // published group operations
                    if ((publishedGroupOperations & allowedGroupOperations) == 0 &&
                        ((requestedGroupOperations & publishedGroupOperations) > 0 || (requestedGroupOperations & allowedGroupOperations) > 0))
                    {
                        // Override naming target group operation with original published group operation,
                        // for example, for the "Evaluate" function, name will not be "SliceEvaluate" but
                        // rather just "Evaluate" since original published group operation was "None"
                        groupOperationNameOverride = publishedGroupOperations;

                        // Published group operation is not a subset of allowed group operations, so
                        // we use the allowed group operations as the published group operations, for
                        // example, in the case of "Evaluate" function, we will publish the "Slice"
                        // group operation which will insert the required slice parameter
                        publishedGroupOperations = allowedGroupOperations;

                        // If the requested group operation is not a subset of the allowed group operations,
                        // then we use the allowed group operations as the requested group operations, for
                        // example, in the case of "Evaluate" function, we will request the "Slice" group
                        // even when the user requested group operation was "None"
                        if ((requestedGroupOperations & allowedGroupOperations) == 0)
                            requestedGroupOperations = allowedGroupOperations;
                    }

                    List<FunctionDescription> descriptions = [];

                    void addFunctionDescription(GroupOperations targetGroupOperation, IReadOnlyList<IParameter> definitions)
                    {
                        string formatFunctionName()
                        {
                            return (groupOperationNameOverride ?? targetGroupOperation) switch
                            {
                                GroupOperations.Slice => $"Slice{function.Name}",
                                GroupOperations.Set => $"Set{function.Name}",
                                _ => function.Name
                            };
                        }

                        static string formatDefaultValue(IParameter parameter)
                        {
                            if (parameter.Name.Equals("expression"))
                                return string.Empty;

                            return parameter.Default switch
                            {
                                string strVal => strVal,
                                DateTime dateTime => dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                                TargetTimeUnit timeUnit => timeUnit.ToString(),
                                null => "null",
                                _ => parameter.Default.ToString()
                            };
                        }

                        static bool published(IParameter parameter) => !parameter.Internal;

                        descriptions.Add(new FunctionDescription
                        {
                            name = formatFunctionName(),
                            description = function.Description,
                            aliases = function.Aliases,
                            returnType = function.ReturnType.ToString(),
                            category = function.Category.ToString(),
                            allowedGroupOperations = function.AllowedGroupOperations.ToString(),
                            publishedGroupOperations = function.PublishedGroupOperations.ToString(),
                            parameters = definitions.Where(published).Select(parameter => new ParameterDescription
                            {
                                name = parameter.Name,
                                description = parameter.Description,
                                type = parameter.Type.GetReflectedTypeName(false),
                                required = parameter.Required,
                                @default = formatDefaultValue(parameter)
                            })
                            .ToArray()
                        });
                    }

                    // Apply any requested group operation filters
                    publishedGroupOperations &= requestedGroupOperations;

                    if (publishedGroupOperations.HasFlag(GroupOperations.None))
                        addFunctionDescription(GroupOperations.None, function.ParameterDefinitions);

                    if (publishedGroupOperations.HasFlag(GroupOperations.Slice))
                        addFunctionDescription(GroupOperations.Slice, function.ParameterDefinitions.WithRequiredSliceParameter);

                    if (publishedGroupOperations.HasFlag(GroupOperations.Set))
                        addFunctionDescription(GroupOperations.Set, function.ParameterDefinitions);

                    return descriptions;
                })
                .OrderBy(function => // Order functions grouped with their "Slice" and "Set" functions
                    function.name.StartsWith("Slice") ? $"{function.name.Substring(5)}1" : 
                    function.name.StartsWith("Set") ? $"{function.name.Substring(3)}2" : 
                    $"{function.name}0");
            });
        },
        cancellationToken);
    }

    /// <summary>
    /// Search data source meta-data for a target.
    /// </summary>
    /// <param name="request">Search request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public virtual Task<string[]> Search(SearchRequest request, CancellationToken cancellationToken)
    {
        if (request is null)
            return Task.FromResult(Array.Empty<string>());

        return Task.Factory.StartNew(() =>
        {
            int dataTypeIndex = request.dataTypeIndex;
            IDataSourceValueType dataSourceValueType = DataSourceValueTypeCache.GetDefaultInstance(dataTypeIndex == -1 ? MeasurementValue.TypeIndex : dataTypeIndex);

            // If an empty expression is specified, query all point tags for data source value type (up to MaximumSearchTargetsPerRequest)
            if (string.IsNullOrWhiteSpace(request.expression))
                request.expression = $"SELECT DISTINCT TOP {MaximumSearchTargetsPerRequest} PointTag FROM {dataSourceValueType.MetadataTableName} WHERE True ORDER BY PointTag";

            request.expression = request.expression.Trim();

            return TargetCache<string[]>.GetOrAdd($"search!{request.dataTypeIndex}:{request.expression}", () =>
            {
                DataSet metadata = Metadata.GetAugmentedDataSet(dataSourceValueType);

                // Attempt to parse search target as a "SELECT" statement
                if (!parseSelectExpression(request.expression, out string tableName, out bool distinct, out string[] fieldNames, out string expression, out string sortField, out int takeCount))
                {
                    // Expression was not a 'SELECT' statement, execute a 'LIKE' statement against primary meta-data table for data source value type
                    // returning matching point tags - this can be a slow operation for large meta-data sets, so results are cached by expression
                    return metadata.Tables[dataSourceValueType.MetadataTableName]
                        .Select($"ID LIKE '{InstanceName}:%' AND PointTag LIKE '%{request.expression}%'")
                        .Take(MaximumSearchTargetsPerRequest)
                        .Select(row => $"{row["PointTag"]}")
                        .ToArray();
                }

                // Search target successfully parsed as a 'SELECT' statement, this operates as a filter for in memory metadata (not a database query)

                // Provided unrestricted metadata table field names if data type index is -1, otherwise if
                // meta-data table does not contain the field names required by the data source value type,
                // return empty results as this is not a table supported by the data source value type:
                if (dataTypeIndex > -1 && !dataSourceValueType.MetadataTableIsValid(metadata, tableName))
                    return [];

                // If table name is not in meta-data for unrestricted search, we have no choice but to attempt
                // meta-data augmentation for each default data source value instance - this is expensive, but
                // operation will be cached and is only a one-time operation
                if (dataTypeIndex == -1 && !metadata.Tables.Contains(tableName))
                {
                    foreach (IDataSourceValueType defaultDataSourceValue in DataSourceValueTypeCache.DefaultInstances)
                        Metadata.GetAugmentedDataSet(defaultDataSourceValue);
                }

                DataTable table = metadata.Tables[tableName];
                List<string> validFieldNames = [];

                for (int i = 0; i < fieldNames?.Length; i++)
                {
                    string fieldName = fieldNames[i].Trim();

                    if (table.Columns.Contains(fieldName))
                        validFieldNames.Add(fieldName);
                }

                // If no specific fields names were selected, assume "*" and select all fields
                fieldNames = validFieldNames.Count == 0 ?
                    table.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToArray() :
                    [.. validFieldNames];

                // If no filter expression or take count was specified, limit search target results - user can
                // still request larger results sets by specifying desired TOP count.
                if (takeCount == int.MaxValue && string.IsNullOrWhiteSpace(expression))
                    takeCount = MaximumSearchTargetsPerRequest;

                // Query metadata table applying optional filter expression and sort field
                DataRow[] filterResult = string.IsNullOrWhiteSpace(expression) ? // No expression returns all rows
                        string.IsNullOrWhiteSpace(sortField) ? table.Select() : table.Select("true", sortField) :
                        table.Select(expression, sortField);

                // Execute transformation on query result to return selected field values as a CSV-type string
                IEnumerable<string> transform = filterResult.Select(row => string.Join(",", fieldNames.Select(fieldName => row[fieldName].ToString())));

                // Apply optional distinct operation and apply take count row limit
                return (distinct ? transform.Distinct(StringComparer.OrdinalIgnoreCase) : transform).Take(takeCount).ToArray();
            });
        },
        cancellationToken);

        // Attempt to parse an expression that has SQL SELECT syntax
        bool parseSelectExpression(string selectExpression, out string tableName, out bool distinct, out string[] fieldNames, out string expression, out string sortField, out int topCount)
        {
            tableName = null;
            distinct = false;
            fieldNames = null;
            expression = null;
            sortField = null;
            topCount = 0;

            if (string.IsNullOrWhiteSpace(selectExpression))
                return false;

            // RegEx instance used to parse meta-data for target search queries using a reduced SQL SELECT statement syntax,
            // see Expresso 'Documentation/SearchSelectRegex.xso' for development details on regex
            s_selectExpression ??= new Regex(
                @"(SELECT\s+((?<Distinct>DISTINCT)\s+)?(TOP\s+(?<MaxRows>\d+)\s+)?(\s*((?<FieldName>\*)|((?<FieldName>\w+)(\s*,\s*(?<FieldName>\w+))*)))?\s*FROM\s+(?<TableName>\w+)\s+WHERE\s+(?<Expression>.+)\s+ORDER\s+BY\s+(?<SortField>\w+))|(SELECT\s+((?<Distinct>DISTINCT)\s+)?(TOP\s+(?<MaxRows>\d+)\s+)?(\s*((?<FieldName>\*)|((?<FieldName>\w+)(\s*,\s*(?<FieldName>\w+))*)))?\s*FROM\s+(?<TableName>\w+)\s+WHERE\s+(?<Expression>.+))|(SELECT\s+((?<Distinct>DISTINCT)\s+)?(TOP\s+(?<MaxRows>\d+)\s+)?((\s*((?<FieldName>\*)|((?<FieldName>\w+)(\s*,\s*(?<FieldName>\w+))*)))?)?\s*FROM\s+(?<TableName>\w+))",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

            Match match = s_selectExpression.Match(selectExpression.ReplaceControlCharacters());

            if (!match.Success)
                return false;

            tableName = match.Result("${TableName}").Trim();
            distinct = match.Result("${Distinct}").Trim().Length > 0;
            fieldNames = match.Groups["FieldName"].Captures.Cast<Capture>().Select(capture => capture.Value).ToArray();
            expression = match.Result("${Expression}").Trim();
            sortField = match.Result("${SortField}").Trim();

            string maxRows = match.Result("${MaxRows}").Trim();

            if (string.IsNullOrEmpty(maxRows) || !int.TryParse(maxRows, out topCount))
                topCount = int.MaxValue;

            return true;
        }
    }

    /// <summary>
    /// Queries data source for annotations in a time-range (e.g., Alarms).
    /// </summary>
    /// <param name="request">Annotation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Queried annotations from data source.</returns>
    public virtual async Task<List<AnnotationResponse>> Annotations(AnnotationRequest request, CancellationToken cancellationToken)
    {
        AnnotationType type = request.ParseQueryType(out bool useFilterExpression);
        DataSet metadata = Metadata.GetAugmentedDataSet<MeasurementValue>();
        Dictionary<string, DataRow> definitions = request.ParseSourceDefinitions(type, metadata, useFilterExpression);
        IEnumerable<TimeSeriesValues> annotationData = await Query(request.ExtractQueryRequest(definitions.Keys, MaximumAnnotationsPerRequest), cancellationToken).ConfigureAwait(false);
        List<AnnotationResponse> responses = [];

        foreach (TimeSeriesValues values in annotationData)
        {
            string target = values.target;

            if (!definitions.TryGetValue(target, out DataRow definition))
                continue;

            int index = 0;

            foreach (double[] datapoint in values.datapoints)
            {
                if (!type.IsApplicable(datapoint))
                {
                    index++;
                    continue;
                }

                AnnotationResponse response;

                if (index == values.datapoints.Length - 1)
                {
                    response = new AnnotationResponse
                    {
                        time = datapoint[MeasurementValue.TimeIndex],
                        endTime = datapoint[MeasurementValue.TimeIndex]
                    };
                }
                else
                {
                    response = new AnnotationResponse
                    {
                        time = datapoint[MeasurementValue.TimeIndex],
                        endTime = values.datapoints[index + 1][MeasurementValue.TimeIndex]
                    };
                }

                type.PopulateResponse(response, target, definition, datapoint, metadata);
                responses.Add(response);

                index++;
            }
        }

        return responses;
    }

    /// <summary>
    /// Queries current alarm device state.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns> Queried device alarm states.</returns>
    public virtual Task<IEnumerable<AlarmDeviceStateView>> GetAlarmState(CancellationToken cancellationToken)
    {
        return Task.Factory.StartNew(() =>
        {
            using AdoDataConnection connection = new(Settings.Default.System);
            return new TableOperations<AlarmDeviceStateView>(connection).QueryRecords("Name");
        },
        cancellationToken);
    }

    /// <summary>
    /// Queries All Available Device Alarm states.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns> Queried device alarm states.</returns>
    public virtual Task<IEnumerable<AlarmState>> GetDeviceAlarms(CancellationToken cancellationToken)
    {
        return Task.Factory.StartNew(() =>
        {
            using AdoDataConnection connection = new(Settings.Default.System);
            return new TableOperations<AlarmState>(connection).QueryRecords("ID");
        },
        cancellationToken);
    }

    /// <summary>
    /// Queries All Available Device Groups.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns> List of Device Groups.</returns>
    public virtual Task<IEnumerable<DeviceGroup>> GetDeviceGroups(CancellationToken cancellationToken)
    {
        return Task.Factory.StartNew(() =>
        {
            using AdoDataConnection connection = new(Settings.Default.System);

            IEnumerable<Device> groups = new TableOperations<Device>(connection).QueryRecordsWhere("AccessID = -99999");

            return groups.Select(item => new DeviceGroup
            {
                ID = item.ID,
                Name = item.Name,
                Devices = processDeviceGroup(item.ConnectionString).ToList()

            });
        },
        cancellationToken);

        IEnumerable<int> processDeviceGroup(string connectionString)
        {
            // Parse the connection string into a dictionary of key-value pairs for easy lookups
            Dictionary<string, string> settings = connectionString.ParseKeyValuePairs();
            return settings.TryGetValue("DeviceIDs", out string setting) ? setting.Split(',').Select(int.Parse) : [];
        }
    }
}