//******************************************************************************************************
//  GrafanaController.cs - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
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
//  09/15/2016 - Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************
// ReSharper disable CompareOfFloatsByEqualityOperator

using System.Data;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Gemstone;
using Gemstone.Collections.CollectionExtensions;
using Gemstone.DateTimeExtensions;
using Gemstone.StringExtensions;
using Gemstone.Timeseries;
using GrafanaAdapters;
using GrafanaAdapters.DataSourceValueTypes;
using GrafanaAdapters.Functions;
using GrafanaAdapters.Model.Annotations;
using GrafanaAdapters.Model.Common;
using GrafanaAdapters.Model.Database;
using GrafanaAdapters.Model.Functions;
using GrafanaAdapters.Model.Metadata;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using openHistorian.Adapters;
using openHistorian.Snap;
using SnapDB.Snap;
using SnapDB.Snap.Filters;
using SnapDB.Snap.Services;
using SnapDB.Snap.Services.Reader;
using DeviceState = GrafanaAdapters.Model.Database.DeviceState;
using CancellationToken = System.Threading.CancellationToken;
using Resolution = openHistorian.Adapters.Resolution;

namespace openHistorian.WebUI.Controllers;

/// <summary>
/// Represents a REST based API for a simple JSON based Grafana data source.
/// </summary>
[Route("api/[controller]/[action]")]
[Route("instance/{instanceName}/[controller]/[action]/{id?}")]
[ApiController]
public class GrafanaController : ControllerBase
{
    #region [ Members ]

    // Nested Types
    private class Peak
    {
        public float Min;
        public float Max;

        public ulong MinTimestamp;
        public ulong MaxTimestamp;

        public ulong MinFlags;
        public ulong MaxFlags;

        public void Set(float value, ulong timestamp, ulong flags)
        {
            if (value < Min)
            {
                Min = value;
                MinTimestamp = timestamp;
                MinFlags = flags;
            }

            if (value > Max)
            {
                Max = value;
                MaxTimestamp = timestamp;
                MaxFlags = flags;
            }
        }

        public void Reset()
        {
            Min = float.MaxValue;
            Max = float.MinValue;
            MinTimestamp = MaxTimestamp = 0UL;
            MinFlags = MaxFlags = 0UL;
        }

        public static readonly Peak Default = new();
    }

    // Represents a historian 2.0 data source for the Grafana adapter.
    internal class OH2DataSource : GrafanaDataSourceBase
    {
        private readonly ulong m_baseTicks = (ulong)UnixTimeTag.BaseTicks.Value;

        /// <summary>
        /// Starts a query that will read data source values, given a set of point IDs and targets, over a time range.
        /// </summary>
        /// <param name="queryParameters">Parameters that define the query.</param>
        /// <param name="targetMap">Set of IDs with associated targets to query.</param>
        /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
        /// <returns>Queried data source data in terms of value and time.</returns>
        protected override async IAsyncEnumerable<DataSourceValue> QueryDataSourceValues(QueryParameters queryParameters, OrderedDictionary<ulong, (string , string)> targetMap, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            SnapServer? server = GetAdapterInstance(InstanceName)?.Server?.Host;

            if (server is null)
                yield break;

            using SnapClient connection = SnapClient.Connect(server);
            using ClientDatabaseBase<HistorianKey, HistorianValue>? database = connection.GetDatabase<HistorianKey, HistorianValue>(InstanceName);

            if (database is null)
                yield break;

            DateTime startTime = queryParameters.StartTime;
            DateTime stopTime = queryParameters.StopTime;
            string interval = queryParameters.Interval;
            bool includePeaks = queryParameters.IncludePeaks;

            if (!TryParseInterval(interval, out TimeSpan resolutionInterval))
            {
                Resolution resolution = TrendValueAPI.EstimatePlotResolution(InstanceName, startTime, stopTime, targetMap.Keys);
                resolutionInterval = resolution.GetInterval();
            }

            BaselineTimeInterval timeInterval = resolutionInterval.Ticks switch
            {
                < Ticks.PerMinute => BaselineTimeInterval.Second,
                < Ticks.PerHour => BaselineTimeInterval.Minute,
                Ticks.PerHour => BaselineTimeInterval.Hour,
                _ => BaselineTimeInterval.Second
            };

            startTime = startTime.BaselinedTimestamp(timeInterval);
            stopTime = stopTime.BaselinedTimestamp(timeInterval);

            if (startTime == stopTime)
                stopTime = stopTime.AddSeconds(1.0D);

            SeekFilterBase<HistorianKey> timeFilter;

            // Set timestamp filter resolution
            if (includePeaks || resolutionInterval == TimeSpan.Zero)
            {
                // Full resolution query
                timeFilter = TimestampSeekFilter.CreateFromRange<HistorianKey>(startTime, stopTime);
            }
            else
            {
                // Interval query
                timeFilter = TimestampSeekFilter.CreateFromIntervalData<HistorianKey>(startTime, stopTime, resolutionInterval, new TimeSpan(TimeSpan.TicksPerMillisecond));
            }

            // Setup point ID selections
            MatchFilterBase<HistorianKey, HistorianValue> pointFilter = PointIDMatchFilter.CreateFromList<HistorianKey, HistorianValue>(targetMap.Keys);
            Dictionary<ulong, ulong> lastTimes = new(targetMap.Count);
            Dictionary<ulong, Peak> peaks = new(targetMap.Count);
            ulong resolutionSpan = (ulong)resolutionInterval.Ticks;

            if (includePeaks)
                resolutionSpan *= 2UL;

            // Start stream reader for the provided time window and selected points
            using TreeStream<HistorianKey, HistorianValue> stream = database.Read(SortedTreeEngineReaderOptions.Default, timeFilter, pointFilter);
            HistorianKey key = new();
            HistorianValue value = new();
            Peak peak = Peak.Default;

            while (await stream.ReadAsync(key, value))
            {
                cancellationToken.ThrowIfCancellationRequested();

                ulong pointID = key.PointID;
                ulong timestamp = key.Timestamp;
                float pointValue;

                if (s_alarmIDs.Contains(pointID))
                {
                    (bool alarmed, _, _) = value.AsAlarm;
                    pointValue = alarmed ? 1.0F : 0.0F;
                }
                else
                {
                    pointValue = value.AsSingle;
                }

                if (includePeaks)
                {
                    peak = peaks.GetOrAdd(pointID, _ => new Peak());
                    peak.Set(pointValue, timestamp, value.Value3);
                }

                if (resolutionSpan > 0UL && timestamp - lastTimes.GetOrAdd(pointID, 0UL) < resolutionSpan)
                    continue;

                // New value is ready for publication
                (string, string) id = targetMap[pointID];

                if (includePeaks)
                {
                    if (peak.MinTimestamp > 0UL)
                    {
                        yield return new DataSourceValue
                        {
                            ID = id,
                            Value = peak.Min,
                            Time = (peak.MinTimestamp - m_baseTicks) / (double)Ticks.PerMillisecond,
                            Flags = (MeasurementStateFlags)peak.MinFlags
                        };
                    }

                    if (peak.MaxTimestamp != peak.MinTimestamp)
                    {
                        yield return new DataSourceValue
                        {
                            ID = id,
                            Value = peak.Max,
                            Time = (peak.MaxTimestamp - m_baseTicks) / (double)Ticks.PerMillisecond,
                            Flags = (MeasurementStateFlags)peak.MaxFlags
                        };
                    }

                    peak.Reset();
                }
                else
                {
                    yield return new DataSourceValue
                    {
                        ID = id,
                        Value = pointValue,
                        Time = (timestamp - m_baseTicks) / (double)Ticks.PerMillisecond,
                        Flags = (MeasurementStateFlags)value.Value3
                    };
                }

                lastTimes[pointID] = timestamp;
            }
        }

        public override Task<List<AnnotationResponse>> Annotations(AnnotationRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new List<AnnotationResponse>());
        }
    }


    // Fields
    private GrafanaDataSourceBase? m_dataSource;

    #endregion

    #region [ Properties ]

    /// <summary>
    /// Gets the default API path string for this controller.
    /// </summary>
    protected virtual string DefaultAPIPath
    {
        get
        {
            if (!string.IsNullOrEmpty(s_defaultAPIPath))
                return s_defaultAPIPath;

            string controllerName = GetType().Name.ToLowerInvariant();

            if (controllerName.EndsWith("controller") && controllerName.Length > 10)
                controllerName = controllerName[..^10];

            s_defaultAPIPath = $"/api/{controllerName}";

            return s_defaultAPIPath;
        }
    }

    /// <summary>
    /// Gets historian data source for this Grafana adapter.
    /// </summary>
    protected GrafanaDataSourceBase? DataSource
    {
        get
        {
            if (m_dataSource is not null)
                return m_dataSource;

            string uriPath = Request.GetEncodedPathAndQuery();
            string instanceName;

            if (uriPath.StartsWith(DefaultAPIPath, StringComparison.OrdinalIgnoreCase))
            {
                // No instance provided in URL, use default instance name
                instanceName = TrendValueAPI.DefaultInstanceName ?? "PPA";
            }
            else
            {
                string[] pathElements = uriPath.Split(["/"], StringSplitOptions.RemoveEmptyEntries);

                if (pathElements.Length > 2)
                    instanceName = pathElements[1].Trim();
                else
                    throw new InvalidOperationException($"Unexpected API URL route destination encountered: {Request.GetEncodedPathAndQuery()}");
            }

            Debug.Assert(!string.IsNullOrWhiteSpace(instanceName));

            //                                                   012345
            // Support optional version in instance name (e.g., "1.0-STAT")
            const string VersionPattern = @"^(\d+\.\d+)-";

            Match match = Regex.Match(instanceName, VersionPattern);
            int version = 0;

            if (match.Success)
            {
                string decimalValue = match.Groups[1].Value;
                instanceName = instanceName[(decimalValue.Length + 1)..];
                version = int.Parse(decimalValue.Split('.')[0]);
            }

            if (version is < 1 or > 2)
                version = 2;

            if (version == 1)
                throw new InvalidOperationException("openHistorian 1.0 data source is not implemented in openHistorian 3.0.");

            LocalOutputAdapter? adapterInstance = GetAdapterInstance(instanceName);

            if (adapterInstance is not null)
            {
                DataSet? metadata = adapterInstance.DataSource;

                m_dataSource = new OH2DataSource
                {
                    InstanceName = instanceName,
                    Metadata = metadata
                };

                // Reestablish alarm ID hash set if metadata has changed
                if (!ReferenceEquals(Interlocked.Exchange(ref s_currentMetadata, metadata), metadata))
                {
                    HashSet<ulong> alarmIDs = [];

                    if (metadata is not null && metadata.Tables.Contains("ActiveMeasurements"))
                    {
                        DataRow[] rows = metadata.Tables["ActiveMeasurements"]!.Select("SignalType = 'ALRM'");

                        foreach (DataRow row in rows)
                        {
                            if (ulong.TryParse(row["ID"].ToNonNullString(), out ulong pointID))
                                alarmIDs.Add(pointID);
                        }
                    }

                    Interlocked.Exchange(ref s_alarmIDs, alarmIDs);
                }
            }

            return m_dataSource;
        }
    }

    #endregion

    #region [ Methods ]

    /// <summary>
    /// Validates that openHistorian Grafana data source is responding as expected.
    /// </summary>
    [HttpGet, Route("/api/[controller]")]
    [HttpGet, Route("/instance/{instanceName}/[controller]")]
    public HttpResponseMessage Index()
    {
        return new HttpResponseMessage(HttpStatusCode.OK);
    }

    /// <summary>
    /// Queries openHistorian as a Grafana data source.
    /// </summary>
    /// <param name="request">Query request.</param>
    /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
    [HttpPost]
    public virtual Task<IEnumerable<TimeSeriesValues>> Query(QueryRequest request, CancellationToken cancellationToken)
    {
        if (request.targets.FirstOrDefault()?.target is null)
            return Task.FromResult(Enumerable.Empty<TimeSeriesValues>());

        if(DataSource is null)
            return Task.FromResult(Enumerable.Empty<TimeSeriesValues>());

        return DataSource.Query(request, cancellationToken);
    }

    /// <summary>
    /// Gets the data source value types, i.e., any type that has implemented <see cref="IDataSourceValueType"/>,
    /// that have been loaded into the application domain.
    /// </summary>
    [HttpPost]
    public virtual IEnumerable<DataSourceValueType> GetValueTypes()
    {
        return DataSource?.GetValueTypes() ?? [];
    }

    /// <summary>
    /// Gets the table names that, at a minimum, contain all the fields that the value type has defined as
    /// required, see <see cref="IDataSourceValueType.RequiredMetadataFieldNames"/>.
    /// </summary>
    /// <param name="request">Search request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPost]
    public virtual Task<IEnumerable<string>> GetValueTypeTables(SearchRequest request, CancellationToken cancellationToken)
    {
        return DataSource?.GetValueTypeTables(request, cancellationToken) ?? Task.FromResult(Enumerable.Empty<string>());
    }

    /// <summary>
    /// Gets the field names for a given table.
    /// </summary>
    /// <param name="request">Search request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPost]
    public virtual Task<IEnumerable<FieldDescription>> GetValueTypeTableFields(SearchRequest request, CancellationToken cancellationToken)
    {
        return DataSource?.GetValueTypeTableFields(request, cancellationToken) ?? Task.FromResult(Enumerable.Empty<FieldDescription>());
    }

    /// <summary>
    /// Gets the functions that are available for a given data source value type.
    /// </summary>
    /// <param name="request">Search request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <remarks>
    /// <see cref="SearchRequest.expression"/> is used to filter functions by group operation, specifically a
    /// value of "None", "Slice", or "Set" as defined in the <see cref="GroupOperations"/> enumeration. If all
    /// function descriptions are desired, regardless of group operation, an empty string can be provided.
    /// Combinations are also supported, e.g., "Slice,Set".
    /// </remarks>
    [HttpPost]
    public virtual Task<IEnumerable<FunctionDescription>> GetValueTypeFunctions(SearchRequest request, CancellationToken cancellationToken)
    {
        return DataSource?.GetValueTypeFunctions(request, cancellationToken) ?? Task.FromResult(Enumerable.Empty<FunctionDescription>());
    }

    /// <summary>
    /// Search openHistorian for a target.
    /// </summary>
    /// <param name="request">Search target.</param>
    /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
    [HttpPost]
    public virtual Task<string[]> Search(SearchRequest request, CancellationToken cancellationToken)
    {
        return DataSource?.Search(request, cancellationToken) ?? Task.FromResult(Array.Empty<string>());
    }

    /// <summary>
    /// Reloads data source value types cache.
    /// </summary>
    /// <remarks>
    /// This function is used to support dynamic data source value type loading. Function only needs to be called
    /// when a new data source value is added to Grafana at run-time and end-user wants to use newly installed
    /// data source value type without restarting host.
    /// </remarks>
    [HttpGet]
    // TODO: Fix auth [AuthorizeControllerRole("Administrator")]
    public virtual void ReloadValueTypes()
    {
        DataSource?.ReloadDataSourceValueTypes();
    }

    /// <summary>
    /// Reloads Grafana functions cache.
    /// </summary>
    /// <remarks>
    /// This function is used to support dynamic loading for Grafana functions. Function only needs to be called
    /// when a new function is added to Grafana at run-time and end-user wants to use newly installed function
    /// without restarting host.
    /// </remarks>
    [HttpGet]
    // TODO: Fix auth [AuthorizeControllerRole("Administrator")]
    public virtual void ReloadGrafanaFunctions()
    {
        DataSource?.ReloadGrafanaFunctions();
    }

    /// <summary>
    /// Queries openHistorian for alarm state.
    /// </summary>
    /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
    [HttpPost]
    public virtual Task<IEnumerable<DeviceStatusView>> GetAlarmState(CancellationToken cancellationToken)
    {
        return DataSource?.GetAlarmState(cancellationToken) ?? Task.FromResult(Enumerable.Empty<DeviceStatusView>());
    }

    /// <summary>
    /// Queries openHistorian for device alarms.
    /// </summary>
    /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
    [HttpPost]
    public virtual Task<IEnumerable<DeviceState>> GetDeviceAlarms(CancellationToken cancellationToken)
    {
        return DataSource?.GetDeviceAlarms(cancellationToken) ?? Task.FromResult(Enumerable.Empty<DeviceState>());
    }

    /// <summary>
    /// Queries openHistorian for device groups.
    /// </summary>
    /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
    [HttpPost]
    public virtual Task<IEnumerable<DeviceGroup>> GetDeviceGroups(CancellationToken cancellationToken)
    {
        return DataSource?.GetDeviceGroups(cancellationToken) ?? Task.FromResult(Enumerable.Empty<DeviceGroup>());
    }

    /// <summary>
    /// Queries openHistorian for annotations in a time-range (e.g., Alarms).
    /// </summary>
    /// <param name="request">Annotation request.</param>
    /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
    [HttpPost]
    public virtual Task<List<AnnotationResponse>> Annotations(AnnotationRequest request, CancellationToken cancellationToken)
    {
        return DataSource?.Annotations(request, cancellationToken) ?? Task.FromResult(new List<AnnotationResponse>());
    }

    #endregion

    #region [ Static ]

    private static DataSet? s_currentMetadata;
    private static HashSet<ulong> s_alarmIDs = [];
    private static readonly Regex s_intervalExpression = new(@"(?<Value>\d+\.?\d*)(?<Unit>\w+)", RegexOptions.Compiled);

    // Static Methods
    private static bool TryParseInterval(string interval, out TimeSpan timeSpan)
    {
        if (string.IsNullOrWhiteSpace(interval))
        {
            timeSpan = default;
            return false;
        }

        Match match = s_intervalExpression.Match(interval);

        if (match.Success && double.TryParse(match.Result("${Value}"), out double value))
        {
            switch (match.Result("${Unit}").Trim().ToLowerInvariant())
            {
                case "ms":
                    timeSpan = TimeSpan.FromMilliseconds(value);
                    return true;
                case "s":
                    timeSpan = TimeSpan.FromSeconds(value);
                    return true;
                case "m":
                    timeSpan = TimeSpan.FromMinutes(value);
                    return true;
                case "h":
                    timeSpan = TimeSpan.FromHours(value);
                    return true;
                case "d":
                    timeSpan = TimeSpan.FromDays(value);
                    return true;
            }
        }

        timeSpan = default;
        return false;
    }

    private static LocalOutputAdapter? GetAdapterInstance(string instanceName)
    {
        if (string.IsNullOrWhiteSpace(instanceName))
            return null;

        return LocalOutputAdapter.Instances.TryGetValue(instanceName, out LocalOutputAdapter? adapterInstance) ? adapterInstance : null;
    }

    private static string? s_defaultAPIPath;

    #endregion
}