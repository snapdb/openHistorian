//******************************************************************************************************
//  DeviceSwitch.cs - Gbtc
//
//  Copyright © 2025, Grid Protection Alliance.  All Rights Reserved.
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
//  04/25/2025 - C. Lackner
//       Generated original version of source code.
//
//******************************************************************************************************

using Gemstone.Data;
using Gemstone.Data.DataExtensions;
using Gemstone.Data.Model;
using Gemstone.Diagnostics;
using Gemstone.Timeseries;
using Gemstone.Timeseries.Adapters;
using GrafanaAdapters.Model.Database;
using System.ComponentModel;
using System.Data;
using System.Text;
using ConfigSettings = Gemstone.Configuration.Settings;
using Device = Gemstone.Timeseries.Model.Device;

namespace DynamicCalculator;

/// <summary>
/// The DynamicCalculator is an action adapter which takes multiple
/// input measurements and performs a calculation on those measurements
/// to generate its own calculated measurement.
/// </summary>
[Description("Device Switch: Copies one of the provided devices if the device state is acceptable")]
[UIResource("AdaptersUI", $".DynamicCalculator.DeviceSwitch.main.js")]
[UIResource("AdaptersUI", $".DynamicCalculator.DeviceSwitch.chunk.js")]
public class DeviceSwitch: FacileActionAdapterBase 
{ 
    #region [ Members ]
        
    // Nested Types
    private class MeasurementMatch
    {
        public string SignalType { get; }
        public string PointTag { get; }
        public char? PhasorType { get; }
        public int SourceIndex { get; }
        public char? Phase { get; }
        public Guid SignalID { get; }


        public MeasurementMatch(DataRow row)
        {
            SignalType = row.ConvertField<string>("SignalType");
            PhasorType = row.ConvertField<char?>("PhasorType");
            SourceIndex = row.ConvertField<int>("SourceIndex");
            Phase = row.ConvertField<char?>("Phase");
            SignalID = row.ConvertField<Guid>("SignalID");
            PointTag = row.ConvertField<string>("PointTag");
        }
    }

    // Constants



    // Fields
    private Dictionary<MeasurementKey, MeasurementKey> m_tagMapping;
    private Dictionary<string, MeasurementMatch[]> m_inputDeviceMatches;
    private MeasurementMatch[] m_outputDeviceMatches;
    private MeasurementKey[] m_outputMeasurementKeys;

    private string m_currentInputDevice;
    private string m_outputDevice;

    private Device[] m_inputDevices;
    private int m_acceptedState;

    #endregion

    #region [ Constructors ]

    /// <summary>
    /// Creates a new instance of the <see cref="DeviceSwitch"/>.
    /// </summary>
    public DeviceSwitch()
    {}

    #endregion

    #region [ Properties ]

    /// <summary>
    /// Gets or sets primary keys of input measurements the dynamic calculator expects.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override MeasurementKey[]? InputMeasurementKeys // Hidden from UI
    {
        get => base.InputMeasurementKeys;
        set
        {
            base.InputMeasurementKeys = value;
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override IMeasurement[]? OutputMeasurements // Hidden from UI
    {
        get => base.OutputMeasurements;
        set
        {
            base.OutputMeasurements = value;
        }
    }

    [ConnectionStringParameter]
    [Description("Defines the output device.")]
    [DefaultValue("")]
    public string OutputDevice { get; set; }

    [ConnectionStringParameter]
    [Description("Defines the input devices(seperated by comma).")]
    [DefaultValue("")]
    public string InputDevices { get; set; }

    [ConnectionStringParameter]
    [Description("Defines the acceptable state of the input device.")]
    [DefaultValue(1)]
    public int AcceptableState { get; set; }


    public override bool SupportsTemporalProcessing => false;

    /// <summary>
    /// Gets or sets the flag indicating whether to use the latest
    /// received values to fill in values missing from the current frame.
    /// </summary>
    public bool UseLatestValues => false;
        
    /// <summary>
    /// Returns the detailed status of the data input source.
    /// </summary>
    public override string Status
    {
        get
        {
            StringBuilder status = new();

            status.Append(base.Status);
            status.AppendLine();
            status.AppendLine($"           Output Device: {OutputDevice}");
            status.AppendLine($"    Current Input Device: {m_currentInputDevice}");
            status.AppendLine($"       All Input Devices: {string.Join(", ",m_inputDevices.Select(d => d.Acronym))}");
            status.AppendLine($"  Number of tags matched: {m_inputDeviceMatches.Count()}");

            return status.ToString();
        }
    }

    /// <inheritdoc/>
    public override void QueueMeasurementsForProcessing(IEnumerable<IMeasurement> measurements)
    {
        List<IMeasurement> outputs = [];
        ValidateDevice();
        foreach (IMeasurement measurement in measurements)
        {
            //Per Ritchie this should not happen but somehow happens anyway
            // #ToDo Figure out why
            if (measurement is null)
                continue;

            if (m_tagMapping.TryGetValue(measurement.Key, out MeasurementKey outputKey))
            {
                IMeasurement outMeasurement = new Measurement()
                {
                    Timestamp = measurement.Timestamp,
                    Value = measurement.Value,
                };
                measurement.Metadata = outputKey.Metadata;
                outputs.Add(measurement);
            }
        }

        OnNewMeasurements(outputs);

    }
    #endregion

    #region [ Methods ]

    /// <summary>
    /// Initializes <see cref="DynamicCalculator"/>.
    /// </summary>
    public override void Initialize()
    {
        Dictionary<string, string> settings = Settings;
        base.Initialize();


        // Load required parameters
        if (!settings.TryGetValue(nameof(OutputDevice), out string? setting))
            throw new ArgumentException($"{nameof(OutputDevice)} is a required requried Setting");
        else
            m_outputDevice = setting;

        if (!settings.TryGetValue(nameof(InputDevices), out setting))
            throw new ArgumentException($"{nameof(InputDevices)} is a required requried Setting");

        string[] inputDeviceAcronyms = setting.Split(',');

        if (inputDeviceAcronyms.Length == 0)
            throw new ArgumentException($"At least 1 {nameof(InputDevices)} must be specified");

        m_currentInputDevice = inputDeviceAcronyms[0];

        using (AdoDataConnection connection = new(ConfigSettings.Instance))
        {
            TableOperations<Device> tblOperation = new(connection);
            m_inputDevices = tblOperation.QueryRecordsWhere($"Acronym IN ('{string.Join("','", inputDeviceAcronyms)}')").ToArray();
        }

        if (!settings.TryGetValue(nameof(AcceptableState), out setting) || !int.TryParse(setting, out m_acceptedState))
            throw new ArgumentException($"{nameof(AcceptableState)} is a required requried Setting");


        m_outputMeasurementKeys = ParseInputMeasurementKeys(DataSource, true, $"Filter ActiveMeasurements WHERE Device LIKE '{m_outputDevice}' AND SIGNALTYPE NOT LIKE 'STAT'");
        m_outputDeviceMatches = DataSource.Tables["ActiveMeasurements"].Select().Where(row => m_outputMeasurementKeys.Contains(MeasurementKey.LookUpBySignalID(row.ConvertField<Guid>("SignalID"))))
            .Select(r => new MeasurementMatch(r)).ToArray();

        m_inputDeviceMatches = new Dictionary<string, MeasurementMatch[]>();
        List<MeasurementKey> inputKeys = new();

        foreach (string device in inputDeviceAcronyms)
        {
            MeasurementKey[] deviceMeasurements = ParseInputMeasurementKeys(DataSource, true, $"Filter ActiveMeasurements WHERE Device LIKE '{device}' AND SIGNALTYPE NOT LIKE 'STAT'");
            inputKeys.AddRange(deviceMeasurements);
            m_inputDeviceMatches[device] = DataSource.Tables["ActiveMeasurements"].Select().Where(row => deviceMeasurements.Contains(MeasurementKey.LookUpBySignalID(row.ConvertField<Guid>("SignalID"))))
                .Select(r => new MeasurementMatch(r)).ToArray();

        }

        InputMeasurementKeys = inputKeys.ToArray();

        m_tagMapping = GenerateMapping();
    }

    public override string GetShortStatus(int maxLength)
    {
        throw new NotImplementedException();
    }

    private Dictionary<MeasurementKey, MeasurementKey> GenerateMapping()
    {
        Dictionary<MeasurementKey, MeasurementKey> mapping = new();
        foreach (MeasurementMatch output in m_outputDeviceMatches)
        {
            MeasurementMatch[] input = m_inputDeviceMatches[m_currentInputDevice].Where(m => Match(m, output)).ToArray();
            if (input.Length == 0)
            {
                OnStatusMessage(MessageLevel.Warning, $"Device {m_currentInputDevice} has no tag matching {output.PointTag}");
                continue;
            }
            if (input.Length > 1)
            {
                string error = $"Device {m_currentInputDevice} has multiple tags matching {output.PointTag}: ";
                int i = input.Length;
                while (i < input.Length && mapping.ContainsKey(MeasurementKey.LookUpBySignalID(input[i].SignalID)))
                    i++;

                if (i == input.Length)
                {
                    OnStatusMessage(MessageLevel.Warning, $"{error} - All matches already used");
                    continue;
                }
                OnStatusMessage(MessageLevel.Warning, $"{error} - using {input[i].PointTag}");
                input = new MeasurementMatch[] { input[i] };
            }
          
            mapping.Add(MeasurementKey.LookUpBySignalID(input.First().SignalID), MeasurementKey.LookUpBySignalID(output.SignalID));
            
        }
        return mapping;
    }

    private bool Match(MeasurementMatch input, MeasurementMatch output)
    {
        if (input.Phase != output.Phase)
            return false;
        if (input.SignalType != output.SignalType)
            return false;
        if (input.PhasorType != output.PhasorType)
            return false;
        if (input.SourceIndex != output.SourceIndex)
            return false;
        return true;
    }

    private void ValidateDevice()
    {
        Dictionary<string, int> stateLookup = new();

        using AdoDataConnection connection = new(ConfigSettings.Instance);
        TableOperations<DeviceStatus> tblOperation = new(connection);
        foreach (Device device in m_inputDevices)
        {
            DeviceStatus status = tblOperation.QueryRecordWhere("DeviceID = {0}", device.ID);
            if (status == null)
                continue;
            stateLookup[device.Acronym] = status.StateID;
        }

        if (stateLookup.TryGetValue(m_currentInputDevice, out int state) && state == m_acceptedState)
            return;

        int i = 0;
        while (i < m_inputDevices.Length && stateLookup.TryGetValue(m_inputDevices[i].Acronym, out state) && state != m_acceptedState)
            i++;

        if (i == m_inputDevices.Length)
        {
            OnStatusMessage(MessageLevel.Warning, $"No input device in acceptable State. Keeping current input device");
            return;
        }


        if (m_inputDevices[i].Acronym == m_currentInputDevice)
            return;

        OnStatusMessage(MessageLevel.Warning, $"Switching input device from {m_currentInputDevice} to {m_inputDevices[i].Acronym}");
        m_currentInputDevice = m_inputDevices[i].Acronym;
        m_tagMapping = GenerateMapping();

    }
    #endregion

    #region [ Static ]

    #endregion
}