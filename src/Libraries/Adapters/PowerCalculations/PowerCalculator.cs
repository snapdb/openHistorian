//******************************************************************************************************
//  PowerCalculator.cs - Gbtc
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
//  05/16/2012 - J. Ritchie Carroll
//       Generated original version of source code.
//  12/13/2012 - Starlynn Danyelle Gilliam
//       Modified Header.
//
//******************************************************************************************************

using Gemstone.Collections.CollectionExtensions;
using Gemstone.StringExtensions;
using Gemstone.Timeseries;
using Gemstone.Timeseries.Adapters;
using Gemstone.Units;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Text;

namespace PowerCalculations;

/// <summary>
/// Calculates MW and MVAR using Voltage and Current Magnitude and Angle signals input to the adapter.
/// </summary>
[Description("Power Calculator: Calculates power and reactive power for synchrophasor measurements")]
[UIResource("AdaptersUI", $".PowerCalculations.PowerCalculator.main.js")]
[UIResource("AdaptersUI", $".PowerCalculations.PowerCalculator.chunk.js")]
public class PowerCalculator : VICalculatedMeasurementBase
{
    #region [ Members ]

    // Constants
    private const double SqrtOf3 = 1.7320508075688772935274463415059D;

    // Fields
    private readonly List<double> m_powerSample = [];
    private readonly List<double> m_reactivePowerSample = [];
    private Dictionary<Output, IMeasurement> m_outputMap = new();

    // Important: Make sure output definition defines points in the following order
    private enum Output
    {
        ActivePower,
        ReactivePower
    }

    #endregion

    #region [ Properties ]

    /// <summary>
    /// Gets or sets flag that determines if the last few values should be monitored.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Flag that determines if the last few values should be monitored.")]
    [DefaultValue(true)]
    public bool TrackRecentValues { get; set; }

    /// <summary>
    /// Gets or sets the sample size of the data to be monitored.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Define the sample size of the data to be monitored.")]
    [DefaultValue(5)]
    public int SampleSize { get; set; }


    [ConnectionStringParameter]
    [Description("Defines the Output Measurement for Active Power.")]
    [DefaultValue("")]
    public IMeasurement ActivePower { get; set; }

    [ConnectionStringParameter]
    [Description("Defines the Output Measurement for Reactive Power.")]
    [DefaultValue("")]
    public IMeasurement ReactivePower { get; set; }

    /// <summary>
    /// Gets the flag indicating if this adapter supports temporal processing.
    /// </summary>
    public override bool SupportsTemporalProcessing => true;

    /// <summary>
    /// Returns the detailed status of the <see cref="PowerCalculator"/>.
    /// </summary>
    public override string Status
    {
        get
        {
            const int ValuesToShow = 3;

            StringBuilder status = new();

            if (TrackRecentValues)
            {
                status.Append("         Last power values: ");

                lock (m_powerSample)
                {
                    // Display last several values
                    status.Append(m_powerSample.Count > ValuesToShow ? 
                        m_powerSample.GetRange(m_powerSample.Count - ValuesToShow - 1, ValuesToShow).Select(v => v.ToString("0.00MW")).ToDelimitedString(", ") : 
                        "Not enough values calculated yet...");
                }
                status.AppendLine();

                status.Append("Last reactive power values: ");

                lock (m_reactivePowerSample)
                {
                    // Display last several values
                    status.Append(m_reactivePowerSample.Count > ValuesToShow ? 
                        m_reactivePowerSample.GetRange(m_reactivePowerSample.Count - ValuesToShow - 1, ValuesToShow).Select(v => v.ToString("0.00MVAR")).ToDelimitedString(", ") : 
                        "Not enough values calculated yet...");
                }
                status.AppendLine();
            }

            status.Append(base.Status);

            return status.ToString();
        }
    }

    #endregion

    #region [ Methods ]

    /// <summary>
    /// Initializes the <see cref="PowerCalculator"/>.
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();

        Dictionary<string, string> settings = Settings;

        if (m_VISets.Length != 1)
            throw new InvalidOperationException("Exactly one VI phasor pair is required for the power calculator.");

        // Validate output measurements
        ValidateOutputMeasurements();

        // Load parameters
        TrackRecentValues = !settings.TryGetValue(nameof(TrackRecentValues), out string? setting) || setting.ParseBoolean();

        // Data sample size to monitor, in seconds
        SampleSize = settings.TryGetValue(nameof(SampleSize), out setting) ? int.Parse(setting) : 5;

        // Assign a default adapter name to be used if power calculator is loaded as part of automated collection
        if (string.IsNullOrWhiteSpace(Name))
            Name = $"PC!{m_outputMap[Output.ActivePower].Key}";
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
                throw new InvalidOperationException("Not enough output measurements were specified for the power calculator, expecting measurements for the \"ActivePower\" and \"ReactivePower\" - in this order.");

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
    /// Publish frame of time-aligned collection of measurement values that arrived within the defined lag time.
    /// </summary>
    /// <param name="frame">Frame of measurements with the same timestamp that arrived within lag time that are ready for processing.</param>
    /// <param name="index">Index of frame within a second ranging from zero to frames per second - 1.</param>
    protected override void PublishFrame(IFrame frame, int index)
    {
        double power = double.NaN, reactivePower = double.NaN;

        try
        {
            ConcurrentDictionary<MeasurementKey, IMeasurement> measurements = frame.Measurements;
            double voltageMagnitude = 0.0D, voltageAngle = 0.0D, currentMagnitude = 0.0D, currentAngle = 0.0D;
            IMeasurement magnitude = null, angle = null;
            bool allValuesReceived = false;

            bool foundV = false;
            for (int j = 0; j < m_VISets![0].VoltageMagnitude.Length; j++)
            {
                if (!measurements.TryGetValue(m_VISets![0].VoltageMagnitude[j], out magnitude) || !measurements.TryGetValue(m_VISets![0].VoltageAngle[j], out angle))
                    continue;
                if (double.IsNaN(magnitude.AdjustedValue) || double.IsNaN(angle.AdjustedValue) || !angle.ValueQualityIsGood() || ! magnitude.ValueQualityIsGood())
                    continue;
                foundV = true;
            }


            // Get each needed value from this frame
            if (foundV)
            {
                // Get voltage magnitude value
                voltageMagnitude = magnitude.AdjustedValue;
                voltageAngle = angle.AdjustedValue;

                if (!measurements.TryGetValue(m_VISets![0].CurrentMagnitude, out magnitude) || !measurements.TryGetValue(m_VISets![0].CurrentAngle, out angle))
                    return;
                if (double.IsNaN(magnitude.AdjustedValue) || double.IsNaN(angle.AdjustedValue) || !angle.ValueQualityIsGood() || !magnitude.ValueQualityIsGood())
                    return;

                currentMagnitude = magnitude.AdjustedValue;
                currentAngle = angle.AdjustedValue;

                allValuesReceived = true;
            }

            if (!allValuesReceived)
                return;

            double angleDifference = Math.Abs(voltageAngle - currentAngle);

            if (angleDifference > 180)
                angleDifference = 360 - angleDifference;

            // Convert phase angle difference to radians
            double impedancePhaseAngle = Angle.FromDegrees(angleDifference);

            // Calculate line-to-neutral apparent power (S) vector magnitude in Mega volt-amps
            double apparentPower = SqrtOf3 * (Math.Abs(voltageMagnitude) / SI.Mega) * Math.Abs(currentMagnitude);

            // Calculate power (P) and reactive power (Q)
            power = apparentPower * Math.Cos(impedancePhaseAngle);
            reactivePower = apparentPower * Math.Sin(impedancePhaseAngle);

            if (!TrackRecentValues)
                return;

            // Add latest calculated power to data sample
            lock (m_powerSample)
            {
                m_powerSample.Add(power);

                // Maintain sample size
                while (m_powerSample.Count > SampleSize)
                    m_powerSample.RemoveAt(0);
            }

            // Add latest calculated reactive power to data sample
            lock (m_reactivePowerSample)
            {
                m_reactivePowerSample.Add(reactivePower);

                // Maintain sample size
                while (m_reactivePowerSample.Count > SampleSize)
                    m_reactivePowerSample.RemoveAt(0);
            }
        }
        finally
        {
            List<IMeasurement> outputMeasurements = new();

            if (m_outputMap.TryGetValue(Output.ActivePower, out IMeasurement output))
                outputMeasurements.Add(Measurement.Clone(output, power, frame.Timestamp));

            if (m_outputMap.TryGetValue(Output.ReactivePower, out output))
                outputMeasurements.Add(Measurement.Clone(output, reactivePower, frame.Timestamp));

            // Provide calculated measurements for external consumption
            OnNewMeasurements(outputMeasurements.ToArray());
        }
    }

    #endregion
}