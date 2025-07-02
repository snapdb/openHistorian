//******************************************************************************************************
//  PowerStability.cs - Gbtc
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
//  11/08/2006 - J. Ritchie Carroll
//      Initial version of source generated
//  12/22/2009 - Jian R. Zuo
//       Converted code to C#;
//  04/12/2010 - J. Ritchie Carroll
//       Performed full code review, optimization and further abstracted code for stability calculator.
//  12/13/2012 - Starlynn Danyelle Gilliam
//       Modified Header.
//
//******************************************************************************************************

using Gemstone.Collections.CollectionExtensions;
using Gemstone.ComponentModel.DataAnnotations;
using Gemstone.Numeric.Analysis;
using Gemstone.Timeseries;
using Gemstone.Timeseries.Adapters;
using Gemstone.Units;
using System.ComponentModel;
using System.Text;

namespace PowerCalculations;

/// <summary>
/// Represents an algorithm that calculates power and stability from a synchrophasor device.
/// </summary>
/// <remarks>
/// <para>
/// This algorithm calculates power and its standard deviation in real-time that can be used to
/// determine if there is an oscillatory signature in the power output.
/// </para>
/// <para>
/// If multiple voltage phasors are provided as inputs to this algorithm, then they are assumed to be
/// redundant values on the same bus, the first energized value will be the voltage phasor that is
/// used in the calculation.<br/>
/// If multiple current phasors are provided as inputs to this algorithm, then they are assumed to be
/// cumulative inputs representing the desired power output summation of the generation source.
/// </para>
/// <para>
/// Individual phase angle and magnitude phasor elements are expected to be defined consecutively.
/// That is the definition order of angles and magnitudes must match so that the angle / magnitude
/// pair can be matched up appropriately. For example: angle1;mag1;  angle2;mag2;  angle3;mag3.
/// </para>
/// </remarks>
[Description("Power Stability: Calculates power and stability for a synchrophasor device")]
[UIResource("AdaptersUI", $".PowerCalculations.PowerStability.main.js")]
[UIResource("AdaptersUI", $".PowerCalculations.PowerStability.chunk.js")]
public class PowerStability : VICalculatedMeasurementBase
{
    #region [ Members ]

    // Fields
    private int m_minimumSamples;
    private readonly List<double> m_powerDataSample = [];
    private double m_lastStdev;
    private Dictionary<Output, IMeasurement> m_outputMap = new();

    // Important: Make sure output definition defines points in the following order
    private enum Output
    {
        Power,
        StDev
    }

    #endregion

    #region [ Properties ]

    /// <summary>
    /// Gets or sets the sample size, in seconds, of the data to be monitored.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Define the sample size, in seconds, of the data to be monitored.")]
    [DefaultValue(15)]
    [Label("Sample Size")]
    public int SampleSize
    {
        get => m_minimumSamples / FramesPerSecond;
        set => m_minimumSamples = value * FramesPerSecond;
    }

    /// <summary>
    /// Gets or sets the energized bus threshold, in volts. The recommended value is 20% of nominal line-to-neutral voltage.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Define the energized bus threshold, in volts. The recommended value is 20% of nominal line-to-neutral voltage.")]
    [DefaultValue(58000.0D)]
    [Label("Energized Threshold")]
    public double EnergizedThreshold { get; set; }

    [ConnectionStringParameter]
    [Description("Defines the Output Measurement for Power.")]
    [DefaultValue("")]
    public IMeasurement Power { get; set; }

    [ConnectionStringParameter]
    [Description("Defines the Output Measurement for St Dev.")]
    [DefaultValue("")]
    public IMeasurement StDev { get; set; }

    /// <summary>
    /// Returns the detailed status of the <see cref="PowerStability"/> monitor.
    /// </summary>
    public override string Status
    {
        get
        {
            const int ValuesToShow = 3;

            StringBuilder status = new();

            status.AppendLine($"          Data sample size: {m_minimumSamples / FramesPerSecond} seconds");
            status.AppendLine($"   Energized bus threshold: {EnergizedThreshold:0.00} volts");
            status.AppendLine($"     Total voltage phasors: {m_VISets.SelectMany(s => s.VoltageMagnitude).Count()}");
            status.AppendLine($"     Total current phasors: {m_VISets.Length}");
            status.Append("         Last power values: ");

            lock (m_powerDataSample)
            {
                // Display last several values
                status.Append(m_powerDataSample.Count > ValuesToShow ?
                    m_powerDataSample.GetRange(m_powerDataSample.Count - ValuesToShow - 1, ValuesToShow).Select(v => v.ToString("0.00MW")).ToDelimitedString(", ") :
                    "Not enough values calculated yet...");
            }

            status.AppendLine();
            status.AppendLine($"     Latest stdev of power: {m_lastStdev}");
            status.Append(base.Status);

            return status.ToString();
        }
    }

    #endregion

    #region [ Methods ]

    /// <summary>
    /// Initializes the <see cref="PowerStability"/> monitor.
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();

        Dictionary<string, string> settings = Settings;

        // Load parameters
        if (settings.TryGetValue(nameof(SampleSize), out string? setting)) // Data sample size to monitor, in seconds
            m_minimumSamples = int.Parse(setting) * FramesPerSecond;
        else
            m_minimumSamples = 15 * FramesPerSecond;

        // Energized bus threshold, in volts, recommended value is 20% of nominal line-to-neutral voltage
        EnergizedThreshold = settings.TryGetValue(nameof(EnergizedThreshold), out setting) ? double.Parse(setting) : 58000.0D;

        if (m_VISets.Length < 1)
            throw new InvalidOperationException("At least one VI phasor pair is required for the power stability adapter.");

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
                throw new InvalidOperationException("Not enough output measurements were specified for the power stability monitor, expecting measurements for the \"Calculated Power\", and the \"Standard Deviation of Power\" - in this order.");

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
        IDictionary<MeasurementKey, IMeasurement> measurements = frame.Measurements;
        IMeasurement? magnitude, angle;
        double voltageMagnitude = double.NaN, voltageAngle = double.NaN, power = 0.0D;

        MeasurementKey[] voltageMagnitudes = m_VISets.SelectMany(s => s.VoltageMagnitude).ToArray();
        MeasurementKey[] voltageAngles = m_VISets.SelectMany(s => s.VoltageAngle).ToArray();
        MeasurementKey[] currentMagnitudes = m_VISets.Select(s => s.CurrentMagnitude).ToArray();
        MeasurementKey[] currentAngles = m_VISets.Select(s => s.CurrentAngle).ToArray();

        // Get first voltage magnitude and angle value pair that is above the energized threshold
        for (int i = 0; i < voltageMagnitudes!.Length; i++)
        {
            if (!measurements.TryGetValue(voltageMagnitudes[i], out magnitude) || !measurements.TryGetValue(voltageAngles![i], out angle))
                continue;

            if (!(magnitude.AdjustedValue > EnergizedThreshold))
                continue;

            voltageMagnitude = magnitude.AdjustedValue;
            voltageAngle = angle.AdjustedValue;
            break;
        }

        // Exit if bus voltage measurements were not available for calculation
        if (double.IsNaN(voltageMagnitude))
            return;

        // Calculate the sum of the current phasors
        for (int i = 0; i < currentMagnitudes!.Length; i++)
        {
            // Retrieve current magnitude and angle measurements as consecutive pairs
            if (measurements.TryGetValue(currentMagnitudes[i], out magnitude) && measurements.TryGetValue(currentAngles![i], out angle))
                power += magnitude.AdjustedValue * Math.Cos(Angle.FromDegrees(angle.AdjustedValue - voltageAngle));
            else
                return; // Exit if current measurements were not available for calculation
        }

        // Apply bus voltage and convert to 3-phase megawatts
        power = power * voltageMagnitude / (SI.Mega / 3.0D);

        // Add latest calculated power to data sample
        lock (m_powerDataSample)
        {
            m_powerDataSample.Add(power);

            // Maintain sample size
            while (m_powerDataSample.Count > m_minimumSamples)
                m_powerDataSample.RemoveAt(0);
        }

        List<IMeasurement> outputMeasurements = new();

        // Check to see if the needed number of samples are available to begin producing the standard deviation output measurement
        // ReSharper disable once InconsistentlySynchronizedField
        if (m_powerDataSample.Count >= m_minimumSamples)
        {
            if (m_outputMap.TryGetValue(Output.Power, out var powerMeas))
                outputMeasurements.Add(Measurement.Clone(powerMeas, power, frame.Timestamp));

            // Update and emit standard deviation
            if (m_outputMap.TryGetValue(Output.StDev, out var stdevMeasurement))
            {

                lock (m_powerDataSample)
                    stdevMeasurement.Value = m_powerDataSample.StandardDeviation();

                outputMeasurements.Add(Measurement.Clone(stdevMeasurement, frame.Timestamp));

                // Track last standard deviation
                m_lastStdev = stdevMeasurement.AdjustedValue;
            }
        }
        else if (power > 0.0D)
        {
            // If not, we can still start publishing power calculation as soon as we have one...
            if (m_outputMap.TryGetValue(Output.Power, out var powerMeas))
                outputMeasurements.Add(Measurement.Clone(powerMeas, power, frame.Timestamp));

        }

        // Provide measurements for external consumption
        OnNewMeasurements(outputMeasurements.ToArray());
    }

    #endregion
}