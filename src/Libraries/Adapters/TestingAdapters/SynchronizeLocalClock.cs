﻿//******************************************************************************************************
//  SynchronizeLocalClock.cs - Gbtc
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
//  12/20/2018 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

// ReSharper disable InconsistentNaming
// ReSharper disable PossibleMultipleEnumeration
// ReSharper disable NotAccessedField.Local
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable UnusedMember.Global

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Timers;
using Gemstone;
using Gemstone.Diagnostics;
using Gemstone.StringExtensions;
using Gemstone.Timeseries;
using Gemstone.Timeseries.Adapters;
using Timer = System.Timers.Timer;

namespace TestingAdapters;

/// <summary>
/// Represents a class used to synchronize the local system clock time to the time from input measurements.
/// </summary>
[Description("System Time Sync: Synchronizes local system clock time to the time from input measurements")]
[SupportedOSPlatform("windows")]
public class SynchronizeLocalClock : FacileActionAdapterBase
{
    #region [ Members ]

    // Nested Types
    #pragma warning disable 169
    #pragma warning disable 414
    #pragma warning disable 649
        
    [StructLayout(LayoutKind.Sequential)]
    private struct LUID
    {
        public int LowPart;
        public int HighPart;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct LUID_AND_ATTRIBUTES
    {
        public LUID Luid;
        public int Attributes;
    }

    // Fixed at one privilege for local use case
    [StructLayout(LayoutKind.Sequential)]
    private struct TOKEN_PRIVILEGES
    {
        public int PrivilegeCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public LUID_AND_ATTRIBUTES[] Privileges;

        public TOKEN_PRIVILEGES()
        {
            PrivilegeCount = 1;
            Privileges = new LUID_AND_ATTRIBUTES[1];
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SYSTEMTIME
    {
        public ushort wYear;
        public ushort wMonth;
        public ushort wDayOfWeek;
        public ushort wDay;
        public ushort wHour;
        public ushort wMinute;
        public ushort wSecond;
        public ushort wMilliseconds;
    }

    #pragma warning restore 169
    #pragma warning restore 414
    #pragma warning restore 649

    // Constants
    private const string SE_SYSTEMTIME_NAME = "SeSystemtimePrivilege";

    /// <summary>
    /// Default value for the <see cref="UpdateFrequency"/> property.
    /// </summary>
    public const int DefaultUpdateFrequency = 1000;

    /// <summary>
    /// Default value for the <see cref="UpdateTolerance"/> property.
    /// </summary>
    public const double DefaultUpdateTolerance = 0.001D; // One millisecond

    /// <summary>
    /// Default value for the <see cref="PreferGoodTimeQuality"/> property.
    /// </summary>
    public const bool DefaultPreferGoodTimeQuality = true;

    // Fields
    private Timer? m_updateTimer;
    private long m_skippedUpdates;
    private long m_failedUpdates;
    private long m_successfulUpdates;
    private long m_badTimeSyncs;
    private long m_timerEvents;
    private long m_updateTolerance;
    private bool m_goodSourceTimeQuality;
    private long m_latestTime;
    private bool m_disposed;

    #endregion

    #region [ Properties ]

    /// <summary>
    /// Gets or sets the update frequency, in milliseconds, for setting the local system clock time.
    /// </summary>
    [ConnectionStringParameter]
    [DefaultValue(DefaultUpdateFrequency)]
    [Description("Defines the update frequency, in milliseconds, for setting the local system clock time.")]
    public int UpdateFrequency { get; set; } = DefaultUpdateFrequency;

    /// <summary>
    /// Gets or sets the minimum update tolerance, in seconds, as deviation to current local time to check before updating the local clock.
    /// </summary>
    [ConnectionStringParameter]
    [DefaultValue(DefaultUpdateTolerance)]
    [Description("Defines the minimum update tolerance, in seconds, as deviation to current local time to check before updating the local clock.")]
    public double UpdateTolerance
    {
        get => new Ticks(m_updateTolerance).ToSeconds();
        set => m_updateTolerance = Ticks.FromSeconds(value);
    }

    /// <summary>
    /// Gets or sets flag that determines if good time quality should be preferred for clock syncs.
    /// If false, absolute latest time will always be used regardless of reported quality.
    /// </summary>
    [ConnectionStringParameter]
    [DefaultValue(DefaultPreferGoodTimeQuality)]
    [Description("Defines flag that determines if good time quality should be preferred for clock syncs. If false, absolute latest time will always be used regardless of reported quality.")]
    public bool PreferGoodTimeQuality { get; set; } = DefaultPreferGoodTimeQuality;

    /// <summary>
    /// Gets or sets flag that determines whether to use the local clock time as real time.
    /// </summary>
    /// <remarks>
    /// Use your local system clock as real time only if the time is locally GPS-synchronized,
    /// or if the measurement values being sorted were not measured relative to a GPS-synchronized clock.
    /// Turn this off if the class is intended to process historical data.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)] // Redeclared to hide property - value should not be changed by user
    public override bool UseLocalClockAsRealTime
    {
        get => base.UseLocalClockAsRealTime;
        set => base.UseLocalClockAsRealTime = value;
    }

    /// <summary>
    /// Gets or sets flag that determines whether to fall back on local clock time as real time when time is unreasonable.
    /// </summary>
    /// <remarks>
    /// This property is only applicable when <see cref="FacileActionAdapterBase.UseLocalClockAsRealTime"/> is <c>false</c>.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)] // Redeclared to hide property - value should not be changed by user
    public override bool FallBackOnLocalClock
    { 
        get => base.FallBackOnLocalClock; 
        set => base.FallBackOnLocalClock = value;
    }

    /// <summary>
    /// Gets or sets flag to start tracking the absolute latest received measurement values.
    /// </summary>
    /// <remarks>
    /// Latest received measurement value will be available via the <see cref="FacileActionAdapterBase.LatestMeasurements"/> property.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)] // Redeclared to hide property - value should not be changed by user
    public override bool TrackLatestMeasurements
    {
        get => base.TrackLatestMeasurements;
        set => base.TrackLatestMeasurements = value;
    }

    /// <summary>
    /// Gets the flag indicating if this adapter supports temporal processing.
    /// </summary>
    public override bool SupportsTemporalProcessing => false;

    /// <summary>
    /// Returns the detailed status of the data input source.
    /// </summary>
    /// <remarks>
    /// Derived classes should extend status with implementation specific information.
    /// </remarks>
    public override string Status
    {
        get
        {
            StringBuilder status = new();

            status.Append(base.Status);

            long latestTime = Volatile.Read(ref m_latestTime);
            status.AppendLine($"Absolute latest time value: {(latestTime > 0L ? $"{new DateTime(latestTime, DateTimeKind.Utc):yyyy-MM-dd HH:mm:ss.fff}" : "No time value has been received")}");
                
            status.AppendLine($"          Update tolerance: {UpdateTolerance:N3} seconds ({TimeSpan.FromSeconds(UpdateTolerance).TotalMilliseconds:N3} milliseconds)");
            status.AppendLine($"          Update frequency: {UpdateFrequency:N0} milliseconds");
            status.AppendLine($"  Prefer good time quality: {PreferGoodTimeQuality}");
            status.AppendLine($"     Skipped clock updates: {m_skippedUpdates:N0} were within update tolerance");
            status.AppendLine($"      Failed clock updates: {m_failedUpdates:N0}");
            status.AppendLine($"  Successful clock updates: {m_successfulUpdates:N0}");
                
            if (PreferGoodTimeQuality)
                status.AppendLine($"    Syncs with bad quality: {m_badTimeSyncs:N0}");
                
            status.AppendLine($"         Total time checks: {m_timerEvents:N0}");

            return status.ToString();
        }
    }

    #endregion

    #region [ Methods ]

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="SynchronizeLocalClock"/> object and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (m_disposed)
            return;

        try
        {
            if (!disposing)
                return;

            if (m_updateTimer is not null)
            {
                m_updateTimer.Stop();
                m_updateTimer.Elapsed -= UpdateTimer_Elapsed;
                m_updateTimer.Dispose();
            }
        }
        finally
        {
            m_disposed = true;       // Prevent duplicate dispose.
            base.Dispose(disposing); // Call base class Dispose().
        }
    }

    /// <summary>
    /// Initializes <see cref="SynchronizeLocalClock"/>.
    /// </summary>
    public override void Initialize()
    {
        Dictionary<string, string> settings = Settings;

        settings[nameof(UseLocalClockAsRealTime)] = false.ToString();
        settings[nameof(FallBackOnLocalClock)] = false.ToString();
        settings[nameof(TrackLatestMeasurements)] = false.ToString();

        base.Initialize();

        if (InputMeasurementKeys.Length == 0)
            throw new InvalidOperationException("No input measurement keys were specified, cannot synchronize local clock without any inputs.");

        if (settings.TryGetValue(nameof(UpdateFrequency), out string? setting) && int.TryParse(setting, out int updateFrequency) && updateFrequency > 0)
            UpdateFrequency = updateFrequency;

        if (settings.TryGetValue(nameof(UpdateTolerance), out setting) && double.TryParse(setting, out double tolerance))
            UpdateTolerance = tolerance;
        else
            UpdateTolerance = DefaultUpdateTolerance;

        if (settings.TryGetValue(nameof(PreferGoodTimeQuality), out setting))
            PreferGoodTimeQuality = setting.ParseBoolean();

        try
        {
            EnableSystemTimePrivilege();
        }
        catch (Exception ex)
        {
            OnProcessException(MessageLevel.Warning, new InvalidOperationException($"Clock synchronizations may fail: {ex.Message}", ex));
        }

        m_updateTimer = new Timer(UpdateFrequency);
        m_updateTimer.Elapsed += UpdateTimer_Elapsed;
        m_updateTimer.Start();
    }

    /// <summary>
    /// Stops the <see cref="SynchronizeLocalClock"/>.
    /// </summary>
    public override void Stop()
    {
        base.Stop();
        m_updateTimer?.Stop();
            
        OnStatusMessage(MessageLevel.Info, "Clock synchronizations stopped...");
    }

    /// <summary>
    /// Starts the <see cref="SynchronizeLocalClock"/>.
    /// </summary>
    public override void Start()
    {
        if (m_updateTimer is null)
            throw new InvalidOperationException("Cannot start clock synchronizations, update timer not been initialized.");

        base.Start();
        m_updateTimer.Start();

        OnStatusMessage(MessageLevel.Info, "Clock synchronizations started...");
    }

    /// <summary>
    /// Attempts to enable system time privilege for the current process. Privilege must be already granted.
    /// </summary>
    [AdapterCommand("Attempts to enable system time privilege for the current process. Privilege must be already granted.", "Administrator")]
    public void EnableSystemTimePrivilege()
    {
        const int SE_PRIVILEGE_ENABLED = 0x00000002;
        const int TOKEN_QUERY = 0x0008;
        const int TOKEN_ADJUST_PRIVILEGES = 0x0020;

        TOKEN_PRIVILEGES newState = new();
        TOKEN_PRIVILEGES previousState = new();
        LUID luid = new();
        int handle = -1;

        if (!LookupPrivilegeValue("", SE_SYSTEMTIME_NAME, ref luid))
            throw new InvalidOperationException($"Failed to enable system time privilege, LUID lookup for \"{SE_SYSTEMTIME_NAME}\" privilege did not succeed.");

        if (!OpenProcessToken(GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref handle))
            throw new InvalidOperationException("Failed to enable system time privilege, could not open access token to adjust privileges for the current process.");

        newState.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;
        newState.Privileges[0].Luid = luid;

        int bufferLength = Marshal.SizeOf(typeof(TOKEN_PRIVILEGES));
        int returnLength = 0;

        // Make sure SE_SYSTEMTIME_NAME privilege is enabled (can only be granted by a system administrator)
        if (!AdjustTokenPrivileges(handle, 0, ref newState, bufferLength, ref previousState, ref returnLength))
            throw new Win32Exception(Marshal.GetLastWin32Error(), $"Failed to enable system time privilege, verify service account has been granted \"{SE_SYSTEMTIME_NAME}\" privilege.");
    }

    /// <summary>
    /// Manually synchronizes local clock to specified date/time.
    /// </summary>
    [AdapterCommand("Manually synchronizes local clock to specified date/time.", "Administrator")]
    public void ManualSync(string dateTime)
    {
        if (!DateTime.TryParse(dateTime, out DateTime targetTime))
        {
            OnStatusMessage(MessageLevel.Warning, $"Failed to parse \"{dateTime}\" as a valid DateTime value.");
            return;
        }

        try
        {
            SetSystemTime(targetTime);
            m_successfulUpdates++;
            OnStatusMessage(MessageLevel.Info, $"Manually updated local clock to {targetTime:yyyy-MM-dd HH:mm:ss.fff}");
        }
        catch (Exception ex)
        {
            m_failedUpdates++;
            OnStatusMessage(MessageLevel.Error, $"Failed to manually update local clock to {targetTime:yyyy-MM-dd HH:mm:ss.fff}: {ex.Message}");
        }
    }

    /// <summary>
    /// Forces local clock synchronization to latest time value.
    /// </summary>
    [AdapterCommand("Forces local clock synchronization to absolute latest received time value.", "Administrator")]
    public void ForceSync()
    {
        long targetTime = Volatile.Read(ref m_latestTime);

        if (targetTime > 0L)
        {
            DateTime newSystemTime = new(targetTime);

            try
            {
                SetSystemTime(newSystemTime);
                m_successfulUpdates++;
                OnStatusMessage(MessageLevel.Info, $"Forced local clock to {newSystemTime:yyyy-MM-dd HH:mm:ss.fff}");
            }
            catch (Exception ex)
            {
                m_failedUpdates++;
                OnStatusMessage(MessageLevel.Error, $"Failed to force local clock to {newSystemTime:yyyy-MM-dd HH:mm:ss.fff}: {ex.Message}");
            }
        }
        else
        {
            int count = InputMeasurementKeys.Length;

            OnStatusMessage(MessageLevel.Warning, count > 0 ? 
                $"Cannot force clock update - no time has been received from any of the {count:N0} defined input sources." : 
                "Cannot force clock update - no input sources have been defined.");
        }
    }

    /// <summary>
    /// Gets a short one-line status of this <see cref="SynchronizeLocalClock"/>.
    /// </summary>
    /// <param name="maxLength">Maximum number of available characters for display.</param>
    /// <returns>A short one-line summary of the current status of this <see cref="SynchronizeLocalClock"/>.</returns>
    public override string GetShortStatus(int maxLength)
    {
        return $"Updated clock {m_successfulUpdates:N0} times out of {m_timerEvents:N0} checks so far...".CenterText(maxLength);
    }

    /// <summary>
    /// Queues a collection of measurements for processing.
    /// </summary>
    /// <param name="measurements">Measurements to queue for processing.</param>
    public override void QueueMeasurementsForProcessing(IEnumerable<IMeasurement> measurements)
    {
        bool goodSourceTimeQuality = false;
        long absoluteLatestTime, latestGoodQualityTime = absoluteLatestTime = Volatile.Read(ref m_latestTime);

        foreach (IMeasurement measurement in measurements)
        {
            long measurementTime = measurement.Timestamp.Value;

            if (PreferGoodTimeQuality && measurement.TimestampQualityIsGood())
            {
                // Track if any source measurements had good time quality
                goodSourceTimeQuality = true;

                // Track latest time with good quality
                if (measurementTime > latestGoodQualityTime)
                    latestGoodQualityTime = measurementTime;
            }

            // Track absolute latest time, regardless of reasonability or quality
            if (measurementTime > absoluteLatestTime)
                absoluteLatestTime = measurementTime;
        }

        Volatile.Write(ref m_latestTime, goodSourceTimeQuality ? latestGoodQualityTime : absoluteLatestTime);

        if (!PreferGoodTimeQuality)
            return;

        // Track the number of times a time with bad quality was used for time sync
        if (!goodSourceTimeQuality)
            m_badTimeSyncs++;

        m_goodSourceTimeQuality = goodSourceTimeQuality;

        // Not passing values to base class, no further measurement processing needed
    }

    private void UpdateTimer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        try
        {
            m_timerEvents++;
                
            long latestTime = Volatile.Read(ref m_latestTime);

            if (latestTime <= 0L || Math.Abs(latestTime - DateTime.UtcNow.Ticks) < m_updateTolerance)
            {
                m_skippedUpdates++;
                return;
            }

            SetSystemTime(new DateTime(latestTime, DateTimeKind.Utc));
            m_successfulUpdates++;

            // There are race conditions with the use of the m_goodSourceTimeQuality which are considered acceptable
            // for this use case since its only function is to provide logged indications of syncs using timestamps
            // sourced with bad time quality. Generally, if the time quality is bad it will remain so for a period
            // long enough to be reported here. Otherwise, counter exists for individual bad time quality blips that
            // will be captured through normal status logging.
            if (PreferGoodTimeQuality && !m_goodSourceTimeQuality)
                OnStatusMessage(MessageLevel.Warning, "WARNING: Clock set with measurement that has bad time quality -- increase measurement sources.");
        }
        catch (Exception ex)
        {
            m_failedUpdates++;
            OnProcessException(MessageLevel.Error, ex);
        }
    }

    #endregion

    #region [ Static ]

    // Static Methods

    [DllImport("advapi32.dll")]
    private static extern bool OpenProcessToken(int ProcessHandle, int DesiredAccess, ref int TokenHandle);

    [DllImport("kernel32.dll")]
    private static extern int GetCurrentProcess();

    [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
    private static extern bool LookupPrivilegeValue(string lpSystemName, string lpName, [MarshalAs(UnmanagedType.Struct)] ref LUID lpLuid);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool AdjustTokenPrivileges(int TokenHandle, int DisableAllPrivileges, [MarshalAs(UnmanagedType.Struct)] ref TOKEN_PRIVILEGES NewState, int BufferLength, [MarshalAs(UnmanagedType.Struct)] ref TOKEN_PRIVILEGES PreviousState, ref int ReturnLength);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetSystemTime(ref SYSTEMTIME lpSystemTime);

    private static void SetSystemTime(DateTime newSystemTime)
    {
        SYSTEMTIME systemTime = new()
        {
            wYear = (ushort)newSystemTime.Year,
            wMonth = (ushort)newSystemTime.Month,
            wDay = (ushort)newSystemTime.Day,
            wHour = (ushort)newSystemTime.Hour,
            wMinute = (ushort)newSystemTime.Minute,
            wSecond = (ushort)newSystemTime.Second,
            wMilliseconds = (ushort)newSystemTime.Millisecond
        };

        // Attempt to adjust system clock
        if (!SetSystemTime(ref systemTime))
            throw new Win32Exception(Marshal.GetLastWin32Error(), $"Failed to set local system time, verify service account has been granted \"{SE_SYSTEMTIME_NAME}\" privilege.");
    }

    #endregion
}