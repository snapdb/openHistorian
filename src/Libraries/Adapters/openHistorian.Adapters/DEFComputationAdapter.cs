
//******************************************************************************************************
//  DEFComputationAdapter.cs - Gbtc
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

namespace DataQualityMonitoring;

/// <summary>
/// Action adapter that generates alarm measurements based on alarm definitions from the database.
/// </summary>
[Description("DEF Computation: Computes the Dissipating Energy Flow")]

public class DEFComputationAdapter : CalculatedMeasurementBase
{
    #region [ Members ]

    List<Line> m_lines = new List<Line>();
    List<MeasurementKey> m_eventMeasurements;
    List<IFrame> m_frameQueue = new List<IFrame>();
    

    private int m_frameQueueLimit = 10;
    private readonly TaskSynchronizedOperation m_computeDEFOperation;
    private readonly ConcurrentQueue<Tuple<EventDetails,IEnumerable<LineData>>> m_oscillationQueue;

    public class PhasorKey
    {
        public MeasurementKey Magnitude;
        public MeasurementKey Angle;

    }

    public class Line
    {
        public PhasorKey Current;
        public PhasorKey Voltage;
        public MeasurementKey Frequency;
    }

    class DeviceDetails
    {
        public MeasurementKey Frequency;
        public string Name;
        public List<MeasurementKey> VPhase;
        public List<MeasurementKey> IPhase;
        public List<MeasurementKey> VMag;
        public List<MeasurementKey> IMag;
    };

    class LineData 
    {
        public PhasorKey VoltageKey;
        public PhasorKey CurrentKey;
        public MeasurementKey FrequencyKey;

        public IEnumerable<ComplexNumber> Voltage;
        public IEnumerable<ComplexNumber> Current;
        public IEnumerable<double> Frequency;
        public IEnumerable<Ticks> Timestamp;
    }

    private enum DataStatus 
    { 
        Success = 0,
        SuspectData = 1,
        BadData = 2
    }

    private enum TrendMethod
    {
        SubtractAverage = 0,
        HighPassFilter = 1,
        SteadyStateRemoval = 2
    }

    private enum DEMethod
    {
        CDEF = 0,
        CPSD = 1,
        Both = 2
    }

    #endregion

    #region [ Constructors ]

    /// <summary>
    /// Creates a new instance of the <see cref="DEFComputationAdapter"/> class.
    /// </summary>
    public DEFComputationAdapter()
    {
        m_computeDEFOperation = new TaskSynchronizedOperation(ComputeDEF, ex => OnProcessException(MessageLevel.Error, ex));
        m_oscillationQueue = new ConcurrentQueue<Tuple<EventDetails, IEnumerable<LineData>>>();
    }

    #endregion

    #region [ Properties ]

    public double TimeMargin = 160; //GenSet(8)
    public double Cmin_w = 5; //GenSet(7)
    public double Cmin_l = 8; //GenSet(6)
    public double Cmin_h = 8; //GenSet(5)
    public double BandThreshold = 0.2; //GenSet(4)
    public double MminP = 1.5001; // GenSet(2)
    public double Mmin = 0.4; // GenSet(3)
    double Tstep = 2.0D; //GenSet(1)
    int AllowedNoiseRatio = 200; // GenSet(19)
    double FrequencyThreshold = 0.07; // GenSet(29)
    int NumberOfElementsForSourceIdentification = 4; // GenSet(31)
    double IntialKFactor = -990.2; // GenSet(15)
    TrendMethod RemoveTrendFlag = TrendMethod.SubtractAverage; // GenSet(18), logic to determine is <0 is steady state, =0 subtract average, >0 highpass
    DEMethod deMethodFlag = DEMethod.Both; //GenSet(16)
    bool IsRealData = true; // GenSet(10)
    double cdefTimeInterval = 0.5; // GenSet(13) 


    /// <summary>
    /// The maximum size of the Buffer in second
    /// </summary>
    [ConnectionStringParameter()]
    [Description("Maximum Buffer Time (in s)")]
    public double MaxTimeResolution 
    {
        get;
        set;
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

        m_eventMeasurements = InputMeasurementKeys.Where((key, index) => InputMeasurementKeyTypes[index] == SignalType.ALRM).ToList();
        m_frameQueue = new List<IFrame>();

        m_frameQueueLimit = (int)Math.Ceiling(MaxTimeResolution*(double)FramesPerSecond);

        // Load line definitions
        LoadLineDefinitions();

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
        List<LineData> lineData = new List<LineData>([
           new LineData()
        {
            Current = Ia.ToColumnArrays().First().Zip(Im.ToColumnArrays().First(),(f,s) => new ComplexNumber(Angle.FromDegrees(f),s)),
            Frequency = f.ToColumnArrays().First(),
            Voltage = Va.ToColumnArrays().First().Zip(Vm.ToColumnArrays().First(), (f, s) => new ComplexNumber(Angle.FromDegrees(f), s)),
            Timestamp = Time,

            CurrentKey = new PhasorKey()
            {
                Magnitude = MeasurementKey.CreateOrUpdate(Guid.NewGuid(),"MAT:1"),
                Angle = MeasurementKey.CreateOrUpdate(Guid.NewGuid(), "MAT:2")
            },
            VoltageKey = new PhasorKey()
            {
                Magnitude = MeasurementKey.CreateOrUpdate(new Guid(oscillation["VoltageSignalID"].ToString()), "MAT:3"),
                Angle = MeasurementKey.CreateOrUpdate(Guid.NewGuid(), "MAT:4")
            },
            FrequencyKey = MeasurementKey.CreateOrUpdate(Guid.NewGuid(), "MAT:5")
           }
        ]);

        EventDetails evt = new EventDetails()
        {
            StartTime = DateTime.Parse("2025-01-26 08:46:19"),
            Details = oscillation.ToString()
        };

        string pmuDataFile = "C:\\Users\\gcsantos\\source\\MATLAB\\m-code\\PMUdata2.mat";
        int phasorForSelTime = 238;
        // Load EventData
        MathNet.Numerics.LinearAlgebra.Matrix<double> pmu_Vm = MatlabReader.Read<double>(pmuDataFile, "Vm");
        MathNet.Numerics.LinearAlgebra.Matrix<double> pmu_Va = MatlabReader.Read<double>(pmuDataFile, "Va");
        MathNet.Numerics.LinearAlgebra.Matrix<double> pmu_Im = MatlabReader.Read<double>(pmuDataFile, "Im");
        MathNet.Numerics.LinearAlgebra.Matrix<double> pmu_Ia = MatlabReader.Read<double>(pmuDataFile, "Ia");
        MathNet.Numerics.LinearAlgebra.Matrix<double> pmu_f = MatlabReader.Read<double>(pmuDataFile, "Fbus");
        MathNet.Numerics.LinearAlgebra.Matrix<double> pmu_map = MatlabReader.Read<double>(pmuDataFile, "I2U");

        Ticks[] pmu_time = pmu_Vm.ToColumnArrays().First().Select((_, i) => (Ticks)(Tstart + (double)i * 1 / 30.0 * Ticks.PerSecond)).ToArray();
        for (int ind =0; ind < pmu_map.RowCount; ind++)
        {
            int[] mapVector = pmu_map.Row(ind).Select(i =>(int) i -1).ToArray();
            lineData.Add(new LineData()
            {
                Current = pmu_Ia.Column(mapVector[0]).Zip(pmu_Im.Column(mapVector[0]), (f, s) => new ComplexNumber(Angle.FromDegrees(f), s)),
                Frequency = pmu_f.Column(mapVector[1]),
                Voltage = pmu_Va.Column(mapVector[1]).Zip(pmu_Vm.Column(mapVector[1]), (f, s) => new ComplexNumber(Angle.FromDegrees(f), s)),
                Timestamp = pmu_time,


                CurrentKey = new PhasorKey()
                {
                    Magnitude = MeasurementKey.CreateOrUpdate(Guid.NewGuid(), "MAT:1"),
                    Angle = MeasurementKey.CreateOrUpdate(Guid.NewGuid(), "MAT:2")
                },
                VoltageKey = new PhasorKey()
                {
                    Magnitude = MeasurementKey.CreateOrUpdate(Guid.NewGuid(), "MAT:3"),
                    Angle = MeasurementKey.CreateOrUpdate(Guid.NewGuid(), "MAT:4")
                },
                FrequencyKey = MeasurementKey.CreateOrUpdate(Guid.NewGuid(), "MAT:5")
            });
        }

        m_lines = lineData.Select(d => new Line()
        {
            Current = d.CurrentKey,
            Voltage = d.VoltageKey,
            Frequency = d.FrequencyKey
        }).ToList();


        ComputeDEF(evt, lineData);

    }

    /// <summary>
    /// Load the V,I Mappings and Angle, Magnitude Mappings into m_Line
    /// </summary>
    private void LoadLineDefinitions()
    {
        Func<DataRow, Gemstone.Timeseries.Model.Measurement?> loadMeasurement= TableOperations<Gemstone.Timeseries.Model.Measurement>.LoadRecordFunction();
        Func<DataRow, PhasorRecord?> loadPhasor  = TableOperations<PhasorRecord>.LoadRecordFunction();
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
                    deviceDetails.Add(deviceID, new DeviceDetails() { 
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

    private async Task ComputeDEF()
    {
        while (m_oscillationQueue.TryDequeue(out Tuple<EventDetails,IEnumerable<LineData>> oscillation))
            ComputeDEF(oscillation.Item1, oscillation.Item2);
    }

    private async Task ComputeDEF(EventDetails oscillation, IEnumerable<LineData> data)
    {
        JObject osc = JObject.Parse(oscillation.Details);

        string alarmKey = osc["VoltageSignalID"].ToString();
        LineData alarmData = data.Where(d => d.VoltageKey.Magnitude.SignalID.ToString() == alarmKey).FirstOrDefault();

        // Get Alarm PMU Data
        IEnumerable<ComplexNumber> alarmCurrent = new List<ComplexNumber>();
        IEnumerable<ComplexNumber> alarmVoltage = new List<ComplexNumber>();

        Ticks Talarm = oscillation.StartTime; // ff{15}

        if (!double.TryParse(osc["Frequency"].ToString(), out double freqAlarm)) //GenSet(23)
            throw new InvalidDataException("Oscillation Event not formated correctly - Can not parse \"Frequency\"");

        if (!double.TryParse(osc["Hysteresis"].ToString(), out double HysteresisAlarm)) // GenSet(24)
            throw new InvalidDataException("Oscillation Event not formated correctly-  Can not parse \"Hysteresis\"");

        SelTime(Talarm, freqAlarm, HysteresisAlarm, alarmData, out double Tstart, out double Tend, out Ticks Ts, out Ticks Te, out Ticks Ts1, out Ticks Te1, out IEnumerable<double> Pline,
            out DataStatus timeCond, out DataStatus pmuCond, out DataStatus trippingCond);

        // Todo: Console logs for warning messages based on conds?
        if (timeCond == DataStatus.BadData || pmuCond == DataStatus.SuspectData || pmuCond == DataStatus.BadData)
        {
            OnStatusMessage(MessageLevel.Warning, "Unable to compute DEF, Possibly false Alarm");
            return;
        }


        Gemstone.Numeric.Matrix<ComplexNumber> complexPower;
        Gemstone.Numeric.Matrix<double> voltageMagnitude;
        Gemstone.Numeric.Matrix<double> voltageAngle;
        Gemstone.Numeric.Matrix<double> frequencyBus;
        double[] voltageMean;
        {
            // ToDo: this might be an artifact of loading data strangely in OSL
            List<LineData> pmuData = data.Where(d => d.VoltageKey.Magnitude.SignalID.ToString() != alarmKey).ToList();
            IEnumerable<IEnumerable<double>> voltageM = pmuData.Select(d => d.Voltage.Select(v => v.Magnitude));
            IEnumerable<IEnumerable<double>> voltageA = pmuData.Select(d => d.Voltage.Select(v => v.Angle.ToDegrees()));
            IEnumerable<IEnumerable<double>> currentM = pmuData.Select(d => d.Current.Select(c => c.Magnitude));
            IEnumerable<IEnumerable<double>> currentA = pmuData.Select(d => d.Current.Select(c => c.Angle.ToDegrees()));
            IEnumerable<IEnumerable<double>> fBus = pmuData.Select(d => d.Frequency);

            PMUDataCleaning(ref voltageM, ref voltageA, ref currentM, ref currentA, ref fBus, true);

       

        return;
        // Remove spikes in Current
        //double Imedian = m_lines.Select(l => m_data[l.Current.Magnitude].Where(d => !double.IsNaN(d)).FirstOrDefault()).Where(d => d != default(double)).Median().FirstOrDefault();
        //alarmCurrent = alarmCurrent.Select(c => c.Magnitude > 100.0 * Imedian ? Complex.NaN : c).ToArray();

        // Clean Alarm PMU Data

        //remove any current values larger than 100X



        //if (dataBad)
        //    return;

        //Calculate Pline 3 Phase MW Flow



        // Save Results in Table Form as appropriate
    }


    private void SelTime(Ticks Talarm, double freqAlarm, double HysteresisAlarm,
        LineData alarmData,
        out double Tstart, out double Tend, out Ticks Ts, out Ticks Te,out Ticks Ts1, out Ticks Te1,out IEnumerable<double> Pline, out DataStatus timeCond, out DataStatus pmuCond, out DataStatus trippingCond) 
    {
        IEnumerable<ComplexNumber> alarmCurrent = alarmData.Current;
        IEnumerable<ComplexNumber> alarmVoltage = alarmData.Voltage;
        IEnumerable<double> alarmFrequency = alarmData.Frequency;

        double Twin = Cmin_w / freqAlarm;
        double Tth = (freqAlarm < BandThreshold ? Cmin_l : Cmin_h) / freqAlarm;

        Ts = Talarm - (long)((HysteresisAlarm + TimeMargin) * (double)Ticks.PerSecond);
        Te = Talarm + (long)((Tth - HysteresisAlarm + 3.0D * Twin) * (double)Ticks.PerSecond);

        Tstart = 0;
        Tend = 0;
        Pline = new List<double>();

        pmuCond = PMUDataCleaning(alarmVoltage, alarmCurrent, alarmFrequency, false);
        timeCond = DataStatus.Success;
        trippingCond = DataStatus.Success;

        Ts1 = Talarm; 
        Te1 = Talarm;

        if ( pmuCond != DataStatus.Success )
            return;
        

        double fs = (double)FramesPerSecond; // Genset(12)

        //Calculate Pline 3 Phase MW Flow
        Pline = alarmVoltage.Zip(alarmCurrent, (v, i) => (v*i.Conjugate * 3.0 / 1000000.0).Real);
        Pline = InterpolateNaN(RemoveOutliers(Pline));

        double Amax;
       
        if (!VerifyAlarm(Pline, freqAlarm, fs, out Amax) || Amax < (0.7-0.001))
        {
            OnStatusMessage(MessageLevel.Warning, "Alarm is most likely false, Amax must be equal to 1.0 for dominant mode");
            pmuCond = DataStatus.BadData;
            return;
        }

        // ### Sliding Window
        IEnumerable<double> t0 = Enumerable.Range(0, alarmVoltage.Count())
            .Select(d => d / fs);
        int nScans = (int)Math.Floor((t0.Last() - Twin) / Tstep);
        List<double> Fint = new List<double>();
        List<double> mm = new List<double>();

        for (int i = 0; i < nScans; i++)
        {
            double Tdstart = i * Tstep;
            double Tdend = i * Tstep + Twin;
            IEnumerable<double> PlineWindow = CutPeriod(Tdstart,Tdend,Pline,fs);
            int nfft = PlineWindow.Count();

            Complex32[] fft = PlineWindow.Select(n => new Complex32((float) n, 0)).ToArray();
            MathNet.Numerics.IntegralTransforms.Fourier.Forward(fft, MathNet.Numerics.IntegralTransforms.FourierOptions.Matlab);
            Complex32 factor = new Complex32(2.0f / nfft, 0);
            IEnumerable<Complex32> S = fft.Select(d => d * factor);

            List<double> f = Enumerable.Range(0, (int)Math.Floor(nfft * 0.5D) + 1).Select(d => (double)(d) * fs / (double)(nfft - 1)).ToList();
            List<double> m = S.Select(d => (double) d.Magnitude).ToList();

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

        double m_th = Math.Max(MminP, Mmin*mm.Max());
        List<double> mm1 = mm.Select(d => d - m_th).ToList();
        int ii;
        List<double> st = new List<double>();
        List<double> en = new List<double>();

        if (mm1.First() > 0)
        {
            ii = 1;
            st.Add(1);
        }
        else
        {
            ii = 0;
        }

        for (int i = 1; i < nScans; i++)
        {
            if (mm1[i] > 0)
            {
                if (mm1[i - 1] < 0)
                {
                    ii = ii + 1;
                    st.Add(i);
                }
            }
            else
            {
                if (mm1[i - 1] > 0)
                    en.Add(i);
                
            }
            if (i == nScans && mm1[i] > 0)
                en.Add(i);
        }

        // ii is the number of intervals with magnitude GT than threshold
        if (ii > 0)
        {
            //double ind_p = en.Zip(st, (e,s) => e - s);
            //Tstart = Tstep * (st(ind_p(1)) - 1);
            //Tend = Tstep * (en(ind_p(1)) - 1);
            if (Math.Abs(Tstart - Tend!) > 0.1)
            {
                Tstart = Tstart + (Ticks)(0.5*Twin);
                Tend = Tend + (Ticks)(0.5 * Twin);

                // ------Reduce Tend if it is beond time when line/ gen was tripped
                double stept = Math.Floor(fs * Tstep);
                int  nn = (int)Math.Floor(t0.Count() / stept);
                int nstop = (int) Math.Floor((Tstart - 0.5 * Twin) * fs / stept);
                //double Pmax = Math.max(abs(Pline));
                //Ttrip = nn * Tstep;

                //while nn > nstop
          //med = median(Pline(stept * (nn - 1):stept * nn));
            //    if abs(med) / Pmax < 0.05
              //     nn = nn - 1; Ttrip = nn * Tstep;
          //else
            //        nn = nstop;
              //  end
             //end
            }
            }


        /*




% ii is the number of intervals with magnitude GT than threshold
if ii>0  
    [~,ind_p]=sort((en-st),'descend');
    Tstart=Tstep*(st(ind_p(1))-1);
    Tend=Tstep*(en(ind_p(1))-1);
    if abs(Tstart-Tend)>0.1 % time limits are NOT the same
        
       % Shift the selected period by half of Twin
       Tstart=Tstart+0.5*Twin; Tend=Tend+0.5*Twin;
    
       %------ Reduce Tend if it is beond time when line/gen was tripped
       % Find time when the line/gen was disconnected (Ttrip)
       % That is time when median value become zero
       stept=floor(fs*Tstep);
       nn=floor(length(t0)/stept);
       nstop=floor((Tstart-0.5*Twin)*fs/stept);
       Pmax=max(abs(Pline));
       Ttrip=nn*Tstep;
       while nn>nstop
          med=median(Pline(stept*(nn-1):stept*nn));
          if abs(med)/Pmax < 0.05
             nn=nn-1; Ttrip=nn*Tstep;
          else
             nn=nstop;
          end
       end
       if Tend > Ttrip; Tend = max([Ttrip Tstart+Twin]) ; end %limit Tend by Ttrip added on 9-6-2018-------------
       %----- end of Reduce Tend if it is beond time when line/gen was tripped
    else
        Tstart=0; Tend=0;
    end
else
    Tstart=0; Tend=0;
end

%%===== Verify on whether the alarm is related to tripping to to bad PMU data
if Tend>0
    %---- Check on spikes within (Tstart, Tend) interval per clastring analysis. 
    %  These spikes are used to detect potential bad PMU data 
    clust=zeros(2,1); % number of clusters of spikes
    spikeT=[max([1/GenSet(23)/16 (2/fs+0.001)]) 1/GenSet(23)/4];% Two spike times for testing
    for j=1:2
      clear range; clear tt;
      % Calculate variability of Pline within spikeT intervals
      ii=0; stepp=floor(fs*spikeT(j));
      for i=floor(Tstart*fs):stepp:floor(Tend*fs-stepp)
          ii=ii+1;
          range(ii)=max(Pline(i:i+stepp))-min(Pline(i:i+stepp));
          tt(ii)=i/fs;
      end
       
      av0=mean(range); th=av0+3.5*std(range); % threshold for spike
      % Count the number of clusters of spikes exceeding threshold (clust)
      %   within (Tstart - Tend) interval
      k=0;   % flag of current sample
      k_1=0; % flag of previous sample
      for i=1:ii
         if abs(range(i))>th
             if k_1==0 & k==0
                 clust(j)=clust(j)+1; k=1; % new cluster has found
             else
                 k_1=1; % the same cluster
             end
         else
             k=0; k_1=0; % end of cluster
         end
      end
    end
    %  Check bad PMU data per clastering analysis
    if clust(1) >2 & clust(2) ==0; cond(2)=1; end % potential bad PMU
    %---- end of Check on spikes by clasters analysis
    
    %----Spike analysis based on the deviation of PSD intergral from
    %    linear function; in the entire frequency range and entire sampling period
    data_sp = [Pline Pline]; % use MW flow as input; need > 1 columns for SpectrumAna
    [ff1,ss1,~] = SpectrumAna(t0,data_sp);   % spectral analysis by using FFT
    nf=length(ff1(:,1));
    ss_i=zeros(nf,1);
    for i=3:nf % skip first two values in spectra
       ss_i(i,1)=ss_i(i-1,1)+ss1(i,1); % Integral of PSD
    end
    ss_i=ss_i/max(ss_i); % normalized integral
    for i=1:nf
       dss(i,1)=abs(ss_i(i)-i/nf); % deviation of PSD integral from linear function
    end
    h1=floor(length(ff1)/ff1(end)); % index of 1Hz point in ff1

    % Check bad PMU data per PSD integral criteria
    if ss_i(h1*2+1)/max(ss_i) < 0.5  % PSD intergal at f=2Hz
        if mean(dss) < 0.2 % PSD integral is close to linear function
            cond(2)=2; % defenetely bad PMU data due to spikes
        else
            cond(2)=1; % potentially bad PMU data due to spikes
        end
    end
    %---- end of Spike analysis based on the deviation of PSD.....
    
    %--Check of potential trippings withn (0 Tend) interval. 
    % Tripping is declated by spikes of MW range values per 1/4 period test
    %  Detected tripping impacts only output message per cond3 value
      i1=0; stepp=floor(fs*spikeT(j));
      for i=1:stepp:floor(Tend*fs-stepp)
          i1=i1+1;
          rangeP(i1)=max(Pline(i:i+stepp))-min(Pline(i:i+stepp));
          tt(i1)=i/fs;
      end
      th2=mean(rangeP)+5*std(rangeP); % threshold for spike
      cond4=0;
      for i=1:i1
          if rangeP(i)>th2; cond4=cond4+1; end % declare spike if larger than threshold
      end
      
      if cond4>0; cond(3)=1; end % potential tripping - set cond3 flag
      
    if GenSet(21)>2 % Graph of spike analysis
        figure(6)
        subplot(3,1,1)
        plot(ff1,ss1(1:length(ff1))/max(ss1),'b')
        ylabel('p.u.','fontsize',10)
        title('Power spectra density');
        
        subplot(3,1,2)
        plot(ff1,ss_i(1:length(ff1))/max(ss_i),'b')
        hold on
        plot([ff1(1) ff1(end)],[0.5 0.5],'r')
        hold on
        plot([2],[0.5],'or')
        hold off
        title('Integral of PSD, p.u.');
        legend('Integral','Threshold','Test point')
        
        subplot(3,1,3)
        plot(ff1,dss,'b')
        hold on
        plot([ff1(1) ff1(end)],[mean(dss) mean(dss)],'g')
        hold on
        plot([ff1(1) ff1(end)],[0.2 0.2],'r')
        hold off
        legend('Deviation','Mean','Threshold')
        title('Deviation of PSD Integral from white noise');
        xlabel('Frequency, Hz','fontsize',10)
        print([ff{5} ff{20}],'-djpeg','-r0') % save with screen resolution
        ff{56}=[ff{5} ff{20}];
    end
    %-- end of Check of potential trippings ...

    
    % ################# Check tripping event within (Tstart - Tend) interval
    %  by using clust(2)>0 criterion (spike per 1/4 period test)
    % Adjust study period to select interval before first spike if interval
    % is sufficient for DEF method, or interval after spike
    if clust(1) >0 & clust(2) >0 % potential tripping
       % Find time for the first spike per 1/4 period test
       ii=0;i=0;
       while ii==0 & i<length(range)
          i=i+1;
          if abs(range(i))>th;ii=i; end
       end
       % Adjust interval per time of first spike
       leng=Tend-Tstart;
       Tspike=ii*spikeT(2)-0.5/GenSet(23); % time of first spike per 1/4 period test
       if Tspike > T_th
           % interval before fist spike is suffucient for DEF; selecting this one
           Tend=min([Tstart+leng Tstart+Tspike]);
       else
           % selecting inteval after spike
           Tstart=Tstart+Tspike;
           Tend=min([Tstart+leng Ttrip]);    
       end
    end 
end
%%===== end of Verify on whether the alarm...
%%----------------------------

% Convert relative time into absolute time
Ts1=datestr(AlarmTime_num-(-Tstart+GenSet(24)+dt)/24/60/60,'yyyy-mm-dd HH:MM:SS'); %Ts as a string
Te1=datestr(AlarmTime_num-(-Tend+GenSet(24)+dt)/24/60/60,'yyyy-mm-dd HH:MM:SS'); %Te as a string
pmu.ind_s=floor(Tstart*fs); % indices of start and end time in Pline
pmu.ind_e=floor(Tend*fs);
AlarmLine=[pmu.LineID{ind},',',pmu.Subs{ind}];
ff{17}=AlarmLine;

% Is selected interval sufficient to fit min number of oscillation periods?
Fav=mean(Fint); % average frequency of interest in interval !!! need to do so only for study interval
% min required time interval:
if Fav<F_th; T_th=Cmin_l/Fav; else T_th=Cmin_h/Fav; end

deal='r';
if (Tend-Tstart)<T_th
    cond(1)=1; % interval is NOT sufficient for analysis
        if Tstart==Tend; 
        cond(1)=2; % no suitable interval is detected
    end
    deal=':r';
    if GenSet(20)>0
      Write_msg(ff{1}, [ff{16} '...Study interval is too short'],1,1,0);
      Write_msg(ff{1}, [ff{16} '...Minimal required interval =' num2str(T_th,'%3.1f') 's'],1,1,0);
      Write_msg(ff{1}, [ff{16} '...Available study interval =' num2str((Tend-Tstart),'%3.1f') 's'],1,1,0);
      Write_msg(ff{1}, [ff{16} '...Frequency=' num2str(Fav,'%5.3f') 'Hz   Pmax=' num2str(max(mm),'%5.1f') 'MW   Tstart=' num2str(Tstart,'%5.1f') 's   Tend=' num2str(Tend,'%5.1f') 's'],1,1,0);
    end
else
  % interval is sufficient for analysis
  % print solution parameters
  if GenSet(20)>0
      buf=['..Study interval is selected  f=' num2str(Fav,'%5.3f') 'Hz   Pmax=' num2str(max(mm),'%5.1f') 'MW   Phasor=' ff{14} '   Tstart=' num2str(Tstart,'%5.1f') 's   Tend=' num2str(Tend,'%5.1f') 's'] ;
      Write_msg(ff{1},[ff{16} buf],1,1,0);
  end
end

if GenSet(21)>0
  %----- plotting graphs
  pm=min(Pline);
  figure(5)
  subplot(2,1,1)
  plot(t0,Pline,'b'),grid
  hold on
  plot([Tstart Tend],[pm pm],deal,'LineWidth',3), grid
  xlabel('Time,s','fontsize',10)
  ylabel('Line flow, MW','fontsize',10)
elem=[pmu.Subs{ind} '_' pmu.LineID_EMS{ind}];
    idx7 = strfind(elem,'_');
    if ~isempty(idx7)
       elem=replace(elem,'_','\_');
    end
  title(['Transmission element: ' elem],'FontSize',10)
  hold off

  subplot(2,1,2)
  plot(mm,'b')
  xlabel('Scans','fontsize',10)
  ylabel('PSD, MW','fontsize',10)
  hold on
  plot([1 length(mm)], [m_th m_th],deal), grid
  hold off
  title(['Magnitude of the mode at ' num2str(GenSet(23),'%3.3f') ' Hz'],'FontSize',10)
  print([ff{5} ff{21}],'-djpeg','-r0') % save with screen resolution
  ff{54}=[ff{5} ff{21}];

end
        */

    }

    /// <summary>
    /// Verify that alarm frequency is really exist and dominat
    /// </summary>
    /// <param name="Pline"></param>
    /// <param name="AlarmFrequency"></param>
    /// <param name="fs"></param>
    /// <param name="Amax">  normalized maximal magnitude of the oscillation around % AlarmFrequency in the range of +/- dFs. </param>
    /// <returns></returns>
    private bool VerifyAlarm(IEnumerable<double> Pline, double AlarmFrequency, double fs, out double Amax)
    {
        double Pth = 5000;
        double dFs = 0.12;
        double dFb = 2.0;

        double Fth;

        Amax = 0;

        if (Math.Abs(Pline.Mean()) > Pth)
            return false;

        if (AlarmFrequency > 1.0)
            Fth = 0.3;
        else if (AlarmFrequency > 0.4 & AlarmFrequency < 1.0)
            Fth = 0.2;
        else if (AlarmFrequency > 0.2 & AlarmFrequency < 0.4)
            Fth = 0.1;
        else if (AlarmFrequency > 0.12 & AlarmFrequency < 0.2)
            Fth = 0.06;
        else
            Fth = 0.02;


        int nfft = Pline.Count();
        int n = (int)(nfft * 0.5D) + 1;
        IEnumerable<double> f = Enumerable.Range(0, n).Select(d => (double)(d+1) * fs * 0.5D/(double)n);
        
        double Fstep = fs / (double)nfft;
        int i_alarmFreq = (int)Math.Floor(AlarmFrequency / Fstep);
        int i_Fth = (int)Math.Floor(Fth / Fstep);

        int i_sUP = Math.Min(n, i_alarmFreq + Math.Max(2, (int)Math.Floor(dFs / Fstep)));
        int i_sDOWN = Math.Max(i_Fth, i_alarmFreq - Math.Max(2, (int)Math.Floor(dFs / Fstep)));

        int i_bUP = Math.Min(n, i_alarmFreq + Math.Max(2, (int)Math.Floor(dFb / Fstep)));
        int i_bDOWN = Math.Max(i_Fth, i_alarmFreq - Math.Max(2, (int)Math.Floor(dFb / Fstep)));

        double[] fft = SteadyStateRemoval(Pline, 10000).GetColumn(0);
        MathNet.Numerics.IntegralTransforms.Fourier.Forward(fft,Pline.Select(d => 0.0D).ToArray(), MathNet.Numerics.IntegralTransforms.FourierOptions.Matlab);
        IEnumerable<double> S = fft.Select(d => 2.0 * d / nfft);
        Amax = S.Select((d, i) => Math.Abs(d) / S.Skip(i_bDOWN).Take(i_bUP - i_bDOWN).Max()).Skip(i_sDOWN).Take(i_sUP - i_sDOWN).Max();
        return true;
    }

    /// <summary>
    /// remove steady state part of the time series
    /// </summary>
    /// <returns></returns>
    public double[] SteadyStateRemoval(IEnumerable<double> x, double penalityFactor)
    {
        Gemstone.Numeric.Matrix<double> imf = VariableModeDecomposition.vmd(x, 500, 5, penalityFactor);
        return imf.GetSubmatrix(0, 0, imf.NRows, imf.NColumns - 1).RowSums;
    }
    {
        Gemstone.Numeric.Matrix<double> imf = Gemstone.Numeric.Analysis.VariableModeDecomposition.vmd(x, 500, 5, penalityFactor);
        return new Gemstone.Numeric.Matrix<double>(imf.GetSubmatrix(0,0,imf.NRows,imf.NColumns - 1).RowSums,1);
    }

    public IEnumerable<double> CutPeriod(double Tstart, double Tend, IEnumerable<double> data, double fs)
    {
        int n = data.Count();
        int nStart = (int)Math.Floor(fs*Tstart);
        int nEnd = (int)Math.Ceiling(fs*Tend);
        return data.Skip(nStart).Take(nEnd - nStart);
    }
    /// <summary>
    /// Clean PMU data and identify bad PMU data
    /// </summary>
    /// <param name="data"></param>
    /// <param name="TNaN"></param>
    /// <param name="epsilon1"></param>
    /// <param name="epsilon2"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    private IEnumerable<IEnumerable<double>> PMU_Cleaning(IEnumerable<IEnumerable<double>> data, double TNaN, double epsilon1, double epsilon2, bool cleanOutlier, out DataStatus condition)
    {
        condition = DataStatus.Success;
        int nVariables = data.Count();
        int nSamples = data.First().Count();

        IEnumerable<IEnumerable<double>> cleanedData = data.Select(d => {
            double median = d.NaNAwareMedian(true);
            int nStalled = d.Count((p) => Math.Abs(p) < epsilon1);
            int nZero = d.Count((p) => Math.Abs(p - median) < epsilon2);

            if (nStalled > TNaN * nSamples || nZero > TNaN * nSamples)
                return d.Select(p => double.NaN);
            
            return d;
        });

        cleanedData = cleanedData.Select(d => d.Select((v) => (Math.Abs(v) < 0.000001 ? double.NaN : v)));
        
        cleanedData = cleanedData.Select((d) => (d.Count((v) => double.IsNaN(v)) > TNaN* nSamples ? d.Select((v) => 1e-8) : d));

        if (nVariables == 1)
        {
            double min = cleanedData.First().Min();
            double max = cleanedData.First().Max();

            if (Math.Abs(min - max) < 0.00005)
            {
                condition = DataStatus.BadData;
                return data;
            }

        }

        // ind = sum(isnan(In1)) == length(In1); used later....
        if (cleanOutlier)
        {
            cleanedData = cleanedData.Select(d => RemoveOutliers(d,0.05));
        }

        // Handle Sitouation where First sample is NaN
        cleanedData = cleanedData.Select((d) => {
            if (!double.IsNaN(d.First()))
                return d;
            
            IEnumerable<Tuple<double,double>> inpData = d.Select((v,i) => new Tuple<double,double>(v,i)).Where((v) => !double.IsNaN(v.Item1)).Take(2*FramesPerSecond);
            
            double a, b;
            CurveFit.LeastSquares(inpData.Select(v => v.Item1).ToArray(), inpData.Select(v => v.Item2).ToArray(), out a, out b);
            int n = d.TakeWhile((v) => double.IsNaN(v)).Count() - 1;

            return d.Select((v,i) => (i<=n? a*i+b: v));
        });

        //Handle the situation when the last sample is NaN
        cleanedData = cleanedData.Select((d) => {
            if (!double.IsNaN(d.Last()))
                return d;

            IEnumerable<Tuple<double, double>> inpData = d.Reverse().Select((v, i) => new Tuple<double, double>(v, i)).Where((v) => !double.IsNaN(v.Item1)).Take(2 * FramesPerSecond);

            double a, b;
            CurveFit.LeastSquares(inpData.Select(v => v.Item1).ToArray(), inpData.Select(v => v.Item2).ToArray(), out a, out b);
            int n = d.TakeWhile((v) => double.IsNaN(v)).Count() - 1;

            return d.Select((v, i) => ((d.Count() - i - 1) <= n ? a * (d.Count() - i - 1) + b : v));
        });

        cleanedData = cleanedData.Select((d,i) =>
        {
            if (!d.Any(v => !double.IsNaN(v)))
                return d.Select((v) => 0.0001);
          
            return InterpolateNaN(d);
        });

        return cleanedData;
    }

    /// <summary>
    /// use PCHip to interpolate data in NaN Locations
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private IEnumerable<double> InterpolateNaN(IEnumerable<double> data)
    {
        IEnumerable<int> NaNIndices = data.Select((v, i) => new { v, i }).Where((v) => double.IsNaN(v.v)).Select((v) => v.i);
        if (NaNIndices.Count() == data.Count() || NaNIndices.Count() == 0)
            return data;

        double[] estimates = Pchip.Interp1(data.Select((v, i) => new { v, i }).Where((v) => double.IsNaN(v.v)).Select((v) => (double)v.i).ToArray(),
               data.Where((v, i) => !double.IsNaN(v)).ToArray(),
               NaNIndices.Select((v) => (double)v).ToArray());

        return data.Select((v, i) =>
        {
            if (!double.IsNaN(v)) return v;

            int index = NaNIndices.Where((n) => n == i).FirstOrDefault();

            return (index >= 0 ? estimates.ElementAt(index) : double.NaN);
        });
    }

    /// <summary>
    /// Note this is skipping the Interpolate step for now.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="threshold"></param>
    /// <returns></returns>
    private IEnumerable<double> RemoveOutliers(IEnumerable<double> data, double threshold = 0.05)
    {
        int nThreshold = (int)Math.Floor(data.Count() * threshold);
        IEnumerable<double> sampleData = data.OrderBy(v => Math.Abs(v)).Take(data.Count() - nThreshold);
        double median = sampleData.NaNAwareMedian(true);
        double stdev = sampleData.StandardDeviation();

        double min = median - 6 * stdev;
        double max = median + 6 * stdev; ;


        return data.Select(v => {
            if (v < min || v > max)
                return double.NaN;
            return v;
        });
    }

    private DataStatus PMUDataCleaning(
        ref IEnumerable<IEnumerable<double>> Vm,
        ref IEnumerable<IEnumerable<double>> Va,
        ref IEnumerable<IEnumerable<double>> Im,
        ref IEnumerable<IEnumerable<double>> Ia,
        ref IEnumerable<IEnumerable<double>> Fbus,
        bool CleanOutlier)
    {
        DataStatus result = DataStatus.Success;

        double[] medianMagnitude = Im.Select(p => p.NaNAwareMedian()).ToArray();
        double medianOfMedians = medianMagnitude.NaNAwareMedian();

        Im = Im.Select((d, i) =>
        {
            if (medianMagnitude[i] / medianOfMedians > 100)
                return d.Select(_ => double.NaN);
            return d;
        });

        DataStatus code;
        Vm = PMU_Cleaning(Vm, 0.5, 100, 2, CleanOutlier, out code);

        if (code == DataStatus.BadData || result == DataStatus.BadData)
            result = DataStatus.BadData;

        Va = PMU_Cleaning(Va, 0.5, 1, 0.05, CleanOutlier, out code);

        if (code == DataStatus.BadData || result == DataStatus.BadData)
            result = DataStatus.BadData;

        Im = PMU_Cleaning(Im, 0.5, 3, 0.05, CleanOutlier, out code);

        if (code == DataStatus.BadData || result == DataStatus.BadData)
            result = DataStatus.BadData;

        Ia = PMU_Cleaning(Ia, 0.5, 1, 0.05, CleanOutlier, out code);

        if (code == DataStatus.BadData || result == DataStatus.BadData)
            result = DataStatus.BadData;

        Fbus = PMU_Cleaning(Fbus, 0.5, 48, 0.00005, CleanOutlier, out code);

        if (code == DataStatus.BadData || result == DataStatus.BadData)
            result = DataStatus.BadData;

        return result;
    }

    private DataStatus PMUDataCleaning(IEnumerable<ComplexNumber> V, IEnumerable<ComplexNumber> I, IEnumerable<double> f, bool CleanOutlier)
    { 
        DataStatus result = DataStatus.Success;

        DataStatus code;
        IEnumerable<double> Vm = PMU_Cleaning(new List<IEnumerable<double>>() { V.Select(p => p.Magnitude) },0.5,100,2,CleanOutlier,out code).First();

        if (code == DataStatus.BadData || result == DataStatus.BadData)
            result = DataStatus.BadData;

        IEnumerable<double> Va = PMU_Cleaning(new List<IEnumerable<double>>() { V.Select(p => p.Angle).Unwrap().Select(a => a.ToDegrees()) }, 0.5, 1, 0.05, CleanOutlier, out code).First();

        if (code == DataStatus.BadData || result == DataStatus.BadData)
            result = DataStatus.BadData;

        IEnumerable<double> Im = PMU_Cleaning(new List<IEnumerable<double>>() { I.Select(p => p.Magnitude) }, 0.5, 3, 0.05, CleanOutlier, out code).First();

        if (code == DataStatus.BadData || result == DataStatus.BadData)
            result = DataStatus.BadData;

        IEnumerable<double> Ia = PMU_Cleaning(new List<IEnumerable<double>>() { I.Select(p => p.Angle).Unwrap().Select(a => a.ToDegrees()) }, 0.5, 1, 0.05, CleanOutlier, out code).First();

        if (code == DataStatus.BadData || result == DataStatus.BadData)
            result = DataStatus.BadData;

        IEnumerable<double> Fbus = PMU_Cleaning(new List<IEnumerable<double>>() { f }, 0.5, 48, 0.00005, CleanOutlier, out code).First();

        if (code == DataStatus.BadData || result == DataStatus.BadData)
            result = DataStatus.BadData;

        V = Vm.Zip(Va, (m, a) => new ComplexNumber(Angle.FromDegrees(a), m));
        I = Im.Zip(Ia, (m, a) => new ComplexNumber(Angle.FromDegrees(a), m));
        f = Fbus;

        return result;
    }

    private Gemstone.Numeric.Matrix<double> RemoveTrend(Gemstone.Numeric.Matrix<double> series)
    {
        switch (RemoveTrendFlag)
        {
            case TrendMethod.SubtractAverage:
                return series.TransformByColumn((col, colIndex) =>
                {
                    double averageValue = col.Average();
                    return col.Select(val => val - averageValue).ToArray();
                });
            case TrendMethod.SteadyStateRemoval:
                return series.TransformByColumn((col, colIndex) => SteadyStateRemoval(col, 10000));
            case TrendMethod.HighPassFilter:
                return HighPassFilter(series);
            default:
                throw new MissingMethodException($"Method flag type {Enum.GetName(typeof(TrendMethod), RemoveTrendFlag)} not yet implemented.");

        }
    }

    private Gemstone.Numeric.Matrix<double> HighPassFilter(Gemstone.Numeric.Matrix<double> series) 
    {
        // This uses a precalculated 8th order elliptical filter with highpass frequency of 0.06 Hz (GenSet(18)), Attenuation of Stopband of 60 dB, allowed ripple of 0.1 dB
        /* precalculated gain (this shouldn't matter for our analysis)
         * 1.35451505239885
         * 2.59271745340576
         * 14.5488376541948
         * 0.0177771674651696
         */
        double[][] filtNum = [
            [1, -1.99909120534373,1],
            [1,-1.99949340650223,1],
            [1,-1.99992550744888,1],
            [1,-1.99890208320654,1]
        ];
        double[][] filtDem = [
            [1, -1.98843962161928, 0.990144977377360],
            [1, -1.96491293974926, 0.967390343219020],
            [1, -1.87830020155431, 0.883690790979502],
            [1, -1.99619138978376, 0.997693111810405]
        ];

        return series.TransformByColumn((col, ind) =>
        {
            double[] newValues = col;
            for(int i = 0; i < filtNum.GetLength(0); i++)
            {
                newValues = DigitalFilter.FiltFilt(filtNum[i], filtDem[i], newValues);
            }
            return newValues;
        });
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
            IEnumerable<LineData> lineData = m_lines.Select((d) => new LineData() {
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
                m_oscillationQueue.Enqueue(new Tuple<EventDetails, IEnumerable<LineData>>(oscillation, lineData));
            }
            m_computeDEFOperation.TryRunAsync();
        }
    }

    #endregion

    #region [ Static ]


    #endregion
}

