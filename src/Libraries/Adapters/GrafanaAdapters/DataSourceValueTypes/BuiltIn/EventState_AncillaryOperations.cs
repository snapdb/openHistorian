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
using Gemstone.Collections.CollectionExtensions;
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

    // Note that 'StartTime' is actual start time of event, whereas 'Time' is the timestamp of the
    // event state in context of Grafana query range. For example, the 'Time' will be at the start
    // of the Grafana query range if the event state started before the query range and is ongoing
    // so that ongoing events will be displayed in the Grafana UI for a given query range.
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

    private static (ulong pointID, Guid signalID) GetPointID(DataSet metadata, string pointTag)
    {
        DataRow row = default(EventState).LookupMetadata(metadata, MetadataTableName, pointTag);

        if (row is null)
            return (0UL, Guid.Empty);

        // Get measurement point ID from metadata
        string measurementKeyValue = row["ID"].ToString();
        bool success = MeasurementKey.TryParse(measurementKeyValue, out MeasurementKey key);
        Debug.Assert(success, $"Failed to parse measurement point ID '{measurementKeyValue}' for '{pointTag}'");

        // Get measurement signal ID from metadata
        string signalIDValue = row["SignalID"].ToString();
        success = Guid.TryParse(signalIDValue, out Guid signalID);
        Debug.Assert(success, $"Failed to parse measurement signal ID '{signalIDValue}' for '{pointTag}'");

        return (key.ID, signalID);
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
        (ulong pointID, Guid signalID) = GetPointID(metadata, target);

        // Create an intermediate measurement for signal ID so we can derive its signal type
        Measurement measurement = new() { Metadata = MeasurementKey.LookUpBySignalID(signalID).Metadata };

        // We treat user selected non alarm measurements for event states as an exception
        if (measurement.GetSignalTypeID(metadata, SignalType.ALRM) != s_alarmSignalTypeID)
            throw new InvalidOperationException($"Measurement {pointID:N0} '{target}' is not an alarm signal type. Only alarm measurements can generate event states.");

        // Get event ID for the current timeseries value - note that the value of incoming dataSourceValue is not used since
        // this is an alarm data type in the historian with special storage and retrieval semantics, only time is used
        (Guid eventID, bool alarmed, MeasurementStateFlags flags) = QueryEvent(instanceName, pointID, dataSourceValue.Time);

        // Verify alarm event ID is defined
        if (eventID == Guid.Empty)
            throw new InvalidOperationException($"Alarm measurement {pointID:N0} '{target}' has no defined event ID. Cannot generate event state."); 

        // See if an existing event state for this event ID already exists
        EventState eventState = timeValueMap.Select(item => item.Value).FirstOrDefault(state => state.EventID == eventID);

        // Check if this is a new event state
        if (eventState.EventID == Guid.Empty)
        {
            double startTime = 0.0D;

            // If this is a new event state and alarm is raised, we can assume this is start of the event,
            // otherwise we need to scan to last alarm raised even state to get the start time which looks
            // to be outside the current Grafana query range
            if (alarmed)
            {
                startTime = dataSourceValue.Time;
            }
            else
            {
                double lastRaisedTime = QueryLastRaisedState(instanceName, pointID, dataSourceValue.Time, eventID);

                if (lastRaisedTime > 0UL)
                    startTime = lastRaisedTime;
            }

            double queryStartTime = ConvertToGrafanaTimestamp((ulong)queryParameters.StartTime.Ticks);

            // Create new event state
            eventState = CreateEventState(instanceName, timeValueMap, target, pointID, flags, eventID, startTime, queryStartTime);

            // Add new event state to time value map
            timeValueMap[eventState.Time] = eventState with
            {
                // Compute event duration if this is a cleared state with a valid event start time
                Duration = !alarmed && startTime > 0.0D ?
                    ComputeDuration(startTime, dataSourceValue.Time, eventID, target) :
                    double.NaN
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
                // Update event state with computed event duration
                timeValueMap[eventState.Time] = eventState with
                {
                    Duration = ComputeDuration(eventState.StartTime, dataSourceValue.Time, eventID, target)
                };
            }
        }
    }

    readonly void IDataSourceValueType<EventState>.TimeValueMapAssignmentsComplete(string instanceName, OrderedDictionary<string, SortedList<double, EventState>> timeValueMaps, DataSet metadata, QueryParameters queryParameters)
    {
        double queryStartTime = ConvertToGrafanaTimestamp((ulong)queryParameters.StartTime.Ticks);

        // Find all ongoing raised events that occured before the current Grafana query range
        QueryLastEventRaisedStates(instanceName, timeValueMaps, metadata, queryStartTime);

        // Complete duration calculations on all event states
        foreach ((string target, SortedList<double, EventState> timeValueMap) in timeValueMaps)
        {
            foreach ((double timestamp, EventState eventState) in timeValueMap)
            {
                if (!double.IsNaN(eventState.Duration))
                    continue;

                // Get point ID for the event state
                ulong pointID = GetPointID(metadata, target).pointID;

                // If the event state duration was not calculated, we need to query for the next cleared state since the
                // event may have been cleared outside the current Grafana query range; otherwise, the event is ongoing
                double clearedTime = QueryNextClearedState(instanceName, pointID, timestamp, eventState.EventID);

                if (clearedTime <= 0.0D)
                    continue;

                // Update event state with computed event duration
                timeValueMap[eventState.Time] = eventState with
                {
                    Duration = ComputeDuration(eventState.StartTime, clearedTime, eventState.EventID, target)
                };
            }
        }
    }

    private static EventState CreateEventState(string instanceName, SortedList<double, EventState> timeValueMap, string target, ulong pointID, MeasurementStateFlags flags, Guid eventID, double startTime, double queryStartTime)
    {
        // If the raised time is not valid or outside current query range, then
        // mark event state time as the start of the query range
        double instanceTime = startTime <= 0.0D || startTime < queryStartTime ? queryStartTime : startTime;
        string eventDetails = null;

        // Technically, multiple event states for the same target could have started and be ongoing before the
        // current Grafana query range or have started at the exact same time. Each of these event states must
        // have a unique timestamp in the time value map, so if there are multiple events with the same instance
        // time, we increment the time by 1ms until it is a unique timestamp in the time value map.
        while (timeValueMap.ContainsKey(instanceTime))
            instanceTime += 1.0D;

        if (eventID != Guid.Empty)
        {
            try
            {
                // Query event details from database
                using AdoDataConnection connection = new(ConfigSettings.Instance);
                TableOperations<EventDetails> eventDetailsTable = new(connection);
                eventDetails = (eventDetailsTable.QueryRecordWhere("EventGuid = {0}", eventID) ?? new EventDetails()).Details;
            }
            catch (Exception ex)
            {
                eventDetails = $"Error loading event '{eventID}' details from database: {ex.Message}";
                Logger.SwallowException(ex, nameof(EventState), nameof(IDataSourceValueType<EventState>.AssignToTimeValueMap));
            }
        }

        string details = $"{(string.IsNullOrWhiteSpace(eventDetails) ? "No details were recorded for event" : eventDetails)}" +
                         $" [{eventID}]<br/><br/>Alarm measurement: '{instanceName}:{pointID}' [{target}]";

        return new EventState
        {
            EventID = eventID,
            Target = target,
            Details = details,
            StartTime = startTime,  // Actual start time of event
            Duration = double.NaN,
            Time = instanceTime,    // Time of event state in context of Grafana query range
            Flags = flags
        };
    }

    private static double ComputeDuration(double startTime, double clearedTime, Guid eventID, string target)
    {
        double duration = double.NaN;

        if (clearedTime > startTime)
        {
            duration = clearedTime - startTime;
        }
        else
        {
            string message = $"Time for cleared state '{clearedTime}' is earlier than start time '{startTime}' for event '{eventID}' on '{target}' -- this is unexpected, this alarm state is being ignored.";
            s_log.Publish(MessageLevel.Warning, message);
            Debug.WriteLine($"WARNING: {message}");
        }

        return duration;
    }

    // These operations make direct queries to openHistorian -- this breaks the abstraction of the base class
    // that normally works against "any" data source. This means the EventState data source value type will
    // only work with openHistorian data sources. This is a limitation of the current design. Abstracting
    // this operation to a more generic data source query would be a non-trivial amount of work. Perhaps a
    // simple solution for now would be to restrict the EventState data source value type in the UI to only
    // openHistorian data sources.

    // Query event ID for the current timeseries value
    private static (Guid, bool, MeasurementStateFlags) QueryEvent(string instanceName, ulong pointID, double time)
    {
        // Re-query current event state for this point tag / time to get alarm based event details. Although
        // this step will re-query the historian for the event state whose value was already queried as part
        // of the GrafanaDataSourceBase time-series query operations, this step is necessary to get proper
        // event details because the alarm data type is not a normal time-series value. It is expected that
        // alarm events are infrequent, so the performance impact should be low.
        using SnapClient connection = SnapClient.Connect(GetHistorianServerInstance(instanceName).Host);
        using ClientDatabaseBase<HistorianKey, HistorianValue> database = connection.GetDatabase<HistorianKey, HistorianValue>(instanceName) ??
            throw new InvalidOperationException($"Failed to get database '{instanceName}' from historian server.");

        HistorianKey key = new();
        HistorianValue value = new();
        ulong timestamp = ConvertFromGrafanaTimestamp(time);
        ulong[] pointIDs = [pointID];
        bool alarmed = false;
        Guid eventID = Guid.Empty;
        MeasurementStateFlags flags = MeasurementStateFlags.Normal;
        const ulong QuarterMillisecond = Ticks.PerMillisecond / 4;

        using TreeStream<HistorianKey, HistorianValue> currentValueStream = database.Read(timestamp - QuarterMillisecond, timestamp + QuarterMillisecond, pointIDs);

        // Interpret current value as alarm state
        if (currentValueStream.Read(key, value))
            (alarmed, eventID, flags) = value.AsAlarm;
        else
            Debug.Fail( $"Failed to read current alarmed state for '{instanceName}:{pointID}' at '{new DateTime((long)timestamp, DateTimeKind.Utc):O}'");

        return (eventID, alarmed, flags);
    }

    // Query last raised state for the event ID
    private static double QueryLastRaisedState(string instanceName, ulong pointID, double time, Guid eventID)
    {
        // Connect to historian server and get database instance
        using SnapClient connection = SnapClient.Connect(GetHistorianServerInstance(instanceName).Host);
        using ClientDatabaseBase<HistorianKey, HistorianValue> database = connection.GetDatabase<HistorianKey, HistorianValue>(instanceName) ??
            throw new InvalidOperationException($"Failed to get database '{instanceName}' from historian server.");

        HistorianKey key = new();
        HistorianValue value = new();
        ulong timestamp = ConvertFromGrafanaTimestamp(time);
        ulong[] pointIDs = [pointID];
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
            Debug.WriteLine($"WARNING: Last alarm state for measurement '{pointID}' / event '{eventID}' at '{raisedTime}' was 'cleared', not 'raised' as expected -- scanned back from {s_alarmSearchLimit / Ticks.PerDay:N3} days ago.");
    #endif

        // Fail with 0 if alarmed state is not 'raised'
        return alarmed ? ConvertToGrafanaTimestamp(raisedTime) : 0.0D;
    }

    // Query next cleared state for the event ID
    private static double QueryNextClearedState(string instanceName, ulong pointID, double time, Guid eventID)
    {
        // Connect to historian server and get database instance
        using SnapClient connection = SnapClient.Connect(GetHistorianServerInstance(instanceName).Host);
        using ClientDatabaseBase<HistorianKey, HistorianValue> database = connection.GetDatabase<HistorianKey, HistorianValue>(instanceName) ??
            throw new InvalidOperationException($"Failed to get database '{instanceName}' from historian server.");

        HistorianKey key = new();
        HistorianValue value = new();
        ulong timestamp = ConvertFromGrafanaTimestamp(time);
        ulong[] pointIDs = [pointID];
        ulong clearedTime = 0UL;
        bool alarmed = false;

        // Query for next alarm change state after current time
        using TreeStream<HistorianKey, HistorianValue> nextValueStream = database.Read(timestamp + 1, timestamp + s_alarmSearchLimit, pointIDs);

        // Scan to next value for event ID
        while (nextValueStream.Read(key, value))
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
            Debug.WriteLine($"WARNING: Last alarm state for measurement '{pointID}' / event '{eventID}' at '{clearedTime}' was 'raised', not 'cleared' as expected -- scanned forward for at up to {s_alarmSearchLimit / Ticks.PerDay:N3} days.");
    #endif

        // Fail with 0 if alarmed state is not 'cleared' -- could also be zero if ongoing event
        return alarmed ? 0.0D : ConvertToGrafanaTimestamp(clearedTime);
    }

    // Query last event raised states for the specified point IDs
    private static void QueryLastEventRaisedStates(string instanceName, OrderedDictionary<string, SortedList<double, EventState>> timeValueMaps, DataSet metadata, double queryStartTime)
    {
        // Connect to historian server and get database instance
        using SnapClient connection = SnapClient.Connect(GetHistorianServerInstance(instanceName).Host);
        using ClientDatabaseBase<HistorianKey, HistorianValue> database = connection.GetDatabase<HistorianKey, HistorianValue>(instanceName) ??
            throw new InvalidOperationException($"Failed to get database '{instanceName}' from historian server.");

        HistorianKey key = new();
        HistorianValue value = new();
        ulong timestamp = ConvertFromGrafanaTimestamp(queryStartTime);

        // Derive point IDs from time value maps
        Dictionary<string, ulong> targetPointIDs = timeValueMaps
            .Select(item => item.Key)
            .ToDictionary(target => target, target => GetPointID(metadata, target).pointID);

        IEnumerable<string> invalidTargets = targetPointIDs
            .Where(item => item.Value <= 0UL)
            .Select(item => item.Key);

        foreach (string invalidTarget in invalidTargets)
            targetPointIDs.Remove(invalidTarget);

        ulong[] pointIDs = targetPointIDs
            .Select(item => item.Value)
            .ToArray();

        // Get set of event IDs that have already been processed
        HashSet<Guid> processedEventIDs = timeValueMaps
            .SelectMany(item => item.Value)
            .Select(item => item.Value.EventID)
            .ToHashSet();

        // Query for last alarm change states prior to query start time
        using TreeStream<HistorianKey, HistorianValue> lastValuesStream = database.Read(timestamp - s_alarmSearchLimit, timestamp - 1, pointIDs);

        Dictionary<Guid, (ulong, ulong, MeasurementStateFlags)> eventRaisedStates = [];

        // Scan all events to include only those that are not already cleared, i.e., ongoing events
        while (lastValuesStream.Read(key, value))
        {
            (bool alarmed, Guid eventID, MeasurementStateFlags flags) = value.AsAlarm;

            // If this event ID is empty or has already been processed, skip it
            if (eventID == Guid.Empty || processedEventIDs.Contains(eventID))
                continue;

            if (alarmed)
                eventRaisedStates[eventID] = (key.PointID, key.Timestamp, flags);
            else
                eventRaisedStates.Remove(eventID);
        }

        Dictionary<ulong, string> pointIDTargets = targetPointIDs.ToDictionary(item => item.Value, item => item.Key);

        // Add event states that start without clearing before Grafana query range to time value maps
        foreach ((Guid eventID, (ulong pointID, ulong startTime, MeasurementStateFlags flags)) in eventRaisedStates)
        {
            string target = pointIDTargets[pointID];
            SortedList<double, EventState> timeValueMap = timeValueMaps.GetOrAdd(target, _ => []);
            EventState eventState = CreateEventState(instanceName, timeValueMap, target, pointID, flags, eventID, ConvertToGrafanaTimestamp(startTime), queryStartTime);
            timeValueMap[eventState.Time] = eventState;
        }
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

    // Returns shared historian server instance for the specified instance name,
    // consumers should not dispose this shared instance for best performance
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

        return historianServer ?? throw new InvalidOperationException($"Failed to get historian server instance '{instanceName}'. Source adapter may still be initializing.");
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