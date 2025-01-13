//******************************************************************************************************
//  CalculatedMeasurementBase.cs - Gbtc
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
//  10/19/2009 - J. Ritchie Carroll
//       Generated original version of source code.
//  04/21/2010 - J. Ritchie Carroll
//       Added signal type summary to the calculated measurement status.
//  12/04/2012 - J. Ritchie Carroll
//       Migrated to PhasorProtocolAdapters project.
//  12/13/2012 - Starlynn Danyelle Gilliam
//       Modified Header.
//
//******************************************************************************************************

using System.ComponentModel;
using System.Text;
using Gemstone.Numeric.EE;
using Gemstone.StringExtensions;
using Gemstone.Timeseries;
using Gemstone.Timeseries.Adapters;
using Gemstone.Timeseries.Data;

namespace PhasorProtocolAdapters;

/// <summary>
/// Represents the base class for calculated measurements that use phasor data.
/// </summary>
/// <remarks>
/// This base class extends <see cref="ActionAdapterBase"/> by automatically looking up the
/// <see cref="SignalType"/> for each of the input and output measurements.
/// </remarks>
public abstract class CalculatedMeasurementBase : ActionAdapterBase
{
    #region [ Members ]

    // Fields
    private string? m_configurationSection;
    private bool m_supportsTemporalProcessing;

    #endregion

    #region [ Properties ]

    /// <summary>
    /// Gets or sets primary keys of input measurements the calculated measurement expects.
    /// </summary>
    [ConnectionStringParameter]
    [DefaultValue(null)]
    [Description("Defines primary keys of input measurements the action adapter expects; can be one of a filter expression, measurement key, point tag or Guid.")]
    public override MeasurementKey[]? InputMeasurementKeys
    {
        get => base.InputMeasurementKeys;
        set
        {
            base.InputMeasurementKeys = value;
            InputMeasurementKeyTypes = DataSource.GetSignalTypes(value);
        }
    }

    /// <summary>
    /// Gets or sets output measurements that the calculated measurement will produce, if any.
    /// </summary>
    [ConnectionStringParameter]
    [DefaultValue(null)]
    [Description("Defines primary keys of output measurements the action adapter expects; can be one of a filter expression, measurement key, point tag or Guid.")]
    public override IMeasurement[]? OutputMeasurements
    {
        get => base.OutputMeasurements;
        set
        {
            base.OutputMeasurements = value;
            OutputMeasurementTypes = DataSource.GetSignalTypes(value);
        }
    }

    /// <summary>
    /// Gets or sets input measurement <see cref="SignalType"/>'s for each of the <see cref="ActionAdapterBase.InputMeasurementKeys"/>, if any.
    /// </summary>
    public virtual SignalType[]? InputMeasurementKeyTypes { get; private set; }

    /// <summary>
    /// Gets or sets output measurement <see cref="SignalType"/>'s for each of the <see cref="ActionAdapterBase.OutputMeasurements"/>, if any.
    /// </summary>
    public virtual SignalType[]? OutputMeasurementTypes { get; private set; }

    /// <summary>
    /// Gets or sets the configuration section to use for this <see cref="CalculatedMeasurementBase"/>.
    /// </summary>
    public virtual string? ConfigurationSection
    {
        get => m_configurationSection;
        set => m_configurationSection = value;
    }

    /// <summary>
    /// Gets the flag indicating if this adapter supports temporal processing.
    /// </summary>
    public override bool SupportsTemporalProcessing => m_supportsTemporalProcessing;

    /// <summary>
    /// Returns the detailed status of the calculated measurement.
    /// </summary>
    /// <remarks>
    /// Derived classes should extend status with implementation specific information.
    /// </remarks>
    public override string Status
    {
        get
        {
            StringBuilder status = new();
            int count;

            status.Append($"     Configuration section: {ConfigurationSection}");
            status.AppendLine();
            status.Append(base.Status);

            if (OutputMeasurements is { Length: > 0 })
            {
                status.AppendLine();
                status.AppendLine("Output measurements signal type summary:");
                status.AppendLine();

                foreach (SignalType signalType in Enum.GetValues(typeof(SignalType)))
                {
                    count = OutputMeasurements.Where((_, index) => OutputMeasurementTypes?[index] == signalType).Count();

                    if (count <= 0)
                        continue;

                    status.Append($"{count.ToString().PadLeft(15)} {signalType.GetFormattedName()} signal{(count > 1 ? "s" : "")}");
                    status.AppendLine();
                }
            }

            if (InputMeasurementKeys is { Length: > 0 })
            {
                status.AppendLine();
                status.AppendLine("Input measurement keys signal type summary:");
                status.AppendLine();

                foreach (SignalType signalType in Enum.GetValues(typeof(SignalType)))
                {
                    count = InputMeasurementKeys.Where((_, index) => InputMeasurementKeyTypes?[index] == signalType).Count();

                    if (count <= 0)
                        continue;

                    status.Append($"{count.ToString().PadLeft(15)} {signalType.GetFormattedName()} signal{(count > 1 ? "s" : "")}");
                    status.AppendLine();
                }
            }

            return status.ToString();
        }
    }

    #endregion

    #region [ Methods ]

    /// <summary>
    /// Initializes <see cref="CalculatedMeasurementBase"/>.
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();

        Dictionary<string, string> settings = Settings;

        // Load optional parameters
        if (!settings.TryGetValue("configurationSection", out m_configurationSection))
            m_configurationSection = Name;

        if (string.IsNullOrEmpty(m_configurationSection))
            m_configurationSection = Name;

        m_supportsTemporalProcessing = settings.TryGetValue("supportsTemporalProcessing", out string? setting) && setting.ParseBoolean();
    }

    #endregion
}