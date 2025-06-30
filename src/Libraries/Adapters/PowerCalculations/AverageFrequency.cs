//******************************************************************************************************
//  AverageFrequency.cs - Gbtc
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
//  11/22/2006 - J. Ritchie Carroll
//       Initial version of source generated
//  12/24/2009 - Jian R. Zuo
//       Converted code to C#
//  04/12/2010 - J. Ritchie Carroll
//       Performed full code review, optimization and further abstracted code for average calculation.
//  12/13/2012 - Starlynn Danyelle Gilliam
//       Modified Header.
//
//******************************************************************************************************

using Gemstone.ComponentModel.DataAnnotations;
using Gemstone.Numeric.EE;
using Gemstone.StringExtensions;
using Gemstone.Timeseries;
using Gemstone.Timeseries.Adapters;
using Gemstone.Units;
using PhasorProtocolAdapters;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Text;

namespace PowerCalculations;

/// <summary>
/// Calculates a real-time average frequency reporting the average, maximum and minimum values.
/// </summary>
[Description("Average Frequency: Calculates a real-time average frequency reporting the average, maximum, and minimum values")]
[UIResource("AdaptersUI", $".PowerCalculations.AverageFrequency.main.js")]
[UIResource("AdaptersUI", $".PowerCalculations.AverageFrequency.chunk.js")]
public class AverageFrequency : CalculatedMeasurementBase
{
    #region [ Members ]

    // Constants
    private const double DefaultLowFrequencyThreshold = 57.0D;
    private const double DefaultHighFrequencyThreshold = 62.0D;
    private const bool DefaultReportUnreasonableResultsAsNaN = false;

    // Fields
    private double m_averageFrequency;
    private double m_maximumFrequency;
    private double m_minimumFrequency;
    private readonly ConcurrentDictionary<Guid, Tuple<int, long>> m_lastValues = new();
    private Dictionary<Output, IMeasurement> m_outputMap = new();

    // Important: Make sure output definition defines points in the following order
    private enum Output
    {
        Average,
        Maximum,
        Minimum
    }

    #endregion

    #region [ Properties ]

    /// <summary>
    /// Gets or sets low frequency reasonability threshold, inclusive.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Defines low frequency reasonability threshold. Value is inclusive, i.e., frequency will be unreasonable at and beyond specified threshold.")]
    [DefaultValue(DefaultLowFrequencyThreshold)]
    [Label("Low Frequency Threshold")]
    public double LowFrequencyThreshold { get; set; }

    /// <summary>
    /// Gets or sets high frequency reasonability threshold, inclusive.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Defines high frequency reasonability threshold. Value is inclusive, i.e., frequency will be unreasonable at and beyond specified threshold.")]
    [DefaultValue(DefaultHighFrequencyThreshold)]
    [Label("High Frequency Threshold")]
    public double HighFrequencyThreshold { get; set; }

    /// <summary>
    /// Gets or sets flag that determines if unreasonable results should be reported as NaN.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Defines flag that determines if unreasonable results should be reported as NaN.")]
    [DefaultValue(DefaultReportUnreasonableResultsAsNaN)]
    [Label("Report Unreasonable Results As NaN")]
    public bool ReportUnreasonableResultsAsNaN { get; set; }

    /// <summary>
    /// Gets or sets output measurements that the calculated measurement will produce, if any.
    /// </summary>
    [ConnectionStringParameter(false)]
    [DefaultValue(null)]
    [Description("Defines primary keys of output measurements the action adapter expects; can be one of a filter expression, measurement key, point tag or Guid.")]
    public override IMeasurement[]? OutputMeasurements
    {
        get => base.OutputMeasurements;
        set
        {
            base.OutputMeasurements = value;
        }
    }

    [ConnectionStringParameter]
    [Description("Defines the Output Measurement for Average.")]
    [DefaultValue("")]
    public IMeasurement Average { get; set; }

    [ConnectionStringParameter]
    [Description("Defines the Output Measurement for Maximum.")]
    [DefaultValue("")]
    public IMeasurement Maximum { get; set; }

    [ConnectionStringParameter]
    [Description("Defines the Output Measurement for Minimum.")]
    [DefaultValue("")]
    public IMeasurement Minimum { get; set; }

    /// <summary>
    /// Returns the detailed status of the <see cref="AverageFrequency"/> calculator.
    /// </summary>
    public override string Status
    {
        get
        {
            StringBuilder status = new();

            status.AppendLine($"   Low frequency threshold: {LowFrequencyThreshold:N3} Hz");
            status.AppendLine($"  High frequency threshold: {HighFrequencyThreshold:N3} Hz");
            status.AppendLine($"Report unreasonable as NaN: {ReportUnreasonableResultsAsNaN}");
            status.AppendLine($"    Last average frequency: {m_averageFrequency:N3} Hz");
            status.AppendLine($"    Last maximum frequency: {m_maximumFrequency:N3} Hz");
            status.AppendLine($"    Last minimum frequency: {m_minimumFrequency:N3} Hz");
            status.Append(base.Status);

            return status.ToString();
        }
    }
    #endregion

    #region [ Methods ]

    /// <summary>
    /// Initializes the <see cref="AverageFrequency"/> calculator.
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();

        Dictionary<string, string> settings = Settings;

        // Get settings
        if (settings.TryGetValue(nameof(LowFrequencyThreshold), out string? setting) && double.TryParse(setting, out double threshold))
            LowFrequencyThreshold = threshold;
        else
            LowFrequencyThreshold = DefaultLowFrequencyThreshold;

        if (settings.TryGetValue(nameof(HighFrequencyThreshold), out setting) && double.TryParse(setting, out threshold))
            HighFrequencyThreshold = threshold;
        else
            HighFrequencyThreshold = DefaultHighFrequencyThreshold;

        ReportUnreasonableResultsAsNaN = settings.TryGetValue(nameof(ReportUnreasonableResultsAsNaN), out setting) && setting.ParseBoolean();

        if (InputMeasurementKeys is null || InputMeasurementKeyTypes is null)
            throw new InvalidOperationException("No input measurements were specified for the average frequency calculator.");

        // Validate input measurements
        MeasurementKey[] validInputMeasurementKeys = InputMeasurementKeys.Where((_, index) => InputMeasurementKeyTypes[index] == SignalType.FREQ).ToArray();

        if (validInputMeasurementKeys.Length == 0)
            throw new InvalidOperationException("No valid frequency measurements were specified as inputs to the average frequency calculator.");

        // Make sure only frequencies are used as input
        InputMeasurementKeys = validInputMeasurementKeys;

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
                throw new InvalidOperationException("Not enough output measurements were specified for the average frequency adapter, expecting measurements for the \"Average\", \"Maximum\", \"Minimum\" - in this order.");

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
    /// Calculates the average frequency for all frequencies that have reported in the specified lag time.
    /// </summary>
    /// <param name="frame">Single frame of measurement data within a one-second sample.</param>
    /// <param name="index">Index of frame within the one-second sample.</param>
    protected override void PublishFrame(IFrame frame, int index)
    {
        if (frame.Measurements.Count > 0)
        {
            double total = 0.0D;
            double maximumFrequency = LowFrequencyThreshold;
            double minimumFrequency = HighFrequencyThreshold;
            double averageFrequency = (HighFrequencyThreshold + LowFrequencyThreshold) / 2;

            int count = 0;

            foreach (IMeasurement measurement in frame.Measurements.Values)
            {
                double frequency = measurement.AdjustedValue;

                // Do some simple flat line avoidance, validating at three decimal places...
                int truncatedFrequency = (int)(frequency * 1000.0D);

                Tuple<int, long> lastValues = m_lastValues.GetOrAdd(measurement.ID, _ => new Tuple<int, long>(truncatedFrequency, 0L));
                long flatLineCount = lastValues.Item2;

                if (lastValues.Item1 == truncatedFrequency)
                {
                    flatLineCount++;

                    // If value remains the same for more than four times, assume flat-line value
                    if (flatLineCount > 4L)
                        frequency = double.NaN;
                }
                else
                {
                    flatLineCount = 0L;
                }

                m_lastValues[measurement.ID] = new Tuple<int, long>(truncatedFrequency, flatLineCount);

                // Validate frequency
                if (double.IsNaN(frequency) || !(frequency > LowFrequencyThreshold) || !(frequency < HighFrequencyThreshold))
                    continue;

                total += frequency;

                if (frequency > maximumFrequency)
                    maximumFrequency = frequency;

                if (frequency < minimumFrequency)
                    minimumFrequency = frequency;

                count++;
            }

            averageFrequency = total / count;

            if (count > 0)
            {
                m_averageFrequency = averageFrequency;
                m_maximumFrequency = maximumFrequency;
                m_minimumFrequency = minimumFrequency;
            }

            if (ReportUnreasonableResultsAsNaN)
            {
                if (m_averageFrequency <= LowFrequencyThreshold || m_averageFrequency >= HighFrequencyThreshold)
                    m_averageFrequency = double.NaN;

                if (m_maximumFrequency <= LowFrequencyThreshold || m_maximumFrequency >= HighFrequencyThreshold)
                    m_maximumFrequency = double.NaN;

                if (m_minimumFrequency <= LowFrequencyThreshold || m_minimumFrequency >= HighFrequencyThreshold)
                    m_minimumFrequency = double.NaN;
            }

            List<IMeasurement> outputMeasurements = new();

            if (m_outputMap.TryGetValue(Output.Average, out var avgMeasurement))
                outputMeasurements.Add(Measurement.Clone(avgMeasurement, averageFrequency, frame.Timestamp));

            if (m_outputMap.TryGetValue(Output.Maximum, out var maxMeasurement))
                outputMeasurements.Add(Measurement.Clone(maxMeasurement, maximumFrequency, frame.Timestamp));

            if (m_outputMap.TryGetValue(Output.Minimum, out var minMeasurement))
                outputMeasurements.Add(Measurement.Clone(minMeasurement, minimumFrequency, frame.Timestamp));

            // Provide measurements for external consumption
            OnNewMeasurements(outputMeasurements.ToArray());
        }
        else
        {
            m_averageFrequency = 0.0D;
            m_maximumFrequency = 0.0D;
            m_minimumFrequency = 0.0D;
        }
    }

    #endregion
}