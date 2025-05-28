
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
using Gemstone.Data;
using Gemstone.Data.Model;
using Gemstone.Diagnostics;
using Gemstone.Threading.SynchronizedOperations;
using Gemstone.Timeseries;
using Gemstone.Timeseries.Adapters;
using Gemstone.Timeseries.Model;
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

public class DEFIdentificationAdapter : CalculatedMeasurementBase
{
    #region [ Members ]

   
    private readonly TaskSynchronizedOperation m_computeRank;
    private readonly ConcurrentQueue<EventDetails> m_computationQueue;

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
            Enabled = row[0] == "1";
            Area = row[1];
            SourceSubstation = row[2];
            SourceGenerator = row[3];
            Label = row.Skip(4).ToArray();
        }
    }

    private class LabelMatch
    {
        public string Label { get; set; }
        public int IndexDE { get; set; }
        public int IndexLabel { get; set; }
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

    /// <summary>
    /// [p.u.] minimal rank for a valid classification
    /// </summary>
    [ConnectionStringParameter]
    [DefaultValue(0.2)]
    [Description("[p.u.] minimal rank for a valid classification")]
    public double MinRank
    {
        get; set;
    }

    /// <summary>
    /// [p.u.] range of rank for selection of substations; (0.2 ... 0.4)
    /// </summary>
    [ConnectionStringParameter]
    [DefaultValue(0.3)]
    [Description("[p.u.] range of rank for selection of substations; (0.2 ... 0.4)")]
    public double RankRange
    {
        get; set;
    }

    /// <summary>
    /// Maximal number of substation for uncertain classification
    /// </summary>
    [ConnectionStringParameter]
    [DefaultValue(5)]
    [Description("Maximal number of substation for uncertain classification")]
    public int MaxSubstations
    {
        get; set;
    }

    /// <summary>
    /// Number of transmission elements with max DE to be used in classifier (5...15)
    /// </summary>
    [ConnectionStringParameter]
    [DefaultValue(15)]
    [Description("Number of transmission elements with max DE to be used in classifier (5...15)")]
    public int NumDEComponents
    {
        get; set;
    }

    /// <summary>
    /// [p.u.] minimal threshold for DE and MW pattern correlation
    /// </summary>
    [ConnectionStringParameter]
    [DefaultValue(0.6)]
    [Description("[p.u.] minimal threshold for DE and MW pattern correlation")]
    public double CorrThresholdMin
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

        // Load File For Rank Idneitfication
        LoadClassificationFile();
    }

    private void LoadClassificationFile()
    {
        List<double[]> numbers = new();
        m_DeLabels = new List<DELabel>();

        using (StreamReader reader = new(ClassificationFile))
        {
            // skip header and get num labels
            int numLabels = 0;
            if (!reader.EndOfStream)
                numLabels = (reader.ReadLine().Split(",").Count() - 5) / 2;

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] cells = line.Split(",");

                numbers.Add(cells.Skip(5 + numLabels).Select(c => double.Parse(c)).ToArray());
                m_DeLabels.Add(new DELabel(cells.Skip(1).Take(4 + numLabels).ToArray()));
            }
        }
        m_DeNum = new Gemstone.Numeric.Matrix<double>(numbers.ToArray());
    }

    public void TestEventDetail(EventDetails osc)
    {
        FramesPerSecond = 30;
        ClassificationFile = "C:\\Users\\gcsantos\\source\\MATLAB\\m-code\\data\\DEpatternTemplate5-30-2024.csv";
        LoadClassificationFile();
        ComputeRank(osc);
    }

    private async Task ComputeRank()
    {
        while (m_computationQueue.TryDequeue(out EventDetails oscillation))
            ComputeRank(oscillation);
    }

    private void ComputeRank(EventDetails oscillation)
    {
        JObject osc = JObject.Parse(oscillation.Details);
        IEnumerable<string> lineIds = DEFComputationAdapter.ParseLineIds(osc);
        IEnumerable<string> substations = DEFComputationAdapter.ParseSubstations(osc);

        string[] lineLabels = lineIds.Zip(substations, (id, sub) => $"{id}__{sub}").ToArray();
        Gemstone.Numeric.Matrix<double> cpsd = DEFComputationAdapter.ParseDeCpsd(osc);
        ComputeRank(cpsd, lineLabels, out double rankProbCpsd, out string rankAreaCpsd, out string rankMsgCpsd, out int rankNSubCpsd);

        Gemstone.Numeric.Matrix<double> cdef = DEFComputationAdapter.ParseDeCdef(osc);
        ComputeRank(cdef, lineLabels, out double rankProbCdef, out string rankAreaCdef, out string rankMsgCdef, out int rankNSubCdef);
        // ToDo: Record results
    }

    private void ComputeRank(Gemstone.Numeric.Matrix<double> DE, string[] LineLabels, out double RankProb, out string RankArea, out string RankMsg, out int RankNSub)
    {
        double[] Correlation = new double[m_DeLabels.Count()];
        double[] Rank = new double[m_DeLabels.Count()];
        int nt = Math.Min(NumDEComponents, m_DeNum.NColumns);

        List<string> pointTags = new List<string>(LineLabels.Length);
        foreach(int index in DE.GetColumn(0))
            pointTags.Add(LineLabels[index]);

        for (int i = 0; i < m_DeLabels.Count(); i++)
        {
            Correlation[i] = double.NaN;
            Rank[i] = double.NaN;
            if (m_DeLabels[i].Enabled == false)
                continue;

            var tags = m_DeLabels[i].Label.Take(nt).Intersect(pointTags).Select((t) => new LabelMatch()
            {
                Label = t,
                IndexLabel = m_DeLabels[i].Label.TakeWhile((l) => l != t).Count(),
                IndexDE = pointTags.TakeWhile((l) => l != t).Count()
            });

            if (tags.Count() == 0)
            {
                Correlation[i] = 0;
                Rank[i] = -999;
            }
            else
            {
                Gemstone.Numeric.Matrix<double> labelMatrix = new(1, tags.Select((k) => m_DeNum[i][k.IndexLabel]).ToArray());
                Gemstone.Numeric.Matrix<double> deMatrix = new(1, tags.Select((k) => DE[k.IndexDE][1]).ToArray());

                Correlation[i] = Corr(labelMatrix, deMatrix)[0][0];
                Rank[i] = tags.Sum((t) => DE[t.IndexDE].Sum(v => v * m_DeNum[i][t.IndexLabel]));
            }

        }

        Tuple<double, int>? rankMax = Rank.Select((r, i) => new Tuple<double, int>(r, i)).MaxBy(tupe => tupe.Item1);
        if (rankMax is null)
            throw new IndexOutOfRangeException("Could not resolve max value index to an accesible index of the array.");

        double rankCorr = Correlation[rankMax.Item2];

        RankProb = 0;
        RankArea = "N/A";

        // Rank is below threshold; no certain identification
        if (rankMax.Item1 < MinRank)
        {
            RankMsg = "Source: cannot be reasonably localized";
            RankNSub = 0;
        }
        else
        {
            // rank, rank index
            IEnumerable<Tuple<double, int>> rankDescSort = Rank
                .Select((r, i) => new Tuple<double, int>(r, i))
                .OrderByDescending(tupe => tupe.Item1);
            double[] sortedRanks = rankDescSort.Select(t => t.Item1).ToArray();

            // finding take so we get all elements that meet the criteria
            int take = -1;
            for (int index = 0; index < sortedRanks.Length; index++)
            {
                if (sortedRanks[index] <= sortedRanks[0] * (1 - RankRange))
                {
                    take = index;
                    break;
                }
            }

            // label, rank index
            IEnumerable<Tuple<string, int>> area = m_DeLabels
                .Select(l => l.Area)
                .Zip(rankDescSort, (label, rankTupe) => new Tuple<string, int>(label, rankTupe.Item2))
                .OrderBy(tupe => tupe.Item2);

            // label, rank index
            List<Tuple<string, int>> substationsUnique = m_DeLabels
                .Select(l => l.SourceSubstation)
                .Zip(rankDescSort, (label, rankTupe) => new Tuple<string, int>(label, rankTupe.Item2))
                .OrderBy(tupe => tupe.Item2)
                .Take(take)
                .GroupBy(label => label.Item1)
                .Select(labelGroup => labelGroup.OrderBy(g => g.Item2).First())
                .ToList();

            RankNSub = take;

            // Definite identification
            if (take == 1)
            {
                DELabel label = m_DeLabels[rankDescSort.First().Item2];
                RankArea = label.Area;
                RankProb = 1;
                RankMsg = $"Source: Area={RankArea} (confidence=100%); Station={label.SourceSubstation}; Unit={label.SourceGenerator}";
            }
            else
            {
                // label, rank index, count
                IEnumerable<Tuple<string, int, int>> areasUnique = area
                    .Where((a, i) => substationsUnique.FindIndex(s => s.Item2 == i) != -1)
                    .GroupBy(label => label.Item1)
                    .Select(labelGroup =>
                    {
                        Tuple<string, int> first = labelGroup.OrderBy(g => g.Item2).First();
                        return new Tuple<string, int, int>(first.Item1, first.Item2, labelGroup.Count());
                    });

                if (areasUnique.Count() == 1)
                {
                    RankArea = areasUnique.First().Item1;
                    RankProb = 1;
                }
                else
                {
                    Tuple<string, int, int> primeArea = areasUnique.MaxBy(area => area.Item3);
                    RankArea = primeArea.Item1;
                    RankProb = (double)primeArea.Item3 / areasUnique.Sum(a => a.Item3);
                }

                if (substationsUnique.Count() == 1)
                {
                    RankMsg = $"Source: Area={RankArea} (confidence={Math.Round(RankProb * 100)}%); Station={substationsUnique.First().Item1}";
                }
                else
                {
                    int subTake = Math.Min(substationsUnique.Count(), MaxSubstations);
                    string buf = string.Join(',', substationsUnique.Take(subTake).Select(s => s.Item1));
                    if (RankNSub > MaxSubstations + 3)
                    {
                        RankMsg = $"Source: Area={RankArea} (confidence={Math.Round(RankProb * 100)}%); station cannot be reasonably localized";
                    }
                    else if (RankNSub > MaxSubstations && RankNSub <= MaxSubstations + 3)
                    {
                        RankMsg = $"Source: Area={RankArea} (confidence={Math.Round(RankProb * 100)}%); uncertain localization within multiple substations: {buf}...";
                    }
                    else
                    {
                        RankMsg = $"Source: Area={RankArea} (confidence={Math.Round(RankProb * 100)}%); likely localized within substations: {buf}...";
                    }
                }
            }

        }

        if (rankCorr < CorrThresholdMin)
            RankMsg = "Source: cannot be reasonably localized";

        return;
    }

    private Gemstone.Numeric.Matrix<double> ParseDE(string[] order, JObject osc)
    {
        double[] buf = JsonConvert.DeserializeObject<double[]>(osc[order[0]].ToString());
        Gemstone.Numeric.Matrix<double> DE = new(buf.Count(), order.Length, 0);
        for (int col = 0; col < DE.NColumns; col++)
        {
            if (col != 0)
                buf = JsonConvert.DeserializeObject<double[]>(osc[order[col]].ToString());
            for (int row = 0; row < DE.NRows; row++)
                DE[row][col] = buf[row];
        }
        return DE;
    }

    private Gemstone.Numeric.Matrix<double> Corr(Gemstone.Numeric.Matrix<double> X, Gemstone.Numeric.Matrix<double> Y)
    {
        // ToDo: Verifiy this function does what its supposed to, probably also change args to arrays
        Gemstone.Numeric.Matrix<double> rho = new(X.NColumns,Y.NColumns, 0.0D);
        double Xmean = 0;
        for (int row = 0; row < X.NRows; row++)
            Xmean += X.GetRow(row).Mean();
        Xmean /= X.NRows;
        double Ymean = 0;
        for (int row = 0; row < Y.NRows; row++)
            Ymean += Y.GetRow(row).Mean();
        Ymean /= Y.NRows;

        for (int j = 0; j < X.NColumns; j++)
        {
            for (int k = 0; k < Y.NColumns; k++)
            {
                double numerator = 0;
                double Xdenominator = 0;
                double Ydenominator = 0;

                double[] colX = X.GetColumn(j);
                double[] colY = Y.GetColumn(k);


               
                for (int i = 0; i < X.NRows; i++)
                {
                    numerator += (colX[i] - Xmean) * (colY[i] - Ymean);
                    Xdenominator += (colX[i] - Xmean) * (colX[i] - Xmean);
                    Ydenominator += (colY[i] - Ymean) * (colY[i] - Ymean);
                }

                rho[j][k] =  numerator / Math.Sqrt(Xdenominator * Ydenominator);
            }
        }

        return rho;      
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
}

