//******************************************************************************************************
//  EventState_AncillaryOperations.cs - Gbtc
//
//  Copyright © 2025, Grid Protection Alliance.  All Rights Reserved.
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
//  03/25/2025 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using GrafanaAdapters.Functions;
using GrafanaAdapters.Metadata;
using GrafanaAdapters.Model.Common;
using Gemstone;
using Gemstone.Diagnostics;
using Gemstone.Timeseries;
using Gemstone.TimeSpanExtensions;
using Gemstone.Timeseries.Model;
using openHistorian.Adapters;
using openHistorian.Net;
using openHistorian.Snap;
using SnapDB.Snap.Services;
using SnapDB.Snap;
using SnapDB.Snap.Services.Reader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Gemstone.Data;
using Gemstone.Data.Model;
using ConfigSettings = Gemstone.Configuration.Settings;
using Measurement = Gemstone.Timeseries.Measurement;
using SignalType = Gemstone.Numeric.EE.SignalType;

namespace GrafanaAdapters.DataSourceValueTypes.BuiltIn;

// IDataSourceValueType implementation for EventState
public partial struct EventState : IDataSourceValueType<EventState>
{
    string IDataSourceValueType.Target
    {
        readonly get => Target;
        init => Target = value;
    }

    double IDataSourceValueType.Value
    {
        readonly get => PrimaryValueTarget == EventStateTarget.StartTime ? StartTime : Duration;
        init
        {
            if (PrimaryValueTarget == EventStateTarget.StartTime)
                StartTime = value;
            else
                Duration = value;
        }
    }

    double IDataSourceValueType.Time
    {
        readonly get => Time;
        init => Time = value;
    }

    MeasurementStateFlags IDataSourceValueType.Flags
    {
        readonly get => Flags;
        init => Flags = value;
    }

    readonly double[] IDataSourceValueType.TimeSeriesValue => [StartTime, Duration, Time];

    readonly string[] IDataSourceValueType.TimeSeriesValueDefinition => [nameof(StartTime), nameof(Duration), nameof(Time)];

    readonly int IDataSourceValueType.ValueIndex => (int)PrimaryValueTarget;

    /// <inheritdoc />
    public readonly int CompareTo(EventState other)
    {
        int result = StartTime.CompareTo(other.StartTime);

        if (result != 0)
            return result;

        result = Duration.CompareTo(other.Duration);

        return result == 0 ? Time.CompareTo(other.Time) : result;
    }

    /// <inheritdoc />
    public readonly bool Equals(EventState other)
    {
        return CompareTo(other) == 0;
    }

    /// <inheritdoc />
    public readonly EventState TransposeCompute(Func<double, double> function)
    {
        return this with
        {
            StartTime = function(StartTime),
            Duration = function(Duration)
        };
    }

    readonly int IDataSourceValueType.LoadOrder => 2;

    readonly string IDataSourceValueType.MetadataTableName => MetadataTableName;

    readonly string[] IDataSourceValueType.RequiredMetadataFieldNames =>
    [
        "ID",       // <string> Measurement key, e.g., PPA:101
        "SignalID", //  <Guid>  Signal ID for associated measurement
        "PointTag"  // <string> Point tag representing event state, e.g, GPA_SHELBY-FREQ:ALARM-HI
    ];

    readonly Action<DataSet> IDataSourceValueType.AugmentMetadata => AugmentMetadata;

    /// <inheritdoc />
    public readonly DataRow LookupMetadata(DataSet metadata, string tableName, string target)
    {
        (DataRow, int) getRecordAndHashCode() =>
            (target.RecordFromTag(metadata, tableName), metadata.GetHashCode());

        string cacheKey = $"{TypeIndex}:{target}";

        (DataRow record, int hashCode) = TargetCache<(DataRow, int)>.GetOrAdd(cacheKey, getRecordAndHashCode);

        // If metadata hasn't changed, return cached record
        if (metadata.GetHashCode() == hashCode)
            return record;

        // Metadata has changed, remove cached record and re-query
        TargetCache<(DataRow, int)>.Remove(cacheKey);
        (record, _) = TargetCache<(DataRow, int)>.GetOrAdd(cacheKey, getRecordAndHashCode);

        return record;
    }

    private readonly (ulong, Guid) GetMeasurementID(DataSet metadata, string pointTag)
    {
        DataRow row = LookupMetadata(metadata, MetadataTableName, pointTag);

        if (row is null)
            return (0UL, Guid.Empty);

        // Get measurement ID from metadata
        string measurementIDValue = row["ID"].ToString();

        bool success = ulong.TryParse(measurementIDValue, out ulong measurementID);
        Debug.Assert(success, $"Failed to parse measurement ID '{measurementIDValue}' for '{pointTag}'");

        string measurementSignalIDValue = row["SignalID"].ToString();
        success = Guid.TryParse(measurementSignalIDValue, out Guid measurementSignalID);
        Debug.Assert(success, $"Failed to parse measurement signal ID '{measurementSignalIDValue}' for '{pointTag}'");

        return (measurementID, measurementSignalID);
    }

    readonly TargetIDSet IDataSourceValueType.GetTargetIDSet(DataRow record)
    {
        // A target ID set is: (target, (measurementKey, pointTag)[])
        // For the simple EventState functionality the target is the point tag
        string pointTag = record["PointTag"].ToString()!;
        return (pointTag, [(record.KeyFromRecord(), pointTag)]);
    }

    readonly DataRow IDataSourceValueType.RecordFromKey(MeasurementKey key, DataSet metadata)
    {
        return key.ToString().RecordFromKey(metadata);
    }

    readonly int IDataSourceValueType.DataTypeIndex => TypeIndex;

    readonly void IDataSourceValueType<EventState>.AssignToTimeValueMap(string instanceName, DataSourceValue dataSourceValue, SortedList<double, EventState> timeValueMap, DataSet metadata, QueryParameters queryParameters)
    {
        string target = dataSourceValue.ID.pointTag;
        (ulong measurementID, Guid signalID) = GetMeasurementID(metadata, target);

        // Create an intermediate measurement for signal ID so we can derive its signal type
        Measurement measurement = new() { Metadata = MeasurementKey.LookUpBySignalID(signalID).Metadata };

        // For now, we will treat non alarm measurements for event states as an exception
        if (measurement.GetSignalTypeID(metadata, SignalType.ALRM) != s_alarmSignalTypeID)
            throw new InvalidOperationException($"Measurement {measurementID:N0} '{target}' is not an alarm signal type. Only alarm measurements can generate event states.");

        // Get event ID for the current timeseries value
        (Guid eventID, bool alarmed, MeasurementStateFlags flags) = QueryEvent(instanceName, measurementID, dataSourceValue.Time);

        // Verify alarm event ID is defined
        if (eventID == Guid.Empty)
            throw new InvalidOperationException($"Alarm measurement {measurementID:N0} '{target}' has no defined event ID. Cannot generate event state."); 

        EventState eventState = default;

        // See if an existing event state for this event ID already exists
        foreach ((_, EventState state) in timeValueMap)
        {
            if (state.EventID != eventID)
                continue;
            
            eventState = state;
            break;
        }

        // Check if this is a new event state
        if (eventState.EventID == Guid.Empty)
        {
            double startTime = 0.0D;    // Actual start time of the event
            double instanceTime;        // Time to use for the event state instance

            // If this is a new event state and alarm is raised, we can assume this is start of the event,
            // otherwise we need to scan to last alarm raised even state to get the start time which looks
            // to be outside the current Grafana query range
            if (alarmed)
            {
                startTime = dataSourceValue.Time;
            }
            else
            {
                double lastRaisedTime = QueryLastRaisedState(instanceName, measurementID, dataSourceValue.Time, eventID);

                if (lastRaisedTime > 0UL)
                    startTime = lastRaisedTime;
            }

            double queryStartTime = ConvertToGrafanaTimestamp((ulong)queryParameters.StartTime.Ticks);

            // If the raised time is not valid or outside current query range, mark event state
            // time as the start of the query range
            if (startTime <= 0.0D || startTime < queryStartTime)
                instanceTime = queryStartTime;
            else
                instanceTime = startTime;

            string eventDetails = null;

            if (eventID != Guid.Empty)
            {
                try
                {
                    // Query event details from database
                    using AdoDataConnection connection = new(ConfigSettings.Instance);
                    TableOperations<EventDetails> eventDetailsTable = new(connection);
                    eventDetails = (eventDetailsTable.QueryRecordWhere("EventID = {0}", eventID) ?? new EventDetails()).Details;
                }
                catch (Exception ex)
                {
                    eventDetails = $"Error loading event '{eventID}' details from database: {ex.Message}";
                    Logger.SwallowException(ex, nameof(EventState), nameof(IDataSourceValueType<EventState>.AssignToTimeValueMap));
                }
            }

            string details = $"{(string.IsNullOrWhiteSpace(eventDetails) ? "No details were recorded for event" : eventDetails)}" +
                             $" [{eventID}]<br/><br/>Alarm measurement: '{instanceName}:{measurementID}' [{target}]";

            double duration = double.NaN;

            // Compute event duration if this is a cleared state with a valid start time
            if (!alarmed && startTime > 0.0D)
            {
                if (dataSourceValue.Time > startTime)
                {
                    duration = dataSourceValue.Time - startTime;
                }
                else
                {
                    string message = $"Time for cleared state '{dataSourceValue.Time}' is earlier than start time '{startTime}' for event '{eventID}' on '{target}' -- this is unexpected, this alarm state is being ignored.";
                    s_log.Publish(MessageLevel.Warning, message);
                    Debug.WriteLine($"WARNING: {message}");
                }
            }

            timeValueMap[instanceTime] = new EventState
            {
                Target = dataSourceValue.ID.target,
                Details = details,
                Duration = duration,
                Time = instanceTime,
                StartTime = startTime,
                Flags = flags
            };
        }
        else
        {
            // If this is an existing event state and alarm is raised, this is unexpected so we log a warning; otherwise, the
            // cleared state is in the Grafana query range and represents the end of the event so we can compute the duration
            if (alarmed)
            {
                string message = $"Encountered additional alarm raised state for the same event '{eventID}' at '{dataSourceValue.Time}' -- this is unexpected, this alarm state is being ignored.";
                s_log.Publish(MessageLevel.Warning, message);
                Debug.WriteLine($"WARNING: {message}");
            }
            else
            {
                if (dataSourceValue.Time > eventState.StartTime)
                {
                    // Update event state with computed event duration
                    timeValueMap[eventState.Time] = eventState with
                    {
                        Duration = dataSourceValue.Time - eventState.StartTime
                    };
                }
                else
                {
                    string message = $"Time for cleared state '{dataSourceValue.Time}' is earlier than start time '{eventState.StartTime}' for event '{eventID}' on '{target}' -- this is unexpected, this alarm state is being ignored.";
                    s_log.Publish(MessageLevel.Warning, message);
                    Debug.WriteLine($"WARNING: {message}");
                }
            }
        }
    }

    readonly void IDataSourceValueType<EventState>.TimeValueMapAssignmentsComplete(string instanceName, OrderedDictionary<string, SortedList<double, EventState>> timeValueMaps, DataSet metadata, QueryParameters queryParameters)
    {
        // Complete duration calculations on all event states
        foreach ((string target, SortedList<double, EventState> timeValueMap) in timeValueMaps)
        {
            foreach ((double timestamp, EventState eventState) in timeValueMap)
            {
                if (!double.IsNaN(eventState.Duration))
                    continue;

                // Get measurement ID for the event state
                (ulong measurementID, _) = GetMeasurementID(metadata, target);

                // If the event state duration was not calculated, we need to query for the next cleared state since the
                // event may have been cleared outside the current Grafana query range; otherwise, the event is ongoing
                double clearedTime = QueryNextClearedState(instanceName, measurementID, timestamp, eventState.EventID);

                if (clearedTime <= 0.0D)
                    continue;

                if (clearedTime > eventState.StartTime)
                {
                    // Update event state with computed event duration
                    timeValueMap[eventState.Time] = eventState with
                    {
                        Duration = clearedTime - eventState.StartTime
                    };
                }
                else
                {
                    string message = $"Time for cleared state '{clearedTime}' is earlier than start time '{eventState.StartTime}' for event '{eventState.EventID}' on '{target}' -- this is unexpected, this alarm state is being ignored.";
                    s_log.Publish(MessageLevel.Warning, message);
                    Debug.WriteLine($"WARNING: {message}");
                }
            }
        }
    }

    // These operations make direct queries to openHistorian -- this breaks the abstraction of the base class
    // that normally works against "any" data source. This means the EventState data source value type will
    // only work with openHistorian data sources. This is a limitation of the current design. Abstracting
    // this operation to a more generic data source query would be a non-trivial amount of work. Perhaps a
    // simple solution for now would be to restrict the EventState data source value type in the UI to only
    // openHistorian data sources.

    // Query event ID for the current timeseries value
    private static (Guid, bool, MeasurementStateFlags) QueryEvent(string instanceName, ulong measurementID, double time)
    {
        // Re-query current event state for this point tag / time to get alarm based event details. Although
        // this step will re-query the historian for the event state whose value was already queried as part
        // of the GrafanaDataSourceBase time-series query operations, this step is necessary to get proper
        // event details because the alarm data type is not a normal time-series value. It is expected that
        // alarm events are infrequent, so the performance impact should be low.
        HistorianServer server = GetHistorianServerInstance(instanceName) ?? 
            throw new InvalidOperationException($"Failed to get historian server instance '{instanceName}'. Source adapter may still be initializing.");

        using SnapClient connection = SnapClient.Connect(server.Host);
        using ClientDatabaseBase<HistorianKey, HistorianValue> database = connection.GetDatabase<HistorianKey, HistorianValue>(instanceName) ??
            throw new InvalidOperationException($"Failed to get database '{instanceName}' from historian server.");

        HistorianKey key = new();
        HistorianValue value = new();
        ulong timestamp = ConvertFromGrafanaTimestamp(time);
        ulong[] pointIDs = [measurementID];
        bool alarmed = false;
        Guid eventID = Guid.Empty;
        MeasurementStateFlags flags = MeasurementStateFlags.Normal;

        using TreeStream<HistorianKey, HistorianValue> currentValueStream = database.Read(timestamp, timestamp, pointIDs);

        // Interpret current value as alarm state
        if (currentValueStream.Read(key, value))
            (alarmed, eventID, flags) = value.AsAlarm;
        else
            Debug.Fail( $"Failed to read current alarmed state for '{measurementID}' at '{time}'");

        return (eventID, alarmed, flags);
    }

    // Query last raised state for the event ID
    private static double QueryLastRaisedState(string instanceName, ulong measurementID, double time, Guid eventID)
    {
        // Get historian server instance
        HistorianServer server = GetHistorianServerInstance(instanceName) ??
            throw new InvalidOperationException($"Failed to get historian server instance '{instanceName}'. Source adapter may still be initializing.");

        // Connect to historian server and get database instance
        using SnapClient connection = SnapClient.Connect(server.Host);
        using ClientDatabaseBase<HistorianKey, HistorianValue> database = connection.GetDatabase<HistorianKey, HistorianValue>(instanceName) ??
            throw new InvalidOperationException($"Failed to get database '{instanceName}' from historian server.");

        HistorianKey key = new();
        HistorianValue value = new();
        ulong timestamp = ConvertFromGrafanaTimestamp(time);
        ulong[] pointIDs = [measurementID];
        ulong raisedTime = 0UL;
        bool alarmed = false;

        // Query for last alarm change state prior to current time
        using TreeStream<HistorianKey, HistorianValue> lastValueStream = database.Read(timestamp - s_alarmSearchLimit, timestamp - 1, pointIDs);

        // Scan to last value for event ID
        while (lastValueStream.Read(key, value))
        {
            (bool readAlarmState, Guid readEventID, _) = value.AsAlarm;

            if (readEventID != eventID)
                continue;
            
            alarmed = readAlarmState;
            raisedTime = key.Timestamp;
        }

    #if DEBUG
        // Display debug warning if previous alarmed 'raised' state was not found -- this is unexpected
        if (!alarmed)
            Debug.WriteLine($"WARNING: Last alarm state for measurement '{measurementID}' / event '{eventID}' at '{raisedTime}' was 'cleared', not 'raised' as expected -- scanned back from {s_alarmSearchLimit / Ticks.PerDay:N3} days ago.");
    #endif

        // Fail with 0 if alarmed state is not 'raised'
        return alarmed ? ConvertToGrafanaTimestamp(raisedTime) : 0.0D;
    }

    // Query next cleared state for the event ID
    private static double QueryNextClearedState(string instanceName, ulong measurementID, double time, Guid eventID)
    {
        // Get historian server instance
        HistorianServer server = GetHistorianServerInstance(instanceName) ??
            throw new InvalidOperationException($"Failed to get historian server instance '{instanceName}'. Source adapter may still be initializing.");

        // Connect to historian server and get database instance
        using SnapClient connection = SnapClient.Connect(server.Host);
        using ClientDatabaseBase<HistorianKey, HistorianValue> database = connection.GetDatabase<HistorianKey, HistorianValue>(instanceName) ??
            throw new InvalidOperationException($"Failed to get database '{instanceName}' from historian server.");

        HistorianKey key = new();
        HistorianValue value = new();
        ulong timestamp = ConvertFromGrafanaTimestamp(time);
        ulong[] pointIDs = [measurementID];
        ulong clearedTime = 0UL;
        bool alarmed = false;

        // Query for next alarm change state after current time
        using TreeStream<HistorianKey, HistorianValue> lastValueStream = database.Read(timestamp + 1, timestamp + s_alarmSearchLimit, pointIDs);

        // Scan to next value for event ID
        while (lastValueStream.Read(key, value))
        {
            (bool readAlarmState, Guid readEventID, _) = value.AsAlarm;

            if (readEventID != eventID)
                continue;
            
            alarmed = readAlarmState;
            clearedTime = key.Timestamp;
            break;
        }

    #if DEBUG
        // Display debug warning if next alarmed 'cleared' state was not found -- this is unexpected (should be either found or ongoing event, not alarmed again)
        if (alarmed)
            Debug.WriteLine($"WARNING: Last alarm state for measurement '{measurementID}' / event '{eventID}' at '{clearedTime}' was 'raised', not 'cleared' as expected -- scanned forward for at up to {s_alarmSearchLimit / Ticks.PerDay:N3} days.");
    #endif

        // Fail with 0 if alarmed state is not 'cleared' -- could also be zero if ongoing event
        return alarmed ? 0.0D : ConvertToGrafanaTimestamp(clearedTime);
    }

    private static ulong ConvertFromGrafanaTimestamp(double timestamp)
    {
        return (ulong)new DateTime((long)(s_baseTicks + timestamp * Ticks.PerMillisecond), DateTimeKind.Utc).Ticks;
    }

    private static double ConvertToGrafanaTimestamp(ulong timestamp)
    {
        return (timestamp - s_baseTicks) / (double)Ticks.PerMillisecond;
    }

    private static WeakReference<HistorianServer> s_historianServer;
    private static readonly string s_primaryHistorianInstanceName;
    private static readonly int s_alarmSignalTypeID;
    private static readonly ulong s_alarmSearchLimit;
    private static readonly ulong s_baseTicks;
    private static readonly LogPublisher s_log;

    private static HistorianServer GetHistorianServerInstance(string instanceName)
    {
        if (s_historianServer is not null && s_historianServer.TryGetTarget(out HistorianServer historianServer) && !historianServer.IsDisposed)
            return historianServer;

        if (string.IsNullOrWhiteSpace(instanceName))
            instanceName = s_primaryHistorianInstanceName;

        if (!LocalOutputAdapter.Instances.TryGetValue(instanceName, out LocalOutputAdapter adapterInstance) && !LocalOutputAdapter.Instances.IsEmpty)
            adapterInstance = LocalOutputAdapter.Instances.Values.First();

        historianServer = adapterInstance?.Server;

        if (historianServer is not null)
            s_historianServer = new WeakReference<HistorianServer>(historianServer);

        return historianServer;
    }

    static EventState()
    {
        const double DefaultAlarmSearchLimit = 1.0D;

        s_baseTicks = (ulong)UnixTimeTag.BaseTicks.Value;

        try
        {
            dynamic section = ConfigSettings.Default[ConfigSettings.SystemSettingsCategory];

            s_primaryHistorianInstanceName = section["PrimaryHistorianInstance", "PPA", "Primary historian instance name to use by default when no other is specified."];

            if (string.IsNullOrWhiteSpace(s_primaryHistorianInstanceName))
                s_primaryHistorianInstanceName = "PPA";

            double alarmSearchLimit = section["AlarmSearchLimit", DefaultAlarmSearchLimit, "Defines the maximum time range, in floating-point days, to execute a query to find next or previous alarm change state."];
            s_alarmSearchLimit = (ulong)(alarmSearchLimit * Ticks.PerDay);
        }
        catch (Exception ex)
        {
            Logger.SwallowException(ex, "Failed to load primary historian instance name from settings, defaulting to 'PPA'.");
            s_primaryHistorianInstanceName = "PPA";
        }

        try
        {
            using AdoDataConnection connection = new(ConfigSettings.Instance);
            s_alarmSignalTypeID = connection.ExecuteScalar<int>($"SELECT ID FROM SignalType WHERE Acronym = '{nameof(SignalType.ALRM)}'");
        }
        catch (Exception ex)
        {
            Logger.SwallowException(ex, $"Failed to lookup \"SignalType.ID\" for \"SignalType.Acronym = '{nameof(SignalType.ALRM)}'\", using default value of {SignalType.ALRM}");
            s_alarmSignalTypeID = (int)SignalType.ALRM;
        }

        s_log = Logger.CreatePublisher(typeof(EventState), MessageClass.Component);
    }

    /// <summary>
    /// Gets the type index for <see cref="EventState"/>.
    /// </summary>
    public static readonly int TypeIndex = DataSourceValueTypeCache.GetTypeIndex(nameof(EventState));

    /// <summary>
    /// Update event state primary target to operate on duration value.
    /// </summary>
    /// <param name="dataValue">Source event state.</param>
    /// <returns>Event state updated to operate on duration value.</returns>
    public static EventState DurationAsTarget(EventState dataValue) => dataValue with
    {
        PrimaryValueTarget = EventStateTarget.Duration
    };

    // Augments metadata for EventState data source
    private static void AugmentMetadata(DataSet metadata)
    {
        const string EventName = $"{nameof(EventState)} Metadata Augmentation";

        // Check if event state metadata augmentation has already been performed for this dataset
        if (metadata.Tables.Contains(MetadataTableName))
            return;

        lock (typeof(EventState))
        {
            // Check again after lock in case another thread already performed augmentation
            if (metadata.Tables.Contains(MetadataTableName))
                return;

            try
            {
                s_log.Publish(MessageLevel.Info, EventName, $"Starting metadata augmentation for {nameof(EventState)} data source value type...");
                long startTime = DateTime.UtcNow.Ticks;

                // Event state metadata is simply a reduced field set of active measurements filtered to alarm signal types
                DataTable activeMeasurements = metadata.Tables[MeasurementValue.MetadataTableName]!;
                DataRow[] alarmRows = activeMeasurements.Select("SignalType = 'ALRM'");

                // Create new metadata table for event state values - this becomes a filterable data source table that can be queried
                DataTable eventStates = metadata.Tables.Add(MetadataTableName);

                // Add columns to event state metadata table
                eventStates.Columns.Add("Device", typeof(string));

                // These are standard required fields for metadata lookup functions,
                // especially as related to AdapterBase.ParseFilterExpression
                eventStates.Columns.Add("PointTag", typeof(string));
                eventStates.Columns.Add("ID", typeof(string));
                eventStates.Columns.Add("SignalID", typeof(Guid));
                eventStates.Columns.Add("EventID", typeof(Guid));
                eventStates.Columns.Add("Longitude", typeof(decimal));
                eventStates.Columns.Add("Latitude", typeof(decimal));
                eventStates.Columns.Add("Company", typeof(string));
                eventStates.Columns.Add("UpdatedOn", typeof(DateTime));

                // Copy event state metadata from associated data in active measurements table
                foreach (DataRow alarmRow in alarmRows)
                {
                    try
                    {
                        // Create a new row that will reference event state metadata
                        DataRow eventStateRow = eventStates.NewRow();

                        // Copy in specific event state metadata
                        eventStateRow["ID"] = alarmRow["ID"];
                        eventStateRow["SignalID"] = alarmRow["SignalID"];
                        eventStateRow["PointTag"] = alarmRow["PointTag"];
                        eventStateRow["Device"] = alarmRow["Device"];

                        if (!alarmRow.IsNull("Longitude"))
                            eventStateRow["Longitude"] = Convert.ToDecimal(alarmRow["Longitude"]);
                        else
                            eventStateRow["Longitude"] = DBNull.Value;

                        if (!alarmRow.IsNull("Latitude"))
                            eventStateRow["Latitude"] = Convert.ToDecimal(alarmRow["Latitude"]);
                        else
                            eventStateRow["Latitude"] = DBNull.Value;

                        eventStateRow["Company"] = alarmRow["Company"];
                        eventStateRow["UpdatedOn"] = alarmRow["UpdatedOn"];

                        eventStates.Rows.Add(eventStateRow);
                    }
                    catch (Exception ex)
                    {
                        s_log.Publish(MessageLevel.Error, EventName, $"Failed while attempting to add augmented metadata row for {nameof(EventState)} data source value type: {ex.Message}", exception: ex);
                    }
                }

                string elapsedTime = new TimeSpan(DateTime.UtcNow.Ticks - startTime).ToElapsedTimeString(3);
                s_log.Publish(MessageLevel.Info, EventName, $"Completed metadata augmentation for {nameof(EventState)} data source value type: added {eventStates.Rows.Count:N0} rows to '{MetadataTableName}' table in {elapsedTime}.");
            }
            catch (Exception ex)
            {
                s_log.Publish(MessageLevel.Error, EventName, $"Failed while attempting to augment metadata for {nameof(EventState)} data source value type: {ex.Message}", exception: ex);
            }
        }
    }
}