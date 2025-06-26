
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

using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using DataQualityMonitoring.Functions;
using DataQualityMonitoring.Model;
using Gemstone;
using Gemstone.Data;
using Gemstone.Data.Model;
using Gemstone.Diagnostics;
using Gemstone.Numeric;
using Gemstone.Threading.SynchronizedOperations;
using Gemstone.Timeseries;
using Gemstone.Timeseries.Adapters;
using Gemstone.Timeseries.Model;
using Gemstone.Units;
using MathNet.Numerics;
using MathNet.Numerics.Data.Matlab;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Numerics.Statistics;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Bcpg;
using PhasorProtocolAdapters;
using ConfigSettings = Gemstone.Configuration.Settings;
using PhasorRecord = Gemstone.Timeseries.Model.Phasor;
using SignalType = Gemstone.Numeric.EE.SignalType;

namespace DataQualityMonitoring;

/// <summary>
/// Action adapter that checks if an oscillation is ongoing.
/// </summary>
[Description("DEF Alarm: checks if an identified oscillation is ongoing")]

public class EMSAlarmMsgAdapter : CalculatedMeasurementBase
{
    #region [ Members ]

    List<Line> m_lines = new List<Line>();
    List<MeasurementKey> m_eventMeasurements;
    List<IFrame> m_frameQueue = new List<IFrame>();

    private int m_frameQueueLimit = 10;
    private readonly TaskSynchronizedOperation m_createMessage;
    private readonly ConcurrentQueue<Tuple<EventDetails, IEnumerable<LineData>>> m_testQueue;

    #endregion

    #region [ Constructors ]

    /// <summary>
    /// Creates a new instance of the <see cref="DEFPowerworldVisualizerAdapter"/> class.
    /// </summary>
    public EMSAlarmMsgAdapter()
    {
        m_createMessage = new TaskSynchronizedOperation(TestData, ex => OnProcessException(MessageLevel.Error, ex));
        m_testQueue = new ConcurrentQueue<Tuple<EventDetails, IEnumerable<LineData>>>();
    }

    #endregion

    #region [ Properties ]

    /// <summary>
    /// The maximum size of the Buffer in second
    /// </summary>
    [ConnectionStringParameter()]
    [Description("Maximum Buffer Time (in s)")]
    public double MaxTimeResolution { get; set; }

    /// <summary>
    /// Time step of a sliding window for FFT analysis
    /// </summary>
    [ConnectionStringParameter]
    [DefaultValue(2.0D)]
    [Description("Time step of a sliding window for FFT analysis")]
    public double Tstep { get; set; } = 2;

    /// <summary>
    /// Number of oscillatory cycles to define the length of an interval for FFT scans.
    /// </summary>
    [ConnectionStringParameter]
    [DefaultValue(5)]
    [Description("Number of oscillatory cycles to define the length of an interval for FFT scans.")]
    public double CMinW { get; set; } = 5;

    /// <summary>
    /// [pu] Magnitude threshold for ceased oscillation; magnitude at the end of observed time interval relative to average in study interval
    /// </summary>
    [ConnectionStringParameter]
    [DefaultValue(0.2)]
    [Description("[pu] Magnitude threshold for ceased oscillation; magnitude at the end of observed time interval relative to average in study interval")]
    public double CeasedOscThreshold { get; set; } = 0.2;

    /// <summary>
    /// Timestamp portion of the output message and file, given in a <see cref="DateTime"/> format string.
    /// </summary>
    [ConnectionStringParameter]
    [DefaultValue("HH:mm:ss")]
    [Description("Timestamp portion of the output message file, given in a DateTime format string.")]
    public string TimeStampFormat { get; set; } = "HH:mm:ss";

    /// <summary>
    /// Directory in which outputfiles are written.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Directory in which outputfiles are written.")]
    public string OutputDirectory { get; set; }

    /// <summary>
    /// Base name of the output file, which the timestamp is appended to.
    /// </summary>
    [ConnectionStringParameter]
    [Description("Directory in which outputfiles are written.")]
    public string OutputFileBaseName { get; set; }

    #endregion

    #region [ Methods ]

    public override void Initialize()
    {
        base.Initialize();
        this.InitializeParameters<VIFCalculatedMeasurementBase>();

        Dictionary<string, string> settings = Settings;

        if (InputMeasurementKeys is null || InputMeasurementKeyTypes is null)
            throw new InvalidOperationException("No input measurements were specified for the DEF ongoing alarm calculator.");

        if (!InputMeasurementKeyTypes.Where(t => t == SignalType.ALRM).Any())
            throw new InvalidOperationException("At least 1 valid event measurement is requried.");

        m_eventMeasurements = InputMeasurementKeys.Where((key, index) => InputMeasurementKeyTypes[index] == SignalType.ALRM).ToList();
        m_frameQueue = new List<IFrame>();

        m_frameQueueLimit = (int)Math.Ceiling(MaxTimeResolution * (double)FramesPerSecond);

        // Load line definitions
        LoadLineDefinitions();
    }

    /// <summary>
    /// Load the V,I Mappings and Angle, Magnitude Mappings into m_Line
    /// </summary>
    private void LoadLineDefinitions()
    {
        Func<DataRow, Gemstone.Timeseries.Model.Measurement?> loadMeasurement = TableOperations<Gemstone.Timeseries.Model.Measurement>.LoadRecordFunction();
        Func<DataRow, PhasorRecord?> loadPhasor = TableOperations<PhasorRecord>.LoadRecordFunction();
        Func<DataRow, Device?> loadDevice = TableOperations<Device>.LoadRecordFunction();

        Dictionary<int, DeviceDetails> deviceDetails = new();

        for (int i = 0; i < InputMeasurementKeys.Length; i++)
        {
            MeasurementKey key = InputMeasurementKeys[i];
            SignalType signalType = InputMeasurementKeyTypes[i];

            Gemstone.Timeseries.Model.Measurement? measurement = loadMeasurement(base.DataSource.Tables["Measurement"].Select($"SignalID={key.SignalID}")[0]);
            if (measurement?.DeviceID is null)
                continue;

            int deviceID = measurement.DeviceID.Value;

            Device? device = loadDevice(base.DataSource.Tables["Device"].Select($"ID={deviceID}")[0]);

            if (signalType == SignalType.FREQ)
            {
                if (deviceDetails.ContainsKey(deviceID))
                    deviceDetails[deviceID].Frequency = key;
                else
                    deviceDetails.Add(deviceID, new DeviceDetails()
                    {
                        Frequency = key,
                        IMag = new List<MeasurementKey>(),
                        VMag = new List<MeasurementKey>(),
                        IPhase = new List<MeasurementKey>(),
                        VPhase = new List<MeasurementKey>(),
                        Name = device?.Name ?? ""
                    });
            }
            else if (signalType == SignalType.VPHA)
            {
                if (deviceDetails.ContainsKey(deviceID))
                    deviceDetails[deviceID].VPhase.Add(key);
                else
                    deviceDetails.Add(deviceID, new DeviceDetails()
                    {
                        IMag = new List<MeasurementKey>(),
                        VMag = new List<MeasurementKey>(),
                        IPhase = new List<MeasurementKey>(),
                        VPhase = new List<MeasurementKey>() { key },
                        Name = device?.Name ?? ""
                    });
            }
            else if (signalType == SignalType.VPHM)
            {
                if (deviceDetails.ContainsKey(deviceID))
                    deviceDetails[deviceID].VMag.Add(key);
                else
                    deviceDetails.Add(deviceID, new DeviceDetails()
                    {
                        IMag = new List<MeasurementKey>(),
                        VMag = new List<MeasurementKey>() { key },
                        IPhase = new List<MeasurementKey>(),
                        VPhase = new List<MeasurementKey>(),
                        Name = device?.Name ?? ""
                    });
            }
            else if (signalType == SignalType.IPHA)
            {
                if (deviceDetails.ContainsKey(deviceID))
                    deviceDetails[deviceID].IPhase.Add(key);
                else
                    deviceDetails.Add(deviceID, new DeviceDetails()
                    {
                        IMag = new List<MeasurementKey>(),
                        VMag = new List<MeasurementKey>(),
                        IPhase = new List<MeasurementKey>() { key },
                        VPhase = new List<MeasurementKey>(),
                        Name = device?.Name ?? ""
                    });
            }
            else if (signalType == SignalType.IPHM)
            {
                if (deviceDetails.ContainsKey(deviceID))
                    deviceDetails[deviceID].IMag.Add(key);
                else
                    deviceDetails.Add(deviceID, new DeviceDetails()
                    {
                        IMag = new List<MeasurementKey>() { key },
                        VMag = new List<MeasurementKey>(),
                        IPhase = new List<MeasurementKey>(),
                        VPhase = new List<MeasurementKey>(),
                        Name = device?.Name ?? ""
                    });
            }
            else if (signalType != SignalType.ALRM)
            {
                OnStatusMessage(MessageLevel.Warning, $"Unexpected signal type \"{signalType}\" in input \"{key}\" [{key.SignalID}] for {nameof(DEFComputationAdapter)}. Expected one of \"{SignalType.VPHM}\", \"{SignalType.VPHA}\", \"{SignalType.IPHM}\", or \"{SignalType.IPHA}\", input excluded.");
            }
        }

        foreach ((int deviceID, DeviceDetails details) in deviceDetails)
        {
            if (details.Frequency is null)
            {
                OnStatusMessage(MessageLevel.Warning, $"No Frequency Signal found for Device \"{details.Name}\"");
                continue;
            }

            if (details.IPhase.Count == 0 || details.IMag.Count == 0)
            {
                OnStatusMessage(MessageLevel.Warning, $"No Current Phasors found for Device \"{details.Name}\"");
                continue;
            }

            if (details.VPhase.Count == 0 || details.VMag.Count == 0)
            {
                OnStatusMessage(MessageLevel.Warning, $"No Voltage Phasors found for Device \"{details.Name}\"");
                continue;
            }

            foreach (MeasurementKey iMagKey in details.IMag)
            {

                Gemstone.Timeseries.Model.Measurement? iMag = loadMeasurement(base.DataSource.Tables["Measurement"].Select($"SignalID={iMagKey.SignalID}")[0]);

                PhasorRecord iPhasor = loadPhasor(base.DataSource.Tables["Phasor"].Select($"DeviceID={deviceID} AND AND SourceIndex = {iMag.PhasorSourceIndex}")[0]);

                if (iPhasor is null)
                {
                    OnStatusMessage(MessageLevel.Warning, $"No Phasor found for current magnitude \"{iMag.PointTag}\"");
                    continue;
                }

                PhasorRecord vPhasor = loadPhasor(base.DataSource.Tables["Phasor"].Select($" ID = {iPhasor.PrimaryVoltageID ?? iPhasor.SecondaryVoltageID}")[0]);

                if (vPhasor is null)
                {
                    OnStatusMessage(MessageLevel.Warning, $"No matching voltage phasor found for  \"{iPhasor.Label}\"");
                    continue;
                }

                Gemstone.Timeseries.Model.Measurement[] vMeas = base.DataSource.Tables["Measurement"].Select($"PhasorSourceIndex={vPhasor.SourceIndex} AND DeviceID={deviceID}")
                    .Select(row => loadMeasurement(row)).ToArray();

                Gemstone.Timeseries.Model.Measurement[] iMeas = base.DataSource.Tables["Measurement"].Select($"PhasorSourceIndex={iPhasor.SourceIndex} AND DeviceID={deviceID}")
                    .Select(row => loadMeasurement(row)).ToArray();

                Line line = new Line()
                {
                    Current = new PhasorKey()
                    {
                        Magnitude = iMagKey,
                        Angle = details.IPhase.Where(key => iMeas.Any(m => m.SignalID == key.SignalID && m.SignalTypeID == (int)SignalType.IPHA)).FirstOrDefault()
                    },
                    Voltage = new PhasorKey()
                    {
                        Magnitude = details.VMag.Where(key => vMeas.Any(m => m.SignalID == key.SignalID && m.SignalTypeID == (int)SignalType.VPHM)).FirstOrDefault(),
                        Angle = details.VPhase.Where(key => vMeas.Any(m => m.SignalID == key.SignalID && m.SignalTypeID == (int)SignalType.VPHA)).FirstOrDefault()
                    },
                    Frequency = details.Frequency
                };
                if (line.Current.Magnitude == null || line.Current.Angle == null || line.Voltage.Magnitude == null || line.Voltage.Angle == null)
                {
                    OnStatusMessage(MessageLevel.Warning, $"Incomplete Phasor set for \"{details.Name}\"");
                    continue;
                }

                m_lines.Add(line);
            }
        }
    }

    public void LoadFile(EventDetails baseDetails)
    {
        FramesPerSecond = 30;

        JObject osc = JObject.Parse(baseDetails.Details);

        string dataFile = "C:\\Users\\gcsantos\\Downloads\\DataAlarmLine.mat";
        // Load EventData
        MathNet.Numerics.LinearAlgebra.Matrix<double> Vm = MatlabReader.Read<double>(dataFile, "Vm");
        MathNet.Numerics.LinearAlgebra.Matrix<double> Va = MatlabReader.Read<double>(dataFile, "Va");
        MathNet.Numerics.LinearAlgebra.Matrix<double> Im = MatlabReader.Read<double>(dataFile, "Im");
        MathNet.Numerics.LinearAlgebra.Matrix<double> Ia = MatlabReader.Read<double>(dataFile, "Ia");
        MathNet.Numerics.LinearAlgebra.Matrix<double> f = MatlabReader.Read<double>(dataFile, "Fbus");

        Ticks Tstart = DateTime.Parse("2025-01-26 08:43:19");

        Ticks[] Time = Vm.ToColumnArrays().First().Select((v, i) => (Ticks)(Tstart + (double)i * 1 / 30.0 * Ticks.PerSecond)).ToArray();
        List<LineData> lineData = new List<LineData>([
           new LineData()
           {
                Current = Ia.ToColumnArrays().First().Zip(Im.ToColumnArrays().First(),(f,s) => new ComplexNumber(Angle.FromDegrees(f),s)).ToList(),
                Frequency = f.ToColumnArrays().First().ToList(),
                Voltage = Va.ToColumnArrays().First().Zip(Vm.ToColumnArrays().First(), (f, s) => new ComplexNumber(Angle.FromDegrees(f), s)).ToList(),
                Timestamp = Time.ToList(),

                CurrentKey = new PhasorKey()
                {
                    Magnitude = MeasurementKey.CreateOrUpdate(Guid.NewGuid(),"MAT:1"),
                    Angle = MeasurementKey.CreateOrUpdate(Guid.NewGuid(), "MAT:2")
                },
                VoltageKey = new PhasorKey()
                {
                    Magnitude = MeasurementKey.CreateOrUpdate(new Guid(osc["VoltageSignalID"].ToString()), "MAT:3"),
                    Angle = MeasurementKey.CreateOrUpdate(Guid.NewGuid(), "MAT:4")
                },
                FrequencyKey = MeasurementKey.CreateOrUpdate(Guid.NewGuid(), "MAT:5")
           }
        ]);

        m_lines = lineData.Select(d => new Line()
        {
            Current = d.CurrentKey,
            Voltage = d.VoltageKey,
            Frequency = d.FrequencyKey
        }).ToList();

        TestData(baseDetails, lineData);
    }

    private async Task TestData()
    {
        while (m_testQueue.TryDequeue(out Tuple<EventDetails, IEnumerable<LineData>> testData))
            TestData(testData.Item1, testData.Item2);
    }

    public void TestData(EventDetails oscillation, IEnumerable<LineData> data)
    {
        JObject osc = JObject.Parse(oscillation.Details);
        string alarmKey = osc["VoltageSignalID"].ToString();
        double tStart = DEFComputationAdapter.ParseStudyIntervalStart(osc);
        double tEnd = DEFComputationAdapter.ParseStudyIntervalEnd(osc);
        string alarmTime = DEFComputationAdapter.ParseAlarmTime(osc).ToString(TimeStampFormat);
        MaximumMagnitude maxRealMag = DEFComputationAdapter.ParseMaximumRealMagnitude(osc);
        MaximumMagnitude maxReactiveMag = DEFComputationAdapter.ParseMaximumReactiveMagnitude(osc);

        if (!DEFIdentificationAdapter.TryParseCDEFRankDetails(osc, out RankDetails rank))
            throw new InvalidDataException("Oscillation Event has no CDEF rank information - Cannot create EMS Alarm Message.");

        if (!double.TryParse(osc["Frequency"].ToString(), out double freqAlarm)) //GenSet(23)
            throw new InvalidDataException("Oscillation Event not formated correctly - Can not parse \"Frequency\"");

        LineData alarmData = data.Where(d => d.VoltageKey.Magnitude.SignalID.ToString() == alarmKey).FirstOrDefault();

        IEnumerable<ComplexNumber> alarmCurrent = alarmData.Current;
        IEnumerable<ComplexNumber> alarmVoltage = alarmData.Voltage;
        IEnumerable<double> alarmFrequency = alarmData.Frequency;

        DataStatus pmuCond = PMUCleaning.PMUDataCleaning(alarmVoltage, alarmCurrent, alarmFrequency, FramesPerSecond, false);
        IEnumerable<double> pline = alarmVoltage.Zip(alarmCurrent, (v, i) => (v * i.Conjugate * 3.0 / 1000000.0).Real);

        bool oscilationCeased = false;
        if (pmuCond == DataStatus.Success)
            oscilationCeased = VerifiyEnd(pline, freqAlarm, tStart, tEnd, FramesPerSecond);
        string alarmMsg = FormAlarmMessage(alarmTime, rank.Message, oscilationCeased, maxRealMag, maxReactiveMag, freqAlarm);

        string outmsgFile = $"{OutputFileBaseName}_{alarmTime}.txt";
        using (StreamWriter writer = new StreamWriter(Path.Combine(OutputDirectory, outmsgFile), false))
            writer.WriteLine(alarmMsg);

        return;
    }

    private bool VerifiyEnd(IEnumerable<double> pline, double freqAlarm, double tStart, double tEnd, double fs)
    {
        IEnumerable<double> t0 = Enumerable.Range(0, pline.Count()).Select(n => (double)n / FramesPerSecond);

        double Twin = CMinW / freqAlarm;
        int nScans = (int)Math.Floor((t0.Last() - Twin) / Tstep);
        List<double> Fint = new List<double>();
        List<double> mm = new List<double>();

        for (int i = 0; i < nScans; i++)
        {
            double Tdstart = i * Tstep;
            double Tdend = i * Tstep + Twin;
            IEnumerable<double> plineWindow = DataCleaning.CutPeriod(Tdstart, Tdend, pline, FramesPerSecond);
            int nfft = plineWindow.Count();

            Complex32[] fft = plineWindow.Select(n => new Complex32((float)n, 0)).ToArray();
            MathNet.Numerics.IntegralTransforms.Fourier.Forward(fft, MathNet.Numerics.IntegralTransforms.FourierOptions.Matlab);
            Complex32 factor = new Complex32(2.0f / nfft, 0);
            IEnumerable<Complex32> S = fft.Select(d => d * factor);

            List<double> f = Enumerable.Range(0, (int)Math.Floor(nfft * 0.5D) + 1).Select(d => (double)(d) * FramesPerSecond / (double)(nfft - 1)).ToList();
            List<double> m = S.Select(d => (double)d.Magnitude).ToList();

            int currentF = 0;
            for (int ix = 1; ix < f.Count(); ix++)
            {
                double comparitor = Math.Abs(f[currentF] - freqAlarm);
                double val = Math.Abs(f[ix] - freqAlarm);
                if (val < comparitor)
                    currentF = ix;
            }

            Fint.Add(f[currentF]);
            mm.Add(m[currentF]);
        }

        // Scan numbers corresponding with study interval
        int scanStart = (int)Math.Floor(tStart / Tstep) - 1;
        int scanEnd = Math.Min((int)Math.Floor(tEnd / Tstep), nScans);
        // Average study interval energy
        double meanEnergy = mm.Skip(scanStart).Take(scanEnd - scanStart).Average();
        // Last number of 20% scans for verification
        int extra = Math.Max((int)Math.Floor((nScans - scanEnd) * 0.2) + 1, 3);
        if (nScans - scanEnd > 3)
        {
            int mmLen = mm.Count();
            double medianEnd = mm.Skip(mmLen - extra - 1).Median();
            if (medianEnd / meanEnergy < CeasedOscThreshold)
                return true;
        }
        return false;
    }

    private string FormAlarmMessage(string timeStamp, string rankMessage, bool hasCeased, MaximumMagnitude mW, MaximumMagnitude mVar, double freqAlarm)
    {
        string freq = freqAlarm.ToString(freqAlarm >= 1 ? "F2" : "F3");
        // Max magnitude and where
        string magString;
        if (mW.Magnitude > mVar.Magnitude)
            magString = $"{mW.Magnitude.ToString("F0")}MW {freq}Hz {mW.Substation}@{mW.LineID}";
        else
            magString = $"{mVar.Magnitude.ToString("D3")}MVar {freq}Hz {mVar.Substation}@{mVar.LineID}";

        string type = hasCeased ? "Ceased" : "Ongoing";

        string[] s = rankMessage.Split("(");
        string source;
        if (s.Length == 1)
            source = "No source identified";
        else
        {
            string[] s1 = s[0].Split("=");
            string area = s1[1].Trim();
            source = $"Source:{area}";
            if (s[1].Contains("Station="))
            {
                s1 = s[1].Split("=");
                string[] s2 = s1[2].Split(";");
                string station = s2[0].Trim();
                source += $",{station}";
                if (s[1].Contains("Unit="))
                {
                    s1 = s[1].Split("=");
                    string unit = s1.Last().Trim();
                    source += $",{unit}";
                }
            }
        }

        return $"OSL {timeStamp} {magString}; {source}; {type}";
    }

    protected override void PublishFrame(IFrame frame, int index)
    {
        // Queue the frame for buffering

        m_frameQueue.Add(frame);

        while (m_frameQueue.Count > m_frameQueueLimit)
            m_frameQueue.RemoveAt(0);

        List<EventDetails> toBeProcessed = new List<EventDetails>();

        // if it contains an alarm that is an oscillation We need to trigger computation
        foreach (MeasurementKey key in m_eventMeasurements)
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
            IEnumerable<LineData> lineData = m_lines.Select((d) => new LineData()
            {
                FrequencyKey = d.Frequency,
                VoltageKey = d.Voltage,
                CurrentKey = d.Current,
                Voltage = new List<ComplexNumber>(),
                Current = new List<ComplexNumber>(),
                Frequency = new List<double>(),
                Timestamp = new List<Ticks>()
            });

            foreach (IFrame f in m_frameQueue)
            {
                // ToDo: we need to add a label to line here, how do we get it?
                foreach (LineData line in lineData)
                {
                    ComplexNumber V = new ComplexNumber(double.NaN, double.NaN);
                    ComplexNumber I = new ComplexNumber(double.NaN, double.NaN);

                    double F = double.NaN;
                    Ticks T = f.Timestamp;

                    if (f.Measurements.TryGetValue(line.FrequencyKey, out IMeasurement measurement))
                    {
                        F = measurement.AdjustedValue;
                    }
                    if (f.Measurements.TryGetValue(line.VoltageKey.Magnitude, out measurement))
                    {
                        V.Magnitude = measurement.AdjustedValue;
                    }
                    if (f.Measurements.TryGetValue(line.VoltageKey.Angle, out measurement))
                    {
                        V.Angle = measurement.AdjustedValue;
                    }
                    if (f.Measurements.TryGetValue(line.CurrentKey.Magnitude, out measurement))
                    {
                        I.Magnitude = measurement.AdjustedValue;
                    }
                    if (f.Measurements.TryGetValue(line.CurrentKey.Angle, out measurement))
                    {
                        I.Angle = measurement.AdjustedValue;
                    }

                    line.Voltage.Append(V);
                    line.Current.Append(I);
                    line.Frequency.Append(F);
                    line.Timestamp.Append(T);
                }
            }

            foreach (EventDetails oscillation in toBeProcessed)
            {
                // Process the Oscillation
                m_testQueue.Enqueue(new Tuple<EventDetails, IEnumerable<LineData>>(oscillation, lineData));
            }
            m_createMessage.TryRunAsync();
        }
    }

    #endregion
}

