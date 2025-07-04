﻿//******************************************************************************************************
//  DeviceStateAdapter.cs - Gbtc
//
//  Copyright © 2018, Grid Protection Alliance.  All Rights Reserved.
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
//  07/18/2018 - J. Ritchie Carroll
//       Generated original version of source code.
//  27/11/2019 - C. Lackner
//      Moved Adapter to GSF
//
//******************************************************************************************************

using Gemstone;
using Gemstone.Collections.CollectionExtensions;
using Gemstone.Data;
using Gemstone.Data.DataExtensions;
using Gemstone.Data.Model;
using Gemstone.Diagnostics;
using Gemstone.IO;
using Gemstone.IO.Collections;
using Gemstone.IO.Parsing;
using Gemstone.StringExtensions;
using Gemstone.Threading.SynchronizedOperations;
using Gemstone.Timeseries;
using Gemstone.Timeseries.Adapters;
using Gemstone.Units;
using GrafanaAdapters.Model.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using ConfigSettings = Gemstone.Configuration.Settings;
using ConnectionStringParser = Gemstone.Configuration.ConnectionStringParser<Gemstone.Timeseries.Adapters.ConnectionStringParameterAttribute>;
using Timer = System.Timers.Timer;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using Gemstone.ActionExtensions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.CodeAnalysis;
using Gemstone.Timeseries.Model;
using System.Diagnostics.Metrics;
using Gemstone.Security.AccessControl;
using GrafanaAdapters.Metadata;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Gemstone.Configuration;
using GrafanaAdapters.Model.Common;
using Gemstone.ComponentModel.DataAnnotations;

namespace GrafanaAdapters;

/// <summary>
/// Represents an adapter that will monitor and report device states.
/// </summary>
[Description("Device State: Monitors and updates states for devices")]
public class DeviceStateAdapter : FacileActionAdapterBase
{
    #region [ Members ]

    // Required AlarmStates
    private enum AlarmState
    {
        Good,           // Everything is kosher
        Alarm,          // Not available for longer than configured alarm time
        NotAvailable,   // Data is missing or timestamp is outside lead/lag time range
        BadData,        // Quality flags report bad data
        BadTime,        // Quality flags report bad time
        OutOfService,   // Source device is disabled
        Acknowledged    // Any issues are considered acknowledged, do not report issues
    }

    // Constants

    /// <summary>
    /// The <see cref="DeviceState"/> that are required by the Adapter. If these are deleted or changed, the system will automatically add them back in.
    /// </summary>
    private static readonly Dictionary<AlarmState, DeviceState> s_baseStates = new ()
    {
        { AlarmState.Good, new DeviceState() { Color="green", RecommendedAction = "", State=nameof(AlarmState.Good), Rules="rule001={Combination=1; Operation=12; SetPoint=55; Query=SignalType LIKE 'FREQ'}", Priority=0 }  },
        { AlarmState.Alarm, new DeviceState() { Color="red", RecommendedAction = "", State=nameof(AlarmState.Alarm), Rules="rule001={Combination=1; Operation=12; SetPoint=55; Query=SignalType LIKE 'FREQ'}", Priority=1 }  },
        { AlarmState.BadData, new DeviceState() { Color="blue", RecommendedAction = "", State=nameof(AlarmState.BadData), Rules="", Priority=2 }  },
        { AlarmState.BadTime, new DeviceState() { Color="purple", RecommendedAction = "", State=nameof(AlarmState.BadTime), Rules="", Priority=3 }  },
        { AlarmState.NotAvailable, new DeviceState() { Color="orange", RecommendedAction = "", State=nameof(AlarmState.NotAvailable), Rules="", Priority=int.MaxValue-3 }  },
        { AlarmState.OutOfService, new DeviceState() { Color="grey", RecommendedAction = "", State=nameof(AlarmState.OutOfService), Rules="", Priority=int.MaxValue-2 }  },
        { AlarmState.Acknowledged, new DeviceState() { Color="rosybrown",  RecommendedAction = "", State=nameof(AlarmState.Acknowledged), Rules="", Priority=int.MaxValue-1 }  }
    };

    private const int UpToDate = 0;
    private const int Modified = 1;

    // Internal Classes

    /// <summary>
    /// Defines the default value for the <see cref="MonitoringRate"/>.
    /// </summary>
    public const int DefaultMonitoringRate = 30000;

    /// <summary>
    /// Defines the default value for the <see cref="NotAvailableMinutes"/>.
    /// </summary>
    public const double DefaultNotAvailableMinutes = 10.0D;

    /// <summary>
    /// Defines the default value for the <see cref="AcknowledgedTransitionHysteresisDelay"/>.
    /// </summary>
    public const double DefaultAcknowledgedTransitionHysteresisDelay = 30.0D;

    // Fields
    private Dictionary<int, Device> m_deviceMetadata;

    private FileBackedDictionary<int, long> m_lastDeviceStateChange;
    private Dictionary<int, int> m_lastState;

    private Dictionary<int, int> m_stateCounts;
    private List<DeviceState> m_deviceStates;
    private Dictionary<AlarmState, int> m_requiredStateIDs;
    private Dictionary<int, List<StateRule>> m_stateRules;

    private Lock m_stateCountLock;
    private Ticks m_alarmTime;
    private long m_alarmStateUpdates;
    private int m_dataSourceState;
    private DataSet? m_deviceStateDataSet;

    private bool m_disposed;
    private bool m_disposing;

    private readonly TaskSynchronizedOperation m_processDeviceStatus;

    #endregion

    #region [ Constructors ]

    /// <summary>
    /// Creates a new <see cref="DeviceStateAdapter"/>.
    /// </summary>
    public DeviceStateAdapter()
    {
        m_alarmTime = TimeSpan.FromMinutes(NotAvailableMinutes).Ticks;

        m_processDeviceStatus = new TaskSynchronizedOperation(MonitoringOperation, ex => OnProcessException(MessageLevel.Warning, ex));

        Default = this;
    }

    #endregion

    #region [ Properties ]

    /// <summary>
    /// Gets or sets monitoring rate, in milliseconds, for devices.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Defines overall monitoring rate, in milliseconds, for devices.")]
    [DefaultValue(DefaultMonitoringRate)]
    [Label("Monitoring Rate")]
    public int MonitoringRate { get; set; }

    /// <summary>
    /// Gets or sets the time, in minutes, for which to change the device state to alarm when no data is received.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Defines the time, in minutes, for which to change the device state to not available when no data is received.")]
    [DefaultValue(DefaultNotAvailableMinutes)]
    [Label("Not Available Minutes")]
    public double NotAvailableMinutes
    {
        get => m_alarmTime.ToMinutes();
        set => m_alarmTime = TimeSpan.FromMinutes(value).Ticks;
    }

    /// <summary>
    /// Gets or sets the flag that determines if alarm states should only target parent devices, i.e., PDCs and direct connect PMUs, or all devices.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Defines the flag that determines if alarm states should only target parent devices, i.e., PDCs and direct connect PMUs, or all devices.")]
    [Label("Target Parent Devices")]
    public bool TargetParentDevices { get; set; }

    /// <summary>
    /// Gets or sets delay time, in minutes, before transitioning the Acknowledged state back to Good.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Defines the delay time, in minutes, before transitioning the Acknowledged state back to Good.")]
    [DefaultValue(DefaultAcknowledgedTransitionHysteresisDelay)]
    [Label("Acknowledged Transition Hysteresis Delay")]
    public double AcknowledgedTransitionHysteresisDelay { get; set; }

    /// <summary>
    /// Gets or sets primary keys of input measurements the adapter expects, if any.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)] // Automatically controlled
    public override MeasurementKey[] InputMeasurementKeys
    {
        get => base.InputMeasurementKeys;
        set => base.InputMeasurementKeys = value;
    }

    /// <summary>
    /// Gets or sets output measurements that the adapter will produce, if any.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)] // Automatically controlled
    public override IMeasurement[] OutputMeasurements
    {
        get => base.OutputMeasurements;
        set => base.OutputMeasurements = value;
    }

    /// <summary>
    /// Gets the flag indicating if this adapter supports temporal processing.
    /// </summary>
    public override bool SupportsTemporalProcessing => false;

    public override bool TrackLatestMeasurements => true;

    
    /// <summary>
    /// Returns the detailed status of the data input source.
    /// </summary>
    public override string Status
    {
        get
        {
            StringBuilder status = new();

            status.Append(base.Status);
            status.AppendLine($"           Monitoring Rate: {MonitoringRate:N0}ms");
            status.AppendLine($"  Targeting Parent Devices: {TargetParentDevices}");
            status.AppendLine($"    Monitored Device Count: {InputMeasurementKeys?.Length ?? 0:N0}");
            status.AppendLine($"     No Data Alarm Timeout: {m_alarmTime.ToElapsedTimeString(2)}");
            status.AppendLine($"             State Updates: {m_alarmStateUpdates:N0}");

            lock (m_stateCountLock)
            {
                foreach (DeviceState s in m_deviceStates)
                    status.AppendLine($"{s.State.PadLeft(30)}: {(m_stateCounts.TryGetValue(s.ID, out int count) ? count : 0)}");
            }

            return status.ToString();
        }
    }

    /// <summary>
    /// Gets or sets <see cref="DataSet"/> based data source available to this <see cref="AdapterBase"/>.
    /// </summary>
    public override DataSet? DataSource
    {
        get => base.DataSource;
        set
        {
            base.DataSource = value;

            if (Interlocked.CompareExchange(ref m_dataSourceState, Modified, UpToDate) == UpToDate && Initialized)
                m_processDeviceStatus.RunAsync();
        }
    }
    #endregion

    #region [ Methods ]

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="DeviceStateAdapter"/> object and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (m_disposed)
            return;

        m_disposing = disposing;

        try
        {
            if (!disposing)
                return;

            m_lastDeviceStateChange?.Dispose();
        }
        finally
        {
            m_disposed = true;          // Prevent duplicate dispose.
            base.Dispose(disposing);    // Call base class Dispose().
        }
    }

    /// <summary>
    /// Initializes <see cref="DeviceStateAdapter" />.
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();

        ConnectionStringParser parser = new();
        parser.ParseConnectionString(ConnectionString, this);

        m_deviceMetadata = new();

        m_lastDeviceStateChange = new FileBackedDictionary<int, long>(FilePath.GetAbsolutePath($"{Name}_LastStateChangeCache.bin".RemoveInvalidFileNameCharacters()));

        m_stateCountLock = new Lock();
        m_deviceStates = new();
        m_requiredStateIDs = new();
        m_lastState = new();
        m_stateRules = new();

        m_stateCounts = CreateNewStateCountsMap();

        if (Interlocked.CompareExchange(ref m_dataSourceState, Modified, Modified) == Modified)
            m_processDeviceStatus.RunAsync();
        else
            new Action(m_processDeviceStatus.RunAsync).DelayAndExecute(MonitoringRate);
    }

    /// <summary>
    /// Queues monitoring operation to update alarm state for immediate execution.
    /// </summary>
    [AdapterCommand("Queues monitoring operation to update alarm state for immediate execution.", ResourceAccessLevel.Admin, ResourceAccessLevel.Edit)]
    public void QueueStateUpdate()
    {
        m_processDeviceStatus?.RunAsync();
    }


    /// <summary>
    ///  Get measurements for the provided query and device ID.
    /// </summary>
    [AdapterCommand("Gets measurements for provided query and device ID.", ResourceAccessLevel.Admin, ResourceAccessLevel.Edit, ResourceAccessLevel.View)]
    public static IActionResult GetMeasurements(ControllerBase controller, string query, string deviceAcronym)
    {
        bool dataSourceAvailable = Default?.DataSource is not null;

        if (string.IsNullOrWhiteSpace(query))
            return controller.Ok(new string[0]);

        List<string> tags = new();
        if (dataSourceAvailable && ParseFilterExpression("Filter ActiveMeasurements WHERE " + query + " AND Device = '" + deviceAcronym + "'", out string tableName, out string expression, out string sortField, out int takeCount))
        {
            foreach (DataRow row in Default?.DataSource!.Tables[tableName].Select(expression, sortField).Take(takeCount))
            {
                tags.Add(row.ConvertField<string>("PointTag"));                
            }
        }

        return controller.Ok(tags);
    }

    /// <summary>
    /// Gets a short one-line status of this adapter.
    /// </summary>
    /// <param name="maxLength">Maximum number of available characters for display.</param>
    /// <returns>A short one-line summary of the current status of this adapter.</returns>
    public override string GetShortStatus(int maxLength)
    {
        return Enabled
            ? $"Monitoring enabled for every {Ticks.FromMilliseconds(MonitoringRate).ToElapsedTimeString()}".CenterText(maxLength)
            : "Monitoring is disabled...".CenterText(maxLength);
    }

    private async Task MonitoringOperation()
    {
        if (m_disposing)
            return;

        if (!await ValidateStates())
        {
            OnStatusMessage(MessageLevel.Warning, "Device state definition updated, need to reload device state.");
            base.OnConfigurationChanged();
            new Action(m_processDeviceStatus.RunAsync).DelayAndExecute(MonitoringRate);
            return;
        }

        lock (m_deviceStates)
        {
            IReadOnlyDictionary<MeasurementKey, IMeasurement> measurementLookup = LatestMeasurements
           .Cast<IMeasurement>()
           .ToDictionary(measurement => measurement.Key);

            List<DeviceStatus> alarmDeviceUpdates = [];
            Dictionary<int, int> stateCounts = CreateNewStateCountsMap();
            Ticks now = DateTime.UtcNow;

            OnStatusMessage(MessageLevel.Info, "Updating device states");

            using (AdoDataConnection connection = new(ConfigSettings.Instance))
            {
                TableOperations<DeviceStatus> alarmDeviceTable = new(connection);

                foreach (DeviceStatus alarmDevice in alarmDeviceTable.QueryRecords())
                {
                    //Skip a Device if there are no measurements or it has no entry in the Device MetaData
                    if (!m_deviceMetadata.TryGetValue(alarmDevice.DeviceID, out Device metadata))
                        continue;

                    if (!m_lastState.TryGetValue(alarmDevice.DeviceID, out int currentState))
                        currentState = m_requiredStateIDs.GetValueOrDefault(AlarmState.NotAvailable);

                    int newState = 0;

                    if (!metadata.Enabled)
                        newState = m_requiredStateIDs.GetValueOrDefault(AlarmState.OutOfService);
                    else
                    {
                        //Loop Through all States In Priority Order. If a state matches we are done. If it does not we continue
                        foreach (DeviceState state in m_deviceStates)
                        {
                            newState = state.ID;
                            if (!m_stateRules.TryGetValue(state.ID, out List<StateRule> rules))
                                rules = new List<StateRule>();

                            if (!rules.Any() || rules.All(r => r.Test(measurementLookup, alarmDevice.DeviceID,now)))
                            {
                                break;
                            }
                        }

                    }

                    // Currently is Acknowledged special Logic applys
                    if (currentState == m_requiredStateIDs.GetValueOrDefault(AlarmState.Acknowledged))
                    {
                        if (newState != m_requiredStateIDs.GetValueOrDefault(AlarmState.Good))
                            newState = currentState;
                    }

                    // Track current state counts
                    stateCounts[(int)newState]++;

                    if (currentState != newState)
                    {
                        alarmDevice.StateID = newState;
                    }
                    m_lastState[alarmDevice.DeviceID] = newState;

                    // Update alarm table record
                    alarmDeviceTable.UpdateRecord(alarmDevice);
                }

                m_alarmStateUpdates++;

                lock (m_stateCountLock)
                    m_stateCounts = stateCounts;
            }
        }
        new Action(m_processDeviceStatus.RunAsync).DelayAndExecute(MonitoringRate);
    }

    private Dictionary<int, int> CreateNewStateCountsMap()
    {
        Dictionary<int, int> counts = new Dictionary<int, int>();
        foreach (DeviceState s in m_deviceStates)
        {
            counts.Add(s.ID, 0);
        }
        return counts;
    }

    private async Task<bool> ValidateStates()
    {
        DataSet? dataSource = DataSource;

        if (dataSource is null)
        {
            OnStatusMessage(MessageLevel.Warning, "Device State definition update skipped, no data source is available.");
            return true;
        }

        DataSet deviceStateDataSet = new();
        deviceStateDataSet.Tables.Add(dataSource.Tables["DeviceState"]!.Copy());



        if (DataSetEqualityComparer.Default.Equals(m_deviceStateDataSet, deviceStateDataSet))
            return true;

        m_deviceStateDataSet = deviceStateDataSet;

        bool addedState = false;

        await using AdoDataConnection connection = new(ConfigSettings.Instance);
        TableOperations<DeviceState> tblOperation = new(connection);

        m_deviceStates = deviceStateDataSet.Tables[0].Rows.Cast<DataRow>().Select((row) => tblOperation.LoadRecord(row)).OrderBy(r => r.Priority).ToList();
        foreach ((AlarmState key, DeviceState state) in s_baseStates)
        {
            DataRow row = deviceStateDataSet.Tables[0].Rows.Cast<DataRow>().Where(x => string.Equals(x.ConvertField<string>("State"), state.State, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            if (row == null)
            {
                tblOperation.AddNewRecord(state);
                addedState = true;

            }
            else
                m_requiredStateIDs.Add(key, row.ConvertField<int>("ID"));
        }

        if (addedState)
            return false;

        Dictionary<int, List<StateRule>> stateRules = new();

        // Get all Devices
        DataSet deviceDataSet = new();
        deviceDataSet.Tables.Add(dataSource.Tables["Device"]!.Copy());
        TableOperations<Device> deviceTblOperation = new(connection);

        // ToDo Verify the Logic for TaregtParentDevices
        // If TargetParenDevices grab everything otherwhise only get PMUs that are not concentrators
        m_deviceMetadata = deviceDataSet.Tables[0].Rows.Cast<DataRow>().Select(row => deviceTblOperation.LoadRecord(row)).Where((d) => !TargetParentDevices || !d.IsConcentrator).ToDictionary(item => item.ID);
        ILookup<string, int> deviceIDLookup = deviceDataSet.Tables[0].Rows.Cast<DataRow>().ToLookup((row) => row["Acronym"].ToString(), (row) => int.Parse(row["ID"].ToString()));


        // Set input measurement keys for measurement routing

        ILookup<Guid, int> deviceLookup = DataSource.Tables["ActiveMeasurements"].Select()
               .ToLookup(row => row.ConvertField<Guid>("SignalID"), row => deviceIDLookup[(row.ConvertField<string?>("Device") ?? row.ConvertField<string?>("PointTag"))].FirstOrDefault());

        // Setup Rules
        foreach (DeviceState state in m_deviceStates)
        {
            if (string.IsNullOrWhiteSpace(state.Rules))
                continue;

            List<StateRule> existingRule = new();

            if (!m_stateRules.TryGetValue(state.ID, out existingRule))
                existingRule = new();

            StateRule[] rules = state.Rules
                .ParseKeyValuePairs()
                .OrderBy(kvp => kvp.Key)
                .Select(kvp => kvp.Value)
                .Select(definition => new StateRule(definition))
                .ToArray();

            int i = 0;

            foreach (StateRule rule in rules)
            {
                MeasurementKey[] ruleMeasurements = new MeasurementKey[0];

                if (!string.IsNullOrWhiteSpace(rule.Query?.ToString()))
                    ruleMeasurements = ParseInputMeasurementKeys(DataSource, true, "Filter ActiveMeasurements WHERE " + rule.Query?.ToString() ?? "");

                rule.MeasurementKeys = ruleMeasurements.GroupBy(meas => deviceLookup[meas.SignalID].First()).ToDictionary(group => group.Key, group => group.ToArray());

                if (existingRule.Count < (i + 1))
                    existingRule.Add(rule); 
                else
                {
                    //Its an existing Rule
                    rule.LastUpdateTime = existingRule[i].LastUpdateTime;
                    existingRule[i] = rule;
                }
                i++;
            }
            existingRule = existingRule.Take(i).ToList();
            stateRules.Add(state.ID, existingRule);
        }

        m_stateRules = stateRules;
        
        HashSet<MeasurementKey> keys = [.. m_stateRules.SelectMany(item => item.Value.SelectMany(rule => rule.MeasurementKeys.Values.SelectMany(x => x)))];
        InputMeasurementKeys = keys.ToArray();

        // Make sure all Devices have an alarmstate. if not we need to add one
        TableOperations<DeviceStatus> deviceStatusTblOperation = new(connection);

        foreach (Device device in m_deviceMetadata.Values)
        {
            if (deviceStatusTblOperation.QueryRecordCountWhere("DeviceID = {0}", device.ID) == 0)
            {
                DeviceStatus deviceStatus = new()
                {
                    DeviceID = device.ID,
                    StateID = m_requiredStateIDs.GetValueOrDefault(AlarmState.NotAvailable),
                    TimeStamp = DateTime.UtcNow,
                    DisplayData = ""
                };
                deviceStatusTblOperation.AddNewRecord(deviceStatus);
            }
          
        }

        return !addedState;

    }
    #endregion

    #region [ Static ]

    private static readonly string[] s_shortTimeNames = [" yr", " yr", " d", " d", " hr", " hr", " m", " m", " s", " s", "< "];
    private static readonly double s_daysPerYear = new Time(Time.SecondsPerYear(DateTime.UtcNow.Year)).ToDays();

    private static string GetOutOfServiceTime(DataRow deviceRow)
    {
        if (deviceRow is null)
            return "U/A";

        try
        {
            return GetShortElapsedTimeString(DateTime.UtcNow.Ticks - Convert.ToDateTime(deviceRow["UpdatedOn"]).Ticks);
        }
        catch
        {
            return "U/A";
        }
    }

    /// <summary>
    /// Get short elapsed time string for specified <paramref name="span"/>.
    /// </summary>
    /// <param name="span"><see cref="Ticks"/> representing time span.</param>
    /// <returns>Short elapsed time string.</returns>
    public static string GetShortElapsedTimeString(Ticks span)
    {
        double days = span.ToDays();

        if (days > s_daysPerYear)
            return $"{days / s_daysPerYear:N2} yrs";

        if (days > 1.0D)
            span = span.BaselinedTimestamp(BaselineTimeInterval.Day);
        else if (span.ToHours() > 1.0D)
            span = span.BaselinedTimestamp(BaselineTimeInterval.Hour);
        else if (span.ToMinutes() > 1.0D)
            span = span.BaselinedTimestamp(BaselineTimeInterval.Minute);
        else if (span.ToSeconds() > 1.0D)
            span = span.BaselinedTimestamp(BaselineTimeInterval.Second);
        else
            return "0";

        string elapsedTimeString = span.ToElapsedTimeString(0, s_shortTimeNames);

        if (elapsedTimeString.Length > 10)
            elapsedTimeString = elapsedTimeString.Substring(0, 10);

        return elapsedTimeString;
    }

    /// <summary>
    /// The default (most recently created) instance of the Device State adapter.
    /// </summary>
    public static DeviceStateAdapter? Default { get; private set; }

    #endregion
}