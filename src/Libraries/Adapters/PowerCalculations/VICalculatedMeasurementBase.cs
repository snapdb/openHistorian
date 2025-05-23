//******************************************************************************************************
//  ImpedanceCalculator.cs - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  10/26/2016 - J. Ritchie Carroll / Vahid Salehi
//       Generated original version of source code.
//
//******************************************************************************************************
// ReSharper disable InconsistentNaming

using System;
using System.ComponentModel;
using System.Data;
using System.Text;
using Gemstone.Configuration;
using Gemstone.Data.Model;
using Gemstone.Numeric;
using Gemstone.Numeric.EE;
using Gemstone.StringExtensions;
using Gemstone.Timeseries;
using Gemstone.Timeseries.Adapters;
using Gemstone.Units;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using PhasorProtocolAdapters;
using static System.Net.Mime.MediaTypeNames;
using PhasorRecord = Gemstone.Timeseries.Model.Phasor;
using MeasurementRecord = Gemstone.Timeseries.Model.Measurement;
using static PowerCalculations.SequenceCalculator;

namespace PowerCalculations;

/// <summary>
/// Represents the base class for a calculated measurements that uses VI phasor data.
/// </summary>
/// <remarks>
/// This base class extends <see cref="CalculatedMeasurementBase"/> by automatically looking up the
/// <see cref="PhasorRecord"/> for currents and associated voltages.
/// </remarks>
public abstract class VICalculatedMeasurementBase : CalculatedMeasurementBase
{
    #region [ Members ]

    protected VISet[] m_VISets;
    public class VISet
    {
        public MeasurementKey CurrentAngle { get; set; }
        public MeasurementKey CurrentMagnitude { get; set; }
        public MeasurementKey[] VoltageAngle { get; set; }
        public MeasurementKey[] VoltageMagnitude { get; set; }

    }

    #endregion

    #region [ Properties ]

    /// <summary>
    /// Gets or sets the Current Magnitude PointTag to be used.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Defines the Current Phasors to be used.")]
    [DefaultValue("")]
    public string Currents { get; set; }


    [ConnectionStringParameter]
    [Description("Defines the Voltage Phasors to be used. If not supplied the system will use Primary or Secondary Voltage associated with each Current Phasor.")]
    [DefaultValue("")]
    public string Voltages { get; set; }


    /// Returns the detailed status of the <see cref="ImpedanceCalculator"/> monitor.
    /// </summary>
    public override string Status
    {
        get
        {
            const int MaxVIPairsToShow = 10;
            StringBuilder status = new();

            status.AppendLine("Voltage Current Pairs:");
            status.AppendLine($"       Voltage Current Pairs: {m_VISets.Length:N0} defined Pairs");
            status.AppendLine();

            for (int i = 0; i < Math.Min(m_VISets.Length, MaxVIPairsToShow); i++)
            {
                status.AppendLine($"   Current Magnitude: {m_VISets[i].CurrentMagnitude.SignalID}");
                status.AppendLine($"   Current Angle: {m_VISets[i].CurrentAngle.SignalID}");
                status.AppendLine($"   Voltage Magnitudes: {string.Join(", ", m_VISets[i].VoltageMagnitude.Select(key => key.SignalID))}");
                status.AppendLine($"   Voltage Angles: {string.Join(", ", m_VISets[i].VoltageAngle.Select((key) => key.SignalID))}");
                status.AppendLine();
            }
            if (m_VISets.Length > MaxVIPairsToShow)
                status.AppendLine("...".PadLeft(26));

            status.AppendLine();
            status.Append(base.Status);

            return status.ToString();
        }
    }

    #endregion

    #region [ Methods ]

    /// <summary>
    /// Initializes the <see cref="ImpedanceCalculator"/> monitor.
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();

        Dictionary<string, string> settings = Settings;

        if (settings.TryGetValue(nameof(Currents), out string setting))
            Currents = setting;
        else
            Currents = string.Empty;
        if (settings.TryGetValue(nameof(Voltages), out setting))
            Voltages = setting;
        else
            Voltages = string.Empty;


        if (!string.IsNullOrEmpty(Currents))
            ParsePhasors(Currents, Voltages);
        else
        {
            OnStatusMessage(Gemstone.Diagnostics.MessageLevel.Warning, $"No current phasors were specified - attempting to use {nameof(InputMeasurementKeys)} instead.");

            if (InputMeasurementKeys is null || InputMeasurementKeyTypes is null)
                throw new InvalidOperationException("No input measurements were specified.");

            MeasurementKey[] voltageAngles = InputMeasurementKeys.Where((_, index) => InputMeasurementKeyTypes[index] == Gemstone.Numeric.EE.SignalType.VPHA).ToArray();
            MeasurementKey[] voltageMagnitudes = InputMeasurementKeys.Where((_, index) => InputMeasurementKeyTypes[index] == Gemstone.Numeric.EE.SignalType.VPHM).ToArray();
            MeasurementKey[] currentAngles = InputMeasurementKeys.Where((_, index) => InputMeasurementKeyTypes[index] == Gemstone.Numeric.EE.SignalType.IPHA).ToArray();
            MeasurementKey[] currentMagnitudes = InputMeasurementKeys.Where((_, index) => InputMeasurementKeyTypes[index] == Gemstone.Numeric.EE.SignalType.IPHM).ToArray();

            if (voltageAngles.Length != voltageMagnitudes.Length)
                throw new InvalidOperationException("A different number of voltage magnitude and angle input measurement keys were supplied - the angles and magnitudes must be supplied in pairs, i.e., one voltage magnitude input measurement must be supplied for each voltage angle input measurement in a consecutive sequence (e.g., VA1;VM1; VA2;VM2)");

            if (currentAngles.Length != currentMagnitudes.Length)
                throw new InvalidOperationException("A different number of current magnitude and angle input measurement keys were supplied - the angles and magnitudes must be supplied in pairs, i.e., one current magnitude input measurement must be supplied for each current angle input measurement in a consecutive sequence (e.g., IA1;IM1; IA2;IM2)");

            if (currentAngles.Length != voltageAngles.Length)
                throw new InvalidOperationException("A different number of current and voltage input measurement keys were supplied - the angles and magnitudes must be supplied in pairs, i.e., one current input measurement must be supplied for each voltage  input measurement");

            m_VISets = new VISet[] {
                new() 
                {
                    CurrentAngle = currentAngles[0],
                    CurrentMagnitude = currentMagnitudes[0],
                    VoltageAngle = voltageAngles[0..0],
                    VoltageMagnitude = voltageMagnitudes[0..0]
                },
                new()
                {
                    CurrentAngle = currentAngles[1],
                    CurrentMagnitude = currentMagnitudes[1],
                    VoltageAngle = voltageAngles[1..1],
                    VoltageMagnitude = voltageMagnitudes[1..1]
                }
            };
            InputMeasurementKeys = m_VISets.SelectMany((s) => s.VoltageMagnitude.Concat(s.VoltageAngle).Concat(new MeasurementKey[] { s.CurrentMagnitude, s.CurrentAngle })).ToArray();
        }

    }

    private void ParsePhasors(string current, string voltage)
    {
        Func<DataRow, MeasurementRecord?> loadMeasurement = TableOperations<MeasurementRecord>.LoadRecordFunction();
        Func<DataRow, PhasorRecord?> loadPhasor = TableOperations<PhasorRecord>.LoadRecordFunction();

        MeasurementKey[] currents = AdapterBase.ParseInputMeasurementKeys(DataSource, true, current);

        PhasorRecord[] currentPhasors = currents.Select((meas) => loadMeasurement(base.DataSource.Tables["Measurement"].Select($"SignalID={meas.SignalID}")[0]))
        .Select((meas) => loadPhasor(DataSource.Tables["Phasor"].Select($"DeviceID={meas?.DeviceID ?? 0} AND SourceIndex = {meas?.PhasorSourceIndex ?? 0}")[0]))
        .DistinctBy((phasor) => phasor?.ID ?? 0).ToArray();

        if (currentPhasors.Length != 2)
            throw new InvalidOperationException("Exactly two current phasors needs to be specified.");

        m_VISets = currentPhasors.Select((i) => new VISet() {
            CurrentAngle = AdapterBase.ParseInputMeasurementKeys(DataSource, true, $"FILTER ActiveMeasurement WHERE SourceIndex={i.SourceIndex} AND DeviceID={i.DeviceID} AND SignalTYPE LIKE 'IPHA'").FirstOrDefault(),
            CurrentMagnitude = AdapterBase.ParseInputMeasurementKeys(DataSource, true, $"FILTER ActiveMeasurement WHERE SourceIndex={i.SourceIndex} AND DeviceID={i.DeviceID} AND SignalTYPE LIKE 'IPHM'").FirstOrDefault(),
            VoltageAngle = new MeasurementKey[2],
            VoltageMagnitude = new MeasurementKey[2]
        }).ToArray() ;

        if (string.IsNullOrEmpty(voltage))
        {
            int index = 0;
            foreach (VISet set in m_VISets)
            {
                PhasorRecord[] v = new PhasorRecord[2];
                if (currentPhasors[index].PrimaryVoltageID is not null)
                    v[0] = loadPhasor(DataSource.Tables["Phasor"].Select($"ID={currentPhasors[index].PrimaryVoltageID}")[0]);

                if (currentPhasors[index].SecondaryVoltageID is not null)
                    v[1] = loadPhasor(DataSource.Tables["Phasor"].Select($"ID={currentPhasors[index].SecondaryVoltageID}")[0]);

                if (v.Length < 1)
                    throw new InvalidOperationException($"Unable to identify the voltage phasor based on the current. A set of voltage phasors must be specified.");

                set.VoltageAngle = v.SelectMany((volt) => AdapterBase.ParseInputMeasurementKeys(DataSource, true, $"FILTER ActiveMeasurement WHERE SourceIndex={volt.SourceIndex} AND DeviceID={volt.DeviceID} AND SignalTYPE LIKE 'VPHA'")).ToArray();
                set.VoltageMagnitude = v.SelectMany((volt) => AdapterBase.ParseInputMeasurementKeys(DataSource, true, $"FILTER ActiveMeasurement WHERE SourceIndex={volt.SourceIndex} AND DeviceID={volt.DeviceID} AND SignalTYPE LIKE 'VPHM'")).ToArray();
                index++;
            }

        }
        else
        {
            MeasurementKey[] voltages = AdapterBase.ParseInputMeasurementKeys(DataSource, true, voltage);
            if (voltages.Length != 2)
                throw new InvalidOperationException("Exactly two voltage phasors needs to be specified.");

            PhasorRecord[] voltagePhasors = voltages.Select((meas) => loadMeasurement(base.DataSource.Tables["Measurement"].Select($"SignalID={meas.SignalID}")[0]))
                .Select((meas) => loadPhasor(DataSource.Tables["Phasor"].Select($"DeviceID={meas?.DeviceID ?? 0} AND SourceIndex = {meas?.PhasorSourceIndex ?? 0}")[0]))
                .ToArray();

            int index = 0;
            foreach (VISet set in m_VISets)
            {
                set.VoltageAngle = AdapterBase.ParseInputMeasurementKeys(DataSource, true, $"FILTER ActiveMeasurement WHERE SourceIndex={voltagePhasors[index].SourceIndex} AND DeviceID={voltagePhasors[index].DeviceID} AND SignalTYPE LIKE 'VPHA'");
                set.VoltageMagnitude = AdapterBase.ParseInputMeasurementKeys(DataSource, true, $"FILTER ActiveMeasurement WHERE SourceIndex={voltagePhasors[index].SourceIndex} AND DeviceID={voltagePhasors[index].DeviceID} AND SignalTYPE LIKE 'VPHM'");
                index++;
            }
        }

        InputMeasurementKeys = m_VISets.SelectMany((s) => s.VoltageMagnitude.Concat(s.VoltageAngle).Concat(new MeasurementKey[] { s.CurrentMagnitude, s.CurrentAngle })).ToArray();
    }

    #endregion
}