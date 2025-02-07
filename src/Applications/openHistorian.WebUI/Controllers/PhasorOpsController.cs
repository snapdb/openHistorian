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
using Phasor = Gemstone.Timeseries.Model.Phasor;
using ConfigSettings = Gemstone.Configuration.Settings;

namespace openHistorian.WebUI.Controllers;

/// <summary>
/// Controller for managing phasor operations.
/// </summary>
[Route("api/PhasorOps")]
[ApiController]
public class PhasorOpsController : Controller, ISupportConnectionTest
{
    private static int? s_ieeeC37_118ProtocolID;
    private static int? s_analogSignalTypeID;
    private static int? s_digitalSignalTypeID;

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
            m_disposed = true;          // Prevent duplicate dispose.
            base.Dispose(disposing);    // Call base class Dispose().
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
            if (!IsDisposed)
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
            EnqueueMessage(JsonSerializer.Serialize(new { level, message }), "StatusMessage");
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
                    Phasors = cell.PhasorValues.Select(phasor => new PhasorValue{Angle = phasor.Angle.ToDegrees(), Magnitude = phasor.Magnitude}).ToArray(),
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

                string[] servers = setting.Split((char[])[','], StringSplitOptions.RemoveEmptyEntries);

                foreach (string server in servers)
                {
                    string[] parts = server.Split((char[])['/'], StringSplitOptions.RemoveEmptyEntries);

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

        private static void Dispose(CacheEntryRemovedArguments arguments)
        {
            if (arguments.RemovedReason != CacheEntryRemovedReason.Removed)
                return;

            if (arguments.CacheItem.Value is ConnectionCache cache)
                cache.Dispose();
        }
    }

    /// <inheritdoc />
    [HttpGet, Route("Connect/{connectionString}/{expiration:double?}")]
    public Task<IActionResult> Connect(string connectionString, double? expiration, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        ConnectionCache cache = ConnectionCache.Create(this, connectionString, expiration ?? 1.0D);

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
            await webSocket.SendAsync(
                new ArraySegment<byte>(messageBytes),
                WebSocketMessageType.Text,
                true,
                cancellationToken
            );
        }
    }

    private Device NewDevice()
    {
        return Table<Device>().NewRecord() ?? throw new NullReferenceException($"{nameof(NewDevice)} is null");
    }

    private Device QueryDeviceByID(int deviceID)
    {
        return Table<Device>().QueryRecordWhere("ID = {0}", deviceID) ?? NewDevice();
    }

    private IEnumerable<Device> QueryChildDevices(int deviceID)
    {
        return Table<Device>().QueryRecordsWhere("ParentID = {0}", deviceID)!;
    }

    private IEnumerable<Phasor> QueryPhasorsForDevice(int deviceID)
    {
        return Table<Phasor>().QueryRecordsWhere("DeviceID = {0}", deviceID).OrderBy(phasor => phasor?.SourceIndex ?? 0)!;
    }

    private IEnumerable<Measurement> QueryDeviceMeasurements(int deviceID)
    {
        return Table<Measurement>().QueryRecordsWhere("DeviceID = {0}", deviceID)!;
    }

    private int GetProtocolID(string protocolAcronym)
    {
        if (Enum.TryParse(protocolAcronym, true, out PhasorProtocol protocol))
            return Table<Protocol>().QueryRecordWhere("Acronym = {0}", protocol.ToString())?.ID ??
                   Table<Protocol>().QueryRecordWhere("Acronym = {0}", getOldProtocolName(protocol))?.ID ?? 0;

        return 0;

        // Helps with older connection setting files where Acronym field may be case-sensitive
        static string getOldProtocolName(PhasorProtocol protocol) => protocol switch
        {
            PhasorProtocol.IEEEC37_118V2 => "IeeeC37_118V2",
            PhasorProtocol.IEEEC37_118V1 => "IeeeC37_118V1",
            PhasorProtocol.IEEEC37_118D6 => "IeeeC37_118D6",
            PhasorProtocol.IEEE1344 => "Ieee1344",
            PhasorProtocol.BPAPDCstream => "BpaPdcStream",
            PhasorProtocol.FNET => "FNet",
            PhasorProtocol.SelFastMessage => "SelFastMessage",
            PhasorProtocol.Macrodyne => "Macrodyne",
            PhasorProtocol.IEC61850_90_5 => "Iec61850_90_5",
            _ => protocol.ToString()
        };
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
            IDCode = (ushort)device.AccessID,
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
                    IDCode = (ushort)childDevice.AccessID,
                    StationName = childDevice.Name,
                    IDLabel = childDevice.Acronym,
                    FrequencyDefinition = new FrequencyDefinition { Label = "Frequency" }
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
                    FrequencyDefinition = new FrequencyDefinition { Label = "Frequency" }
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
                IDCode = (ushort)device.AccessID,
                StationName = device.Name,
                IDLabel = device.Acronym,
                FrequencyDefinition = new FrequencyDefinition { Label = "Frequency" }
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
            DestinationPhasorID = phasor.DestinationPhasorID,
            NominalVoltage = phasor.BaseKV,
            SourceIndex = phasor.SourceIndex,
            CreatedOn = phasor.CreatedOn,
            UpdatedOn = phasor.UpdatedOn
        };
    }

    private void AddAnalogsAndDigitals(int deviceID, ConfigurationCell derivedCell)
    {
        Measurement[] measurements = QueryDeviceMeasurements(deviceID).ToArray();

        // Extract analog definitions
        IEnumerable<Measurement> analogs = measurements.Where(measurement => measurement.SignalTypeID == AnalogSignalTypeID).OrderBy(measurement => new SignalReference(measurement.SignalReference).Index);
        derivedCell.AnalogDefinitions.AddRange(analogs.Select(analog => new AnalogDefinition { Label = analog.AlternateTag, AnalogType = nameof(AnalogType.SinglePointOnWave) }));

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
        int protocolID = 0, deviceID = 0, phasorID = -1; // Start phasor ID's at less than -1 since associated voltage == -1 is reserved as unselected

        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            Dictionary<string, string> settings = connectionString.ParseKeyValuePairs();
            protocolID = GetProtocolID(settings["phasorProtocol"]);
        }

        ConfigurationFrame derivedFrame = new()
        {
            IDCode = sourceFrame.IDCode,
            FrameRate = sourceFrame.FrameRate,
            ConnectionString = connectionString,
            ProtocolID = protocolID
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
                IDLabel = sourceCell.IDLabel
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
                derivedCell.AnalogDefinitions.Add(new AnalogDefinition { Label = sourceAnalog.Label, AnalogType = sourceAnalog.AnalogType.ToString() });

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
}

