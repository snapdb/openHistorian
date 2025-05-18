
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

using Gemstone;
using Gemstone.Collections.CollectionExtensions;
using Gemstone.Data;
using Gemstone.Data.Model;
using Gemstone.Diagnostics;
using Gemstone.Numeric.Analysis;
using Gemstone.Numeric.EE;
using Gemstone.StringExtensions;
using Gemstone.Threading.SynchronizedOperations;
using Gemstone.Timeseries;
using Gemstone.Timeseries.Adapters;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using MathNet.Numerics.Data.Matlab;
using Gemstone.Numeric;
using Gemstone.Numeric.Interpolation;
using Gemstone.Numeric.UnitExtensions;
using Gemstone.Units;
using MathNet.Numerics.Statistics;
using SignalType = Gemstone.Numeric.EE.SignalType;
using PhasorRecord = Gemstone.Timeseries.Model.Phasor;
using ConfigSettings = Gemstone.Configuration.Settings;
using MathNet.Numerics.LinearAlgebra;
using PhasorProtocolAdapters;
using Gemstone.Timeseries.Model;
using MathNet.Numerics.LinearAlgebra.Factorization;

namespace DataQualityMonitoring;

/// <summary>
/// Action adapter that generates alarm measurements based on alarm definitions from the database.
/// </summary>
[Description("DEF Identification: uses the computed Dissipating Energy Flow to identify the source of an oscillation")]

public class DEFIdentificationAdapter : CalculatedMeasurementBase
{
    #region [ Members ]

   
    private readonly TaskSynchronizedOperation m_computeRank;
    private readonly ConcurrentQueue<EventDetails> m_computationQueue;

    private int m_numDEComponents = 15; // GENSET_28

    private Gemstone.Numeric.Matrix<double> m_DeNum;
    private List<DELabel> m_DeLabels;

    private class DELabel
    {
        public bool Enabled { get; set; }
        public string Area { get; set; }
        public string SourceSubstation { get; set; }
        public string SourceGenerator { get; set; }
        public string[] Label { get; set; }

        public DELabel(string[] row)
        {
            Enabled = bool.Parse(row[0]);
            Area = row[1];
            SourceSubstation = row[2];
            SourceGenerator = row[3];
            Label = row.Skip(4).ToArray();
        }
    }

    #endregion

    #region [ Constructors ]

    /// <summary>
    /// Creates a new instance of the <see cref="DEFIdentificationAdapter"/> class.
    /// </summary>
    public DEFIdentificationAdapter()
    {
        m_computeRank = new TaskSynchronizedOperation(ComputeRank, ex => OnProcessException(MessageLevel.Error, ex));
        m_computationQueue = new ConcurrentQueue<EventDetails>();
    }

    #endregion

    #region [ Properties ]

    [ConnectionStringParameter()]
    public string ClassificationFile { get; set; } = string.Empty;

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

        // Load File For Rank Idneitfication
        LoadClassificationFile();

    }

    private void LoadClassificationFile()
    {
        int numLabels = 0;
        List<double[]> numbers = new();
        m_DeLabels = new List<DELabel>();

        using (StreamReader reader = new(ClassificationFile))
        {
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] cells = line.Split(",");

                if (numLabels == 0)
                {
                    numLabels = (cells.Count() - 5) / 2;
                }

                numbers.Add(cells.Skip(5 + numLabels).Select(c => double.Parse(c)).ToArray());
                m_DeLabels.Add(new DELabel(cells.Skip(1).Take(4 + numLabels).ToArray());
            }
        }
        m_DeNum = new Gemstone.Numeric.Matrix<double>(numbers.ToArray());

    }
    public void LoadFile()
    {
        FramesPerSecond = 30;

        JObject oscillation = new JObject();
        oscillation.Add("VoltageSignalID", Guid.NewGuid());
        oscillation.Add("Frequency", "1.343");
        oscillation.Add("Hysteresis", "20");

        string dataFile = "C:\\Users\\clackner\\Downloads\\DataAlarmLine.mat";
        // Load EventData
        MathNet.Numerics.LinearAlgebra.Matrix<double> Vm = MatlabReader.Read<double>(dataFile, "Vm");
        MathNet.Numerics.LinearAlgebra.Matrix<double> Va = MatlabReader.Read<double>(dataFile, "Va");
        MathNet.Numerics.LinearAlgebra.Matrix<double> Im = MatlabReader.Read<double>(dataFile, "Im");
        MathNet.Numerics.LinearAlgebra.Matrix<double> Ia = MatlabReader.Read<double>(dataFile, "Ia");
        MathNet.Numerics.LinearAlgebra.Matrix<double> f = MatlabReader.Read<double>(dataFile, "Fbus");

        Ticks Tstart = DateTime.Parse("2025-01-26 08:43:19");

        Ticks[] Time = Vm.ToColumnArrays().First().Select((v,i) => (Ticks)(Tstart + (double)i*1/30.0*Ticks.PerSecond)).ToArray();


       

        EventDetails evt = new EventDetails()
        {
            StartTime = DateTime.Parse("2025-01-26 08:46:19"),
            Details = oscillation.ToString()
        };

        ComputeRank(evt, new List<LineData>() { data });

    }


    private async Task ComputeRank()
    {
        while (m_computationQueue.TryDequeue(out EventDetails oscillation))
            ComputeRank(oscillation);
    }

    private void ComputeRank(EventDetails oscillation)
    {
        JObject osc = JObject.Parse(oscillation.Details);

        // This should come from the Event
        Gemstone.Numeric.Matrix<double> DE = new Gemstone.Numeric.Matrix<double>(1, 1, 30);

        // This should also come from the Event
        List<string> PointTags = new List<string>();


        double[] Correlation = new double[m_DeLabels.Count()];
        double[] Rank = new double[m_DeLabels.Count()];

        Gemstone.Numeric.Matrix<double> rank = null;

        int nt = Math.Min(m_numDEComponents, m_DeNum.NColumns);

        for (int i = 0; i < m_DeLabels.Count(); i++)
        {
            Correlation[i] = double.NaN;
            Rank[i] = double.NaN;
            if (m_DeLabels[i].Enabled == false)
                continue;

            List<string> tags = m_DeLabels[i].Label.Take(nt).Intersect(PointTags).ToList();

            if (tags.Count() == 0)
            {
                Correlation[i] = 0;
                Rank[i] = -999;
            }
            else
            {
                Correlation[i] = Corr(new (m_DeNum[i].Where((item,j) => tags.Contains(m_DeLabels[i].Label[j])).ToArray(),1) ,DE(id2,1));
                for j = 1:numel(id2)
                    rank.r(i) = rank.r(i) + DEnum(i, id3(j)) * DE(id2(j));
                end
            }

        }

        return;
    }

    private Gemstone.Numeric.Matrix<double> Corr(Gemstone.Numeric.Matrix<double> X, Gemstone.Numeric.Matrix<double> Y)
    {
    
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

