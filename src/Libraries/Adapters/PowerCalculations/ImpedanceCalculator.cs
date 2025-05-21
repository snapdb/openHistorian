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

namespace PowerCalculations;

/// <summary>
/// Represents an algorithm that calculates power and stability from a synchrophasor device.
/// </summary>
[Description("Impedance Calculator: Calculates impedance from phasors on two ends of a line - specify Vs/Is phasors followed by Vr/Ir phasors.")]
[UIResource("AdaptersUI", $".PowerCalculations.ImpedanceCalculator.main.js")]
[UIResource("AdaptersUI", $".PowerCalculations.ImpedanceCalculator.chunk.js")]
public class ImpedanceCalculator : CalculatedMeasurementBase
{
    #region [ Members ]

    // Constants
    private const double SqrtOf3 = 1.7320508075688772935274463415059D;
        
    // Fields
    private double m_lastResistance;
    private double m_lastReactance;
    private double m_lastConductance;
    private double m_lastSusceptance;
    private double m_lastLineImpedance;
    private double m_lastLineImpedanceAngle;
    private double m_lastLineAdmittance;
    private double m_lastLineAdmittanceAngle;
    private VISet[] m_VISets;
    private Dictionary<Output,IMeasurement> m_OutputMap;

    // Important: Make sure output definition defines points in the following order
    private enum Output
    {
        // Rectangular Values
        Resistance,
        Reactance,
        Conductance,
        Susceptance,

        // Polar Values
        LineImpedance,
        LineImpedanceAngle,
        LineAdmittance,
        LineAdmittanceAngle
    }

    private class VISet
    {
        public MeasurementKey CurrentAngle { get; set; }
        public MeasurementKey CurrentMagnitude { get; set; }
        public MeasurementKey[] VoltageAngle { get; set; }
        public MeasurementKey[] VoltageMagnitude { get; set; }

    }

    #endregion

    #region [ Properties ]

    /// <summary>
    /// Gets or sets the flag that determines if line-to-line adjustment should be applied.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Defines flag that determines if line-to-line adjustment should be applied.")]
    [DefaultValue(true)]
    public bool ApplyLineToLineAdjustment { get; set; }

    /// <summary>
    /// Gets or sets the Current Magnitude PointTag to be used.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Defines the Current Magnitude to be used.")]
    [DefaultValue("")]
    public string CurrentMagnitude { get; set; }


    [ConnectionStringParameter]
    [Description("Defines the Voltage Magnitude to be used. If not supplied the system will use Primary or Secondary Voltage associated with the Current Phasor.")]
    [DefaultValue("")]
    public string VoltageMagnitude { get; set; }

    [ConnectionStringParameter]
    [Description("Defines the Output Measurement for Resistance.")]
    [DefaultValue("")]
    public IMeasurement Resistance { get; set; }

    [ConnectionStringParameter]
    [Description("Defines the Output Measurement for Reactance.")]
    [DefaultValue("")]
    public IMeasurement Reactance { get; set; }

    [ConnectionStringParameter]
    [Description("Defines the Output Measurement for Conductance.")]
    [DefaultValue("")]
    public IMeasurement Conductance { get; set; }

    [ConnectionStringParameter]
    [Description("Defines the Output Measurement for Susceptance.")]
    [DefaultValue("")]
    public IMeasurement Susceptance { get; set; }

    // Polar Values
    [ConnectionStringParameter]
    [Description("Defines the Output Measurement for LineImpedance.")]
    [DefaultValue("")]
    public IMeasurement LineImpedance { get; set; }

    [ConnectionStringParameter]
    [Description("Defines the Output Measurement for LineImpedanceAngle.")]
    [DefaultValue("")]
    public IMeasurement LineImpedanceAngle { get; set; }

    [ConnectionStringParameter]
    [Description("Defines the Output Measurement for LineAdmittance.")]
    [DefaultValue("")]
    public IMeasurement LineAdmittance { get; set; }

    [ConnectionStringParameter]
    [Description("Defines the Output Measurement for LineAdmittanceAngle.")]
    [DefaultValue("")]
    public IMeasurement LineAdmittanceAngle { get; set; }

    /// Returns the detailed status of the <see cref="ImpedanceCalculator"/> monitor.
    /// </summary>
    public override string Status
    {
        get
        {
            StringBuilder status = new();

            status.AppendLine("Latest Rectangular Values:");
            status.AppendLine($"                Resistance: {m_lastResistance:0.000} Ohm");
            status.AppendLine($"                 Reactance: {m_lastReactance:0.000} Ohm");
            status.AppendLine($"               Conductance: {m_lastConductance:0.000} Mho");
            status.AppendLine($"               Susceptance: {m_lastSusceptance:0.000} Mho");
            status.AppendLine("Latest Polar Values:");
            status.AppendLine($"            Line Impedance: {m_lastLineImpedance:0.000} Ohm");
            status.AppendLine($"      Line Impedance Angle: {m_lastLineImpedanceAngle:0.000}°");
            status.AppendLine($"           Line Admittance: {m_lastLineAdmittance:0.000} Ohm");
            status.AppendLine($"     Line Admittance Angle: {m_lastLineAdmittanceAngle:0.000}°");
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

        // Parse Current Input
        if (string.IsNullOrEmpty(CurrentMagnitude))
            throw new InvalidOperationException("A Current Magnitude must be specified.");

        if (!string.IsNullOrEmpty(CurrentMagnitude))
            ParsePhasors(CurrentMagnitude, VoltageMagnitude);
        else
        {
            if (InputMeasurementKeys is null || InputMeasurementKeyTypes is null)
                throw new InvalidOperationException("No input measurements were specified for the impedance calculator.");

            MeasurementKey[] voltageAngles = InputMeasurementKeys.Where((_, index) => InputMeasurementKeyTypes[index] == Gemstone.Numeric.EE.SignalType.VPHA).ToArray();
            MeasurementKey[] voltageMagnitudes = InputMeasurementKeys.Where((_, index) => InputMeasurementKeyTypes[index] == Gemstone.Numeric.EE.SignalType.VPHM).ToArray();
            MeasurementKey[] currentAngles = InputMeasurementKeys.Where((_, index) => InputMeasurementKeyTypes[index] == Gemstone.Numeric.EE.SignalType.IPHA).ToArray();
            MeasurementKey[] currentMagnitudes = InputMeasurementKeys.Where((_, index) => InputMeasurementKeyTypes[index] == Gemstone.Numeric.EE.SignalType.IPHM).ToArray();

            if (voltageAngles.Length != voltageMagnitudes.Length)
                throw new InvalidOperationException("A different number of voltage magnitude and angle input measurement keys were supplied - the angles and magnitudes must be supplied in pairs, i.e., one voltage magnitude input measurement must be supplied for each voltage angle input measurement in a consecutive sequence (e.g., VA1;VM1; VA2;VM2)");

            if (currentAngles.Length != currentMagnitudes.Length)
                throw new InvalidOperationException("A different number of current magnitude and angle input measurement keys were supplied - the angles and magnitudes must be supplied in pairs, i.e., one current magnitude input measurement must be supplied for each current angle input measurement in a consecutive sequence (e.g., IA1;IM1; IA2;IM2)");

            if (voltageAngles.Length != 2)
                throw new InvalidOperationException("Exactly two voltage angle input measurements are required for the impedance calculator, note that \"Vs\" angle/magnitude pair should be specified first followed by \"Vr\" angle/magnitude pair second.");

            if (voltageMagnitudes.Length != 2)
                throw new InvalidOperationException("Exactly two voltage magnitude input measurements are required for the impedance calculator, note that \"Vs\" angle/magnitude pair should be specified first followed by \"Vr\" angle/magnitude pair second.");

            if (currentAngles.Length != 2)
                throw new InvalidOperationException("Exactly two current angle input measurements are required for the impedance calculator, note that \"Is\" angle/magnitude pair should be specified first followed by \"Ir\" angle/magnitude pair second.");

            if (currentMagnitudes.Length != 2)
                throw new InvalidOperationException("Exactly two current magnitude input measurements are required for the impedance calculator, note that \"Is\" angle/magnitude pair should be specified first followed by \"Ir\" angle/magnitude pair second.");

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

        // Validate output measurements
        ValidateOutputMeasurements();

        Dictionary<string, string> settings = Settings;

        ApplyLineToLineAdjustment = !settings.TryGetValue(nameof(ApplyLineToLineAdjustment), out string? setting) || setting.ParseBoolean();
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
                set.VoltageAngle = AdapterBase.ParseInputMeasurementKeys(DataSource, true, $"FILTER ActiveMeasurement WHERE SourceIndex={voltagePhasors[index].SourceIndex} AND DeviceID={voltagePhasors[index].DeviceID} AND SignalTYPE LIKE 'VPHA'");
                index++;
            }
        }

        InputMeasurementKeys = m_VISets.SelectMany((s) => s.VoltageMagnitude.Concat(s.VoltageAngle).Concat(new MeasurementKey[] { s.CurrentMagnitude, s.CurrentAngle })).ToArray();
    }

    private void ValidateOutputMeasurements()
    {
        List<IMeasurement> measurementKeys = new();
        Dictionary<string, string> settings = Settings;

        for (int i = 0; i < Enum.GetValues(typeof(Output)).Length; i++)
        {
            if (settings.TryGetValue(Enum.GetNames<Output>()[i], out string setting))
                measurementKeys.Add(AdapterBase.ParseOutputMeasurements(DataSource, true, setting).FirstOrDefault());
            else
                measurementKeys.Add(null);
        }

        if (!measurementKeys.Any(item => item is not null))
        {
            if (OutputMeasurements is null || OutputMeasurements.Length < Enum.GetValues(typeof(Output)).Length)
                throw new InvalidOperationException("Not enough output measurements were specified for the impedance calculator, expecting measurements for the \"Resistance\", \"Reactance\", \"Conductance\", \"Susceptance\", \"LineImpedance\", \"LineImpedanceAngle\", \"LineAdmittance\" and \"LineAdmittanceAngle\" - in this order.");

            m_OutputMap = new Dictionary<Output, IMeasurement>();

            foreach (Output o in Enum.GetValues<Output>())
            {
                m_OutputMap.Add(o, OutputMeasurements[(int)o]);
            }
            return;
        }
        m_OutputMap = new Dictionary<Output, IMeasurement>();
        for (int i = 0; i < Enum.GetValues(typeof(Output)).Length; i++)
        {
            if (measurementKeys[i] is not null)
                m_OutputMap.Add((Output)i, measurementKeys[i]);
        }
    }

    /// <summary>
    /// Publishes the <see cref="IFrame"/> of time-aligned collection of <see cref="IMeasurement"/> values that arrived within the
    /// adapter's defined <see cref="ConcentratorBase.LagTime"/>.
    /// </summary>
    /// <param name="frame"><see cref="IFrame"/> of measurements with the same timestamp that arrived within <see cref="ConcentratorBase.LagTime"/> that are ready for processing.</param>
    /// <param name="index">Index of <see cref="IFrame"/> within a second ranging from zero to <c><see cref="ConcentratorBase.FramesPerSecond"/> - 1</c>.</param>
    protected override void PublishFrame(IFrame frame, int index)
    {
        IDictionary<MeasurementKey, IMeasurement> measurements = frame.Measurements;
        IMeasurement? magnitude, angle;
        ComplexNumber Vs = 0.0, Vr = 0.0, Is = 0.0, Ir = 0.0;
        int count = 0;

        // Get voltage magnitude and angle pairs
        for (int i = 0; i < 2; i++)
        {
            if (!measurements.TryGetValue(m_VISets![i].CurrentMagnitude, out magnitude) || !measurements.TryGetValue(m_VISets![i].CurrentAngle, out angle))
                continue;

            if (i == 0)
                Is = new ComplexNumber(Angle.FromDegrees(angle.AdjustedValue), magnitude.AdjustedValue);
            else
                Ir = new ComplexNumber(Angle.FromDegrees(angle.AdjustedValue), magnitude.AdjustedValue) * -1;

            bool foundV = false;
            for (int j = 0; j < m_VISets![i].VoltageMagnitude.Length; j++)
            {
                if (!measurements.TryGetValue(m_VISets![i].VoltageMagnitude[j], out magnitude) || !measurements.TryGetValue(m_VISets![i].VoltageAngle[j], out angle))
                    continue;
                if (double.IsNaN(magnitude.AdjustedValue) || double.IsNaN(angle.AdjustedValue))
                    continue;
                foundV = true;
            }

            if (!foundV)
                continue;

            double voltageMagnitude = magnitude.AdjustedValue;

            if (ApplyLineToLineAdjustment)
                voltageMagnitude *= SqrtOf3;

            if (i == 0)
                Vs = new ComplexNumber(Angle.FromDegrees(angle.AdjustedValue), voltageMagnitude);
            else
                Vr = new ComplexNumber(Angle.FromDegrees(angle.AdjustedValue), voltageMagnitude);

            count++;
        }

        // Exit if all measurements were not available for calculation
        if (count != 2)
            return;

        // Calculate resistance and reactance
        ComplexNumber Zl = (Vs * Vs - Vr * Vr) / (Vs * Ir + Vr * Is);

        if (ApplyLineToLineAdjustment)
            Zl /= SqrtOf3;

        // Calculate conductance and susceptance
        ComplexNumber Yl = 2 * (Is - Ir) / (Vs + Vr);

        if (ApplyLineToLineAdjustment)
            Yl *= SqrtOf3;

        // Provide calculated measurements for external consumption
        Measurement[] outputMeasurements = new Measurement[Enum.GetValues(typeof(Output)).Length];

        if (m_OutputMap.TryGetValue(Output.Resistance, out IMeasurement output))
            outputMeasurements[(int)Output.Resistance] = Measurement.Clone(output, Zl.Real, frame.Timestamp);

        if (m_OutputMap.TryGetValue(Output.Reactance, out output))
            outputMeasurements[(int)Output.Reactance] = Measurement.Clone(output, Zl.Imaginary, frame.Timestamp);

        if (m_OutputMap.TryGetValue(Output.Conductance, out output))
            outputMeasurements[(int)Output.Conductance] = Measurement.Clone(output, Yl.Real, frame.Timestamp);

        if (m_OutputMap.TryGetValue(Output.Susceptance, out output))
            outputMeasurements[(int)Output.Susceptance] = Measurement.Clone(output, Yl.Imaginary, frame.Timestamp);

        if (m_OutputMap.TryGetValue(Output.LineImpedance, out output))
            outputMeasurements[(int)Output.LineImpedance] = Measurement.Clone(output, Zl.Magnitude, frame.Timestamp);

        if (m_OutputMap.TryGetValue(Output.LineImpedanceAngle, out output))
            outputMeasurements[(int)Output.LineImpedanceAngle] = Measurement.Clone(output, Zl.Angle.ToDegrees(), frame.Timestamp);

        if (m_OutputMap.TryGetValue(Output.LineAdmittance, out output))
            outputMeasurements[(int)Output.LineAdmittance] = Measurement.Clone(output, Yl.Magnitude, frame.Timestamp);

        if (m_OutputMap.TryGetValue(Output.LineAdmittanceAngle, out output))
            outputMeasurements[(int)Output.LineAdmittanceAngle] = Measurement.Clone(output, Yl.Angle.ToDegrees(), frame.Timestamp);

        

        OnNewMeasurements(outputMeasurements);

        // Track last calculated values...
        m_lastResistance = Zl.Real;
        m_lastReactance = Zl.Imaginary;
        m_lastConductance = Yl.Real;
        m_lastSusceptance = Yl.Imaginary;
        m_lastLineImpedance = Zl.Magnitude;
        m_lastLineImpedanceAngle = Zl.Angle.ToDegrees();
        m_lastLineAdmittance = Yl.Magnitude;
        m_lastLineAdmittanceAngle = Yl.Angle.ToDegrees();
    }

    #endregion
}