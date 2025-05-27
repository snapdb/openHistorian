
//******************************************************************************************************
//  DEFIdentificationAdapter.cs - Gbtc
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
//  02/21/2025 - C. Lackner
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
using MathNet.Numerics.LinearAlgebra.Complex;
using MathNet.Numerics.Statistics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PhasorProtocolAdapters;
using ConfigSettings = Gemstone.Configuration.Settings;
using SignalType = Gemstone.Numeric.EE.SignalType;

namespace DataQualityMonitoring;

/// <summary>
/// Action adapter that generates alarm measurements based on alarm definitions from the database.
/// </summary>
[Description("DEF Identification: uses the computed Dissipating Energy Flow to identify the source of an oscillation")]

public class DEFPowerworldVisualizerAdapter : CalculatedMeasurementBase
{
    #region [ Members ]

   
    private readonly TaskSynchronizedOperation m_computeRank;
    private readonly ConcurrentQueue<EventDetails> m_computationQueue;

    private bool VisualizeCPSD = true; // ff31
    private bool VisualizeCDEF = true; // ff26
    private string ModelCaseFile = "model_pf_pwrflow_ttc_calculator_ver3211.aux"; // ff32
    private string OneLineFile = "3211_oneline_rev22.pwd"; // ff28
    private string DEFromFile = "DE_From.aux"; // fn5
    private string DEToFile = "DE_To.aux"; // fn5
    private int imageQuality = 80; // ff29, scale of 1-100, 100 being highest
    private int imageResolution = 5; // ff30, [5,12] recommended

    #endregion

    #region [ Constructors ]

    /// <summary>
    /// Creates a new instance of the <see cref="DEFPowerworldVisualizerAdapter"/> class.
    /// </summary>
    public DEFPowerworldVisualizerAdapter()
    {
        m_computeRank = new TaskSynchronizedOperation(CreateVisual, ex => OnProcessException(MessageLevel.Error, ex));
        m_computationQueue = new ConcurrentQueue<EventDetails>();
    }

    #endregion

    #region [ Properties ]

    [ConnectionStringParameter()]
    public string PowerworldScriptDirectory { get; set; } = string.Empty; // ff27

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
        while (m_computationQueue.TryDequeue(out EventDetails oscillation))
            CreateVisual(oscillation);
    }

    public void CreateVisual(EventDetails oscillation)
    {
        JObject osc = JObject.Parse(oscillation.Details);
        IEnumerable<string> lineIds = DEFComputationAdapter.ParseLineIds(osc);
        IEnumerable<string> substations = DEFComputationAdapter.ParseSubstations(osc);
        string alarmTime = DEFComputationAdapter.ParseAlarmTime(osc);
        string[] lineLabels = lineIds.Zip(substations, (i, s) => $"\"{s}|{i}\"").ToArray();

        PowerworldScriptDirectory = "C:\\Users\\gcsantos\\source\\MATLAB\\m-code\\temp";
        // ToDo: Stop non-windows from using this before it throws an exception here?
        Type simAuto = Type.GetTypeFromProgID("pwrworld.SimulatorAuto");
        object simAutoConnection = Activator.CreateInstance(simAuto);
        if (simAutoConnection is null)
            throw new NullReferenceException("Unable to create connection to powerworld simAuto addon.");
        string cdefJpg;
        string cpsdJpg;
        try
        {
            MethodInfo scriptCommandMethod = simAuto.GetMethod("RunScriptCommand");

            // Load model case
            scriptCommandMethod.Invoke(simAutoConnection, [$"NewCase; OpenCase(\"{Path.Combine(PowerworldScriptDirectory, ModelCaseFile)}\",AUX);"]);

            if (VisualizeCDEF)
            {
                Matrix<double> cdef = DEFComputationAdapter.ParseDeCdef(osc);
                cdefJpg = CreateVisual(cdef, lineLabels, "DE_NEPEX", alarmTime, simAutoConnection, scriptCommandMethod);
            }
            if (VisualizeCPSD)
            {
                Matrix<double> cpsd = DEFComputationAdapter.ParseDeCpsd(osc);
                cpsdJpg = CreateVisual(cpsd, lineLabels, "DE_NEPEXcpsd", alarmTime, simAutoConnection, scriptCommandMethod);
            }
        }
        finally
        {
            Marshal.FinalReleaseComObject(simAutoConnection);
        }
        // ToDo: Do something with jpgs made
    }

    private string CreateVisual(Matrix<double> DE, string[] lineLabels, string label, string alarmTimeStamp, object simAutoConnection, MethodInfo scriptCommandMethod)
    {
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
        string jpgName = $"label_{alarmTimeStamp}.jpg";
        string jpgpath = Path.Combine(PowerworldScriptDirectory, jpgName);
        scriptCommandMethod.Invoke(simAutoConnection, [$"ExportOneline(\"{jpgName}\", \"{OneLineFile}\", JPG,,,YES,[{imageQuality.ToString()},{imageResolution.ToString()}]);"]);
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
                m_computationQueue.Enqueue(oscillation);
            }
            m_computeRank.TryRunAsync();
        }
    }

    #endregion

    #region [ Static ]


    #endregion
}

