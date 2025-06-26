
//******************************************************************************************************
//  OSCDummyAlarm.cs - Gbtc
//
//  Copyright © 2025, Grid Protection Alliance.  All Rights Reserved.
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
//  06/22/2025 - G. Santos
//       Generated original version of source code.
//******************************************************************************************************

using System.ComponentModel;
using Gemstone.Data;
using Gemstone.Data.Model;
using Gemstone.Timeseries;
using Gemstone.Timeseries.Adapters;
using Gemstone.Timeseries.Model;
using Newtonsoft.Json;
using ConfigSettings = Gemstone.Configuration.Settings;

namespace DataQualityMonitoring;

/// <summary>
/// Action adapter that generates alarm measurements based on alarm definitions from the database.
/// </summary>
[Description("Temp OSC Alarm Generator")]

public class OSCDummyAlarm : ActionAdapterBase
{
    #region [ Constructors ]

    /// <summary>
    /// Creates a new instance of the <see cref="OSCDummyAlarm"/> class.
    /// </summary>
    public OSCDummyAlarm() { }

    #endregion

    #region [ Properties ]

    private double lastMeasurement = 0;
    private Guid currentGuid;
    public override bool SupportsTemporalProcessing => true;

    #endregion

    #region [ Methods ]

    public override void Initialize()
    {
        base.Initialize();

        Dictionary<string, string> settings = Settings;

        if (InputMeasurementKeys is null)
            throw new InvalidOperationException("No input measurements were specified for the dummy alarm.");

        if (InputMeasurementKeys.Length != 1 || OutputMeasurements.Length != 1)
            throw new InvalidOperationException("Dummy alarm only alows one input signal and one output signal.");
    }

    protected override void PublishFrame(IFrame frame, int index)
    {
        // if it contains an alarm that is an oscillation We need to trigger computation
        if (frame.Measurements.TryGetValue(InputMeasurementKeys[0], out IMeasurement inputMeasurement))
        {
            if (inputMeasurement.AdjustedValue != lastMeasurement)
            {
                using AdoDataConnection connection = new(ConfigSettings.Instance);
                TableOperations<EventDetails> tableOperations = new(connection);
                EventDetails details;
                if (inputMeasurement.AdjustedValue != 0)
                {
                    OnStatusMessage(Gemstone.Diagnostics.MessageLevel.Info, $"Oscillation started at {frame.Timestamp}");
                    currentGuid = Guid.NewGuid();
                    details = new()
                    {
                        StartTime = frame.Timestamp,
                        EventGuid = currentGuid,
                        Type = "oscillation",
                        MeasurementID = OutputMeasurements[0].ID,
                        Details = JsonConvert.SerializeObject(new
                        {
                            VoltageSignalID = "NORTHFLD-NFD34:VM",
                            Frequency = "1.343"
                        })
                    };
                }
                else
                {
                    OnStatusMessage(Gemstone.Diagnostics.MessageLevel.Info, $"Oscillation ended at {frame.Timestamp}");
                    details = tableOperations.QueryRecordWhere("EventGuid={0}", currentGuid);
                    details.EndTime = frame.Timestamp;
                }
                tableOperations.AddNewOrUpdateRecord(details);
                AlarmMeasurement measurement = new AlarmMeasurement
                {
                    Timestamp = frame.Timestamp,
                    Value = inputMeasurement.AdjustedValue,
                    AlarmID = currentGuid
                };
                measurement.Metadata = MeasurementKey.LookUpBySignalID(OutputMeasurements[0].ID).Metadata;
                OnNewMeasurements([measurement]);
            }
            lastMeasurement = inputMeasurement.AdjustedValue;
        }
    }

    #endregion
}

