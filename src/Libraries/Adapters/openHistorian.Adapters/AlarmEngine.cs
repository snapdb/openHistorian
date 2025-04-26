
//******************************************************************************************************
//  AlarmEngine.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  01/31/2012 - Stephen C. Wills
//       Generated original version of source code.
//  12/13/2012 - Starlynn Danyelle Gilliam
//       Modified Header.
//  02/02/2021 - C. Lackner
//       Added Settings for Alarm Retention
//  02/21/2025 - C. Lackner
//       Redesigned for .NET Core
//
//******************************************************************************************************
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable CheckNamespace

using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Text;
using Gemstone;
using Gemstone.ActionExtensions;
using Gemstone.ComponentModel.DataAnnotations;
using Gemstone.Data;
using Gemstone.Data.DataExtensions;
using Gemstone.Data.Model;
using Gemstone.Diagnostics;
using Gemstone.EnumExtensions;
using Gemstone.IO.Collections;
using Gemstone.IO.Parsing;
using Gemstone.StringExtensions;
using Gemstone.Threading.SynchronizedOperations;
using Gemstone.Timeseries;
using Gemstone.Timeseries.Adapters;
using Gemstone.Timeseries.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ConfigSettings = Gemstone.Configuration.Settings;

// TODO: Move to the data quality monitoring assembly
namespace DataQualityMonitoring;

/// <summary>
/// Action adapter that generates alarm measurements based on configured alarm definitions in a database.
/// </summary>
[Description("Alarm Engine: Manages Alarms defined in the database")]
public class AlarmEngine : FacileActionAdapterBase
{
    #region [ Members ]

    // Nested Types
    private sealed class AlarmProcessor : IDisposable
    {
        public required FrameQueue FrameQueue;
        public required Alarm Alarm;
        public required double LagTime;
        public Ticks LastProcessed;
        public int ExpectedMeasurements;
        private bool m_disposed;

        public void AssignMeasurement(IMeasurement measurement)
        {
            TrackingFrame? frame = FrameQueue.GetFrame(measurement.Timestamp);
            
            if (frame is null)
                return; // This means a measurement came in too late.

            IFrame sourceFrame = frame.SourceFrame;

            // Access published flag within critical section to ensure no updates will
            // be made to frame while it is being published
            frame.Lock.EnterReadLock();

            try
            {
                if (sourceFrame.Published)
                    return;

                // Assign derived measurement to its source frame using user customizable function.
                sourceFrame.Measurements[measurement.Key] = measurement;
                sourceFrame.LastSortedMeasurement = measurement;
            }
            finally
            {
                frame.Lock.ExitReadLock();
            }
        }

        /// <summary>
        /// Releases all the resources used by the <see cref="AlarmProcessor"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="AlarmProcessor"/> object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (m_disposed)
                return;

            try
            {
                if (!disposing)
                    return;

                FrameQueue.Dispose();
            }
            finally
            {
                m_disposed = true;  // Prevent duplicate dispose.
            }
        }

        public bool Test(Ticks realTime)
        {
            // Get top frame
            TrackingFrame? frame = FrameQueue.Head;

            // If no frame is ready to publish, exit
            if (frame is null)
                return false;

            IFrame sourceFrame = frame.SourceFrame;
            Ticks timestamp = sourceFrame.Timestamp;

            if (LagTime * Ticks.PerSecond - (realTime - timestamp) > 0.0D)
            {
                // It's not the scheduled time to publish this frame, however, if preemptive publishing is enabled,
                // an expected number of measurements per-frame have been defined and the frame has received its
                // expected number of measurements, we can go ahead and publish the frame ahead of schedule. This
                // is useful if the lag time is high to ensure no data is missed, but it's desirable to publish
                // the frame as soon as the expected data has arrived.
                if (ExpectedMeasurements < 1 || sourceFrame.SortedMeasurements < ExpectedMeasurements)
                    return false;
            }

            frame.Lock.EnterWriteLock();

            try
            {
                sourceFrame.Published = true;
            }
            finally
            {
                frame.Lock.ExitWriteLock();
            }

            bool test;

            try
            {
                test = Alarm.Test(sourceFrame);
            }
            finally
            {
                // Remove the frame from the queue whether it is successfully published or not
                FrameQueue.Pop();
            }

            return test;
        }
    }

    private sealed class AlarmEvent : ISupportStreamSerialization<AlarmEvent>
    {
        public Guid ID;
        public DateTime StartTime;
        public DateTime? EndTime;
        public string SignalTag = default!;
        public AlarmSeverity Severity;
        public AlarmOperation Operation;
        public double SetPoint;
        public int AlarmID;
        public Guid MeasurementID;

        /// <summary>
        /// Deserializes the <see cref="AlarmEvent"/> from a <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">Source stream.</param>
        /// <returns>New deserialized instance.</returns>
        public static AlarmEvent ReadFrom(Stream stream)
        {
            BinaryReader reader = new(stream, Encoding.UTF8, true);

            return new AlarmEvent
            {
                ID = new Guid(reader.ReadBytes(16)),
                StartTime = new DateTime(reader.ReadInt64(), DateTimeKind.Utc),
                EndTime = reader.ReadBoolean() ? new DateTime(reader.ReadInt64(), DateTimeKind.Utc) : null,
                SignalTag = reader.ReadString(),
                Severity = (AlarmSeverity)reader.ReadInt32(),
                Operation = (AlarmOperation)reader.ReadInt32(),
                SetPoint = reader.ReadDouble(),
                AlarmID = reader.ReadInt32(),
                MeasurementID = new Guid(reader.ReadBytes(16))
            };
        }

        /// <summary>
        /// Serializes the <see cref="AlarmEvent"/> to a <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">Target stream.</param>
        /// <param name="instance"><see cref="AlarmEvent"/> to serialize.</param>
        public static void WriteTo(Stream stream, AlarmEvent instance)
        {
            BinaryWriter writer = new(stream, Encoding.UTF8, true);

            writer.Write(instance.ID.ToByteArray());
            writer.Write(instance.StartTime.Ticks);
            writer.Write(instance.EndTime.HasValue);

            if (instance.EndTime.HasValue)
                writer.Write(instance.EndTime.Value.Ticks);
            
            writer.Write(instance.SignalTag);
            writer.Write((int)instance.Severity);
            writer.Write((int)instance.Operation);
            writer.Write(instance.SetPoint);
            writer.Write(instance.AlarmID);
            writer.Write(instance.MeasurementID.ToByteArray());
        }
    }

    // Constants
    private const int UpToDate = 0;
    private const int Modified = 1;
    private const double DefaultAlarmRetention = 24.0D;

    // Fields
    private readonly object m_alarmLock;

    private readonly ConcurrentQueue<IMeasurement> m_measurementQueue;
    private readonly ConcurrentQueue<AlarmEvent> m_eventDetailsQueue;
    private readonly TaskSynchronizedOperation m_eventDetailsOperation;
    private readonly TaskSynchronizedOperation m_processMeasurementsOperation;
    private Func<bool>? m_cancelMeasurementProcessingDelay;

    private DataSet? m_alarmDataSet;
    private int m_dataSourceState;

    private Dictionary<Guid, List<int>> m_measurementAlarmMap;
    private Dictionary<int, AlarmProcessor> m_alarmProcessors;

    private const string FileBackedDictionaryPath = "./ActiveAlarmEvents.bin";

    private long m_eventCount;

    private bool m_supportsTemporalProcessing;
    private bool m_disposed;
    private bool m_disposing;

    #endregion

    #region [ Constructors ]

    /// <summary>
    /// Creates a new instance of the <see cref="AlarmEngine"/> class.
    /// </summary>
    public AlarmEngine()
    {
        m_alarmLock = new object();
        m_alarmProcessors = new Dictionary<int, AlarmProcessor>();
        m_measurementAlarmMap = new Dictionary<Guid, List<int>>();

        m_measurementQueue = new ConcurrentQueue<IMeasurement>();
        m_processMeasurementsOperation = new TaskSynchronizedOperation(ProcessMeasurements, ex => OnProcessException(MessageLevel.Warning, ex));

        m_eventDetailsQueue = new ConcurrentQueue<AlarmEvent>();
        m_eventDetailsOperation = new TaskSynchronizedOperation(ProcessAlarmEvents, ex => OnProcessException(MessageLevel.Warning, ex));

        Default = this;
    }

    #endregion

    #region [ Properties ]

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
                m_processMeasurementsOperation.RunAsync();
        }
    }

    /// <summary>
    /// Gets the flag indicating if this adapter supports temporal processing.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Define the flag indicating if this adapter supports temporal processing.")]
    [DefaultValue(false)]
    [Label("Supports Temporal Processing")]
    public override bool SupportsTemporalProcessing => m_supportsTemporalProcessing;

    /// <summary>
    /// Gets or sets the amount of time, in hours, an alarm change will be kept n the database.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Define the amount of time, in hours, the alarms will be kept in the AlarmTable.")]
    [DefaultValue(DefaultAlarmRetention)]
    [Label("Default Alarm Retention")]
    public double AlarmRetention { get; set; }

    /// <inheritdoc/>
    public override string Status
    {
        get
        {
            StringBuilder status = new();

            status.Append(base.Status);
            status.AppendLine($"           Alarm retention: {AlarmRetention:N0} hours");

            return status.ToString();
        }
    }

    #endregion

    #region [ Methods ]

    /// <summary>
    /// Initializes the <see cref="AlarmEngine"/>.
    /// </summary>
    public override void Initialize()
    {
        // Run base class initialization
        base.Initialize();

        Dictionary<string, string> settings = Settings;

        // Load optional parameters
        m_supportsTemporalProcessing = settings.TryGetValue(nameof(SupportsTemporalProcessing), out string? setting) && setting.ParseBoolean();

        if (settings.TryGetValue(nameof(AlarmRetention), out setting) && double.TryParse(setting, out double alarmRetention))
            AlarmRetention = alarmRetention;
        else
            AlarmRetention = DefaultAlarmRetention;

        // Run the process measurements operation to ensure that the alarm configuration is up-to-date
        if (Interlocked.CompareExchange(ref m_dataSourceState, Modified, Modified) == Modified)
            m_processMeasurementsOperation.RunAsync();
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="AlarmEngine"/> object and optionally releases the managed resources.
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

            // Force alarm database processing before shutdown, if not already running
            if (!m_eventDetailsOperation.IsRunning)
                ProcessAlarmEvents().Wait();

            lock (m_alarmLock)
            {
                foreach (AlarmProcessor alarmProcessor in m_alarmProcessors.Values)
                    alarmProcessor.Dispose();
            }
        }
        finally
        {
            m_disposed = true;          // Prevent duplicate dispose.
            base.Dispose(disposing);    // Call base class Dispose().
        }
    }

    /// <summary>
    /// Starts the <see cref="AlarmEngine"/>, or restarts it if it is already running.
    /// </summary>
    public override void Start()
    {
        base.Start();
        Interlocked.Exchange(ref m_eventCount, 0L);
    }

    /// <summary>
    /// Queues a collection of measurements for processing.
    /// </summary>
    /// <param name="measurements">Measurements to queue for processing.</param>
    public override void QueueMeasurementsForProcessing(IEnumerable<IMeasurement> measurements)
    {
        foreach (IMeasurement measurement in measurements)
        {
            //Per Ritchie this should not happen but somehow happens anyway
            // #ToDo Figure out why
            if (measurement is null)
                continue;
            m_measurementQueue.Enqueue(measurement);
        }


        m_processMeasurementsOperation.RunAsync();
    }

    /// <summary>
    /// Gets a short one-line status of this <see cref="AdapterBase"/>.
    /// </summary>
    /// <param name="maxLength">Maximum number of available characters for display.</param>
    /// <returns>A short one-line summary of the current status of this <see cref="AdapterBase"/>.</returns>
    public override string GetShortStatus(int maxLength)
    {
        return $"{Interlocked.Read(ref m_eventCount):N0} events processed since last start".CenterText(maxLength);
    }

    /// <summary>
    /// Gets a collection containing all the raised alarms in the system.
    /// </summary>
    /// <returns>A collection containing all the raised alarms.</returns>
    public IActionResult GetRaisedAlarms(ControllerBase controller, IEnumerable<AlarmSeverity>? severities = null)
    {
        lock (m_alarmLock)
        {
            var raisedAlarms = m_alarmProcessors.Values
                .Where(ap => ap.Alarm.State == AlarmState.Raised &&
                             (severities is null || severities.Contains(ap.Alarm.Severity)))
                .Select(ap => ap.Alarm.Clone());
            return controller.Ok(raisedAlarms);
        }
    }

    // Dequeues measurements from the measurement queue and processes them.
    private async Task ProcessMeasurements()
    {
        if (m_disposing)
            return;

        // Cancel any pending measurement processing delay
        Interlocked.Exchange(ref m_cancelMeasurementProcessingDelay, null)?.Invoke();

        Ticks measurementsReceived = DateTime.UtcNow.Ticks;

        // Get the current state of the data source
        int dataSourceState = Interlocked.CompareExchange(ref m_dataSourceState, Modified, Modified);

        // If the data source has been modified, elevate the synchronized operation
        // to a dedicated thread and ensure that it runs at least one more time
        if (dataSourceState == Modified)
        {
            await UpdateAlarmDefinitions();
            Interlocked.Exchange(ref m_dataSourceState, UpToDate);
        }

        // Attempt to dequeue measurements from the measurement queue and,
        // if there are measurements left in the queue, ensure that the
        // process measurements operation runs again
        IList<IMeasurement> measurements = new List<IMeasurement>();

        while (m_measurementQueue.TryDequeue(out IMeasurement? measurement))
            measurements.Add(measurement);

        ProcessSingleMeasurementAlarms(measurements, measurementsReceived);

        // Increment total count of processed measurements
        IncrementProcessedMeasurements(measurements.Count);

        lock (m_alarmLock)
        {
            // Determine next time this Action needs to run to handle any alarms that timed out.
            Ticks[] timeouts = m_alarmProcessors.Values
                .Where(alarmProcessor => alarmProcessor.Alarm is { State: AlarmState.Raised, Timeout: > 0 })
                .Select(alarmProcessor => alarmProcessor.LastProcessed + (long)(alarmProcessor.Alarm.Timeout * Ticks.PerSecond))
                .ToArray();

            if (timeouts.Length > 0)
            {
                Interlocked.Exchange(ref m_cancelMeasurementProcessingDelay, 
                    new Action(m_processMeasurementsOperation.RunAsync)
                        .DelayAndExecute((int)(timeouts.Min() - measurementsReceived).ToMilliseconds()));
            }
        }
    }


    // Dequeue and write alarm Events
    private async Task ProcessAlarmEvents()
    {
        IList<AlarmEvent> alarmEvents = new List<AlarmEvent>();
        StringBuilder deleteQuery = new("DELETE FROM EventDetails WHERE ");
        List<object> deleteParameters = [];

        // Attempt to dequeue Alarm Events from the queue and,
        // if there are events left in the queue, ensure that the
        // process Alarm Events operation runs again
        while (m_eventDetailsQueue.TryDequeue(out AlarmEvent? alarmEvent))
            alarmEvents.Add(alarmEvent);

        // If items were successfully dequeued, process them;
        await using AdoDataConnection connection = new(ConfigSettings.Instance);
        TableOperations<EventDetails> tableOperations = new(connection);
        int paramIndex = 0;

        foreach (AlarmEvent alarmEvent in alarmEvents)
        {
            if (deleteParameters.Count > 0)
                deleteQuery.Append(" OR ");

            deleteQuery.Append($"(MeasurementID = {{{paramIndex++}}} AND EndTime < {{{paramIndex++}}})");
            deleteParameters.Add(alarmEvent.MeasurementID);
            deleteParameters.Add(alarmEvent.StartTime.AddHours(-AlarmRetention));

            EventDetails? currentRecord = tableOperations.QueryRecordWhere("EventGuid = {0}", alarmEvent.ID);
            EventDetails updatedRecord = GenerateAlarmDetails(alarmEvent);

            if (currentRecord is not null)
                updatedRecord.ID = currentRecord.ID;

            tableOperations.AddNewOrUpdateRecord(updatedRecord);
        }

        if (alarmEvents.Count > 0)
            connection.ExecuteNonQuery(deleteQuery.ToString(), deleteParameters.ToArray());
    }

    // Updates alarm definitions when the data source has been updated.
    private Task UpdateAlarmDefinitions()
    {
        return Task.Factory.StartNew(() =>
        {
            DataSet? dataSource = DataSource;

            if (dataSource is null)
            {
                OnStatusMessage(MessageLevel.Warning, "Alarm definition update skipped, no data source is available.");
                return;
            }

            DateTime now = DateTime.UtcNow;

            // Get the latest version of the table of defined alarms
            DataSet alarmDataSet = new();
            alarmDataSet.Tables.Add(dataSource.Tables["Alarms"]!.Copy());

            // Compare the latest alarm table with the previous
            // alarm table to determine if anything needs to be done
            if (DataSetEqualityComparer.Default.Equals(m_alarmDataSet, alarmDataSet))
                return;

            m_alarmDataSet = alarmDataSet;

            // Get list of alarms defined in the latest version of the alarm table
            Dictionary<int, Alarm> alarms = alarmDataSet.Tables[0].Rows.Cast<DataRow>()
                .Where(row => row.ConvertField<bool>("Enabled"))
                .Select(CreateAlarm)
                .ToDictionary(alarm => alarm.ID);

            // Create a list to store alarm events generated by this process
            List<IMeasurement> alarmEvents = [];

            lock (m_alarmLock)
            {
                foreach (Alarm existingAlarm in m_alarmProcessors.Values.Select(alarmProcessor => alarmProcessor.Alarm))
                {
                    // Attempt to locate the defined alarm corresponding to the existing alarm
                    alarms.TryGetValue(existingAlarm.ID, out Alarm? definedAlarm);

                    // Determine if a change to the alarm's
                    // configuration has changed the alarm's behavior
                    if (BehaviorChanged(existingAlarm, definedAlarm))
                    {
                        if (existingAlarm.State != AlarmState.Raised)
                            continue;

                        existingAlarm.State = AlarmState.Cleared;

                        // Remove the alarm from the active alarm list
                        using FileBackedDictionary<int, AlarmEvent> activeAlarms = new(FileBackedDictionaryPath);

                        IMeasurement? alarmEvent = CreateAlarmEvent(now, existingAlarm, activeAlarms);

                        if (alarmEvent is not null)
                            alarmEvents.Add(alarmEvent);
                        else
                            OnStatusMessage(MessageLevel.Error, $"@{nameof(UpdateAlarmDefinitions)}: Alarm ID {existingAlarm.ID:N0} is in an invalid state: {existingAlarm.State}");
                    }
                    else if (definedAlarm is not null)
                    {
                        // Update functionally irrelevant configuration info
                        existingAlarm.TagName = definedAlarm.TagName;
                        existingAlarm.Description = definedAlarm.Description;

                        // Use the existing alarm since the alarm is functionally the same
                        alarms[definedAlarm.ID] = existingAlarm;
                    }
                }

                // Create the new alarm lookup to replace the old one
                Dictionary<int, AlarmProcessor> alarmProcessors = alarms.Values.ToDictionary(definedAlarm => definedAlarm.ID, definedAlarm =>
                {
                    if (m_alarmProcessors.TryGetValue(definedAlarm.ID, out AlarmProcessor? existingAlarmProcessor))
                    {
                        existingAlarmProcessor.Alarm = definedAlarm;
                        existingAlarmProcessor.ExpectedMeasurements = ParseInputMeasurementKeys(DataSource, true, definedAlarm.InputMeasurementKeys).Length;
                        return existingAlarmProcessor;
                    }

                    AlarmState initialState = AlarmState.Cleared;

                    // Any lingering alarms need to be reset to the raised state, so they get closed out properly
                    using (FileBackedDictionary<int, AlarmEvent> activeAlarms = new(FileBackedDictionaryPath))
                    {
                        if (activeAlarms.ContainsKey(definedAlarm.ID))
                            initialState = AlarmState.Raised;
                    }

                    definedAlarm.State = initialState;

                    return new AlarmProcessor
                    {
                        Alarm = definedAlarm,
                        FrameQueue = new FrameQueue(timestamp => new Frame(timestamp))
                        {
                            FramesPerSecond = FramesPerSecond > 0 ? FramesPerSecond : 30,
                            TimeResolution = 0,
                            RoundToNearestTimestamp = false,
                            DownsamplingMethod = DownsamplingMethod.Closest
                        },
                        LastProcessed = now,
                        ExpectedMeasurements = definedAlarm.InputMeasurementKeys.Length,
                        LagTime = LagTime
                    };
                });

                Dictionary<Guid, List<int>> measurementAlarmMap = alarms.Values.SelectMany(alarm => ParseInputMeasurementKeys(DataSource, true, alarm.InputMeasurementKeys)
                    .Select(measurementKey => (signalID: measurementKey.SignalID, alarmID: alarm.ID)))
                    .GroupBy(measurement => measurement.signalID)
                    .ToDictionary(grouping => grouping.Key, grouping => grouping.Select(measurement => measurement.alarmID).ToList());

                // Since alarm processors are being reused, we do not dispose of them here
                m_alarmProcessors = alarmProcessors;
                m_measurementAlarmMap = measurementAlarmMap;

                // Only automatically update input measurement keys if the setting is not explicitly defined
                if (m_measurementAlarmMap.Count <= 0 || Settings.ContainsKey("inputMeasurementKeys"))
                    return;
                
                // Generate filter expression for input measurements
                string filterExpression = string.Join(";", m_measurementAlarmMap.Select(kvp => kvp.Key.ToString()));

                // Set input measurement keys for measurement routing
                InputMeasurementKeys = ParseInputMeasurementKeys(DataSource, true, filterExpression);
            }

            // Publish new alarm measurements
            if (alarmEvents.Count > 0)
                OnNewMeasurements(alarmEvents);
        });
    }

    /// <summary>
    /// Process any Alarms that rely on a single measurement only.
    /// </summary>
    /// <param name="measurements">Source measurements.</param>
    /// <param name="measurementsReceived">Timestamp of when the measurements were received.</param>
    private void ProcessSingleMeasurementAlarms(IEnumerable<IMeasurement> measurements, Ticks measurementsReceived)
    {
        List<IMeasurement> alarmMeasurements = [];

        using (FileBackedDictionary<int, AlarmEvent> activeAlarms = new(FileBackedDictionaryPath))
        {
            AlarmProcessor? alarmProcessor;

            // Change status on any alarms that timed out.
            foreach ((int alarmID, _) in activeAlarms)
            {
                lock (m_alarmLock)
                {
                    if (!m_alarmProcessors.TryGetValue(alarmID, out alarmProcessor))
                    {
                        OnStatusMessage(MessageLevel.Error, $"@{nameof(ProcessSingleMeasurementAlarms)}: Alarm ID {alarmID:N0} no longer exists but is still active. Throwing away active alarm details.");
                        continue;
                    }

                    if (alarmProcessor.Alarm.Timeout <= 0 || !(alarmProcessor.LastProcessed + alarmProcessor.Alarm.Timeout * Ticks.PerSecond < measurementsReceived))
                        continue;
                    
                    alarmProcessor.Alarm.State = AlarmState.Cleared;

                    IMeasurement? alarmEvent = CreateAlarmEvent(measurementsReceived, alarmProcessor.Alarm, activeAlarms);

                    if (alarmEvent is not null)
                        alarmMeasurements.Add(alarmEvent);
                    else
                        OnStatusMessage(MessageLevel.Error, $"@{nameof(ProcessSingleMeasurementAlarms)}: Alarm ID {alarmID:N0} is in an invalid state: {alarmProcessor.Alarm.State}");
                }
            }

            // Process each Measurement
            foreach (IMeasurement measurement in measurements)
            {
                lock (m_alarmLock)
                {
                    // Get alarms that apply to the measurement being processed
                    if (!m_measurementAlarmMap.TryGetValue(measurement.ID, out List<int>? alarmIDs))
                        continue;

                    foreach (int alarmID in alarmIDs)
                    {
                        if (!m_alarmProcessors.TryGetValue(alarmID, out alarmProcessor))
                        {
                            OnStatusMessage(MessageLevel.Error, $"@{nameof(ProcessSingleMeasurementAlarms)}: Alarm ID {alarmID:N0} no longer exists but is still receiving measurements. Discarding active alarm details.");
                            continue;
                        }

                        alarmProcessor.LastProcessed = measurementsReceived;

                        // Test each alarm to determine whether their states have changed

                        // Build the Frame
                        alarmProcessor.AssignMeasurement(measurement);

                        // Test the Alarm
                        if (!alarmProcessor.Test(RealTime))
                            continue;

                        IMeasurement? alarmEvent = CreateAlarmEvent(measurementsReceived, alarmProcessor.Alarm, activeAlarms);

                        if (alarmEvent is not null)
                            alarmMeasurements.Add(alarmEvent);
                        else
                            OnStatusMessage(MessageLevel.Error, $"@ {nameof(ProcessSingleMeasurementAlarms)}: Alarm ID  {alarmID:N0}  is in an invalid state");
                    }
                }
            }
        }

        // Update alarm history by sending new alarm events into the system
        if (alarmMeasurements.Count > 0)
            OnNewMeasurements(alarmMeasurements);
    }

    private IMeasurement? CreateAlarmEvent(Ticks timestamp, Alarm alarm, FileBackedDictionary<int, AlarmEvent> activeAlarms)
    {
        IMeasurement? measurement = null;
        AlarmEvent? alarmEvent = null;

        switch (alarm.State)
        {
            case AlarmState.Raised when !activeAlarms.TryGetValue(alarm.ID, out alarmEvent):
                measurement = CreateAlarmRaisedEvent(timestamp, alarm, out alarmEvent);
                activeAlarms.Add(alarm.ID, alarmEvent);
                break;
            case AlarmState.Cleared when activeAlarms.TryGetValue(alarm.ID, out alarmEvent):
                measurement = CreateAlarmClearedEvent(timestamp, alarm, alarmEvent);
                activeAlarms.Remove(alarm.ID);
                break;
            default:
                Logger.SwallowException(new InvalidOperationException($"Alarm ID {alarm.ID:N0} is in an invalid state: {alarm.State}"), nameof(AlarmEngine), nameof(CreateAlarmEvent));
                break;
        }

        // Exiting with a null measurement is an indication of invalid state, i.e., alarm was not raised or cleared
        if (alarmEvent is null)
            return null;

        m_eventDetailsQueue.Enqueue(alarmEvent);

        if (!m_disposing)
            m_eventDetailsOperation.RunAsync();

        return measurement;

    }

    #endregion

    #region [ Static ]

    // Static Properties

    /// <summary>
    /// The default (most recently created) instance of the alarm adapter.
    /// </summary>
    public static AlarmEngine? Default { get; private set; }

    // Returns true if a change to the alarm's configuration also changed the alarm's behavior.
    private static bool BehaviorChanged(Alarm existingAlarm, Alarm? definedAlarm)
    {
        return definedAlarm is null ||
               existingAlarm.SignalID != definedAlarm.SignalID ||
               existingAlarm.InputMeasurementKeys != definedAlarm.InputMeasurementKeys ||
               existingAlarm.Operation != definedAlarm.Operation ||
               existingAlarm.SetPoint != definedAlarm.SetPoint ||
               existingAlarm.Tolerance != definedAlarm.Tolerance ||
               existingAlarm.Delay != definedAlarm.Delay ||
               existingAlarm.Hysteresis != definedAlarm.Hysteresis;
    }

    // Static Methods

    /// <summary>
    /// Gets the raised alarms from the default instance of the AlarmAdapter.
    /// </summary>
    /// <param name="controller">The controller from which to generate the IActionResult.</param>
    /// <returns>An IActionResult containing the raised alarms</returns>
    [AdapterCommand("Gets the Raised Alarms of the Default instance.", "Viewer", "Adminastrator", "Editor")]
    public static IActionResult GetRaisedAlarmsStatic(ControllerBase controller, string? severities = null)
    {
        IEnumerable<AlarmSeverity>? parsedSeverities = null;

        // If the severities string is provided, split it into parts and parse each value.
        if (!string.IsNullOrWhiteSpace(severities))
        {
            parsedSeverities = severities.Split(',')
                                         .Select(s => s.Trim())
                                         .Select(s => (AlarmSeverity)Enum.Parse(typeof(AlarmSeverity), s, true))
                                         .ToList();
        }

        if (Default is null)
            return controller.Ok(new List<Alarm>());

        return controller.Ok(Default.GetRaisedAlarms(controller, parsedSeverities));
    }

    // Creates an alarm using data defined in the database.
    private static Alarm CreateAlarm(DataRow row)
    {
        object? associatedMeasurementID = row.Field<object>("InputMeasurementKeys");

        return new Alarm((AlarmOperation)row.ConvertField<int>("Operation"))
        {
            ID = row.ConvertField<int>("ID"),
            TagName = row.Field<object>("TagName")?.ToString(),
            SignalID = Guid.Parse(row.Field<object>("SignalID")?.ToString() ?? Guid.Empty.ToString()),
            InputMeasurementKeys = associatedMeasurementID?.ToString() ?? string.Empty,
            Description = row.Field<object>("Description").ToNonNullString(),
            Severity = row.ConvertField<int>("Severity").GetEnumValueOrDefault<AlarmSeverity>(AlarmSeverity.None),
            SetPoint = row.ConvertNullableField<double>("SetPoint"),
            Tolerance = row.ConvertNullableField<double>("Tolerance"),
            Delay = row.ConvertNullableField<double>("Delay"),
            Hysteresis = row.ConvertNullableField<double>("Hysteresis"),
            State = AlarmState.Cleared
        };
    }

    // Creates an alarm event from the given alarm and measurement.
    private static IMeasurement CreateAlarmRaisedEvent(Ticks timestamp, Alarm alarm, out AlarmEvent alarmEvent)
    {
        alarmEvent = new AlarmEvent
        {
            ID = Guid.NewGuid(),
            StartTime = timestamp,
            EndTime = null,
            AlarmID = alarm.ID,
            MeasurementID = alarm.SignalID,
            Operation = alarm.Operation,
            SetPoint = alarm.SetPoint ?? 0,
            Severity = alarm.Severity,
            SignalTag = alarm.InputMeasurementKeys,
        };

        IMeasurement measurement = new AlarmMeasurement
        {
            Timestamp = timestamp,
            Value = (int)alarm.State,
            AlarmID = alarmEvent.ID
        };

        measurement.Metadata = MeasurementKey.LookUpBySignalID(alarm.SignalID).Metadata;

        return measurement;
    }

    private static IMeasurement CreateAlarmClearedEvent(Ticks timestamp, Alarm alarm, AlarmEvent alarmEvent)
    {
        alarmEvent.EndTime = timestamp;

        IMeasurement measurement = new AlarmMeasurement
        {
            Timestamp = timestamp,
            Value = (int)alarm.State,
            AlarmID = alarmEvent.ID
        };

        measurement.Metadata = MeasurementKey.LookUpBySignalID(alarm.SignalID).Metadata;

        return measurement;
    }

    private static EventDetails GenerateAlarmDetails(AlarmEvent alarmEvent)
    {
        return new EventDetails
        {
            StartTime = alarmEvent.StartTime,
            EndTime = alarmEvent.EndTime,
            EventGuid = alarmEvent.ID,
            Type = "alarm",
            MeasurementID = alarmEvent.MeasurementID,
            Details = JsonConvert.SerializeObject(new
            {
                alarmEvent.AlarmID,
                alarmEvent.SetPoint,
                alarmEvent.SignalTag,
                alarmEvent.Severity,
                alarmEvent.Operation
            })
        };
    }

    #endregion
}

