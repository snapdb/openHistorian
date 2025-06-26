//******************************************************************************************************
//  VICalculatedMeasurementBase.cs - Gbtc
//
//  Copyright © 2025, Grid Protection Alliance.  All Rights Reserved.
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
//  04/29/2025 - C. Lackner
//       Generated original version of source code.
//
//******************************************************************************************************
// ReSharper disable InconsistentNaming

using System.ComponentModel;
using System.Data;
using System.Text;
using Gemstone.Data.Model;
using Gemstone.Timeseries;
using Gemstone.Timeseries.Adapters;
using PhasorProtocolAdapters;
using PhasorRecord = Gemstone.Timeseries.Model.PhasorValues;

namespace PowerCalculations;

/// <summary>
/// Represents the base class for a calculated measurements that uses VI phasor data.
/// </summary>
/// <remarks>
/// This base class extends <see cref="CalculatedMeasurementBase"/> by automatically looking up the
/// <see cref="PhasorValues"/> for currents and associated voltages.
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
    /// Initializes the <see cref="VICalculatedMeasurementBase"/> monitor.
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

            m_VISets = currentAngles.Select((angI, i) => new VISet()
            {
                CurrentAngle = angI,
                CurrentMagnitude = currentMagnitudes[i],
                VoltageAngle = voltageAngles[i..i],
                VoltageMagnitude = voltageMagnitudes[i..i]
            }).ToArray();
            InputMeasurementKeys = m_VISets.SelectMany((s) => s.VoltageMagnitude.Concat(s.VoltageAngle).Concat(new MeasurementKey[] { s.CurrentMagnitude, s.CurrentAngle })).ToArray();
        }

    }

    private void ParsePhasors(string current, string voltage)
    {
        Func<DataRow, PhasorRecord?> loadPhasor = TableOperations<PhasorRecord>.LoadRecordFunction();

        MeasurementKey[] currents = AdapterBase.ParseInputMeasurementKeys(DataSource, true, current);

        if (DataSource?.Tables["PhasorValues"] is null)
            throw new InvalidOperationException("PhasorValues table is not available in the data source. Ensure that ConfigurationEntity is properly configured.");

        DataTable phasorValuesTable = DataSource.Tables["PhasorValues"];

        PhasorRecord[] currentPhasors = currents
        .SelectMany((meas) => phasorValuesTable.Select($"AngleSignalID = '{meas.SignalID}' OR MagnitudeSignalID = '{meas.SignalID}'").Select(loadPhasor))
        .DistinctBy((phasor) => phasor!.SignalID).ToArray();

        m_VISets = currentPhasors.Select((i) => new VISet()
        {
            CurrentAngle = MeasurementKey.LookUpBySignalID(i.AngleSignalID ?? Guid.Empty),
            CurrentMagnitude = MeasurementKey.LookUpBySignalID(i.MagnitudeSignalID ?? Guid.Empty),
        }).ToArray();

        if (string.IsNullOrEmpty(voltage))
        {
            int index = 0;
            foreach (VISet set in m_VISets)
            {
                List<PhasorRecord> v = new();
                if (currentPhasors[index].PrimaryVoltagePhasorID is not null)
                    v.AddRange(phasorValuesTable.Select($"PhasorID = {currentPhasors?[index]?.PrimaryVoltagePhasorID}")?.Select(p => loadPhasor(p)));

                if (currentPhasors[index].SecondaryVoltagePhasorID is not null)
                    v.AddRange(phasorValuesTable.Select($"PhasorID = {currentPhasors?[index]?.SecondaryVoltagePhasorID}")?.Select(p => loadPhasor(p)));

                if (v.Count() < 1)
                    throw new InvalidOperationException($"Unable to identify the voltage phasor based on the current. A set of voltage phasors must be specified.");


                set.VoltageAngle = v.Select((volt) => MeasurementKey.LookUpBySignalID(volt?.AngleSignalID ?? Guid.Empty)).ToArray();
                set.VoltageMagnitude = v.Select((volt) => MeasurementKey.LookUpBySignalID(volt?.MagnitudeSignalID ?? Guid.Empty)).ToArray();
                index++;
            }
 
        }
        else
        {
            MeasurementKey[] voltages = AdapterBase.ParseInputMeasurementKeys(DataSource, true, voltage);

            PhasorRecord?[] voltagePhasors = voltages
                .Select((meas) => loadPhasor(phasorValuesTable.Select($"AngleSignalID = '{meas.SignalID}' OR MagnitudeSignalID = '{meas.SignalID}'")[0]))
                .ToArray();

            int index = 0;
            foreach (VISet set in m_VISets)
            {
                set.VoltageAngle = [MeasurementKey.LookUpBySignalID(voltagePhasors?[index]?.AngleSignalID ?? Guid.Empty)];
                set.VoltageMagnitude = [MeasurementKey.LookUpBySignalID(voltagePhasors?[index]?.MagnitudeSignalID ?? Guid.Empty)];
                index++;
            }
        }

        InputMeasurementKeys = m_VISets.SelectMany((s) => s.VoltageMagnitude.Concat(s.VoltageAngle).Concat([s.CurrentMagnitude, s.CurrentAngle])).ToArray();
    }

    #endregion
}