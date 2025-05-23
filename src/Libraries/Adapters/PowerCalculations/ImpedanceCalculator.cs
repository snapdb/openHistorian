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
public class ImpedanceCalculator : VICalculatedMeasurementBase
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

    #endregion

    #region [ Properties ]

    /// <summary>
    /// Gets or sets the flag that determines if line-to-line adjustment should be applied.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Defines flag that determines if line-to-line adjustment should be applied.")]
    [DefaultValue(true)]
    public bool ApplyLineToLineAdjustment { get; set; }

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

        if (m_VISets.Length != 2)
            throw new InvalidOperationException("Exactly two VI phasor pairs are required for the impedance calculator.");

        // Validate output measurements
        ValidateOutputMeasurements();

        Dictionary<string, string> settings = Settings;

        ApplyLineToLineAdjustment = !settings.TryGetValue(nameof(ApplyLineToLineAdjustment), out string? setting) || setting.ParseBoolean();
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
        List<Measurement> outputMeasurements = new ();

        if (m_OutputMap.TryGetValue(Output.Resistance, out IMeasurement output))
            outputMeasurements.Add(Measurement.Clone(output, Zl.Real, frame.Timestamp));

        if (m_OutputMap.TryGetValue(Output.Reactance, out output))
            outputMeasurements.Add(Measurement.Clone(output, Zl.Imaginary, frame.Timestamp));

        if (m_OutputMap.TryGetValue(Output.Conductance, out output))
            outputMeasurements.Add(Measurement.Clone(output, Yl.Real, frame.Timestamp));

        if (m_OutputMap.TryGetValue(Output.Susceptance, out output))
            outputMeasurements.Add(Measurement.Clone(output, Yl.Imaginary, frame.Timestamp));

        if (m_OutputMap.TryGetValue(Output.LineImpedance, out output))
            outputMeasurements.Add(Measurement.Clone(output, Zl.Magnitude, frame.Timestamp));

        if (m_OutputMap.TryGetValue(Output.LineImpedanceAngle, out output))
            outputMeasurements.Add(Measurement.Clone(output, Zl.Angle.ToDegrees(), frame.Timestamp));

        if (m_OutputMap.TryGetValue(Output.LineAdmittance, out output))
            outputMeasurements.Add(Measurement.Clone(output, Yl.Magnitude, frame.Timestamp));

        if (m_OutputMap.TryGetValue(Output.LineAdmittanceAngle, out output))
            outputMeasurements.Add(Measurement.Clone(output, Yl.Angle.ToDegrees(), frame.Timestamp));

        

        OnNewMeasurements(outputMeasurements.ToArray());

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