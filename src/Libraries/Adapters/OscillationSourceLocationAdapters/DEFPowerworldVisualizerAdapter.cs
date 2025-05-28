
//******************************************************************************************************
//  DEFPowerworldVisualizerAdapter.cs - Gbtc
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
//  05/21/2025 - G. Santos
//       Generated original version of source code.
//******************************************************************************************************

using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Runtime.InteropServices;
using Gemstone.Data;
using Gemstone.Data.Model;
using Gemstone.Diagnostics;
using Gemstone.Numeric;
using Gemstone.Threading.SynchronizedOperations;
using Gemstone.Timeseries;
using Gemstone.Timeseries.Adapters;
using Gemstone.Timeseries.Model;
using Newtonsoft.Json.Linq;
using PhasorProtocolAdapters;
using ConfigSettings = Gemstone.Configuration.Settings;
using SignalType = Gemstone.Numeric.EE.SignalType;

namespace DataQualityMonitoring;

/// <summary>
/// Action adapter that generates alarm measurements based on alarm definitions from the database.
/// </summary>
[Description("DEF Powerworld Visualizer: uses the computed Dissipating Energy Flow to produce visualization files")]

public class DEFPowerworldVisualizerAdapter : CalculatedMeasurementBase
{
    #region [ Members ]

    private readonly TaskSynchronizedOperation m_visualDE;
    private readonly ConcurrentQueue<EventDetails> m_visualizationQueue;

    #endregion

    #region [ Constructors ]

    /// <summary>
    /// Creates a new instance of the <see cref="DEFPowerworldVisualizerAdapter"/> class.
    /// </summary>
    public DEFPowerworldVisualizerAdapter()
    {
        m_visualDE = new TaskSynchronizedOperation(CreateVisual, ex => OnProcessException(MessageLevel.Error, ex));
        m_visualizationQueue = new ConcurrentQueue<EventDetails>();
    }

    #endregion

    #region [ Properties ]

    /// <summary>
    /// Flag to control creation of CPSD Visualization Files
    /// </summary>
    [ConnectionStringParameter]
    [DefaultValue(true)]
    [Description("Flag to control creation of CPSD Visualization Files")]
    public bool VisualizeCPSD
    {
        get; set;
    }

    /// <summary>
    /// Flag to control creation of CDEF Visualization Files
    /// </summary>
    [ConnectionStringParameter]
    [DefaultValue(true)]
    [Description("Flag to control creation of CDEF Visualization Files")]
    public bool VisualizeCDEF
    {
        get; set;
    }

    /// <summary>
    /// Quality of the image; 1...100 with 100 being the highest quality image
    /// </summary>
    [ConnectionStringParameter]
    [DefaultValue(80)]
    [Description("Quality of the image; 1...100 with 100 being the highest quality image")]
    public int ImageQuality
    {
        get; set;
    }

    /// <summary>
    /// The image resolution scalar; recommended for this application 5..12
    /// </summary>
    [ConnectionStringParameter]
    [DefaultValue(5)]
    [Description("The image resolution scalar; recommended for this application 5..12")]
    public int ImageResolution
    {
        get; set;
    }

    /// <summary>
    /// Directory which contains the powerworld scripts
    /// </summary>
    [ConnectionStringParameter]
    [Description("Directory which contains the powerworld scripts")]
    public string PowerworldScriptDirectory
    {
        get; set;
    }

    /// <summary>
    /// Power flow model file name
    /// </summary>
    [ConnectionStringParameter]
    [DefaultValue("model_pf_pwrflow_ttc_calculator_ver3211.aux")]
    [Description("Power flow model file name")]
    public string ModelCaseFile
    {
        get; set;
    }

    /// <summary>
    /// One line diagram of ISO-NE system file name
    /// </summary>
    [ConnectionStringParameter]
    [DefaultValue("3211_oneline_rev22.pwd")]
    [Description("One line diagram of ISO-NE system file name")]
    public string OneLineFile
    {
        get; set;
    }

    /// <summary>
    /// Temporary file that holds DE input information file name
    /// </summary>
    [ConnectionStringParameter]
    [DefaultValue("DE_From.aux")]
    [Description("Temporary file that holds DE input information file name")]
    public string DEFromFile
    {
        get; set;
    }

    /// <summary>
    /// Temporary file that holds DE input information file name
    /// </summary>
    [ConnectionStringParameter]
    [DefaultValue("DE_To.aux")]
    [Description("Temporary file that holds DE input information file name")]
    public string DEToFile
    {
        get; set;
    }

    /// <summary>
    /// Text portion of the output visualization file for CDEF
    /// </summary>
    [ConnectionStringParameter]
    [DefaultValue("DE_NEPEX")]
    [Description("Text portion of the output visualization file for CDEF")]
    public string CDEFLabel
    {
        get; set;
    }

    /// <summary>
    /// Text portion of the output visualization file for CPSD
    /// </summary>
    [ConnectionStringParameter]
    [DefaultValue("DE_NEPEXcpsd")]
    [Description("Text portion of the output visualization file for CPSD")]
    public string CPSDLabel
    {
        get; set;
    }

    #endregion

    #region [ Methods ]

    public override void Initialize()
    {

        base.Initialize();

        Dictionary<string, string> settings = Settings;

        if (InputMeasurementKeys is null || InputMeasurementKeyTypes is null)
            throw new InvalidOperationException("No input measurements were specified for the DEF Computation calculator.");

        if (!InputMeasurementKeyTypes.Where(t => t == SignalType.ALRM).Any())
            throw new InvalidOperationException("At least 1 valid event measurement is requried.");
    }

    private async Task CreateVisual()
    {
        while (m_visualizationQueue.TryDequeue(out EventDetails oscillation))
            CreateVisual(oscillation);
    }

    public void CreateVisual(EventDetails oscillation)
    {
        JObject osc = JObject.Parse(oscillation.Details);
        IEnumerable<string> lineIds = DEFComputationAdapter.ParseLineIds(osc);
        IEnumerable<string> substations = DEFComputationAdapter.ParseSubstations(osc);
        string alarmTime = DEFComputationAdapter.ParseAlarmTime(osc);
        string[] lineLabels = lineIds.Zip(substations, (i, s) => $"\"{s}|{i}\"").ToArray();

        // ToDo: Stop non-windows from using this before it throws an exception here?
        Type simAuto = Type.GetTypeFromProgID("pwrworld.SimulatorAuto");
        object simAutoConnection = Activator.CreateInstance(simAuto);
        if (simAutoConnection is null)
            throw new NullReferenceException("Unable to create connection to powerworld simAuto addon.");
        string? cdefJpg;
        string? cpsdJpg;
        try
        {
            MethodInfo scriptCommandMethod = simAuto.GetMethod("RunScriptCommand");

            // Load model case
            scriptCommandMethod.Invoke(simAutoConnection, [$"NewCase; OpenCase(\"{Path.Combine(PowerworldScriptDirectory, ModelCaseFile)}\",AUX);"]);

            if (VisualizeCDEF)
            {
                Matrix<double> cdef = DEFComputationAdapter.ParseDeCdef(osc);
                cdefJpg = CreateVisual(cdef, lineLabels, CDEFLabel, alarmTime, simAutoConnection, scriptCommandMethod);
            }
            if (VisualizeCPSD)
            {
                Matrix<double> cpsd = DEFComputationAdapter.ParseDeCpsd(osc);
                cpsdJpg = CreateVisual(cpsd, lineLabels, CPSDLabel, alarmTime, simAutoConnection, scriptCommandMethod);
            }
        }
        finally
        {
            Marshal.FinalReleaseComObject(simAutoConnection);
        }
        // ToDo: Do something with jpgs made
    }

    private string? CreateVisual(Matrix<double> DE, string[] lineLabels, string label, string alarmTimeStamp, object simAutoConnection, MethodInfo scriptCommandMethod)
    {
        if (DE.NRows == 0) return null;
        CreatePowerworldInputFile(DE, lineLabels);
        // Load the rest of script for DE visualization
        scriptCommandMethod.Invoke(simAutoConnection, ["LoadAux(\"DE_VisualPrepare.aux\", CreateIfNotFound);"]);
        // Open Oneline on full view
        scriptCommandMethod.Invoke(simAutoConnection, [$"OpenOneline(\"{OneLineFile}\",,NO,YES,NAMENOMKV);"]);
        // Load display formatting for Branch Arrows
        scriptCommandMethod.Invoke(simAutoConnection, [$"LoadAXD(\"DE_CreateArrows_Branches.axd\",\"{OneLineFile}\",CreateIfNotFound);"]);
        // Load display formatting for Gen Arrows
        scriptCommandMethod.Invoke(simAutoConnection, [$"LoadAXD(\"DE_CreateArrows_Generators.axd\",\"{OneLineFile}\",CreateIfNotFound);"]);
        // Load display formatting for DE
        scriptCommandMethod.Invoke(simAutoConnection, [$"LoadAXD(\"DE_DisplaySettings.axd\",\"{OneLineFile}\",CreateIfNotFound);"]);
        // Switch to RUN mode to enable Dynamic Formatting
        scriptCommandMethod.Invoke(simAutoConnection, [$"EnterMode(RUN);"]);
        // Save oneline in JPG format
        string jpgName = $"{label}_{alarmTimeStamp}.jpg";
        string jpgpath = Path.Combine(PowerworldScriptDirectory, jpgName);
        scriptCommandMethod.Invoke(simAutoConnection, [$"ExportOneline(\"{jpgName}\", \"{OneLineFile}\", JPG,,,YES,[{ImageQuality.ToString()},{ImageResolution.ToString()}]);"]);
        return jpgpath;
    }

    private void CreatePowerworldInputFile(Matrix<double> de, string[] lineLabels)
    {
        using (StreamWriter writer = new StreamWriter(Path.Combine(PowerworldScriptDirectory, DEFromFile), false))
        {
            writer.WriteLine("DATA (Branch, [Label,CustomFloat])\n{");
            WriteDELines(de, lineLabels, writer);
            writer.WriteLine("}\nDATA (GEN, [Label,CustomFloat])\n{");
            WriteDELines(de, lineLabels, writer);
            writer.WriteLine("}");
        }
        using (StreamWriter writer = new StreamWriter(Path.Combine(PowerworldScriptDirectory, DEToFile), false))
        {
            writer.WriteLine("DATA (Branch, [Label,CustomFloat])\n{");
            WriteDELines(de, lineLabels, writer);
            writer.WriteLine("}");
        }
    }

    private void WriteDELines(Matrix<double> de, string[] lineLabels, StreamWriter writer)
    {
        for (int row = 0; row < de.NRows; row++)
        {
            double value = de[row][1];
            if (Math.Abs(value) <= 0.0001) continue;
            int index = (int)de[row][0];
            writer.WriteLine($"{lineLabels[index]} {value.ToString("F4")}");
        }
    }

    protected override void PublishFrame(IFrame frame, int index)
    {

        List<EventDetails> toBeProcessed = new List<EventDetails>();

        // if it contains an alarm that is an oscillation We need to trigger computation
        foreach (MeasurementKey key in InputMeasurementKeys)
        {
            if (frame.Measurements.TryGetValue(key, out IMeasurement alarm) && alarm is AlarmMeasurement && ((AlarmMeasurement)alarm).Value == 1)
            {
                // Grab the Alarm Details.
                using AdoDataConnection connection = new(ConfigSettings.Instance);
                TableOperations<EventDetails> tableOperations = new(connection);
                EventDetails details = tableOperations.QueryRecordWhere("EventGuid = {0}", ((AlarmMeasurement)alarm).AlarmID);
                if (details.Type == "oscillation")
                {
                    toBeProcessed.Add(details);
                }            
            }
        }
        
        if (toBeProcessed.Count > 0)
        {
            foreach (EventDetails oscillation in toBeProcessed)
            {
                // Process the Oscillation
                m_visualizationQueue.Enqueue(oscillation);
            }
            m_visualDE.TryRunAsync();
        }
    }

    #endregion
}

