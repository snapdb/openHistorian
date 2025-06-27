//******************************************************************************************************
//  LossOfField.cs - Gbtc
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
//  12/02/2009 - Jian R. Zuo
//       Generated original version of source code.
//  12/16/2009 - Jian R. Zuo
//       Reading parameters configuration from database
//  04/12/2010 - J. Ritchie Carroll
//       Performed full code review, optimization and further abstracted code for LOF detection.
//  12/13/2012 - Starlynn Danyelle Gilliam
//       Modified Header.
//
//******************************************************************************************************
// ReSharper disable InconsistentNaming

using Gemstone.ComponentModel.DataAnnotations;
using Gemstone.Diagnostics;
using Gemstone.Timeseries;
using Gemstone.Timeseries.Adapters;
using Gemstone.Units;
using System.ComponentModel;
using System.Text;

namespace PowerCalculations.EventDetection;

/// <summary>
/// Represents an algorithm that detects Loss of Field from a synchrophasor device.
/// </summary>
[Description("Loss of Field: Detects Loss-of-Field from a synchrophasor device")]
[UIResource("AdaptersUI", $".PowerCalculations.LossOfField.main.js")]
[UIResource("AdaptersUI", $".PowerCalculations.LossOfField.chunk.js")]
public class LossOfField : VICalculatedMeasurementBase
{
    #region [ Members ]

    // Constants
    private const double SqrtOf3 = 1.7320508075688772935274463415059D;
    private const double DefaultPSet = -600.0D;
    private const double DefaultQSet = 200.0D;
    private const double DefaultQAreaSet = 500.0D;
    private const double DefaultVoltageThreshold = 475000.0D;
    private const int DefaultAnalysisInterval = 0;
    private Dictionary<Output, IMeasurement> m_outputMap = new();

    // Fields
    private double m_qAreamVar;                 // Calculated Q area value                 
    private long m_count;                       // Running frame count
    private long m_count1;                      // Last frame count
    private long m_count2;                      // Current frame count

    // Important: Make sure output definition defines points in the following order
    private enum Output
    {
        WarningSignal,
        RealPower,
        ReactivePower,
        QAreaValue
    }

    #endregion

    #region [ Properties ]

    /// <summary>
    /// Gets or sets the threshold of P-set MW.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Define the threshold of P-set MW.")]
    [DefaultValue(DefaultPSet)] // default value -600 mW  
    public double PSet { get; set; }

    /// <summary>
    /// Gets or sets the threshold of Q-set MVar.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Define the threshold of Q-set MVar.")]
    [DefaultValue(DefaultQSet)] // default value 200 mVar
    public double QSet { get; set; }

    /// <summary>
    /// Gets or sets the threshold of Q-area MVar-sec.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Define the threshold of Q-area MVar-sec.")]
    [DefaultValue(DefaultQAreaSet)] // default value 500 mVar-sec
    [Label("QArea Set")]
    public double QAreaSet { get; set; }

    /// <summary>
    /// Gets or sets the threshold of voltage, in volts.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Define the threshold of voltage, in volts.")]
    [DefaultValue(DefaultVoltageThreshold)] // default value 0.95 p.u. or 475 kV
    [Label("Voltage Threshold")]
    public double VoltageThreshold { get; set; }

    /// <summary>
    /// Gets or sets the interval between adjacent calculations.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Define the interval between adjacent calculations. The default value is the frame-rate defined in the connection string for this Loss of Field.")]
    [DefaultValue(DefaultAnalysisInterval)]
    [Label("Analysis Interval")]
    public int AnalysisInterval { get; set; }

    /// <summary>
    /// Returns the detailed status of the <see cref="LossOfField"/> detector.
    /// </summary>
    public override string Status
    {
        get
        {
            StringBuilder status = new();

            status.AppendLine($"   Calculated Q-area value: {m_qAreamVar:0.000}");
            status.AppendLine($"               P-Set value: {PSet:0.000}");
            status.AppendLine($"               Q-Set value: {QSet:0.000}");
            status.AppendLine($"          Q-Area set value: {QAreaSet:0.000}");
            status.AppendLine($"         Voltage threshold: {VoltageThreshold:0.000}");
            status.AppendLine($"      Calculation interval: {AnalysisInterval:0.000}");

            status.Append(base.Status);

            return status.ToString();
        }
    }

    #endregion

    #region [ Methods ]

    /// <summary>
    /// Initializes the <see cref="LossOfField"/> detector.
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();

        Dictionary<string, string> settings = Settings;

        // Load parameters
        PSet = settings.TryGetValue(nameof(PSet), out string? setting) && double.TryParse(setting, out double value) ? value : DefaultPSet;
        QSet = settings.TryGetValue(nameof(QSet), out setting) && double.TryParse(setting, out value) ? value : DefaultQSet;
        QAreaSet = settings.TryGetValue(nameof(QAreaSet), out setting) && double.TryParse(setting, out value) ? value : DefaultQAreaSet;
        VoltageThreshold = settings.TryGetValue(nameof(VoltageThreshold), out setting) && double.TryParse(setting, out value) ? value : DefaultVoltageThreshold;
        AnalysisInterval = settings.TryGetValue(nameof(AnalysisInterval), out setting) && int.TryParse(setting, out int interval) && interval > 0 ? interval : FramesPerSecond;

        m_count = 0;
        m_count1 = 0;
        m_count2 = 0;

        if (m_VISets.Length != 1)
            throw new InvalidOperationException("Exactly one VI phasor pair is required for the loss of field detector.");

        // Validate output measurements
        ValidateOutputMeasurements();
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
                throw new InvalidOperationException("Not enough output measurements were specified for the loss of field detector, expecting measurements for \"Warning Signal Status (0 = Not Signaled, 1 = Signaled)\", \"Real Power\", \"Reactive Power\" and \"Q-Area Value\" - in this order.");

            m_outputMap = new Dictionary<Output, IMeasurement>();

            foreach (Output o in Enum.GetValues<Output>())
            {
                m_outputMap.Add(o, OutputMeasurements[(int)o]);
            }
            return;
        }

        m_outputMap = new Dictionary<Output, IMeasurement>();
        for (int i = 0; i < Enum.GetValues(typeof(Output)).Length; i++)
        {
            if (measurementKeys[i] is not null)
                m_outputMap.Add((Output)i, measurementKeys[i]);
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
        // Increment frame counter
        m_count++;

        if (m_count % AnalysisInterval != 0)
            return;

        IDictionary<MeasurementKey, IMeasurement> measurements = frame.Measurements;
        bool warningSignaled = false;

        m_count1 = m_count2;
        m_count2 = m_count;

        VISet set = m_VISets[0];         // first (or only) VI pair

        if (!measurements.TryGetValue(set.VoltageMagnitude[0], out var vMag)) return;
        if (!measurements.TryGetValue(set.VoltageAngle[0], out var vAng)) return;
        if (!measurements.TryGetValue(set.CurrentMagnitude, out var iMag)) return;
        if (!measurements.TryGetValue(set.CurrentAngle, out var iAng)) return;

        double voltageMagnitude = vMag.AdjustedValue;
        double voltageAngle = Angle.FromDegrees(vAng.AdjustedValue);
        double currentMagnitude = iMag.AdjustedValue;
        double currentAngle = Angle.FromDegrees(iAng.AdjustedValue);

        double realPower = 3.0D * voltageMagnitude * currentMagnitude * Math.Cos(voltageAngle - currentAngle) / SI.Mega;
        double reactivePower = 3.0D * voltageMagnitude * currentMagnitude * Math.Sin(voltageAngle - currentAngle) / SI.Mega;
        double deltaT = (m_count2 - m_count1) / (double)FramesPerSecond;

        if (realPower < PSet && reactivePower > QSet)
        {
            m_qAreamVar += deltaT * (reactivePower - QSet);

            if (m_qAreamVar > QAreaSet && voltageMagnitude < VoltageThreshold / SqrtOf3)
            {
                warningSignaled = true;
                OutputLOFWarning(realPower, reactivePower, m_qAreamVar);
            }
        }
        else
        {
            m_qAreamVar = 0.0D;
        }

        // Expose output measurement values
        List<IMeasurement> outputMeasurements = new();

        if (m_outputMap.TryGetValue(Output.WarningSignal, out var warningSignal))
            outputMeasurements.Add(Measurement.Clone(warningSignal, warningSignaled ? 1.0D : 0.0D, frame.Timestamp));

        if (m_outputMap.TryGetValue(Output.RealPower, out var realPowerMeas))
            outputMeasurements.Add(Measurement.Clone(realPowerMeas, realPower, frame.Timestamp));

        if (m_outputMap.TryGetValue(Output.ReactivePower, out var reactivePowmeas))
            outputMeasurements.Add(Measurement.Clone(reactivePowmeas, (int)reactivePower, frame.Timestamp));

        if (m_outputMap.TryGetValue(Output.QAreaValue, out var qAreaMeas))
            outputMeasurements.Add(Measurement.Clone(qAreaMeas, m_qAreamVar, frame.Timestamp));

        // Provide measurements for external consumption
        OnNewMeasurements(outputMeasurements.ToArray());

    }

    private void OutputLOFWarning(double realPower, double reactivePower, double qAreamVar)
    {
        OnStatusMessage(MessageLevel.Info, $"Loss of Field Detected!{Environment.NewLine}" +
                                           $"        Real power = {realPower}{Environment.NewLine}" + 
                                           $"    Reactive Power = {reactivePower}{Environment.NewLine}" +
                                           $"            Q Area = {qAreamVar}{Environment.NewLine}");
    }

    #endregion
}