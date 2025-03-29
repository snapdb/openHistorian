//******************************************************************************************************
//  PhasorOpsController.cs - Gbtc
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
//  12/27/2024 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMethodReturnValue.Local
// ReSharper disable StringLiteralTypo
// ReSharper disable IdentifierTypo
#pragma warning disable SYSLIB0050

using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Runtime.Caching;
using System.Runtime.Serialization.Formatters.Soap;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Text.Json;
using Gemstone;
using Gemstone.Caching;
using Gemstone.Data;
using Gemstone.Data.Model;
using Gemstone.Diagnostics;
using Gemstone.Numeric.EE;
using Gemstone.PhasorProtocols.IEEEC37_118;
using Gemstone.PhasorProtocols;
using Gemstone.StringExtensions;
using Microsoft.AspNetCore.Mvc;
using openHistorian.Model;
using PhasorProtocolAdapters;
using ServiceInterface;
using DataFrame = openHistorian.Model.DataFrame;
using DataCell = openHistorian.Model.DataCell;
using PhasorValue = openHistorian.Model.PhasorValue;
using AnalogDefinition = openHistorian.Model.AnalogDefinition;
using ConfigurationCell = openHistorian.Model.ConfigurationCell;
using DigitalDefinition = openHistorian.Model.DigitalDefinition;
using FrequencyDefinition = openHistorian.Model.FrequencyDefinition;
using PhasorDefinition = openHistorian.Model.PhasorDefinition;
using Device = Gemstone.Timeseries.Model.Device;
using Phasor = Gemstone.Timeseries.Model.Phasor;
using Measurement = Gemstone.Timeseries.Model.Measurement;
using SignalType = Gemstone.Timeseries.Model.SignalType;
using ConfigSettings = Gemstone.Configuration.Settings;
using Gemstone.Timeseries.Model;

namespace openHistorian.WebUI.Controllers;

/// <summary>
/// Controller for managing phasor operations.
/// </summary>
[Route("api/PhasorOps")]
[ApiController]
public class PhasorOpsController : Controller, ISupportConnectionTest
{
    private AdoDataConnection? m_connection;
    private bool m_disposed;

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="PhasorOpsController"/> object and optionally releases the managed resources.
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

            m_connection?.Dispose();
        }
        finally
        {
            m_disposed = true; // Prevent duplicate dispose.
            base.Dispose(disposing); // Call base class Dispose().
        }
    }

    public AdoDataConnection Connection => m_connection ??= new AdoDataConnection(ConfigSettings.Instance);

    private int IeeeC37_118ProtocolID => s_ieeeC37_118ProtocolID ??= Connection.ExecuteScalar<int>("SELECT ID FROM Protocol WHERE Acronym='IeeeC37_118V1'");

    private int AnalogSignalTypeID => s_analogSignalTypeID ??= Connection.ExecuteScalar<int>("SELECT ID FROM SignalType WHERE Acronym='ALOG'");

    private int DigitalSignalTypeID => s_digitalSignalTypeID ??= Connection.ExecuteScalar<int>("SELECT ID FROM SignalType WHERE Acronym='DIGI'");

    public TableOperations<TModel> Table<TModel>() where TModel : class, new()
    {
        return new TableOperations<TModel>(Connection);
    }

    private class ConnectionCache : IDisposable
    {
        public string Token { get; } = Guid.NewGuid().ToString();
        public bool IsDisposed { get; private set; }

        private readonly MultiProtocolFrameParser FrameParser;
        private bool ReadyToReceive;

        private readonly PhasorOpsController m_parent;
        private readonly ConcurrentQueue<string> m_messageQueue;
        private long m_lastExceptionTime;

        private ConnectionCache(PhasorOpsController parent)
        {
            m_parent = parent;

            // Create a new phasor protocol frame parser used to dynamically request device configuration frames
            // and return them to remote clients so that the frame can be used in system setup and configuration
            FrameParser = new MultiProtocolFrameParser();

            // Attach to events on new frame parser reference
            FrameParser.ConnectionAttempt += m_frameParser_ConnectionAttempt;
            FrameParser.ConnectionEstablished += m_frameParser_ConnectionEstablished;
            FrameParser.ConnectionException += m_frameParser_ConnectionException;
            FrameParser.ConnectionTerminated += m_frameParser_ConnectionTerminated;
            FrameParser.ExceededParsingExceptionThreshold += m_frameParser_ExceededParsingExceptionThreshold;
            FrameParser.ParsingException += m_frameParser_ParsingException;
            FrameParser.ReceivedConfigurationFrame += m_frameParser_ReceivedConfigurationFrame;
            FrameParser.ReceivedDataFrame += m_frameParser_ReceivedDataFrame;

            // We only want to try to connect to device and retrieve configuration/data as quickly as possible
            FrameParser.MaximumConnectionAttempts = 1;
            FrameParser.SourceName = $"{nameof(PhasorOpsController)}";

            m_messageQueue = new ConcurrentQueue<string>();
        }

        public void Dispose()
        {
            try
            {
                FrameParser.Stop();
                FrameParser.Dispose();
            }
            finally
            {
                IsDisposed = true;
            }
        }

        public bool TryGetNextMessage(out string? message)
        {
            // So long as messages are being accessed, keep memory cache alive
            if (!IsDisposed && KeepAlive(Token))
                return m_messageQueue.TryDequeue(out message);

            message = null;
            return false;
        }

        private void EnqueueMessage(string message, string source)
        {
            if (m_messageQueue.Count > 100)
            {
                // Throttle exception messages to prevent log flooding
                if (DateTime.UtcNow.Ticks - m_lastExceptionTime <= Ticks.FromSeconds(2.0D))
                    return;

                m_lastExceptionTime = DateTime.UtcNow.Ticks;
                Logger.SwallowException(new InvalidOperationException($"{nameof(PhasorOpsController)}: Message queue is full for connection \"{Token}\", dropping {source}..."));

                return;
            }

            // Format message as JSON object, source and message (already JSON)
            m_messageQueue.Enqueue($"{{ \"{source}\": {message} }}");
        }

        private void OnStatusMessage(MessageLevel logLevel, string message)
        {
            string level = logLevel.ToString();
            EnqueueMessage(JsonSerializer.Serialize(new
            {
                level,
                message
            }), "StatusMessage");
        }

        private void m_frameParser_ReceivedConfigurationFrame(object? sender, EventArgs<IConfigurationFrame> e)
        {
            OnStatusMessage(MessageLevel.Info, "Successfully received configuration frame.");

            // Generate simple model of configuration frame
            ConfigurationFrame configFrame = m_parent.LoadConfigurationFrame(e.Argument, FrameParser.ConnectionString);

            // Serialize configuration frame to JSON
            EnqueueMessage(JsonSerializer.Serialize(configFrame), "ConfigFrame");
        }

        private void m_frameParser_ReceivedDataFrame(object? sender, EventArgs<IDataFrame> e)
        {
            // Only start enqueuing data frames once web socket has been requested
            if (!ReadyToReceive)
                return;

            IDataFrame phasorFrame = e.Argument;

            // Validate data frame content
            if (phasorFrame.Cells.Count == 0)
                return;

            // Generate simple model of data frame
            DataFrame dataFrame = new()
            {
                IDCode = phasorFrame.IDCode,
                Timestamp = phasorFrame.Timestamp,
                QualityFlags = phasorFrame.QualityFlags,
                Cells = phasorFrame.Cells.Select(cell => new DataCell
                {
                    IDCode = cell.IDCode,
                    Frequency = cell.FrequencyValue.Frequency,
                    DfDt = cell.FrequencyValue.DfDt,
                    StatusFlags = cell.StatusFlags,
                    Phasors = cell.PhasorValues.Select(phasor => new PhasorValue
                    {
                        Angle = phasor.Angle.ToDegrees(),
                        Magnitude = phasor.Magnitude
                    }).ToArray(),
                    Analogs = cell.AnalogValues.Select(analog => analog.Value).ToArray(),
                    Digitals = cell.DigitalValues.Select(digital => digital.Value).ToArray()
                }).ToList()
            };

            // Serialize data frame to JSON
            EnqueueMessage(JsonSerializer.Serialize(dataFrame), "DataFrame");
        }

        private void m_frameParser_ConnectionTerminated(object? sender, EventArgs e)
        {
            // Communications layer closed connection (close not initiated by system) - so we report termination
            if (FrameParser.Enabled)
                OnStatusMessage(MessageLevel.Warning, "Connection closed by remote device, request for configuration canceled.");
        }

        private void m_frameParser_ConnectionEstablished(object? sender, EventArgs e)
        {
            OnStatusMessage(MessageLevel.Info, "Connected to remote device, requesting configuration frame...");

            // Send manual request for configuration frame
            FrameParser.SendDeviceCommand(DeviceCommand.SendLatestConfigurationFrameVersion);
        }

        private void m_frameParser_ConnectionException(object? sender, EventArgs<Exception, int> e)
        {
            OnStatusMessage(MessageLevel.Error, $"Connection attempt failed, request for configuration canceled: {e.Argument1.Message}");
        }

        private void m_frameParser_ParsingException(object? sender, EventArgs<Exception> e)
        {
            OnStatusMessage(MessageLevel.Error, $"Check protocol: parsing exception during request for configuration: {e.Argument.Message}");
        }

        private void m_frameParser_ExceededParsingExceptionThreshold(object? sender, EventArgs e)
        {
            OnStatusMessage(MessageLevel.Error, "Request for configuration canceled due to an excessive number of exceptions...");
        }

        private void m_frameParser_ConnectionAttempt(object? sender, EventArgs e)
        {
            OnStatusMessage(MessageLevel.Info, $"Attempting {FrameParser.PhasorProtocol.GetFormattedProtocolName()} {FrameParser.TransportProtocol.ToString().ToUpper()} based connection...");
        }

        public static ConnectionCache Create(PhasorOpsController parent, string connectionString, double expiration)
        {
            ConnectionCache cache = new(parent);

            MemoryCache<ConnectionCache>.GetOrAdd(cache.Token, expiration, () => cache, Dispose);

            Dictionary<string, string> settings = connectionString.ParseKeyValuePairs();
            List<ushort> accessIDList = [];
            int serverCount;

            // Parse any defined access ID
            if (!settings.TryGetValue("accessID", out string? setting) || string.IsNullOrWhiteSpace(setting) || !ushort.TryParse(setting, out ushort defaultAccessID))
                defaultAccessID = 1;

            // Parse any defined access IDs from server list, this assumes TCP connection since this is currently the only connection type that supports multiple end points
            if (settings.TryGetValue("server", out setting) && !string.IsNullOrWhiteSpace(setting))
            {
                List<string> serverList = [];

                string[] servers = setting.Split((char[]) [','], StringSplitOptions.RemoveEmptyEntries);

                foreach (string server in servers)
                {
                    string[] parts = server.Split((char[]) ['/'], StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length == 0)
                        continue;

                    if (parts.Length < 2 || !ushort.TryParse(parts[1], out ushort accessID))
                        accessID = defaultAccessID;

                    serverList.Add(parts[0].Trim());
                    accessIDList.Add(accessID);
                }

                settings["server"] = string.Join(",", serverList);
                connectionString = settings.JoinKeyValuePairs();

                if (accessIDList.Count == 0)
                    accessIDList.Add(defaultAccessID);

                serverCount = serverList.Count;
            }
            else
            {
                accessIDList.Add(defaultAccessID);
                serverCount = 1;
            }

            // Try connection for server in list
            for (int i = 0; i < serverCount; i++)
            {
                // Most of the parameters in the connection string will be for the data source in the frame parser,
                // so we provide all of them, other parameters will simply be ignored
                cache.FrameParser.ConnectionString = connectionString;

                // Provide access ID to frame parser as this may be necessary to make a phasor connection
                cache.FrameParser.DeviceID = accessIDList[cache.FrameParser.ServerIndex];

                // Start the frame parser - this will attempt connection
                cache.FrameParser.Start();
            }

            return cache;
        }

        public static bool TryGet(string token, out ConnectionCache? cache)
        {
            bool result = MemoryCache<ConnectionCache>.TryGet(token, out cache);

            if (result && cache is not null)
                cache.ReadyToReceive = true;

            return result;
        }

        public static bool Close(string token)
        {
            if (!TryGet(token, out ConnectionCache? cache))
                return false;

            if (cache is not null)
            {
                cache.ReadyToReceive = false;
                cache.Dispose();
            }

            MemoryCache<ConnectionCache>.Remove(token);

            return true;
        }

        private static bool KeepAlive(string token)
        {
            return MemoryCache<ConnectionCache>.KeepAlive(token);
        }

        private static void Dispose(CacheEntryRemovedArguments arguments)
        {
            if (arguments.RemovedReason != CacheEntryRemovedReason.Removed)
                return;

            if (arguments.CacheItem.Value is ConnectionCache cache)
                cache.Dispose();
        }
    }

    /// <inheritdoc />
    [HttpPost, Route("Connect/{expiration:double?}")]
    public Task<IActionResult> Connect([FromBody] ConnectionRequest request, double? expiration, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.ConnectionString);

        ConnectionCache cache = ConnectionCache.Create(this, request.ConnectionString, expiration ?? 1.0D);

        return Task.FromResult<IActionResult>(Ok(cache.Token));
    }

    /// <inheritdoc />
    [HttpGet, Route("Close/{token}")]
    public IActionResult Close(string token)
    {
        return ConnectionCache.Close(token) ? Ok() : NotFound();
    }

    /// <inheritdoc />
    [HttpGet("DataStream/{token}")]
    public async Task GetDataStream(string token, CancellationToken cancellationToken)
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            HttpContext.Response.StatusCode = 400; // Bad Request if not a WebSocket request.
            return;
        }

        if (!ConnectionCache.TryGet(token, out ConnectionCache? cache) || cache is null)
        {
            HttpContext.Response.StatusCode = 404; // Not Found if token is not found.
            return;
        }

        using WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
        await StreamMessages(cache, webSocket, cancellationToken);
    }

    private static async Task StreamMessages(ConnectionCache cache, WebSocket webSocket, CancellationToken cancellationToken)
    {
        // Continue streaming messages as long as the WebSocket is open.
        while (webSocket.State == WebSocketState.Open && !cache.IsDisposed && !cancellationToken.IsCancellationRequested)
        {
            if (!cache.TryGetNextMessage(out string? message) || string.IsNullOrWhiteSpace(message))
            {
                Thread.Sleep(500);
                continue;
            }

            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            // Send the message
            await webSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, cancellationToken);
        }
    }

    /// <summary>
    /// Returns a collection of system time zones: ID -> DisplayName.
    /// </summary>
    /// <param name="isOptional">Indicates if selection on UI is optional for this collection.</param>
    /// <returns><see cref="Dictionary{T1,T2}"/> type collection of system time zones.</returns>
    [HttpGet, Route("GetTimeZones/{isOptional:bool?}")]
    public Dictionary<string, string> GetTimeZones(bool? isOptional = null)
    {
        Dictionary<string, string> timeZonesList = new();

        if (isOptional.GetValueOrDefault())
            timeZonesList.Add("", "Select Time Zone");

        foreach (TimeZoneInfo timeZoneInfo in TimeZoneInfo.GetSystemTimeZones())
        {
            if (!timeZonesList.ContainsKey(timeZoneInfo.Id))
                timeZonesList.Add(timeZoneInfo.Id, timeZoneInfo.DisplayName);
        }

        return timeZonesList;
    }

    /// <summary>
    /// Saves the configuration frame to the database.
    /// </summary>
    /// <param name="configFrame">Source config frame.</param>
    /// <param name="deviceID">Device ID to save configuration for, leave blank for new devices.</param>
    [HttpPost, Route("SaveConfiguration/{deviceID:int?}")]
    public IActionResult SaveConfiguration(ConfigurationFrame configFrame, int? deviceID = null)
    {
        // Query existing device record, or create new one
        Device device = deviceID is null ? NewDevice() : QueryDeviceByID(deviceID.Value);
        string connectionString = configFrame.ConnectionString;

        // Set device properties
        device.Acronym = configFrame.Acronym;
        device.Name = configFrame.IDLabel;
        device.ProtocolID = configFrame.ProtocolID;
        device.FramesPerSecond = configFrame.FrameRate;
        device.AccessID = configFrame.IDCode;
        device.IsConcentrator = !string.IsNullOrWhiteSpace(connectionString) && (configFrame.IsConcentrator || configFrame.Cells.Count > 1);
        
        // TODO: Thought these were removed as fields in new schema?
        device.AutoStartDataParsingSequence = true;
        device.SkipDisableRealTimeData = false;

        if (string.IsNullOrWhiteSpace(device.Name))
            device.Name = device.Acronym;

        // Only update connection string if one has been defined, prevents changing any existing one to blank
        if (connectionString.Length > 0)
            device.ConnectionString = connectionString;

        device.Enabled = true;

        // Add new or update existing device record
        if (deviceID is null)
            deviceID = AddNewDevice(device);
        else
            UpdateDevice(device);

        SaveDeviceRecords(configFrame, deviceID.Value);
        MarkConfigurationAsSynchronized(device.Acronym);

        return Ok();
    }

    private void SaveDeviceRecords(ConfigurationFrame configFrame, int deviceID)
    {
        if (configFrame.Cells.Count == 0)
            throw new InvalidOperationException("Cannot save device records: no devices were defined in loaded configuration.");

        if (configFrame.IsConcentrator || configFrame.Cells.Count > 1)
        {
            foreach (ConfigurationCell cell in configFrame.Cells)
            {
                // Query existing child device record, or create new one
                Device device = cell.ID == 0 ? NewDevice() : QueryDeviceByID(cell.ID);

                // Check if this is a direct update to child device
                if (configFrame.Cells.Count == 1 && device.ID == deviceID)
                {
                    cell.ID = deviceID;
                    UpdateDevice(device);
                    
                    // Save measurements and calculations associated with a directly edited child device
                    SaveDeviceMeasurements(cell);
                }
                else
                {
                    // Set device properties
                    device.ParentID = cell.ParentID ?? deviceID;
                    device.HistorianID = cell.HistorianID;
                    device.ProtocolID = configFrame.ProtocolID;
                    device.FramesPerSecond = configFrame.FrameRate;
                    device.AccessID = cell.IDCode;
                    device.Acronym = cell.IDLabel;

                    if (string.IsNullOrWhiteSpace(device.Name))
                        device.Name = device.Acronym;

                    device.Enabled = true;

                    // Check if this is a new device or an edit to an existing one
                    if (device.ID == 0)
                    {
                        if (cell.UniqueID is not null)
                            device.UniqueID = cell.UniqueID.Value;

                        cell.ID = AddNewDevice(device);
                    }
                    else
                    {
                        UpdateDevice(device);
                    }

                    SaveDeviceMeasurements(cell);
                }
            }
        }
        else
        {
            // Save measurements and calculations  associated with a directly connected device
            ConfigurationCell cell = configFrame.Cells[0];
            SaveDeviceMeasurements(cell);
        }
    }

    private void SaveDeviceMeasurements(ConfigurationCell cell)
    {
        s_freqSignalType ??= GetDeviceSignalType("FREQ") ?? throw new InvalidOperationException("Failed to load FREQ signal type.");
        s_dfdtSignalType ??= GetDeviceSignalType("DFDT") ?? throw new InvalidOperationException("Failed to load DFDT signal type.");
        s_flagSignalType ??= GetDeviceSignalType("FLAG") ?? throw new InvalidOperationException("Failed to load FLAG signal type.");
        s_alogSignalType ??= GetDeviceSignalType("ALOG") ?? throw new InvalidOperationException("Failed to load ALOG signal type.");
        s_digiSignalType ??= GetDeviceSignalType("DIGI") ?? throw new InvalidOperationException("Failed to load DIGI signal type.");

        // Save frequency, dF/dt signal types and status flags
        SaveFixedMeasurement(cell, s_freqSignalType, cell.FrequencyDefinition.Label);
        SaveFixedMeasurement(cell, s_dfdtSignalType);
        SaveFixedMeasurement(cell, s_flagSignalType);

        // Add analogs
        int index = 0;

        foreach (AnalogDefinition analogDefinition in cell.AnalogDefinitions)
        {
            if (string.IsNullOrWhiteSpace(cell.OriginalAcronym))
                cell.OriginalAcronym = cell.Acronym;

            ++index;
            string orgSignalReference = $"{cell.OriginalAcronym}-{s_alogSignalType.Suffix}{index}";
            string signalReference = $"{cell.Acronym}-{s_alogSignalType.Suffix}{index}";

            // Query existing measurement record for specified signal reference - function will create a new blank measurement record if one does not exist
            Measurement measurement = QueryMeasurement(orgSignalReference);
            string pointTag = CreateIndexedPointTag(cell.Acronym, s_alogSignalType.Acronym, index, analogDefinition.Label);

            measurement.DeviceID = cell.ID;
            measurement.HistorianID = cell.HistorianID;
            measurement.PointTag = pointTag;
            measurement.AlternateTag = analogDefinition.Label;
            measurement.Description = $"{cell.IDLabel} Analog Value {index}:{analogDefinition.AnalogType}: {analogDefinition.Label}";
            measurement.SignalReference = signalReference;
            measurement.SignalTypeID = s_alogSignalType.ID;
            measurement.Internal = true;
            measurement.Enabled = true;

            AddNewOrUpdateMeasurement(measurement);
        }

        // Add digitals
        index = 0;

        foreach (DigitalDefinition digitalDefinition in cell.DigitalDefinitions)
        {
            if (string.IsNullOrWhiteSpace(cell.OriginalAcronym))
                cell.OriginalAcronym = cell.Acronym;

            ++index;
            string orgSignalReference = $"{cell.OriginalAcronym}-{s_digiSignalType.Suffix}{index}";
            string signalReference = $"{cell.Acronym}-{s_digiSignalType.Suffix}{index}";

            // Query existing measurement record for specified signal reference - function will create a new blank measurement record if one does not exist
            Measurement measurement = QueryMeasurement(orgSignalReference);
            string pointTag = CreateIndexedPointTag(cell.Acronym, s_digiSignalType.Acronym, index, digitalDefinition.Label);

            measurement.DeviceID = cell.ID;
            measurement.HistorianID = cell.HistorianID;
            measurement.PointTag = pointTag;
            measurement.AlternateTag = digitalDefinition.Label;
            measurement.Description = $"{cell.IDLabel} Digital Value {index}: {digitalDefinition.Label}";
            measurement.SignalReference = signalReference;
            measurement.SignalTypeID = s_digiSignalType.ID;
            measurement.Internal = true;
            measurement.Enabled = true;

            AddNewOrUpdateMeasurement(measurement);
        }

        // Save phasor definitions
        SaveDevicePhasors(cell);
    }

    private void SaveFixedMeasurement(ConfigurationCell cell, SignalType signalType, string? label = null)
    {
        if (string.IsNullOrWhiteSpace(cell.OriginalAcronym))
            cell.OriginalAcronym = cell.Acronym;

        string orgSignalReference = $"{cell.OriginalAcronym}-{signalType.Suffix}";
        string signalReference = $"{cell.Acronym}-{signalType.Suffix}";

        // Query existing measurement record for specified signal reference - function will create a new blank measurement record if one does not exist
        Measurement measurement = QueryMeasurement(orgSignalReference);
        string pointTag = CreatePointTag(cell.Acronym, signalType.Acronym);

        measurement.DeviceID = cell.ID;
        measurement.HistorianID = cell.HistorianID;
        measurement.PointTag = pointTag;
        measurement.Description = $"{cell.Acronym} {signalType.Name}{(string.IsNullOrWhiteSpace(label) ? "" : " - " + label)}";
        measurement.SignalReference = signalReference;
        measurement.SignalTypeID = signalType.ID;
        measurement.Internal = true;
        measurement.Enabled = true;

        AddNewOrUpdateMeasurement(measurement);
    }

    private void SavePhasorMeasurement(ConfigurationCell cell, SignalType signalType, PhasorDefinition phasorDefinition, int index)
    {
        if (string.IsNullOrWhiteSpace(cell.OriginalAcronym))
            cell.OriginalAcronym = cell.Acronym;

        string orgSignalReference = $"{cell.OriginalAcronym}-{signalType.Suffix}";
        string signalReference = $"{cell.Acronym}-{signalType.Suffix}";

        // Query existing measurement record for specified signal reference - function will create a new blank measurement record if one does not exist
        Measurement measurement = QueryMeasurement(orgSignalReference);

        // Determine defined nominal voltage kV level of phasor
        int baseKV = phasorDefinition.IsVoltage ?
            phasorDefinition.NominalVoltage ?? 500 :
            phasorDefinition.GetAssociatedVoltage(cell)?.NominalVoltage ?? 500;

        string pointTag = CreatePhasorPointTag(cell.Acronym, signalType.Acronym, phasorDefinition.Label, phasorDefinition.Phase, index, baseKV);

        measurement.DeviceID = cell.ID;
        measurement.HistorianID = cell.HistorianID;
        measurement.PointTag = pointTag;
        measurement.Description = $"{cell.Acronym} {phasorDefinition.Label} {phasorDefinition.Phase} {signalType.Name}";
        measurement.PhasorSourceIndex = index;
        measurement.SignalReference = signalReference;
        measurement.SignalTypeID = signalType.ID;
        measurement.Internal = true;
        measurement.Enabled = phasorDefinition.Enabled;
        
        AddNewOrUpdateMeasurement(measurement);
    }

    private void SaveDevicePhasors(ConfigurationCell cell)
    {
        s_iphmSignalType ??= GetDeviceSignalType("IPHM") ?? throw new InvalidOperationException("Failed to load IPHM signal type.");
        s_iphaSignalType ??= GetDeviceSignalType("IPHA") ?? throw new InvalidOperationException("Failed to load IPHA signal type.");
        s_vphmSignalType ??= GetDeviceSignalType("VPHM") ?? throw new InvalidOperationException("Failed to load VPHM signal type.");
        s_vphaSignalType ??= GetDeviceSignalType("VPHA") ?? throw new InvalidOperationException("Failed to load VPHA signal type.");

        Phasor[] phasors = QueryPhasorsForDevice(cell.ID).ToArray();

        // TODO: This drop and add logic is primitive and should be improved with input from the user
        // NOTE: #1 improvement goal should be to preserve existing measurement / SignalID Guids
        bool dropAndAdd = phasors.Length != cell.PhasorDefinitions.Count;

        if (!dropAndAdd)
        {
            // Also perform drop/add operation if these are new records, e.g., from new config
            if (cell.PhasorDefinitions.Any(definition => definition.ID == 0))
                dropAndAdd = true;
        }

        if (!dropAndAdd)
        {
            // Also perform drop/add operation if phasor source index records are not sequential
            if (phasors.Where((phasor, i) => phasor.SourceIndex != i + 1).Any())
                dropAndAdd = true;
        }

        // Separate voltage and current phasor definitions
        List<PhasorDefinition> voltagePhasors = [];
        List<PhasorDefinition> currentPhasors = [];
        Dictionary<PhasorDefinition, int> voltageIDMap = [];

        foreach (PhasorDefinition phasorDefinition in cell.PhasorDefinitions)
        {
            if (phasorDefinition.TaggedForDelete)
            {
                IEnumerable<Measurement> measurements = QueryDeviceMeasurements(cell.ID);

                // Remove measurements associated with this phasor
                foreach (Measurement measurement in measurements)
                {
                    if (measurement.PhasorSourceIndex == phasorDefinition.SourceIndex)
                        DeleteMeasurement(measurement.PointID);
                }

                // Remove phasor
                Phasor? phasorToRemove = phasors.FirstOrDefault(phasor => phasor.SourceIndex == phasorDefinition.SourceIndex);

                if (phasorToRemove is null)
                    continue;
                
                DeletePhasor(phasorToRemove.ID);
            }
            else
            {
                if (phasorDefinition.IsVoltage)
                    voltagePhasors.Add(phasorDefinition);
                else
                    currentPhasors.Add(phasorDefinition);
            }
        }

        if (dropAndAdd)
        {
            // Handle drop and (re)add phasors operation
            if (cell.PhasorDefinitions.Count > 0)
                DeletePhasorsForDevice(cell.ID); // <-- Try to avoid this step in future iterations...

            foreach (PhasorDefinition voltage in voltagePhasors)
            {
                Phasor phasor = NewPhasorWithTimestamps(voltage.UpdatedOn, voltage.CreatedOn);

                phasor.DeviceID = cell.ID;
                phasor.Label = voltage.Label;
                phasor.Type = 'V';
                phasor.Phase = string.IsNullOrWhiteSpace(voltage.Phase) ? GuessPhase(null, voltage.Label)[0] : voltage.Phase.Trim()[0];
                phasor.BaseKV = voltage.NominalVoltage ?? int.Parse(GuessBaseKV(null, voltage.Label, cell.Acronym));
                phasor.PrimaryVoltageID = null;
                phasor.SecondaryVoltageID = null;
                phasor.SourceIndex = voltage.SourceIndex;

                AddNewPhasor(phasor);

                SavePhasorMeasurement(cell, s_vphmSignalType, voltage, phasor.SourceIndex);
                SavePhasorMeasurement(cell, s_vphaSignalType, voltage, phasor.SourceIndex);

                phasor = QueryPhasorForDevice(cell.ID, phasor.SourceIndex);
                voltageIDMap[voltage] = phasor.ID;
            }

            foreach (PhasorDefinition current in currentPhasors)
            {
                Phasor phasor = NewPhasorWithTimestamps(current.UpdatedOn, current.CreatedOn);
                PhasorDefinition? associatedVoltage = current.GetAssociatedVoltage(cell);

                phasor.DeviceID = cell.ID;
                phasor.Label = current.Label;
                phasor.Type = 'I';
                phasor.Phase = string.IsNullOrWhiteSpace(current.Phase) ? GuessPhase(null, current.Label)[0] : current.Phase.Trim()[0];
                phasor.BaseKV = associatedVoltage?.NominalVoltage ?? 500;

                if (current.PrimaryVoltageID is null && associatedVoltage is not null && voltageIDMap.TryGetValue(associatedVoltage, out int voltageID))
                    phasor.PrimaryVoltageID = voltageID;
                else
                    phasor.PrimaryVoltageID = current.PrimaryVoltageID;

                phasor.SecondaryVoltageID = current.SecondaryVoltageID;
                phasor.SourceIndex = current.SourceIndex;

                AddNewPhasor(phasor);

                SavePhasorMeasurement(cell, s_iphmSignalType, current, phasor.SourceIndex);
                SavePhasorMeasurement(cell, s_iphaSignalType, current, phasor.SourceIndex);
            }
        }
        else
        {
            // Handle update phasors operation
            foreach (PhasorDefinition voltage in voltagePhasors)
            {
                Phasor phasor = QueryPhasorForDevice(cell.ID, voltage.SourceIndex);

                phasor.DeviceID = cell.ID;
                phasor.Label = voltage.Label;
                phasor.Type = 'V';
                phasor.Phase = voltage.Phase.Trim()[0];
                phasor.BaseKV = voltage.NominalVoltage ?? int.Parse(GuessBaseKV(null, voltage.Label, cell.Acronym));
                phasor.PrimaryVoltageID = null;
                phasor.SecondaryVoltageID = null;
                phasor.SourceIndex = voltage.SourceIndex;

                voltageIDMap[voltage] = phasor.ID;

                UpdatePhasor(phasor);

                SavePhasorMeasurement(cell, s_vphmSignalType, voltage, phasor.SourceIndex);
                SavePhasorMeasurement(cell, s_vphaSignalType, voltage, phasor.SourceIndex);
            }

            foreach (PhasorDefinition current in currentPhasors)
            {
                Phasor phasor = QueryPhasorForDevice(cell.ID, current.SourceIndex);
                PhasorDefinition? associatedVoltage = current.GetAssociatedVoltage(cell);

                phasor.DeviceID = cell.ID;
                phasor.Label = current.Label;
                phasor.Type = 'I';
                phasor.Phase = current.Phase.Trim()[0];
                phasor.BaseKV = associatedVoltage?.NominalVoltage ?? 500;

                if (current.PrimaryVoltageID is null && associatedVoltage is not null && voltageIDMap.TryGetValue(associatedVoltage, out int voltageID))
                    phasor.PrimaryVoltageID = voltageID;
                else
                    phasor.PrimaryVoltageID = current.PrimaryVoltageID;

                phasor.SecondaryVoltageID = current.SecondaryVoltageID;
                phasor.SourceIndex = current.SourceIndex;

                UpdatePhasor(phasor);

                SavePhasorMeasurement(cell, s_iphmSignalType, current, phasor.SourceIndex);
                SavePhasorMeasurement(cell, s_iphaSignalType, current, phasor.SourceIndex);
            }
        }
    }

    #region [ Device Table Operations ]

    private Device NewDevice()
    {
        return Table<Device>().NewRecord() ?? throw new NullReferenceException($"{nameof(NewDevice)} is null");
    }

    private int AddNewDevice(Device device)
    {
        if ((device.ProtocolID ?? 0) == 0)
            device.ProtocolID = IeeeC37_118ProtocolID;

        if (device.UniqueID == Guid.Empty)
            device.UniqueID = Guid.NewGuid();

        Table<Device>().AddNewRecord(device);

        // Re-query new device by unique acronym to get device ID
        Device? lookupDevice = Table<Device>().QueryRecordWhere("Acronym = {0}", device.Acronym);

        if (lookupDevice is null)
            throw new InvalidOperationException($"Failed while adding new device \"{device.Acronym}\" to database.");

        return lookupDevice.ID;
    }

    private void UpdateDevice(Device device)
    {
        if ((device.ProtocolID ?? 0) == 0)
            device.ProtocolID = IeeeC37_118ProtocolID;

        if (device.UniqueID == Guid.Empty)
            device.UniqueID = Guid.NewGuid();

        Table<Device>().UpdateRecord(device);
    }

    private Device QueryDeviceByID(int deviceID)
    {
        return Table<Device>().QueryRecordWhere("ID = {0}", deviceID) ?? NewDevice();
    }

    private IEnumerable<Device> QueryChildDevices(int deviceID)
    {
        return Table<Device>().QueryRecordsWhere("ParentID = {0}", deviceID)!;
    }

    #endregion

    #region [ Measurement Table Operations ]

    private IEnumerable<Measurement> QueryDeviceMeasurements(int deviceID)
    {
        return Table<Measurement>().QueryRecordsWhere("DeviceID = {0}", deviceID)!;
    }

    private Measurement NewMeasurement()
    {
        return Table<Measurement>().NewRecord()!;
    }

    private Measurement QueryMeasurement(string signalReference)
    {
        return Table<Measurement>().QueryRecordWhere("SignalReference = {0}", signalReference) ?? NewMeasurement();
    }

    public void DeleteMeasurement(int id)
    {
        Table<Measurement>().DeleteRecord(id);
    }

    private void AddNewOrUpdateMeasurement(Measurement measurement)
    {
        Table<Measurement>().AddNewOrUpdateRecord(measurement);
    }

    #endregion

    #region [ Phasor Table Operations ]

    private IEnumerable<Phasor> QueryPhasorsForDevice(int deviceID)
    {
        return Table<Phasor>().QueryRecordsWhere("DeviceID = {0}", deviceID).OrderBy(phasor => phasor?.SourceIndex ?? 0)!;
    }

    private void DeletePhasor(int id)
    {
        Table<Phasor>().DeleteRecord(id);
    }

    private int DeletePhasorsForDevice(int deviceID)
    {
        return Connection.ExecuteScalar<int>("DELETE FROM Phasor WHERE DeviceID = {0}", deviceID);
    }

    private Phasor NewPhasor()
    {
        return Table<Phasor>().NewRecord()!;
    }

    private Phasor NewPhasorWithTimestamps(DateTime updatedOn, DateTime createdOn)
    {
        Phasor newPhasor = NewPhasor();

        if (updatedOn > DateTime.MinValue)
            newPhasor.UpdatedOn = updatedOn;

        if (createdOn > DateTime.MinValue)
            newPhasor.CreatedOn = createdOn;

        return newPhasor;
    }

    private void AddNewPhasor(Phasor phasor)
    {
        Table<Phasor>().AddNewRecord(phasor);
    }

    private Phasor QueryPhasorForDevice(int deviceID, int sourceIndex)
    {
        return Table<Phasor>().QueryRecordWhere("DeviceID = {0} AND SourceIndex = {1}", deviceID, sourceIndex) ?? NewPhasor();
    }
    
    private void UpdatePhasor(Phasor phasor)
    {
        Table<Phasor>().UpdateRecord(phasor);
    }

    #endregion

    #region [ SignalType Table Operations ]

    private IEnumerable<SignalType> LoadSignalTypes(string source)
    {
        return Table<SignalType>().QueryRecordsWhere("Source = {0}", source)!;
    }

    private SignalType? GetDeviceSignalType(string acronym)
    {
        s_deviceSignalTypes ??= LoadSignalTypes("PMU").ToArray();
        return s_deviceSignalTypes.FirstOrDefault(deviceSignalType => string.Compare(deviceSignalType.Acronym, acronym, StringComparison.OrdinalIgnoreCase) == 0);
    }

    #endregion

    private static string CreatePointTag(string deviceAcronym, string signalTypeAcronym)
    {
        return CommonPhasorServices.CreatePointTag(s_companyAcronym, deviceAcronym, null, signalTypeAcronym);
    }

    private static string CreateIndexedPointTag(string deviceAcronym, string signalTypeAcronym, int signalIndex, string label)
    {
        return CommonPhasorServices.CreatePointTag(s_companyAcronym, deviceAcronym, null, signalTypeAcronym, label, signalIndex);
    }

    private static string CreatePhasorPointTag(string deviceAcronym, string signalTypeAcronym, string phasorLabel, string phase, int signalIndex, int baseKV)
    {
        return CommonPhasorServices.CreatePointTag(s_companyAcronym, deviceAcronym, null, signalTypeAcronym, phasorLabel, signalIndex, string.IsNullOrWhiteSpace(phase) ? '_' : phase.Trim()[0], baseKV);
    }

    /// <summary>
    /// Extracts a configuration frame from the database for a given device ID.
    /// </summary>
    /// <param name="deviceID">Device ID for which to extract configuration frame.</param>
    /// <returns>JSON configuration frame.</returns>
    [HttpGet]
    public ConfigurationFrame ExtractConfigurationFrame(int deviceID)
    {
        Device device = QueryDeviceByID(deviceID);

        if (device.ID == 0)
            return new ConfigurationFrame();

        ConfigurationFrame derivedFrame = new()
        {
            IDCode = device.AccessID,
            StationName = device.Name,
            IDLabel = device.Acronym,
            ConnectionString = device.ConnectionString,
            ProtocolID = device.ProtocolID ?? IeeeC37_118ProtocolID
        };

        if ((device.FramesPerSecond ?? 0) > 0)
            derivedFrame.FrameRate = (ushort)device.FramesPerSecond.GetValueOrDefault();

        if (device.ParentID is null)
        {
            IEnumerable<Device> devices = QueryChildDevices(deviceID);

            foreach (Device childDevice in devices)
            {
                // Create new configuration cell
                ConfigurationCell derivedCell = new()
                {
                    ID = childDevice.ID,
                    ParentID = device.ID,
                    UniqueID = device.UniqueID,
                    Longitude = device.Longitude,
                    Latitude = device.Latitude,
                    IDCode = childDevice.AccessID,
                    StationName = childDevice.Name,
                    IDLabel = childDevice.Acronym,
                    FrequencyDefinition = new FrequencyDefinition { Label = "Frequency" },

                    // Load proxied device field properties
                    TimeZone = childDevice.TimeZone,
                    HistorianID = childDevice.HistorianID,
                    InterconnectionID = childDevice.InterconnectionID,
                    VendorDeviceID = childDevice.VendorDeviceID,
                    CompanyID = childDevice.CompanyID,
                    ContactList = childDevice.ContactList
                };

                // Extract phasor definitions
                derivedCell.PhasorDefinitions.AddRange(QueryPhasorsForDevice(childDevice.ID).Select(GetPhasorDefinition));

                // Extract analog and digital definitions
                AddAnalogsAndDigitals(childDevice.ID, derivedCell);

                // Add cell to frame
                derivedFrame.Cells.Add(derivedCell);
            }

            if (derivedFrame.Cells.Count > 0)
            {
                derivedFrame.IsConcentrator = true;
            }
            else
            {
                // This is a directly connected device
                derivedFrame.IsConcentrator = false;

                ConfigurationCell derivedCell = new()
                {
                    ID = device.ID,
                    UniqueID = device.UniqueID,
                    Longitude = device.Longitude,
                    Latitude = device.Latitude,
                    ParentID = null,
                    IDCode = derivedFrame.IDCode,
                    StationName = device.Name,
                    IDLabel = device.Acronym,
                    FrequencyDefinition = new FrequencyDefinition { Label = "Frequency" },

                    // Load proxied device field properties
                    TimeZone = device.TimeZone,
                    HistorianID = device.HistorianID,
                    InterconnectionID = device.InterconnectionID,
                    VendorDeviceID = device.VendorDeviceID,
                    CompanyID = device.CompanyID,
                    ContactList = device.ContactList
                };

                // Extract phasor definitions
                derivedCell.PhasorDefinitions.AddRange(QueryPhasorsForDevice(device.ID).Select(GetPhasorDefinition));

                // Extract analog and digital definitions
                AddAnalogsAndDigitals(device.ID, derivedCell);

                // Add cell to frame
                derivedFrame.Cells.Add(derivedCell);
            }
        }
        else
        {
            derivedFrame.IsConcentrator = true;

            // Create new configuration cell
            ConfigurationCell derivedCell = new()
            {
                ID = device.ID,
                UniqueID = device.UniqueID,
                Longitude = device.Longitude,
                Latitude = device.Latitude,
                ParentID = null,
                IDCode = device.AccessID,
                StationName = device.Name,
                IDLabel = device.Acronym,
                FrequencyDefinition = new FrequencyDefinition { Label = "Frequency" },

                // Load proxied device field properties
                TimeZone = device.TimeZone,
                HistorianID = device.HistorianID,
                InterconnectionID = device.InterconnectionID,
                VendorDeviceID = device.VendorDeviceID,
                CompanyID = device.CompanyID,
                ContactList = device.ContactList
            };

            // Extract phasor definitions
            derivedCell.PhasorDefinitions.AddRange(QueryPhasorsForDevice(device.ID).Select(GetPhasorDefinition));

            // Extract analog and digital definitions
            AddAnalogsAndDigitals(device.ID, derivedCell);

            // Add cell to frame
            derivedFrame.Cells.Add(derivedCell);
        }

        return derivedFrame;
    }

    /// <summary>
    /// Marks a device configuration as synchronized with host system - called when configuration is successfully synced to database.
    /// </summary>
    /// <param name="configurationName">Configuration name to mark as synchronized.</param>
    [HttpPost]
    public void MarkConfigurationAsSynchronized(string configurationName)
    {
        // Remove marker that indicates device configuration is out of sync with host system
        PhasorMeasurementMapper.SaveConfigurationOutOfSyncMarker(configurationName, false);
    }

    private static PhasorDefinition GetPhasorDefinition(Phasor? phasor)
    {
        if (phasor is null)
            return new PhasorDefinition();

        return new PhasorDefinition
        {
            ID = phasor.ID,
            Label = phasor.Label,
            PhasorType = phasor.Type == 'V' ? "Voltage" : "Current",
            Phase = phasor.Phase.ToString(),
            PrimaryVoltageID = phasor.PrimaryVoltageID,
            SecondaryVoltageID = phasor.SecondaryVoltageID,
            NominalVoltage = phasor.BaseKV,
            SourceIndex = phasor.SourceIndex,
            CreatedOn = phasor.CreatedOn,
            UpdatedOn = phasor.UpdatedOn,
        };
    }

    private void AddAnalogsAndDigitals(int deviceID, ConfigurationCell derivedCell)
    {
        Measurement[] measurements = QueryDeviceMeasurements(deviceID).ToArray();

        // Extract analog definitions
        IEnumerable<Measurement> analogs = measurements.Where(measurement => measurement.SignalTypeID == AnalogSignalTypeID).OrderBy(measurement => new SignalReference(measurement.SignalReference).Index);
        derivedCell.AnalogDefinitions.AddRange(analogs.Select(analog => new AnalogDefinition
        {
            Label = analog.AlternateTag,
            AnalogType = nameof(AnalogType.SinglePointOnWave)
        }));

        // Extract digital definitions
        IEnumerable<Measurement> digitals = measurements.Where(measurement => measurement.SignalTypeID == DigitalSignalTypeID).OrderBy(measurement => new SignalReference(measurement.SignalReference).Index);
        derivedCell.DigitalDefinitions.AddRange(digitals.Select(digital => new DigitalDefinition { Label = digital.AlternateTag }));
    }

    /// <summary>
    /// Load configuration frame from source data: either a connection string, a configuration frame, or a connection settings object.
    /// </summary>
    /// <param name="sourceData">Serialized source data.</param>
    /// <returns>JSON configuration frame.</returns>
    [HttpGet]
    public ConfigurationFrame LoadConfigurationFrame(string sourceData)
    {
        IConfigurationFrame sourceFrame = GetConfigurationFrame(sourceData, out string connectionString);
        return LoadConfigurationFrame(sourceFrame, connectionString);
    }

    // Serializes an IConfigurationFrame instance to a JSON configuration frame
    private ConfigurationFrame LoadConfigurationFrame(IConfigurationFrame sourceFrame, string connectionString)
    {
        if (sourceFrame is ConfigurationErrorFrame)
            return new ConfigurationFrame();

        // Create a new simple concrete configuration frame for JSON serialization converted from equivalent configuration information
        int deviceID = 0, phasorID = -1; // Start phasor ID's at less than -1 since associated voltage == -1 is reserved as unselected

        ConfigurationFrame derivedFrame = new()
        {
            IDCode = sourceFrame.IDCode,
            FrameRate = sourceFrame.FrameRate,
            ConnectionString = connectionString,
        };

        foreach (IConfigurationCell sourceCell in sourceFrame.Cells)
        {
            // Create new derived configuration cell
            ConfigurationCell derivedCell = new()
            {
                ID = --deviceID, // Provide a negative index so any database lookup will return null
                ParentID = null,
                IDCode = sourceCell.IDCode,
                StationName = sourceCell.StationName,
                IDLabel = sourceCell.IDLabel,
                NominalFrequency = (double)sourceCell.NominalFrequency
            };

            if (sourceCell is ConfigurationCell3 configCell3)
            {
                derivedCell.UniqueID = configCell3.GlobalID;
                derivedCell.Longitude = configCell3.LongitudeM;
                derivedCell.Latitude = configCell3.LatitudeM;
            }

            // Create equivalent derived frequency definition
            IFrequencyDefinition? sourceFrequency = sourceCell.FrequencyDefinition;

            if (sourceFrequency is not null)
                derivedCell.FrequencyDefinition = new FrequencyDefinition { Label = sourceFrequency.Label };

            int sourceIndex = 0;

            // Create equivalent derived phasor definitions
            foreach (IPhasorDefinition sourcePhasor in sourceCell.PhasorDefinitions)
            {
                string configPhase = string.Empty;
                int? nominalVoltage = null;

                if (sourcePhasor is PhasorDefinition3 phasor3)
                {
                    // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                    switch (phasor3.PhasorComponent)
                    {
                        case PhasorComponent.ZeroSequence:
                            configPhase = "0";
                            break;
                        case PhasorComponent.PositiveSequence:
                            configPhase = "+";
                            break;
                        case PhasorComponent.NegativeSequence:
                            configPhase = "-";
                            break;
                        case PhasorComponent.PhaseA:
                            configPhase = "A";
                            break;
                        case PhasorComponent.PhaseB:
                            configPhase = "B";
                            break;
                        case PhasorComponent.PhaseC:
                            configPhase = "C";
                            break;
                        case PhasorComponent.ReservedPhase:
                            break;
                        default:
                            configPhase = string.Empty;
                            break;
                    }

                    if (Enum.TryParse(phasor3.UserFlags.ToString(), out VoltageLevel level))
                        nominalVoltage = level.Value();
                }

                DateTime now = DateTime.UtcNow;

                derivedCell.PhasorDefinitions.Add(new PhasorDefinition
                {
                    ID = --phasorID,
                    Label = sourcePhasor.Label,
                    PhasorType = sourcePhasor.PhasorType.ToString(),
                    Phase = configPhase,
                    NominalVoltage = nominalVoltage,
                    SourceIndex = ++sourceIndex,
                    CreatedOn = now,
                    UpdatedOn = now
                });
            }

            // Create equivalent derived analog definitions (assuming analog type = SinglePointOnWave)
            foreach (IAnalogDefinition sourceAnalog in sourceCell.AnalogDefinitions)
                derivedCell.AnalogDefinitions.Add(new AnalogDefinition
                {
                    Label = sourceAnalog.Label,
                    AnalogType = sourceAnalog.AnalogType.ToString()
                });

            // Create equivalent derived digital definitions
            foreach (IDigitalDefinition sourceDigital in sourceCell.DigitalDefinitions)
                derivedCell.DigitalDefinitions.Add(new DigitalDefinition { Label = sourceDigital.Label });

            // Add cell to frame
            derivedFrame.Cells.Add(derivedCell);
        }

        derivedFrame.IsConcentrator = derivedFrame.Cells.Count > 1;

        return derivedFrame;
    }

    // Attempts to derive a configuration frame from source data - can be a connection string, a configuration frame, or a connection settings object
    private static IConfigurationFrame GetConfigurationFrame(string sourceData, out string connectionString)
    {
        connectionString = "";

        try
        {
            // Try deserializing input as connection settings
            ConnectionSettings? connectionSettings;

            SoapFormatter formatter = new()
            {
                AssemblyFormat = FormatterAssemblyStyle.Simple,
                TypeFormat = FormatterTypeStyle.TypesWhenNeeded,
                Binder = Serialization.LegacyBinder
            };

            using (MemoryStream source = new(Encoding.UTF8.GetBytes(sourceData)))
                connectionSettings = formatter.Deserialize(source) as ConnectionSettings;

            if (connectionSettings is not null)
            {
                // If provided input was a connection settings object, get a valid connection string
                connectionString = connectionSettings.ConnectionString;

                Dictionary<string, string> connectionStringKeyValues = connectionString.ParseKeyValuePairs();

                connectionString = "transportProtocol=" + connectionSettings.TransportProtocol + ";" + connectionStringKeyValues.JoinKeyValuePairs();

                if (connectionSettings.ConnectionParameters is not null)
                {
                    // Handle protocol-specific connection parameters
                    switch (connectionSettings.PhasorProtocol)
                    {
                        case PhasorProtocol.BPAPDCstream:
                            if (connectionSettings.ConnectionParameters is Gemstone.PhasorProtocols.BPAPDCstream.ConnectionParameters bpaParameters)
                                connectionString += "; iniFileName=" + bpaParameters.ConfigurationFileName + "; refreshConfigFileOnChange=" + bpaParameters.RefreshConfigurationFileOnChange + "; parseWordCountFromByte=" + bpaParameters.ParseWordCountFromByte;
                            break;
                        case PhasorProtocol.FNET:
                            if (connectionSettings.ConnectionParameters is Gemstone.PhasorProtocols.FNET.ConnectionParameters fnetParameters)
                                connectionString += "; timeOffset=" + fnetParameters.TimeOffset + "; stationName=" + fnetParameters.StationName + "; frameRate=" + fnetParameters.FrameRate + "; nominalFrequency=" + (int)fnetParameters.NominalFrequency;
                            break;
                        case PhasorProtocol.SelFastMessage:
                            if (connectionSettings.ConnectionParameters is Gemstone.PhasorProtocols.SelFastMessage.ConnectionParameters selParameters)
                                connectionString += "; messagePeriod=" + selParameters.MessagePeriod;
                            break;
                        case PhasorProtocol.IEC61850_90_5:
                            if (connectionSettings.ConnectionParameters is Gemstone.PhasorProtocols.IEC61850_90_5.ConnectionParameters iecParameters)
                                connectionString += "; useETRConfiguration=" + iecParameters.UseETRConfiguration + "; guessConfiguration=" + iecParameters.GuessConfiguration + "; parseRedundantASDUs=" + iecParameters.ParseRedundantASDUs + "; ignoreSignatureValidationFailures=" + iecParameters.IgnoreSignatureValidationFailures + "; ignoreSampleSizeValidationFailures=" + iecParameters.IgnoreSampleSizeValidationFailures;
                            break;
                        case PhasorProtocol.Macrodyne:
                            if (connectionSettings.ConnectionParameters is Gemstone.PhasorProtocols.Macrodyne.ConnectionParameters macrodyneParameters)
                                connectionString += "; protocolVersion=" + macrodyneParameters.ProtocolVersion + "; iniFileName=" + macrodyneParameters.ConfigurationFileName + "; refreshConfigFileOnChange=" + macrodyneParameters.RefreshConfigurationFileOnChange + "; deviceLabel=" + macrodyneParameters.DeviceLabel;
                            break;
                    }
                }

                connectionString += "; accessID=" + connectionSettings.PmuID;
                connectionString += "; phasorProtocol=" + connectionSettings.PhasorProtocol;

                // Parse connection string and return retrieved configuration frame
                return RequestConfigurationFrame(connectionString);
            }

            // Try deserializing input as a configuration frame
            IConfigurationFrame? configurationFrame;

            using (MemoryStream source = new(Encoding.UTF8.GetBytes(sourceData)))
                configurationFrame = formatter.Deserialize(source) as IConfigurationFrame;

            // Finally, assume input is simply a connection string and attempt to return retrieved configuration frame
            return configurationFrame ?? RequestConfigurationFrame(sourceData);
        }
        catch
        {
            try
            {
                return RequestConfigurationFrame(sourceData);
            }
            catch
            {
                return new ConfigurationErrorFrame();
            }
        }
    }

    private static IConfigurationFrame RequestConfigurationFrame(string connectionString)
    {
        using CommonPhasorServices phasorServices = new();

        phasorServices.StatusMessage += (_, e) => WebServer.Logger.LogInformation($"[{nameof(PhasorOpsController)}] {e.Argument.Replace("**", "")}");
        phasorServices.ProcessException += (_, e) => WebServer.Logger.LogError(e.Argument, $"[{nameof(PhasorOpsController)}] {e.Argument.Message}");

        return phasorServices.RequestDeviceConfiguration(connectionString);
    }

    private static bool PhaseMatchExact(string phaseLabel, IEnumerable<string> phaseMatches)
    {
        return phaseMatches.Any(match => phaseLabel.Equals(match, StringComparison.Ordinal));
    }

    private static bool PhaseEndsWith(string phaseLabel, IEnumerable<string> phaseMatches, bool ignoreCase)
    {
        return phaseMatches.Any(match => phaseLabel.EndsWith(match, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
    }

    private static bool PhaseStartsWith(string phaseLabel, IEnumerable<string> phaseMatches, bool ignoreCase)
    {
        return phaseMatches.Any(match => phaseLabel.StartsWith(match, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
    }

    private static bool PhaseContains(string phaseLabel, IEnumerable<string> phaseMatches, bool ignoreCase)
    {
        return phaseMatches.Any(match => phaseLabel.IndexOf(match, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) > -1);
    }

    private static bool PhaseMatchHighConfidence(string phaseLabel, IEnumerable<string> containsMatches, IEnumerable<string> endsWithMatches)
    {
        IEnumerable<string> phaseMatches = containsMatches as string[] ?? containsMatches.ToArray();

        if (PhaseEndsWith(phaseLabel, phaseMatches, true))
            return true;

        if (PhaseStartsWith(phaseLabel, phaseMatches, true))
            return true;

        foreach (string match in phaseMatches.Concat(endsWithMatches))
        {
            string[] variations = [$" {match}", $"_{match}", $"-{match}", $".{match}"];

            if (PhaseEndsWith(phaseLabel, variations, false))
                return true;
        }

        foreach (string match in phaseMatches)
        {
            string[] variations = [$" {match} ", $"_{match}_", $"-{match}-", $"-{match}_", $"_{match}-", $".{match}."];

            if (PhaseContains(phaseLabel, variations, false))
                return true;
        }

        return false;
    }

    private static bool PhaseMatchLowConfidence(string phaseLabel, IEnumerable<string> phaseMatches)
    {
        foreach (string match in phaseMatches)
        {
            string[] variations = [$" {match}", $"{match} ", $"_{match}", $"{match}_", $"_{match}_", $"-{match}", $"{match}-", $"-{match}-", $"-{match}_", $"_{match}-", $".{match}", $"{match}.", $".{match}."];

            if (PhaseContains(phaseLabel, variations, true))
                return true;
        }

        return false;
    }

    private static string GuessPhase(string? phase, string phasorLabel)
    {
        if (!string.IsNullOrWhiteSpace(phase) && phase != "+" && !phase.EndsWith("?"))
            return phase;

        // Handle high confidence phase matches when no phase is defined or when phase is "+" - since positive sequence is often default value, it's treated with suspicion
        if (string.IsNullOrWhiteSpace(phase) || phase == "+")
        {
            if (PhaseMatchExact(phasorLabel, ["V1PM", "I1PM"]) || PhaseMatchHighConfidence(phasorLabel, ["V1", "VP", "I1", "IP", "VSEQ1", "ISEQ1"], ["POS", "V1PM", "I1PM", "PS", "PSV", "PSI"]) || PhaseEndsWith(phasorLabel, ["+SV", "+SI", "+V", "+I"], true))
                return "+";

            if (PhaseMatchExact(phasorLabel, ["V0PM", "I0PM", "VZPM", "IZPM"]) || PhaseMatchHighConfidence(phasorLabel, ["V0", "I0", "VSEQ0", "ISEQ0"], ["ZERO", "ZPV", "ZPI", "VSPM", "V0PM", "I0PM", "VZPM", "IZPM", "ZS", "ZSV", "ZSI"]) || PhaseEndsWith(phasorLabel, ["0SV", "0SI"], true))
                return "0";

            if (PhaseMatchExact(phasorLabel, ["VAPM", "IAPM"]) || PhaseMatchHighConfidence(phasorLabel, ["VA", "IA"], ["APV", "API", "VAPM", "IAPM", "AV", "AI"]))
                return "A";

            if (PhaseMatchExact(phasorLabel, ["VBPM", "IBPM"]) || PhaseMatchHighConfidence(phasorLabel, ["VB", "IB"], ["BPV", "BPI", "VBPM", "IBPM", "BV", "BI"]))
                return "B";

            if (PhaseMatchExact(phasorLabel, ["VCPM", "ICPM"]) || PhaseMatchHighConfidence(phasorLabel, ["VC", "IC"], ["CPV", "CPI", "VCPM", "ICPM", "CV", "CI"]))
                return "C";

            if (PhaseMatchExact(phasorLabel, ["VNPM", "INPM"]) || PhaseMatchHighConfidence(phasorLabel, ["VN", "IN"], ["NEUT", "NPV", "NPI", "VNPM", "INPM", "NV", "NI"]))
                return "N";

            if (PhaseMatchExact(phasorLabel, ["V2PM", "I2PM"]) || PhaseMatchHighConfidence(phasorLabel, ["V2", "I2", "VSEQ2", "ISEQ2"], ["NEG", "-SV", "-SI", "V2PM", "I2PM", "NS", "NSV", "NSI"]))
                return "-";
        }

        // Handle lower confidence phase matches only when phase is not defined
        if (string.IsNullOrWhiteSpace(phase))
        {
            // Since positive sequence is the default and always treated with accuracy suspicion, verify its value first
            if (PhaseMatchLowConfidence(phasorLabel, ["V1", "VP", "I1", "IP", "POS", "V1PM", "I1PM", "PS", "PSV", "PSI", "+SV", "+SI", "+V", "+I"]))
                return "+?";

            if (PhaseMatchLowConfidence(phasorLabel, ["V0", "I0", "ZERO", "ZPV", "ZPI", "VSPM", "VZPM", "IZPM", "ZS", "ZSV", "ZSI", "0SV", "0SI"]))
                return "0?";

            if (PhaseMatchLowConfidence(phasorLabel, ["VA", "IA", "APV", "API", "VAPM", "IAPM", "AV", "AI"]))
                return "A?";

            if (PhaseMatchLowConfidence(phasorLabel, ["VB", "IB", "BPV", "BPI", "VBPM", "IBPM", "BV", "BI"]))
                return "B?";

            if (PhaseMatchLowConfidence(phasorLabel, ["VC", "IC", "CPV", "CPI", "VCPM", "ICPM", "CV", "CI"]))
                return "C?";

            if (PhaseMatchLowConfidence(phasorLabel, ["VN", "IN", "NEUT", "NPV", "NPI", "VNPM", "INPM", "NV", "NI"]))
                return "N?";

            if (PhaseMatchLowConfidence(phasorLabel, ["V2", "I2", "NEG", "-SV", "-SI", "V2PM", "I2PM", "NS", "NSV", "NSI"]))
                return "-?";

            // Test for contains after checks with separators
            if (PhaseContains(phasorLabel, ["V1", "I1", "POS", "V1PM", "I1PM", "PS", "PSV", "PSI", "+SV", "+SI", "+V", "+I"], true))
                return "+?";

            if (PhaseContains(phasorLabel, ["V0", "I0", "ZERO", "ZPV", "ZPI", "VSPM", "VZPM", "IZPM", "ZS", "ZSV", "ZSI", "0SV", "0SI"], true))
                return "0?";

            if (PhaseContains(phasorLabel, ["VA", "IA", "APV", "API", "VAPM", "IAPM", "AV", "AI"], true))
                return "A?";

            if (PhaseContains(phasorLabel, ["VB", "IB", "BPV", "BPI", "VBPM", "IBPM", "BV", "BI"], true))
                return "B?";

            if (PhaseContains(phasorLabel, ["VC", "IC", "CPV", "CPI", "VCPM", "ICPM", "CV", "CI"], true))
                return "C?";

            if (PhaseContains(phasorLabel, ["VN", "IN", "NEUT", "NPV", "NPI", "VNPM", "INPM", "NV", "NI"], true))
                return "N?";

            if (PhaseContains(phasorLabel, ["V2", "I2", "NEG", "-SV", "-SI", "V2PM", "I2PM", "NS", "NSV", "NSI"], true))
                return "-?";

            // -V and -I may match too often, so check these last
            if (PhaseMatchLowConfidence(phasorLabel, ["-V", "-I"]) || PhaseContains(phasorLabel, ["-V", "-I"], true))
                return "-?";

            return "+?";
        }

        return phase;
    }

    private static string GuessBaseKV(string? baseKV, string phasorLabel, string deviceLabel)
    {
        if (!string.IsNullOrWhiteSpace(baseKV) && int.TryParse(baseKV, out int value) && value > 0)
            return baseKV;

        // Check phasor label before device
        foreach (string voltageLevel in s_commonVoltageLevels)
        {
            if (phasorLabel.IndexOf(voltageLevel, StringComparison.Ordinal) > -1)
                return voltageLevel;
        }

        foreach (string voltageLevel in s_commonVoltageLevels)
        {
            if (deviceLabel.IndexOf(voltageLevel, StringComparison.Ordinal) > -1)
                return voltageLevel;
        }

        return "0";
    }

    private static string GetPhasorPhase(IPhasorDefinition phasor)
    {
        string configPhase = string.Empty;

        if (phasor is PhasorDefinition3 phasor3)
        {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (phasor3.PhasorComponent)
            {
                case PhasorComponent.ZeroSequence:
                    configPhase = "0";
                    break;
                case PhasorComponent.PositiveSequence:
                    configPhase = "+";
                    break;
                case PhasorComponent.NegativeSequence:
                    configPhase = "-";
                    break;
                case PhasorComponent.PhaseA:
                    configPhase = "A";
                    break;
                case PhasorComponent.PhaseB:
                    configPhase = "B";
                    break;
                case PhasorComponent.PhaseC:
                    configPhase = "C";
                    break;
                case PhasorComponent.ReservedPhase:
                    break;
                default:
                    configPhase = string.Empty;
                    break;
            }
        }

        return GuessPhase(configPhase, phasor.Label);
    }

    private static string GetPhasorBaseKV(IPhasorDefinition phasor, string deviceAcronym)
    {
        string configBaseKV = "0";

        if (phasor is PhasorDefinition3 phasor3 && Enum.TryParse(phasor3.UserFlags.ToString(), out VoltageLevel level))
            configBaseKV = level.Value().ToString();

        return GuessBaseKV(configBaseKV, phasor.Label, deviceAcronym);
    }

    private static float GetMagnitudeMultiplier(IPhasorDefinition phasor)
    {
        return phasor is PhasorDefinition3 phasor3 ? phasor3.MagnitudeMultiplier : 1.0F;
    }

    private static float GetAngleAdder(IPhasorDefinition phasor)
    {
        return phasor is PhasorDefinition3 phasor3 ? phasor3.AngleAdder : 0.0F;
    }

    private static Tuple<float, float>? GetAnalogScalarSet(IAnalogDefinition analog)
    {
        return analog is AnalogDefinition3 analog3 ? new Tuple<float, float>(analog3.Adder, analog3.Multiplier) : null;
    }

    private static Tuple<float, float>?[] GetAnalogScalars(AnalogDefinitionCollection analogs)
    {
        return analogs.Select(GetAnalogScalarSet).ToArray();
    }

    private static int? s_ieeeC37_118ProtocolID;
    private static int? s_analogSignalTypeID;
    private static int? s_digitalSignalTypeID;

    private static SignalType[]? s_deviceSignalTypes;
    private static SignalType? s_freqSignalType;
    private static SignalType? s_dfdtSignalType;
    private static SignalType? s_flagSignalType;
    private static SignalType? s_alogSignalType;
    private static SignalType? s_digiSignalType;
    private static SignalType? s_iphmSignalType;
    private static SignalType? s_iphaSignalType;
    private static SignalType? s_vphmSignalType;
    private static SignalType? s_vphaSignalType;

    private static readonly string[] s_commonVoltageLevels;
    private static readonly string s_companyAcronym;

    static PhasorOpsController()
    {
        s_commonVoltageLevels = CommonVoltageLevels.Values;

        try
        {
            s_companyAcronym = ConfigSettings.Default.System.CompanyAcronym;
        }
        catch (Exception ex)
        {
            Logger.SwallowException(ex, nameof(PhasorOpsController), "Failed to load company acronym from configuration settings. Defaulting to \"GPA\".");
            s_companyAcronym = "GPA";
        }
    }
}