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
        readonly get => PrimaryValueTarget == EventStateTarget.Raised ? Raised : Duration;
        init
        {
            if (PrimaryValueTarget == EventStateTarget.Raised)
                Raised = value;
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

    readonly double[] IDataSourceValueType.TimeSeriesValue => [Raised, Duration, Time];

    readonly string[] IDataSourceValueType.TimeSeriesValueDefinition => [nameof(Raised), nameof(Duration), nameof(Time)];

    readonly int IDataSourceValueType.ValueIndex => (int)PrimaryValueTarget;

    /// <inheritdoc />
    public readonly int CompareTo(EventState other)
    {
        int result = Raised.CompareTo(other.Raised);

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
            Raised = function(Raised),
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

    private readonly ulong GetMeasurementID(DataSet metadata, string pointTag)
    {
        DataRow row = LookupMetadata(metadata, MetadataTableName, pointTag);

        if (row is null)
            return 0UL;

        // Get measurement ID from metadata
        string measurementIDValue = row["ID"].ToString();

        bool success = ulong.TryParse(measurementIDValue, out ulong measurementID);
        Debug.Assert(success, $"Failed to parse measurement ID '{measurementIDValue}' for '{pointTag}'");

        return measurementID;
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

    readonly void IDataSourceValueType<EventState>.AssignToTimeValueMap(string instanceName, DataSourceValue dataSourceValue, SortedList<double, EventState> timeValueMap, DataSet metadata)
    {
        string pointTag = dataSourceValue.ID.pointTag;
        ulong measurementID = GetMeasurementID(metadata, pointTag);
        bool raised = dataSourceValue.Value > 0;

        // Execute historian query to resolve event state
        (Guid eventID, double duration) = QueryEventData(instanceName, measurementID, dataSourceValue.Time, raised);

        string eventDetails = null;

        if (eventID != Guid.Empty)
        {
            try
            {
                using AdoDataConnection connection = new(ConfigSettings.Instance);
                TableOperations<EventDetails> eventDetailsTable = new(connection);
                eventDetails = (eventDetailsTable.QueryRecordWhere("EventID = {0}", eventID) ?? new EventDetails()).Details;
            }
            catch (Exception ex)
            {
                eventDetails = $"Error loading event details from database: {ex.Message}";
                Logger.SwallowException(ex, nameof(EventState), nameof(IDataSourceValueType<EventState>.AssignToTimeValueMap));
            }
        }

        string details = eventID == Guid.Empty ?
            "No event ID was found for details record" :
            $"{(string.IsNullOrWhiteSpace(eventDetails) ? "No details were recorded for event" : eventDetails)}";

        details += $" [{eventID}]<br/><br/>Alarm measurement: '{instanceName}:{measurementID}' [{pointTag}]";

        timeValueMap[dataSourceValue.Time] = new EventState
        {
            Target = dataSourceValue.ID.target,
            Details = details,
            Raised = raised ? 1.0D : 0.0D,
            Duration = duration,
            Time = dataSourceValue.Time,
            Flags = dataSourceValue.Flags
        };
    }

    // This operation makes a direct query to openHistorian -- this breaks the abstraction of the base class
    // that normally works against "any" data source. This means the EventState data source value type will
    // only work with openHistorian data sources. This is a limitation of the current design. Abstracting
    // this operation to a more generic data source query would be a non-trivial amount of work. Perhaps a
    // simple solution for now would be to restrict the EventState data source value type in the UI to only
    // openHistorian data sources.
    private static (Guid, double) QueryEventData(string instanceName, ulong measurementID, double time, bool raised)
    {
        // Convert Grafana timestamp back to UTC DateTime
        ulong timestamp = (ulong)new DateTime((long)(s_baseTicks + time * Ticks.PerMillisecond), DateTimeKind.Utc).Ticks;

        // Re-query current event state for this point tag / time to get event ID. Although this step
        // will re-query the historian for the event state whose value was already queried as part of
        // the GrafanaDataSourceBase time-series query operations, it is necessary to get the event ID.
        // It is expected that alarm events are infrequent, so the performance impact should be low.
        HistorianServer server = GetHistorianServerInstance(instanceName);

        if (server is null)
            throw new InvalidOperationException($"Failed to get historian server instance '{instanceName}'. Source adapter may still be initializing.");

        using SnapClient connection = SnapClient.Connect(server.Host);
        using ClientDatabaseBase<HistorianKey, HistorianValue> database = connection.GetDatabase<HistorianKey, HistorianValue>(instanceName);

        if (database is null)
            throw new InvalidOperationException($"Failed to get database '{instanceName}' from historian server.");

        HistorianKey key = new();
        HistorianValue value = new();
        ulong[] pointIDs = [measurementID];
        bool alarmed = raised;
        Guid eventID = Guid.Empty;

        using TreeStream<HistorianKey, HistorianValue> currentValueStream = database.Read(timestamp, timestamp, pointIDs);

        // Interpret current value as alarm state
        if (currentValueStream.Read(key, value))
            (alarmed, eventID, _) = value.AsAlarm;

        Debug.Assert(alarmed == raised, $"Alarmed state mismatch for '{measurementID}' at '{time}' - expected {raised}, got {alarmed}");

        TimeSpan duration = TimeSpan.Zero;

        // If current alarm is 'cleared', then we need to find the last alarm 'raised' state
        if (!alarmed)
        {
            // Query for last alarm change state prior to current time
            using TreeStream<HistorianKey, HistorianValue> lastValueStream = database.Read(timestamp - s_reverseAlarmSearchLimit, timestamp - 1, pointIDs);

            // Scan to last value
            while (lastValueStream.Read(key, value)) { }

            // Interpret last value as alarm state
            (bool lastAlarmed, Guid lasEventID, _) = value.AsAlarm;

            // If last alarm was not cleared, duration will return NaN, i.e., ongoing
            if (!lastAlarmed)
            {
                // If last value event ID defined and current value event ID is empty, use last event ID as primary
                if (eventID == Guid.Empty)
                    eventID = lasEventID;

                // Calculate duration as time since last alarm change state
                duration = TimeSpan.FromMilliseconds((timestamp - key.Timestamp) / (double)Ticks.PerMillisecond);
            }
            else
            {
                Debug.Fail($"Last alarmed state for '{measurementID}' at '{time}' was not 'raised' as expected");
            }
        }

        return (eventID, duration == TimeSpan.Zero ? double.NaN : duration.TotalMilliseconds);
    }

    private static WeakReference<HistorianServer> s_historianServer;
    private static readonly string s_primaryHistorianInstanceName;
    private static readonly ulong s_reverseAlarmSearchLimit;
    private static readonly ulong s_baseTicks;

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
        const double DefaultReverseAlarmSearchLimit = 1.0D;

        s_baseTicks = (ulong)UnixTimeTag.BaseTicks.Value;

        try
        {
            dynamic section = ConfigSettings.Default[ConfigSettings.SystemSettingsCategory];

            s_primaryHistorianInstanceName = section["PrimaryHistorianInstance", "PPA", "Primary historian instance name to use by default when no other is specified."];

            if (string.IsNullOrWhiteSpace(s_primaryHistorianInstanceName))
                s_primaryHistorianInstanceName = "PPA";

            double reverseAlarmSearchLimit = section["ReverseAlarmSearchLimit", DefaultReverseAlarmSearchLimit, "Defines the maximum time range, in floating-point days, to execute a reverse order query to find last alarm change state."];
            s_reverseAlarmSearchLimit = (ulong)(reverseAlarmSearchLimit * Ticks.PerDay);
        }
        catch (Exception ex)
        {
            Logger.SwallowException(ex, "Failed to load primary historian instance name from settings, defaulting to 'PPA'.");
            s_primaryHistorianInstanceName = "PPA";
        }
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

    private static readonly LogPublisher s_log = Logger.CreatePublisher(typeof(EventState), MessageClass.Component);
}