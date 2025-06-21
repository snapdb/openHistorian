//******************************************************************************************************
//  VIFCalculatedMeasurementBase.cs - Gbtc
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
//  06/20/2025 - C. Lackner
//       Generated original version of source code.
//
//******************************************************************************************************
// ReSharper disable InconsistentNaming

using Gemstone.Data.Model;
using Gemstone.Timeseries;
using Gemstone.Timeseries.Adapters;
using Gemstone.Units;
using PhasorProtocolAdapters;
using System.ComponentModel;
using System.Data;
using System.Text;
using MeasurementRecord = Gemstone.Timeseries.Model.Measurement;
using PhasorRecord = Gemstone.Timeseries.Model.Phasor;

namespace PowerCalculations;

/// <summary>
/// Represents the base class for a calculated measurements that uses VI phasor data and frequency data.
/// </summary>
/// <remarks>
/// This base class extends <see cref="CalculatedMeasurementBase"/> by automatically looking up the
/// <see cref="PhasorRecord"/> for currents and associated voltages as well as the associated frequency.
/// </remarks>
public abstract class VIFCalculatedMeasurementBase : VICalculatedMeasurementBase
{
    #region [ Members ]

    protected VIFSet[] m_VIFSets;
    public class VIFSet: VISet
    {
        public MeasurementKey[] Frequency { get; set; }

    }

    #endregion

    #region [ Properties ]

    /// <summary>
    /// Gets or sets the Frequency PointTag to be used.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Defines the Frequencies to be used. If not supplied the system will use the first frequency attached to the same PMU as the curren phasor.")]
    [DefaultValue("")]
    public string Frequencies { get; set; }

    public override string Status
    {
        get
        {
            const int MaxVIPairsToShow = 10;
            StringBuilder status = new();

            status.Append(base.Status);

            status.AppendLine("Associated Frequencies:");
            status.AppendLine();

            for (int i = 0; i < Math.Min(m_VIFSets.Length, MaxVIPairsToShow); i++)
            {
             
                status.AppendLine($"   Voltage Magnitudes: {string.Join(", ", m_VISets[i].VoltageMagnitude.Select(key => key.SignalID))}");
                status.AppendLine($"   Voltage Angles: {string.Join(", ", m_VISets[i].VoltageAngle.Select((key) => key.SignalID))}");
                status.AppendLine($"   Frequencies: {string.Join(", ", m_VIFSets[i].Frequency.Select((key) => key.SignalID))}");
                status.AppendLine();
            }
            if (m_VISets.Length > MaxVIPairsToShow)
                status.AppendLine("...".PadLeft(26));

            status.AppendLine();

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

        if (settings.TryGetValue(nameof(Frequencies), out string setting))
            Frequencies = setting;
        else
            Frequencies = string.Empty;

        ParseFrequencies(Frequencies);

    }

    private void ParseFrequencies(string frequency)
    {
        Func<DataRow, MeasurementRecord?> loadMeasurement = TableOperations<MeasurementRecord>.LoadRecordFunction();
        Func<DataRow, PhasorRecord?> loadPhasor = TableOperations<PhasorRecord>.LoadRecordFunction();
       
        m_VIFSets = m_VISets.Select((s) => new VIFSet()
        {
            CurrentAngle = s.CurrentAngle,
            CurrentMagnitude = s.CurrentMagnitude,
            VoltageAngle = s.VoltageAngle,
            VoltageMagnitude = s.VoltageMagnitude,
            Frequency = new MeasurementKey[0]
        }).ToArray();

       

        if (string.IsNullOrEmpty(frequency))
        {
            foreach (VIFSet set in m_VIFSets)
            {
                MeasurementRecord current = loadMeasurement(DataSource.Tables["ActiveMeasurement"].Select($"ID={set.CurrentMagnitude}")[0]);
                set.Frequency = AdapterBase.ParseInputMeasurementKeys(DataSource, true, $"FILTER ActiveMeasurement WHERE DeviceID={current.DeviceID} AND SignalTYPE LIKE 'FREQ'").ToArray();
            }
        }
        else
        {
            MeasurementKey[] frequencies = AdapterBase.ParseInputMeasurementKeys(DataSource, true, frequency);

            int index = 0;
            if (frequencies.Count() > m_VIFSets.Count())
                OnStatusMessage(Gemstone.Diagnostics.MessageLevel.Warning, $"There are more frequencies supplied than neccesarry. The Adapter will ignore the last {frequencies.Count() - m_VIFSets.Count()} freuqencies.");
            if (frequencies.Count() < m_VIFSets.Count())
                OnStatusMessage(Gemstone.Diagnostics.MessageLevel.Warning, $"There are fewer frequencies supplied than voltage/current pairs. The Adapter will repeat {m_VIFSets.Count() - frequencies.Count()} frequencies.");

            foreach (VIFSet set in m_VIFSets)
            {
                set.Frequency = new MeasurementKey[1] { frequencies[index % frequencies.Count()] };
                index++;
            }
        }

        InputMeasurementKeys = m_VIFSets.SelectMany((s) => s.VoltageMagnitude.Concat(s.VoltageAngle).Concat(s.Frequency).Concat(new MeasurementKey[] { s.CurrentMagnitude, s.CurrentAngle })).ToArray();
    }

    #endregion
}