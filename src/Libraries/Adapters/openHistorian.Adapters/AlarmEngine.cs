
//******************************************************************************************************
//  AlarmAdapter.cs - Gbtc
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

using Gemstone;
using Gemstone.ActionExtensions;
using Gemstone.Collections.CollectionExtensions;
using Gemstone.Data;
using Gemstone.Data.DataExtensions;
using Gemstone.Data.Model;
using Gemstone.Diagnostics;
using Gemstone.EnumExtensions;
using Gemstone.IO.Collections;
using Gemstone.StringExtensions;
using Gemstone.Threading.SynchronizedOperations;
using Gemstone.Timeseries;
using Gemstone.Timeseries.Adapters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Parameters;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Text;


namespace DataQualityMonitoring;

/// <summary>
/// Action adapter that generates alarm measurements based on alarm definitions from the database.
/// </summary>
[Description("Alarm Engine: Manages Alarms defined in the database")]

public class AlarmAdapter : FacileActionAdapterBase
{
    #region [ Members ]

    // Nested Types
    private class AlarmProcessor
    {
        public FrameQueue frameQueue;
        public Alarm Alarm;
        public Gemstone.Ticks LastProcessed;
        public int ExpectedMeasurements;
        public int LagTime = 10;

        public void AssignMeasurement(IMeasurement measurement)
        {
            TrackingFrame? frame = frameQueue.GetFrame(measurement.Timestamp);
            if (frame is null)
            {
                // This means a measurement came in too late.
                return;
            }

            IFrame sourceFrame = frame.SourceFrame;

            // Access published flag within critical section to ensure no updates will
            // be made to frame while it is being published
            frame.Lock.EnterReadLock();
            try
            {
                if (!sourceFrame.Published)
                {
                    // Assign derived measurement to its source frame using user customizable function.
                    sourceFrame.Measurements[measurement.Key] = measurement;
                    sourceFrame.LastSortedMeasurement = measurement;

                }
            }
            finally
            {
                frame.Lock.ExitReadLock();
            }
        }

        public bool Test(Ticks RealTime)
        {
            // Get top frame
            TrackingFrame? frame = frameQueue!.Head;
            // If no frame is ready to publish, exit
            if (frame is null)
                return false;

            IFrame sourceFrame = frame.SourceFrame;
            Ticks timestamp = sourceFrame.Timestamp;

            if ((long)(LagTime * Ticks.PerSecond) - (RealTime - timestamp) > 0)
            {
                // It's not the scheduled time to publish this frame, however, if preemptive publishing is enabled,
                // an expected number of measurements per-frame have been defined and the frame has received this
                // expected number of measurements, we can go ahead and publish the frame ahead of schedule. This
                // is useful if the lag time is high to ensure no data is missed but it's desirable to publish the
                // frame as soon as the expected data has arrived.
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
                frameQueue!.Pop();
            }
            return test;
        }
    }

    private class AlarmEvent
    {
        public Guid Guid;
        public DateTime StartTime;
        public DateTime? EndTime;
        public string SignalTag;
        public AlarmSeverity Severity;
        public AlarmOperation Operation;
        public double Setpoint;
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
                Guid = new Guid(reader.ReadBytes(16)),
                StartTime = new DateTime(reader.ReadInt64()),
                EndTime = reader.ReadBoolean() ? new DateTime(reader.ReadInt64()) : null,
                SignalTag = reader.ReadString(),
                Severity = (AlarmSeverity)reader.ReadInt32(),
                Operation = (AlarmOperation)reader.ReadInt32(),
                Setpoint = reader.ReadDouble(),
                AlarmID = reader.ReadInt32(),
                MeasurementID = new Guid(reader.ReadBytes(16))
            };
        }

        /// <summary>
        /// Serializes the <see cref="AlarmEvent"/> to a <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">Target stream.</param>
        /// <param name="obj"><see cref="AlarmEvent"> to serialize.</param>
        public static void WriteTo(Stream stream, AlarmEvent instance)
        {
            BinaryWriter writer = new(stream, Encoding.UTF8, true);

            writer.Write(instance.Guid.ToByteArray());
            writer.Write(instance.StartTime.Ticks);
            writer.Write(instance.EndTime.HasValue);
            if (instance.EndTime.HasValue)
                writer.Write(instance.EndTime.Value.Ticks);
            writer.Write(instance.SignalTag);
            writer.Write((int)instance.Severity);
            writer.Write((int)instance.Operation);
            writer.Write(instance.Setpoint);
            writer.Write(instance.AlarmID);
            writer.Write(instance.MeasurementID.ToByteArray());
        }
    }

    // Constants
    private const int UpToDate = 0;
    private const int Modified = 1;
    private const int DefaultAlarmRetention = 24;


    // Fields
    private readonly object m_alarmLock;

    private readonly ConcurrentQueue<IMeasurement> m_measurementQueue;
    private readonly ConcurrentQueue<AlarmEvent> m_eventDetailsQueue;
    private readonly TaskSynchronizedOperation m_eventDetailsOperation;
    private readonly TaskSynchronizedOperation m_processMeasurementsOperation;

    private CancellationTokenSource m_timerCancelationTokenSource;

    private DataSet m_alarmDataSet;
    private int m_dataSourceState;

    private Dictionary<Guid, List<int>> m_measurementAlarmMapping;
    private Dictionary<int, AlarmProcessor> m_alarmLookup;

    private const string FilebackedDictionartPath = "./ActiveAlarmEvents.bin";

    private long m_eventCount;

    private bool m_supportsTemporalProcessing;
    private bool m_disposed;
    private int m_alarmRetention;


    #endregion

    #region [ Constructors ]

    /// <summary>
    /// Creates a new instance of the <see cref="AlarmAdapter"/> class.
    /// </summary>
    public AlarmAdapter()
    {
        m_alarmLock = new object();
        m_alarmLookup = new Dictionary<int, AlarmProcessor>();
        m_measurementAlarmMapping = new Dictionary<Guid, List<int>>();

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
        get
        {
            return base.DataSource;
        }
        set
        {
            base.DataSource = value;

            if (Interlocked.CompareExchange(ref m_dataSourceState, Modified, UpToDate) == UpToDate && Initialized)
            {
                m_processMeasurementsOperation.RunAsync();
            }
        }
    }

    /// <summary>
    /// Gets the flag indicating if this adapter supports temporal processing.
    /// </summary>
    [ConnectionStringParameter,
    Description("Define the flag indicating if this adapter supports temporal processing."),
    DefaultValue(false)]
    public override bool SupportsTemporalProcessing => m_supportsTemporalProcessing;

    /// <summary>
    /// Returns the detailed status of the data input source.
    /// </summary>
    public override string Status
    {
        get
        {
            StringBuilder statusBuilder = new StringBuilder(base.Status);

            return statusBuilder.ToString();
        }
    }

    /// <summary>
    /// Gets or sets the amount of time, in hours, an Alarm change will be kept n the database.
    /// </summary>
    [ConnectionStringParameter,
    Description("Define the amount of time, in hour, the alarms will be kept in the AlarmTable."),
    DefaultValue(DefaultAlarmRetention)]
    public int AlarmRetention
    {
        get
        {
            return m_alarmRetention;
        }
        set
        {
            m_alarmRetention = value;
        }
    }
    #endregion

    #region [ Methods ]

    /// <summary>
    /// Initializes the <see cref="AlarmAdapter"/>.
    /// </summary>
    public override void Initialize()
    {
        Dictionary<string, string> settings;
        string setting;

        // Run base class initialization
        base.Initialize();
        settings = Settings;

        // Load optional parameters
        if (settings.TryGetValue("supportsTemporalProcessing", out setting))
            m_supportsTemporalProcessing = setting.ParseBoolean();
        else
            m_supportsTemporalProcessing = false;


        if (!settings.TryGetValue("alarmRetention", out setting) || !int.TryParse(setting, out m_alarmRetention))
            m_alarmRetention = DefaultAlarmRetention;

        m_timerCancelationTokenSource = new CancellationTokenSource();

        // Run the process measurements operation to ensure that the alarm configuration is up to date
        if (Interlocked.CompareExchange(ref m_dataSourceState, Modified, Modified) == Modified)
        {
            m_processMeasurementsOperation.RunAsync();
        }

    }

    /// <summary>
    /// Starts the <see cref="AlarmAdapter"/>, or restarts it if it is already running.
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
            if (measurement is not null)
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
        return $"{Interlocked.Read(ref m_eventCount)} events processed since last start".CenterText(maxLength);
    }

    /// <summary>
    /// Gets a collection containing all the raised alarms in the system.
    /// </summary>
    /// <returns>A collection containing all the raised alarms.</returns>
    [AdapterCommand("Gets a collection containing all the raised alarms in the system.", "Administrator", "Editor", "Viewer")]
    public IActionResult GetRaisedAlarms(ControllerBase controller)
    {
        lock (m_alarmLock)
            return controller.Ok(m_alarmLookup.Values.Where(alarm => alarm.Alarm.State == Gemstone.Timeseries.AlarmState.Raised)
                .Select(alarm => alarm.Alarm.Clone()));
    }


    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="AlarmAdapter"/> object and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (!m_disposed)
        {
            try
            {
                if (disposing)
                {
                    // #ToDO FIgure out how to dispose the FrameQueues
                    lock (m_alarmLock)
                    {
                        //foreach (SingleSignalAlarm signalAlarms in m_singleSignalAlarmLookup.Values)
                        //StatisticsEngine.Unregister(signalAlarms.Statistics);
                    }
                    m_eventDetailsOperation?.RunAsync();

                }
            }
            finally
            {
                m_disposed = true;          // Prevent duplicate dispose.
                base.Dispose(disposing);    // Call base class Dispose().
            }
        }
    }


    // Creates an alarm using data defined in the database.
    private Alarm CreateAlarm(DataRow row)
    {
        object associatedMeasurementId = row.Field<object>("InputMeasurementKeys");

        return new Alarm((AlarmOperation)row.ConvertField<int>("Operation"))
        {
            ID = row.ConvertField<int>("ID"),
            TagName = row.Field<object>("TagName").ToString(),
            SignalID = Guid.Parse(row.Field<object>("SignalID").ToString()),
            InputMeasurementKeys = ((object)associatedMeasurementId != null) ? associatedMeasurementId.ToString() : "",
            Description = row.Field<object>("Description").ToNonNullString(),
            Severity = row.ConvertField<int>("Severity").GetEnumValueOrDefault<AlarmSeverity>(AlarmSeverity.None),
            SetPoint = row.ConvertNullableField<double>("SetPoint"),
            Tolerance = row.ConvertNullableField<double>("Tolerance"),
            Delay = row.ConvertNullableField<double>("Delay"),
            Hysteresis = row.ConvertNullableField<double>("Hysteresis"),
            State = Gemstone.Timeseries.AlarmState.Cleared
        };
    }

    // Dequeues measurements from the measurement queue and processes them.
    private async Task ProcessMeasurements()
    {
        m_timerCancelationTokenSource.Cancel();
        IList<IMeasurement> measurements;
        int dataSourceState;
        Ticks measurementsRecieved = DateTime.UtcNow.Ticks;

        // Get the current state of the data source
        dataSourceState = Interlocked.CompareExchange(ref m_dataSourceState, Modified, Modified);

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
        measurements = new List<IMeasurement>();

        while (m_measurementQueue.TryDequeue(out IMeasurement measurement))
            measurements.Add(measurement);

        ProcessSingleMeasurementAlarms(measurements, measurementsRecieved);
        // Increment total count of processed measurements
        IncrementProcessedMeasurements(measurements.Count);

        lock (m_alarmLock)
        {
            // Determine next time this Action needs to run to time out any alarms that timed out.
            IEnumerable<Ticks> timeouts = m_alarmLookup.Values
                .Where(a => a.Alarm.State == Gemstone.Timeseries.AlarmState.Raised && a.Alarm.Timeout > 0)
                .Select(a => a.LastProcessed + (long)(a.Alarm.Timeout * Ticks.PerSecond));

            if (timeouts.Any())
            {
                Ticks nextTimeout = timeouts.Min();
                m_timerCancelationTokenSource = new CancellationTokenSource();
                Action timerElapsed = () => { m_processMeasurementsOperation.RunAsync(); };
                timerElapsed.DelayAndExecute((int)(nextTimeout - measurementsRecieved).ToMilliseconds(), m_timerCancelationTokenSource.Token);
            }
        }
    }

    //Dequeue and write Alarm Events
    private async Task ProcessAlarmEvents()
    {
        IList<AlarmEvent> events = new List<AlarmEvent>();
        StringBuilder deleteQuery = new StringBuilder();
        List<object> deleteParameters = new List<object>();
        deleteQuery.Append("DELETE FROM EventDetails WHERE ");
        bool includeOr = false;

        // Attempt to dequeue Alarm Events from the  queue and,
        // if there are events left in the queue, ensure that the
        // process Alarm Events operation runs again
        while (m_eventDetailsQueue.TryDequeue(out AlarmEvent evt))
            events.Add(evt);

        // If items were successfully dequeued, process them;
        using (AdoDataConnection connection = new AdoDataConnection(Gemstone.Configuration.Settings.Instance))
        {
            TableOperations<EventDetails> tableOperations = new TableOperations<EventDetails>(connection);
            foreach (AlarmEvent evt in events)
            {

                if (includeOr)
                    deleteQuery.Append(" OR ");
                else
                    includeOr = true;

                deleteQuery.Append("(MeasurementID = {0} AND EndTime < {1})");
                deleteParameters.Add(evt.MeasurementID);
                deleteParameters.Add(evt.StartTime.AddHours(-m_alarmRetention));

                EventDetails? currentRecord = tableOperations.QueryRecordWhere("EventGuid = {0}", evt.Guid);
                EventDetails updatedRecord = GenerateAlarmDetails(evt);
                if (currentRecord is not null)
                    updatedRecord.ID = currentRecord.ID;

                tableOperations.AddNewOrUpdateRecord(updatedRecord);
            }

            if (events.Count > 0)
            {
                connection.ExecuteNonQuery(deleteQuery.ToString(), deleteParameters.ToArray());
            }
        }

    }

    // Updates alarm definitions when the data source has been updated.
    private async Task UpdateAlarmDefinitions()
    {
        DateTime now = DateTime.UtcNow; ;
        DataSet alarmDataSet;

        Dictionary<int, Alarm> definedAlarmsLookup;

        Dictionary<int, AlarmProcessor> newAlarmLookup;
        Dictionary<Guid, List<int>> newMeasurementMap;
        List<IMeasurement> alarmEvents;

        // Get the latest version of the table of defined alarms
        alarmDataSet = new DataSet();
        alarmDataSet.Tables.Add(DataSource.Tables["Alarms"].Copy());

        // Compare the latest alarm table with the previous
        // alarm table to determine if anything needs to be done
        if (DataSetEqualityComparer.Default.Equals(m_alarmDataSet, alarmDataSet))
            return;

        m_alarmDataSet = alarmDataSet;

        // Get list of alarms defined in the latest version of the alarm table
        definedAlarmsLookup = alarmDataSet.Tables[0].Rows.Cast<DataRow>()
            .Where(row => row.ConvertField<bool>("Enabled"))
            .Select(CreateAlarm)
            .ToDictionary(alarm => alarm.ID);

        // Create a list to store alarm events generated by this process
        alarmEvents = new List<IMeasurement>();

        lock (m_alarmLock)
        {
            foreach (Alarm existingAlarm in m_alarmLookup.Values.Select(a => a.Alarm))
            {
                Alarm definedAlarm;

                // Attempt to locate the defined alarm corresponding to the existing alarm
                definedAlarmsLookup.TryGetValue(existingAlarm.ID, out definedAlarm);

                // Determine if a change to the alarm's
                // configuration has changed the alarm's behavior
                if (BehaviorChanged(existingAlarm, definedAlarm))
                {
                    if (existingAlarm.State == Gemstone.Timeseries.AlarmState.Raised)
                    {
                        existingAlarm.State = Gemstone.Timeseries.AlarmState.Cleared;
                        // Remove the alarm from the active alarm list
                        using (FileBackedDictionary<int, AlarmEvent> activeAlarms = new FileBackedDictionary<int, AlarmEvent>(FilebackedDictionartPath))
                        {
                            alarmEvents.Add(CreateAlarmEvent(now, existingAlarm, activeAlarms));
                        }
                    }

                }
                else if ((object)definedAlarm != null)
                {
                    // Update functionally irrelevant configuration info
                    existingAlarm.TagName = definedAlarm.TagName;
                    existingAlarm.Description = definedAlarm.Description;

                    // Use the existing alarm since the alarm is functionally the same
                    definedAlarmsLookup[definedAlarm.ID] = existingAlarm;
                }
            }

            // Create the new alarm lookup to replace the old one
            newAlarmLookup = definedAlarmsLookup.Values.ToDictionary(definedAlarm => definedAlarm.ID, definedAlarm =>
                {
                    if (m_alarmLookup.TryGetValue(definedAlarm.ID, out AlarmProcessor existingAlarmProcessor))
                    {
                        existingAlarmProcessor.Alarm = definedAlarm;
                        existingAlarmProcessor.ExpectedMeasurements = ParseInputMeasurementKeys(DataSource, true, definedAlarm.InputMeasurementKeys).Count();
                        return existingAlarmProcessor;
                    }
                    return new AlarmProcessor()
                    {
                        Alarm = definedAlarm,
                        frameQueue = new FrameQueue((Ticks timestamp) => new Frame(timestamp))
                        {
                            FramesPerSecond = FramesPerSecond > 0 ? FramesPerSecond : 30,
                            TimeResolution = 0,
                            RoundToNearestTimestamp = false,
                            DownsamplingMethod = DownsamplingMethod.Closest
                        },
                        LastProcessed = now,
                        ExpectedMeasurements = ParseInputMeasurementKeys(DataSource, true, definedAlarm.InputMeasurementKeys).Count()
                    };

                });

            newMeasurementMap = definedAlarmsLookup.Values.SelectMany((alarm) => ParseInputMeasurementKeys(DataSource, true, alarm.InputMeasurementKeys)
                .Select<MeasurementKey, Tuple<Guid, int>>((measurementKey) => new(measurementKey.SignalID, alarm.ID)))
                .GroupBy(measurement => measurement.Item1)
                .ToDictionary((grouping) => grouping.Key, (grouping) => grouping.Select(measurement => measurement.Item2).ToList());

            m_alarmLookup = newAlarmLookup;
            m_measurementAlarmMapping = newMeasurementMap;

            // Only automatically update input measurement keys if the setting is not explicitly defined
            if (m_measurementAlarmMapping.Count > 0 && !Settings.ContainsKey("inputMeasurementKeys"))
            {
                // Generate filter expression for input measurements
                string filterExpression = string.Join(";", m_measurementAlarmMapping.Select(kvp => kvp.Key.ToString()));
                // Set input measurement keys for measurement routing
                InputMeasurementKeys = ParseInputMeasurementKeys(DataSource, true, filterExpression);
            }
        }
    }

    /// <summary>
    /// Process any Alarms that rely on a single measurement only.
    /// </summary>
    /// <param name="measurements"></param>
    private void ProcessSingleMeasurementAlarms(IList<IMeasurement> measurements, Ticks measurementsRecieved)
    {
        AlarmProcessor alarmProcessor;

        List<IMeasurement> alarmMeasurements = new List<IMeasurement>();

        using (FileBackedDictionary<int, AlarmEvent> activeAlarms = new FileBackedDictionary<int, AlarmEvent>(FilebackedDictionartPath))
        {
            // Change status on any alarms that timed out.
            foreach (KeyValuePair<int, AlarmEvent> evt in activeAlarms)
            {
                lock (m_alarmLock)
                {
                    if (!m_alarmLookup.TryGetValue(evt.Key, out alarmProcessor))
                    {
                        OnStatusMessage(MessageLevel.Error, $"Alarm ID {evt.Key} no longer exists but is still active. Throwing away active alarm details.");
                        continue;
                    }

                    if (alarmProcessor.Alarm.Timeout > 0 && alarmProcessor.LastProcessed + (alarmProcessor.Alarm.Timeout * Ticks.PerSecond) < measurementsRecieved)
                    {
                        alarmProcessor.Alarm.State = Gemstone.Timeseries.AlarmState.Cleared;
                        alarmMeasurements.Add(CreateAlarmEvent(measurementsRecieved, alarmProcessor.Alarm, activeAlarms));
                    }

                }
            }

            // Process each Measurement
            foreach (IMeasurement measurement in measurements)
            {
                lock (m_alarmLock)
                {
                    // Get alarms that apply to the measurement being processed
                    if (!m_measurementAlarmMapping.TryGetValue(measurement.ID, out List<int> alarmIDs))
                        continue;

                    foreach (int alarmID in alarmIDs)
                    {
                        if (!m_alarmLookup.TryGetValue(alarmID, out alarmProcessor))
                        {
                            OnStatusMessage(MessageLevel.Error, $"Alarm ID {alarmID} no longer exists but is still recieving Measurements. Throwing away active alarm details.");
                            continue;
                        }
                        alarmProcessor.LastProcessed = measurementsRecieved;
                        // Test each alarm to determine whether their states have changed

                        // Build the Frame
                        alarmProcessor.AssignMeasurement(measurement);

                        // Test the Alarm
                        if (alarmProcessor.Test(RealTime))
                        {
                            AlarmEvent alarmEvent = new AlarmEvent();
                            if (activeAlarms.TryGetValue(alarmProcessor.Alarm.ID, out alarmEvent))
                                alarmMeasurements.Add(CreateAlarmEvent(measurementsRecieved, alarmProcessor.Alarm, activeAlarms));
                            else
                            {
                                alarmMeasurements.Add(CreateAlarmEvent(measurementsRecieved, alarmProcessor.Alarm, activeAlarms));
                            }
                        }
                    }
                }
            }
        }

        if (alarmMeasurements.Count > 0)
        {
            // Update alarm history by sending
            // new alarm events into the system
            OnNewMeasurements(alarmMeasurements);
        }

    }

    // Returns true if a change to the alarm's configuration also changed the alarm's behavior.
    private bool BehaviorChanged(Alarm existingAlarm, Alarm definedAlarm)
    {
        return (object)definedAlarm == null ||
               (existingAlarm.SignalID != definedAlarm.SignalID) ||
               (existingAlarm.InputMeasurementKeys != definedAlarm.InputMeasurementKeys) ||
               (existingAlarm.Operation != definedAlarm.Operation) ||
               (existingAlarm.SetPoint != definedAlarm.SetPoint) ||
               (existingAlarm.Tolerance != definedAlarm.Tolerance) ||
               (existingAlarm.Delay != definedAlarm.Delay) ||
               (existingAlarm.Hysteresis != definedAlarm.Hysteresis);
    }

    private IMeasurement? CreateAlarmEvent(Ticks timestamp, Alarm alarm, FileBackedDictionary<int, AlarmEvent> activeAlarms)
    {
        IMeasurement measurement = null;
        AlarmEvent alarmEvent = null;

        if (alarm.State == Gemstone.Timeseries.AlarmState.Raised && !activeAlarms.TryGetValue(alarm.ID, out alarmEvent))
        {
            measurement = CreateAlarmRaisedEvent(timestamp, alarm, out alarmEvent);
            activeAlarms.Add(alarm.ID, alarmEvent);
        }
        else if (alarm.State == Gemstone.Timeseries.AlarmState.Cleared && activeAlarms.TryGetValue(alarm.ID, out alarmEvent))
        {
            measurement = CreateAlarmClearedEvent(timestamp, alarm, alarmEvent);
            activeAlarms.Remove(alarm.ID);
        }

        if (alarmEvent is not null)
        {
            m_eventDetailsQueue.Enqueue(alarmEvent);
            m_eventDetailsOperation.RunAsync();
        }

        return measurement;

    }

    // Creates an alarm event from the given alarm and measurement.
    private IMeasurement CreateAlarmRaisedEvent(Ticks timestamp, Alarm alarm, out AlarmEvent alarmEvent)
    {
        alarmEvent = new AlarmEvent()
        {
            Guid = Guid.NewGuid(),
            StartTime = timestamp,
            EndTime = null,
            AlarmID = alarm.ID,
            MeasurementID = alarm.SignalID,
            Operation = alarm.Operation,
            Setpoint = alarm.SetPoint ?? 0,
            Severity = alarm.Severity,
            SignalTag = alarm.InputMeasurementKeys.ToString(),
        };

        IMeasurement measurement = new AlarmMeasurement()
        {
            Timestamp = timestamp,
            Value = (int)alarm.State,
            AlarmID = alarmEvent.Guid
        };

        measurement.Metadata = MeasurementKey.LookUpBySignalID(alarm.SignalID).Metadata;

        return measurement;
    }

    private IMeasurement CreateAlarmClearedEvent(Ticks timestamp, Alarm alarm, AlarmEvent alarmEvent)
    {
        alarmEvent.EndTime = timestamp;

        IMeasurement measurement = new AlarmMeasurement()
        {
            Timestamp = timestamp,
            Value = (int)alarm.State,
            AlarmID = alarmEvent.Guid
        };

        measurement.Metadata = MeasurementKey.LookUpBySignalID(alarm.SignalID).Metadata;

        return measurement;
    }

    private EventDetails GenerateAlarmDetails(AlarmEvent alarmEvent)
    {
        return new EventDetails()
        {
            StartTime = alarmEvent.StartTime,
            EndTime = alarmEvent.EndTime,
            EventGuid = alarmEvent.Guid,
            Type = "alarm",
            MeasurementID = alarmEvent.MeasurementID,
            Details = JsonConvert.SerializeObject(new
            {
                AlarmID = alarmEvent.AlarmID,
                Setpoint = alarmEvent.Setpoint,
                SignalTag = alarmEvent.SignalTag,
                Severity = alarmEvent.Severity,
                Operation = alarmEvent.Operation
            })
        };
    }

    #endregion

    #region [ Static ]

    // Static Properties

    /// <summary>
    /// The default (most recently created) instance of the alarm adapter.
    /// </summary>
    public static AlarmAdapter Default { get; private set; }



    #endregion
}

