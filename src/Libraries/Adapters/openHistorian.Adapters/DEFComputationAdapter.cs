
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

        double Ttrip = 0;
        {
        int ii;
            List<int> st = new List<int>(new int[nScans]);
            List<int> en = new List<int>(new int[nScans]);

        if (mm1.First() > 0)
        {
                ii = 0;
                st[0] = 1;
        }
        else
        {
                ii = -1;
        }

        for (int i = 1; i < nScans; i++)
        {
            if (mm1[i] > 0)
            {
                if (mm1[i - 1] < 0)
                {
                        ii++;
                        st[ii] = i + 1;
                }
            }
            else
            {
                if (mm1[i - 1] > 0)
                        en[ii] = i + 1;
                
            }
                if (i == nScans - 1 && mm1[i] > 0)
                    en[ii] = i + 1;
        }

            st = st.Take(ii + 1).ToList();
            en = en.Take(ii + 1).ToList();

        // ii is the number of intervals with magnitude GT than threshold
            if (ii > -1)
        {
                int ind_p = en.Select((e, i) => new Tuple<int, int>(e - st[i], i)).OrderBy(tupe => -tupe.Item1).First().Item2;
                Tstart = Tstep * (st[ind_p] - 1);
                Tend = Tstep * (en[ind_p] - 1);
            if (Math.Abs(Tstart - Tend!) > 0.1)
            {
                    Tstart = Tstart + 0.5 * Twin;
                    Tend = Tend + 0.5 * Twin;

                // ------Reduce Tend if it is beond time when line/ gen was tripped
                    int stept = (int)Math.Floor(fs * Tstep);
                    int nn = (int)Math.Floor(t0.Count() / (double)stept);
                    int nstop = (int)Math.Floor((Tstart - 0.5 * Twin) * fs / (double)stept);
                    double Pmax = Pline.Select(d => Math.Abs(d)).Max();
                    Ttrip = nn * Tstep;

                    while (nn > nstop)
                    {
                        double medAbs = Math.Abs(Pline.Skip(stept * (nn - 1)-1).Take(stept+1).Median());
                        if (medAbs / Pmax < 0.05)
                        {
                            nn = nn - 1;
                            Ttrip = nn * Tstep;
            }
                        else
                            nn = nstop;
            }
                    if (Tend > Ttrip)
                        Tend = Math.Max(Ttrip, Tstart + Twin);
                }
                else
                {
                    Tstart = 0;
                    Tend = 0;
                }
            }
        }

        // Verify on whether the alarm is related to tripping to to bad PMU data
        if (Tend > 0)
        {
            int[] clust = { 0, 0 };
            double[] spikeT = { Math.Max(1 / (freqAlarm * 16), 2 / fs + 0.001), 1 / (freqAlarm * 4) };
            List<double> range = new List<double>();
            double th = 0;
            for (int j=0; j < 2; j++)
            {
                int stepp = (int) Math.Floor(fs * spikeT[j]);
                range = new List<double>();
                List<double> tt = new List<double>();
                for (double i =  Math.Floor(Tstart  * fs); i <= Math.Floor(Tend*fs-stepp); i += stepp)
                {
                    IEnumerable<double> test = Pline.Skip((int)i-1).Take(stepp+1);
                    range.Add(Pline.Skip((int) i-1).Take(stepp+1).Max() - Pline.Skip((int) i-1).Take(stepp+1).Min());
                    tt.Add(i / fs);
                }
                double av0 = range.Mean(); 
                th = av0 + 3.5 * range.StandardDeviation();
                int k = 0;   // flag of current sample
                int k_1 = 0; // flag of previous sample
                for (int i = 0; i < range.Count(); i++)
                {
                    if (Math.Abs(range[i]) > th)
                        if (k_1 == 0 & k == 0) // new cluster has been found
                        {
                            clust[j] = clust[j] + 1; 
                            k = 1; 
                        }
                        else k_1 = 1;
                    else
                    {
                        k = 0;
                        k_1 = 0;
                    }

                }
            }
            if (clust[0] > 2 & clust[1] == 0) 
                pmuCond = DataStatus.SuspectData;


            // Spike analysis based on the deviation of PSD intergral from
            Gemstone.Numeric.Matrix<double> data_sp = new Gemstone.Numeric.Matrix<double>(Pline.ToArray(), 2);
            (double[] ff1, double[] ss1) = SpectrumAna(t0, data_sp);
            int nf = ff1.Count();
            // skip first two values in spectra, get normalized PSD integral
            double[] ss_i = new double[nf];
            for (int i = 2; i < nf; i++)
                ss_i[i] = ss_i[i - 1] + ss1[i];

            double ss_i_max = ss_i.Max();
            for(int i = 0; i < nf; i++)
                ss_i[i] /= ss_i_max;

            // deviation of PSD integral from linear function
            double[] dss = new double[nf];
            for (int i = 0; i < nf; i++)
                dss[i] = Math.Abs(ss_i[i] - (double) (i + 1) / nf);
        
            // Check bad PMU data per PSD integral criteria
            int h1 = (int) Math.Floor(nf / ff1.Last());
            if (ss_i[h1 * 2] / ss_i.Max() < 0.5)
            {
                if (dss.Mean() < 0.2) // PSD integral is close to linear function
                    pmuCond = DataStatus.BadData; // defenetely bad PMU data due to spikes
          else
                    pmuCond = DataStatus.SuspectData; // potentially bad PMU data due to spikes
            }

            // Check of potential trippings withn(0 Tend) interval.
            // Tripping is declated by spikes of MW range values per 1 / 4 period test
            {
                int stepp = (int)Math.Floor(fs * spikeT.Last());
                int i1 = 0;
                List<double> rangeP = new List<double>();
                List<double> tt = new List<double>();
       
                for (int i = 1; i < (int)Math.Floor(Tend * fs - stepp); i += stepp)
                {
                    i1 = i1 + 1;
                    IEnumerable<double> test = Pline.Skip(i - 1).Take(stepp + 1);
                    rangeP.Add(Pline.Skip(i - 1).Take(stepp + 1).Max() - Pline.Skip(i - 1).Take(stepp + 1).Min());
                    tt.Add(i / fs);
                }
                double rangeThreshold = rangeP.Mean() + 5 * rangeP.StandardDeviation();
    
                if (rangeP.Any(range => range > rangeThreshold))
                    trippingCond = DataStatus.SuspectData;
            }

            // Check tripping event within (Tstart - Tend) interval
            if (clust[0] > 0 && clust[1] > 0)
            {
                int ii = 0;
                int i = -1;
                while (ii == 0 && i < range.Count())
                {
                    i++;
                    if (Math.Abs(range[i]) > th) ii = i + 1;
                }
    
                double leng = Tend - Tstart;
                double Tspike = ii * spikeT[1] - 0.5 / freqAlarm;
                if (Tspike > Tth)
                    Tend = Math.Min(Tstart + leng, Tstart + Tspike);
       else
                {
                    Tstart = Tstart + Tspike;
                    Tend = Math.Min(Tstart + leng, Ttrip);
                }
            }
        }

        Ts1 = Talarm - (long)((HysteresisAlarm + TimeMargin - Tstart) * Ticks.PerSecond);
        Te1 = Talarm - (long)((HysteresisAlarm + TimeMargin - Tend) * Ticks.PerSecond);

        double fav = Fint.Mean();
        if (fav < BandThreshold)
            Tth = Cmin_l / fav;
else
            Tth = Cmin_h / fav;

        if (Tstart == Tend)
            timeCond = DataStatus.BadData;
        else if (Tend - Tstart < Tth)
            timeCond = DataStatus.SuspectData;
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
        IEnumerable<double> f = Enumerable.Range(0, n).Select(d => (double)(d) * fs * 0.5D/(double)(n-1));
        
        double Fstep = fs / (double)nfft;
        int i_alarmFreq = (int)Math.Floor(AlarmFrequency / Fstep);
        int i_Fth = (int)Math.Floor(Fth / Fstep);

        int i_sUP = Math.Min(n, i_alarmFreq + Math.Max(2, (int)Math.Floor(dFs / Fstep)));
        int i_sDOWN = Math.Max(i_Fth, i_alarmFreq - Math.Max(2, (int)Math.Floor(dFs / Fstep)));

        int i_bUP = Math.Min(n, i_alarmFreq + Math.Max(2, (int)Math.Floor(dFb / Fstep)));
        int i_bDOWN = Math.Max(i_Fth, i_alarmFreq - Math.Max(2, (int)Math.Floor(dFb / Fstep)));

        Complex32[] fft = SteadyStateRemoval(Pline, 10000).Select(d => new Complex32((float)d,0)).ToArray();
        MathNet.Numerics.IntegralTransforms.Fourier.Forward(fft, MathNet.Numerics.IntegralTransforms.FourierOptions.Matlab);
        Complex32 factor = new Complex32(2.0f / nfft, 0);
        IEnumerable<Complex32> S = fft.Select(d => d * factor);
        double Smax = S.Skip(i_bDOWN).Take(i_bUP - i_bDOWN).Select(d => d.Magnitude).Max();
        Amax =  S.Select((d, i) => d.Magnitude / Smax).Skip(i_sDOWN).Take(i_sUP - i_sDOWN).Max();
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

    /// <summary>
    /// Spectrum analysis for time data series given as a matrix
    /// </summary>
    /// <returns></returns>
    public (double[], double[]) SpectrumAna(IEnumerable<double> t, Gemstone.Numeric.Matrix<double> data)
    {
        // Spectrum analysis
        double fs = Math.Round((t.Count() - 1) / (t.Last() - t.First()));
        int nfft = data.NRows;
        Complex32 factor = new Complex32(2.0f / nfft, 0);

        double[] f = Enumerable.Range(0, (int)Math.Floor(nfft * 0.5D) + 1).Select(d => (double)(d) * fs / (double)(nfft - 1)).ToArray();
        double[] sum = new double[data.NRows]; 

        for(int colNum =0; colNum < data.NColumns; colNum++)
        {
            IEnumerable<double> col = data.GetColumn(colNum);
            double mean = col.Mean();
            Complex32[] fft = col.Select(d => new Complex32((float)(d - mean), 0)).ToArray();
            MathNet.Numerics.IntegralTransforms.Fourier.Forward(fft, MathNet.Numerics.IntegralTransforms.FourierOptions.Matlab);
            Complex32[] S = fft.Select(c => c * factor).ToArray();
            for(int i = 0; i < data.NRows; i++)
                sum[i] += S[i].Magnitude;
    }

        return (f, sum);
        /* Some extra code for the dominant if we need it
            % find dominant frequency above 0.05Hz (beyond DC bias)
            [~,idx1] = sort(ss_sum(1:length(f),1),'descend');
            Fdominant=f(idx1(1)); % dominant frequency
            i=1;
            while Fdominant<0.05 % disregard frequencies coming from trend (DC bias)
                i=i+1;
                Fdominant=f(idx1(i)); 
            end 
         */
    }

    public IEnumerable<double> CutPeriod(double Tstart, double Tend, IEnumerable<double> data, double fs)
    {
        int n = data.Count();
        int nStart = (int)Math.Floor(fs*Tstart);
        int nEnd = (int)Math.Ceiling(fs*Tend) + 1;
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
        List<int> NaNIndices = data.Select((v, i) => new { v, i }).Where((v) => double.IsNaN(v.v)).Select((v) => v.i).ToList();
        if (NaNIndices.Count() == data.Count() || NaNIndices.Count() == 0)
            return data;

        double[] estimates = Pchip.Interp1(data.Select((v, i) => new { v, i }).Where((v) => !double.IsNaN(v.v)).Select((v) => (double)v.i).ToArray(),
               data.Where((v, i) => !double.IsNaN(v)).ToArray(),
               NaNIndices.Select((v) => (double)v).ToArray());


        return data.Select((v, i) =>
        {
            if (!double.IsNaN(v)) return v;

            int index = NaNIndices.FindIndex((n) => n == i);

            return (index >= 0 ? estimates[index] : double.NaN);
        });
    }

    /// <summary>
    /// Note this is skipping the Interpolate step for now.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="threshold"></param>
    /// <returns></returns>
    private IEnumerable<double> RemoveOutliers(IEnumerable<double> data, double threshold = 0.05, IEnumerable<int>? ind = null)
    {
        int nThreshold = (int)Math.Floor(data.Count() * threshold);
        if (ind is null || ind.Count() == 0)
            ind = data.Select((v, i) => new Tuple<double, int>(v, i)).OrderBy(v => Math.Abs(v.Item1)).Take(nThreshold).Select(v => v.Item2);
        IEnumerable<double> sampleData = data.Where((_, i) => !ind.Any(ii => ii == i));
        double median = sampleData.NaNAwareMedian(false);
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

    private Gemstone.Numeric.Matrix<double> BandPassFilter(Gemstone.Numeric.Matrix<double> data, double fDominant)
    {
        // fs = FramesPerSecond Genset 12
        // ddF = BandThreshold Genset 4

        double dF = BandThreshold * fDominant;
        double fStop1 = fDominant * (1 - 2 * BandThreshold);
        double fPass1 = fDominant * (1 - BandThreshold);
        double fStop2 = fDominant * (1 + 2 * BandThreshold);
        double fPass2 = fDominant * (1 + BandThreshold);
        ButterworthFilter butterFilter = new ButterworthFilter(fStop1, fPass1, fPass2, fStop2, 10, 1, FramesPerSecond);
        return data.TransformByColumn((col, _) => butterFilter.FiltFilt(col));
    }

    private void RemoveTrippedLines(ref Gemstone.Numeric.Matrix<ComplexNumber> power, double threshold, int allowedLowContiguous)
    {
        Gemstone.Numeric.Matrix<double> abs_real = power.TransformByValue<double>((value) => Math.Abs(value.Real));
        for(int j = 0; j < power.NColumns; j++)
        {
            int count = 0;
            for (int i = 0; i < power.NRows; i++)
            {
                if (abs_real[i][j] < threshold)
                {
                    count++;
                    if (count > allowedLowContiguous)
                    {
                        power.ReplaceSubmatrix(new Gemstone.Numeric.Matrix<ComplexNumber>(power.NRows, [new ComplexNumber(0.00001, 0.00001)]), 0, j);
                        i = power.NRows; // breaks our loop, since we found our result
                    }
                }
                else count = 0;
            }
        }
    }

    private void RemoveNoisySignals(ref Gemstone.Numeric.Matrix<ComplexNumber> power)
    {
        double value = 0.00001;
        double threshold = 10 * value;
        RemoveNoisySignals(ref power, value, threshold, true);
        RemoveNoisySignals(ref power, value, threshold, false);
    }

    private void RemoveNoisySignals(ref Gemstone.Numeric.Matrix<ComplexNumber> power, double value, double threshold, bool checkReal = true)
    {
        int nfft = (int) Math.Floor(power.NRows / 2.0D) + 1;
        List<double> ffSums = new List<double>();
        List<int> goodIndices = new List<int>();
        for (int i = 0; i < power.NColumns; i++)
        {
            IEnumerable<double> series = power.GetColumnEnumerable(i).Select(c => checkReal ? c.Real : c.Imaginary);
            if (Math.Abs(series.Mean()) <= threshold)
                continue;

            // The case of using series.Imaginary for real here is intentional
            Complex32[] fft = series.Select(d => new Complex32((float) d, 0)).ToArray();
            MathNet.Numerics.IntegralTransforms.Fourier.Forward(fft, MathNet.Numerics.IntegralTransforms.FourierOptions.Matlab);

            ffSums.Add(fft.Select(c => c.Magnitude).Take(nfft).Sum());
            goodIndices.Add(i);
        }
        double medianSum = ffSums.Median();
        goodIndices = ffSums.Zip(goodIndices, (sum, index) => new { sum, index }).Where(o => o.sum / medianSum > AllowedNoiseRatio).Select(o => o.index).ToList();

        power.OperateByColumn((col, i) =>
        {
            if (goodIndices.Any(index => index == i))
                col = col.Select((_) => new ComplexNumber(value,value)).ToArray();
        });
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

    private double CalculateCumulativePQRatio(Gemstone.Numeric.Matrix<double> realPower, Gemstone.Numeric.Matrix<double> reactivePower, double? frequency = null)
    {
        int window = realPower.NRows - 1;
        int overlap = window - 1;
        (Gemstone.Numeric.Matrix<ComplexNumber> realCpsd, double[] freqVector) = CPSD(realPower, realPower, window, overlap);
        freqVector = freqVector.Select(val => val * FramesPerSecond / Math.PI / 2).ToArray();
        Gemstone.Numeric.Matrix<ComplexNumber> imagCpsd = CPSD(reactivePower, reactivePower, window, overlap).Item1;

        double nearestFind = frequency ?? FrequencyThreshold;
        double comparitor = double.PositiveInfinity;
        int closestIndex = -1;
        for(int index = 0; index < freqVector.Count(); index++)
        {
            double comparison = Math.Abs(nearestFind - freqVector[index]);
            if (comparison < comparitor)
            {
                comparitor = comparison;
                closestIndex = index;
            }
        }

        if (frequency is null)
        {
            double maxValue = 0;
            int valueIndex = -1;
            for (int rowIndex = closestIndex; rowIndex < realCpsd.NRows; rowIndex++)
            {
                double pSum = realCpsd.GetRow(rowIndex).Select(c => c.Magnitude).Sum();
                double qSum = imagCpsd.GetRow(rowIndex).Select(c => c.Magnitude).Sum();
                if (pSum > maxValue)
                {
                    maxValue = pSum;
                    valueIndex = rowIndex;
                }
                if (qSum > maxValue)
                {
                    maxValue = qSum;
                    valueIndex = rowIndex;
                }
            }
            closestIndex = valueIndex;
        }

        // Calculate cumulatine PQratio from max(max(Magnitude)) to max(sum(Magnitude))
        double[] aP = realCpsd.GetRow(closestIndex).Select(c => c.Magnitude).OrderDescending().ToArray();
        double[] aQ = imagCpsd.GetRow(closestIndex).Select(c => c.Magnitude).OrderDescending().ToArray();
        
        for(int index = 1; index < aP.Length; index++)
        {
            aP[index] += aP[index - 1];
            aQ[index] += aQ[index - 1];
        }

        double[] PQcum_cpsd = aP.Zip(aQ, (p, q) => p / q).ToArray();
        return PQcum_cpsd[Math.Min(NumberOfElementsForSourceIdentification, PQcum_cpsd.Length)];
    }

    private double SelectKFactor(double cumulativePQRatio, double intialKFactor, double frequency)
    {
        if (intialKFactor >= -90) return intialKFactor;

        double[][] rPQ = [[0, 0.4, 0.8, 1, 1.2, 1.5, 2, 3, 4, 1e18], [0.4, 0.35, 0.21, 0.2, 0.19, 0.15, 0.08, 0.02, 0.00, 0]]; // PQ rapper points

        int indexPq = rPQ[0].IndexOf(v => v > cumulativePQRatio);
        double slopePq = (rPQ[1][indexPq-1] - rPQ[1][indexPq]) / (rPQ[0][indexPq - 1] - rPQ[0][indexPq]);
        double kFactor = rPQ[1][indexPq] + slopePq * (cumulativePQRatio - rPQ[0][indexPq]);

        double[][] rF = [[0, 2, 4, 6, 10, 20, 40, 1e18], [1, 1, 0.85, 0.6, 0.25, 0.12, 0.07, 0.05]];

        int indexF = rF[0].IndexOf(v => v > frequency);
        double slopeF = (rF[1][indexF - 1] - rF[1][indexF]) / (rF[0][indexF - 1] - rF[0][indexF]);
        double kFactorScalar = rF[1][indexF] + slopeF * (frequency - rF[0][indexF]);

        return kFactor * kFactorScalar;
    }

    // Calculates cross spectrum phase density using welch's method (eq. to matlab CSPD func)
    private (Complex32[], double[]) CPSD(IEnumerable<Complex32> x, IEnumerable<Complex32> y, int window, int overlap)
    {
        if (x.Count() != y.Count()) throw new InvalidOperationException("Cannot calculate CSPD for series of different legnths.");
        if (x.Count() < window) throw new InvalidOperationException("Window is larger than series length.");

        // hamming window for the data windowing
        IEnumerable<Complex32> hammingWindow = Enumerable.Range(0, window).Select(n => 
            new Complex32((float)(0.54 - 0.46 * Math.Cos(2.0D * Math.PI * (double) n / (window - 1))), 0)
        );
        double windowNormalization = hammingWindow.Select(d => (float)Math.Pow(d.Real, 2)).Sum();

        // for n-point DFT
        int nfft = (int)Math.Max(
            256,
            Math.Pow(2, Math.Ceiling(Math.Log(window, 2)))
        );
        IEnumerable<Complex32> zeros = Enumerable.Repeat(Complex32.Zero, nfft - window);


        int numSegments = (int) Math.Round((double)(x.Count() - overlap) / (window - overlap));
        IEnumerable<int> segmentStartIndices = Enumerable.Range(0, numSegments).Select(n => n * (window - overlap));
        IEnumerable<Complex32> periodogramAvg = Enumerable.Repeat(Complex32.Zero, nfft);
        foreach (int index in segmentStartIndices)
        {
            // Window signals and append 0's to match nfft length
            Complex32[] windowedX = x.Skip(index).Take(window).Zip(hammingWindow, (yVal, winVal) => yVal * winVal).Concat(zeros).ToArray();
            Complex32[] windowedY = y.Skip(index).Take(window).Zip(hammingWindow, (yVal, winVal) => yVal * winVal).Concat(zeros).ToArray();
            MathNet.Numerics.IntegralTransforms.Fourier.Forward(windowedX, MathNet.Numerics.IntegralTransforms.FourierOptions.Matlab);
            MathNet.Numerics.IntegralTransforms.Fourier.Forward(windowedY, MathNet.Numerics.IntegralTransforms.FourierOptions.Matlab);
            // Get Cross Spectrum
            IEnumerable<Complex32> cs = windowedX.Zip(windowedY, (x, y) => x * y.Conjugate());
            periodogramAvg = periodogramAvg.Zip(cs, (sum, newVal) => sum + newVal);
        }

        // Right now we assume a frequency of 2pi, if we change this assumption or allow it to be specified, the rest will need to change
        // We take just [0,pi], as matlab does
        int take = nfft / 2 + 1; // No rounding here, nfft is always a power of 2
        // Our scaling for this is 2/(fs * window normalization) (we also just go ahead and roll in finishing up the avg calculation too, while we're at it)
        Complex32 scale = new Complex32((float) (numSegments * Math.PI * windowNormalization), 0);
        periodogramAvg = periodogramAvg.Take(take).Select((val , ind) => {
            // For some reason, matlab gives half for the first point
            Complex32 factor = (ind == 0) ? new Complex32(2,0) : Complex32.One;
            return (val / scale / factor);
            });

        double fstep = Math.PI / (take-1);

        return (periodogramAvg.ToArray(), Enumerable.Range(0, take).Select(n => n * fstep).ToArray());
    }

    private (Gemstone.Numeric.Matrix<ComplexNumber>, double[]) CPSD(Gemstone.Numeric.Matrix<double> x, Gemstone.Numeric.Matrix<double> y, int window, int overlap)
    {
        if (x.NColumns != y.NColumns) throw new InvalidOperationException("CPSD of matrices of different dimensions not supported.");

        double[] freqVector = [];
        Gemstone.Numeric.Matrix<ComplexNumber> cpsdOut;

        for (int colIndex = 0; colIndex < x.NColumns; colIndex++)
        {
            IEnumerable<Complex32> xVector = x.GetColumn(colIndex).Select(v => new Complex32((float)v, 0));
            IEnumerable<Complex32> yVector = y.GetColumn(colIndex).Select(v => new Complex32((float)v, 0));
            if (colIndex == 0)
            {
                // Frequency vector is the same for all results for this calculation
                (Complex32[] cpsd, freqVector) = CPSD(xVector, yVector, window, overlap);
                cpsdOut = new Gemstone.Numeric.Matrix<ComplexNumber>(cpsd.Select(c => new ComplexNumber((double)c.Real, c.Imaginary)).ToArray(), x.NColumns);
            }
            else
            {
                Complex32[] cpsd = CPSD(xVector, yVector, window, overlap).Item1;
                for (int rowIndex = 0; rowIndex < cpsd.Length; rowIndex++)
                {
                    cpsdOut[rowIndex][colIndex] = new ComplexNumber((double)cpsd[rowIndex].Real, cpsd[rowIndex].Imaginary);
                }
            }
        }
        return (cpsdOut, freqVector);
    }

    private (Gemstone.Numeric.Matrix<double>, double) DE_CPSD(
        Gemstone.Numeric.Matrix<double> realPower,
        Gemstone.Numeric.Matrix<double> reactivePower,
        Gemstone.Numeric.Matrix<double> voltageMagnitude,
        Gemstone.Numeric.Matrix<double> voltageAngle,
        double[] voltageAverage,
        double kFactor,
        double frequency
    )
    {
        // Transform to radians
        voltageAngle = voltageAngle.TransformByValue(v => v * Math.PI / 180);
        // Normalize power  and voltage magnitude
        realPower = realPower.TransformByValue(v => v / 3e8);
        reactivePower = reactivePower.TransformByValue(v => v / 3e8);
        voltageMagnitude = voltageMagnitude.TransformByColumn((col, colIndex) => col.Select(v => v / voltageAverage[colIndex]).ToArray());

        int window = realPower.NRows - 1;
        int overlap = window - 1;

        Gemstone.Numeric.Matrix<ComplexNumber> cP;
        IEnumerable<double> wP;
        {
            Gemstone.Numeric.Matrix<double> pq = new(realPower.NRows, 2*realPower.NColumns, 0);
            for(int colIndex = 0; colIndex < pq.NColumns; colIndex++)
            {
                int pqColIndex = colIndex % realPower.NColumns;
                bool useReal = pqColIndex == colIndex;
                for(int rowIndex = 0; rowIndex < pq.NRows; rowIndex++)
                    pq[rowIndex][colIndex] = (useReal ? realPower : reactivePower)[rowIndex][pqColIndex];
            }
            (cP, wP) = CPSD(pq, pq, window, overlap);
            wP = wP.Select(v => v * FramesPerSecond / Math.PI / 2);
        }

        int fMaxEnergyOscInd = wP.Select((freq, ind) => new { freq, ind }).OrderBy(o => Math.Abs(o.freq-frequency)).First().ind;
        int maxIndex = -1;
        double comparitor = -1;
        for (int rowIndex = fMaxEnergyOscInd; rowIndex < cP.NRows; rowIndex++)
        {
            double magSum = cP.GetRow(rowIndex).Select(v => v.Magnitude).Sum();
            if (magSum > comparitor)
            {
                maxIndex = rowIndex;
                comparitor = magSum;
            }
        }

        double fCpsd = wP.ElementAt(maxIndex); // rectified studied requency

        // 1 original index, 2 sum of rest, 3 mPVa, 4 mQV, 5 mPV, 6 mQVA
        Gemstone.Numeric.Matrix<double> DE_cpsd = new Gemstone.Numeric.Matrix<double>(realPower.NColumns, 6, 0);
        double realScale = 1 / Math.Sqrt(1 + Math.Pow(kFactor, 2));
        double reactiveScale = realScale * kFactor;
        Gemstone.Numeric.Matrix<ComplexNumber> mPVa = CPSD(realPower, voltageAngle, window, overlap).Item1;
        Gemstone.Numeric.Matrix<ComplexNumber> mQV = CPSD(reactivePower, voltageMagnitude, window, overlap).Item1;
        Gemstone.Numeric.Matrix<ComplexNumber> mPV = CPSD(realPower, voltageMagnitude, window, overlap).Item1;
        Gemstone.Numeric.Matrix<ComplexNumber> mQVa = CPSD(reactivePower, voltageAngle, window, overlap).Item1.TransformByValue(v => -v);
        double[] mPVaRow = mPVa.GetRow(maxIndex).Select(v => (v * realScale).Imaginary).ToArray();
        double[] mQVRow = mQV.GetRow(maxIndex).Select(v => (v * realScale).Imaginary).ToArray();
        double[] mQVaRow = mQVa.GetRow(maxIndex).Select(v => (v * reactiveScale).Imaginary).ToArray();
        double[] mPVRow = mPV.GetRow(maxIndex).Select(v => (v * reactiveScale).Imaginary).ToArray();
        Tuple<int, double>[] sums = Enumerable.Range(0, realPower.NColumns).Select(i => {
            double sum = mPVaRow[i] + mQVRow[i] + mQVaRow[i] + mPVRow[i];
            return new Tuple<int, double>(i, sum);
        }).OrderByDescending(tupe => Math.Abs(tupe.Item2)).ToArray();

        double normalizer = Math.Abs(sums.First().Item2);
        for(int rowIndex = 0; rowIndex < DE_cpsd.NRows; rowIndex++)
        {
            int sumIndex = sums[rowIndex].Item1;
            DE_cpsd[rowIndex][0] = sumIndex;
            DE_cpsd[rowIndex][1] = sums[rowIndex].Item2 / normalizer;
            DE_cpsd[rowIndex][2] = mPVaRow[sumIndex] / normalizer;
            DE_cpsd[rowIndex][3] = mQVRow[sumIndex] / normalizer;
            DE_cpsd[rowIndex][4] = mQVaRow[sumIndex] / normalizer;
            DE_cpsd[rowIndex][5] = mPVRow[sumIndex] / normalizer;
        }

        return (DE_cpsd, fCpsd);
    }

    private void CDEF(
        Gemstone.Numeric.Matrix<double> realPower,
        Gemstone.Numeric.Matrix<double> reactivePower,
        Gemstone.Numeric.Matrix<double> vMagnitude,
        Gemstone.Numeric.Matrix<double> vAngle,
        Gemstone.Numeric.Matrix<double> fBus,
        double[] vMean,
        double kfactor,
        out Gemstone.Numeric.Matrix<double> DE_ranked,
        out double[] DE_time,
        out Gemstone.Numeric.Matrix<double> DE_curve,
        out Gemstone.Numeric.Matrix<double> DE_curveP,
        out Gemstone.Numeric.Matrix<double> DE_curveQ,
        out Gemstone.Numeric.Matrix<double> DE_curveQF, 
        out Gemstone.Numeric.Matrix<double> DE_curvePV
    )
    {
        List<double> time = Enumerable.Range(0, vMagnitude.NRows).Select(n => (double) n / FramesPerSecond).ToList();
        // Select middle part of interval to be used
        int startInd = time.FindIndex(t => t > time.First() + Math.Floor((time.Last() - time.First()) * (1 - cdefTimeInterval) / 2));
        if (startInd > 0) startInd--;
        int endInd = time.FindIndex(t => t > time.First() + Math.Floor((time.Last() - time.First()) * (1 + cdefTimeInterval) / 2));
        if (endInd == -1) endInd = time.Count - 1;


        time = time.Skip(startInd).Take(endInd - startInd + 1).ToList();
        realPower = realPower.GetSubmatrix(startInd, 0, endInd - startInd + 1, realPower.NColumns);
        reactivePower = reactivePower.GetSubmatrix(startInd, 0, endInd - startInd + 1, reactivePower.NColumns);
        vMagnitude = vMagnitude.GetSubmatrix(startInd, 0, endInd - startInd + 1, vMagnitude.NColumns);

        Gemstone.Numeric.Matrix<double> usedData;
        if (IsRealData)
        {
            usedData = fBus.GetSubmatrix(startInd, 0, endInd - startInd + 1, fBus.NColumns);
        }
        else
        {
            usedData = vAngle.GetSubmatrix(startInd, 0, endInd - startInd + 1, vAngle.NColumns);
        }

        int nn = 2;
        (int start, int end) = GetStartEndNumericalDerivative(vMagnitude.NRows, nn);
        DE_time = time.Skip(start).Take(end - start + 1).ToArray();
        // We're gonna use this in a lambda, so we need to make it not out yet
        Gemstone.Numeric.Matrix<double> DE_curve_temp = new Gemstone.Numeric.Matrix<double>(end - start + 1, realPower.NColumns, 0);
        DE_curveP = new Gemstone.Numeric.Matrix<double>(end - start + 1, realPower.NColumns, 0);
        DE_curveQ = new Gemstone.Numeric.Matrix<double>(end - start + 1, realPower.NColumns, 0);
        DE_curveQF = new Gemstone.Numeric.Matrix<double>(end - start + 1, realPower.NColumns, 0);
        DE_curvePV = new Gemstone.Numeric.Matrix<double>(end - start + 1, realPower.NColumns, 0);

        for (int col = 0; col < realPower.NColumns; col++)
        {
            double[] ddV = NumericalDerivative(vMagnitude.GetColumn(col).ToArray(), nn);

            IEnumerable<double> usedMulti;
            if (!IsRealData) usedMulti = NumericalDerivative(usedData.GetColumn(col).ToArray(), nn).Select(v => v * Math.PI / 180);
            else usedMulti = usedData.GetColumn(col).Skip(start).Take(end - start + 1).Select(v => v * 2 * Math.PI);

            IEnumerable<double> realPowerCol = realPower.GetColumn(col).Skip(start).Take(end - start + 1);
            IEnumerable<double> reactivePowerCol = realPower.GetColumn(col).Skip(start).Take(end - start + 1);
            IEnumerable<double> voltageMagCol = vMagnitude.GetColumn(col).Skip(start).Take(end - start + 1);
            double[] tempP = realPowerCol.Zip(usedMulti, (p, m) => p * m).ToArray();
            double[] tempQF = reactivePowerCol.Zip(usedMulti, (p, m) => -p * m).ToArray();
            double[] tempQ = reactivePowerCol.Zip(ddV, (p, v) => p * v).Zip(voltageMagCol, (val, vm) => val / (vMean[col] + vm)).ToArray();
            double[] tempPV = realPowerCol.Zip(ddV, (p, v) => p * v).Zip(voltageMagCol, (val, vm) => val / (vMean[col] + vm)).ToArray();

            double[] dPdF = new double[end - start + 1];
            double[] dQdV = new double[end - start + 1];
            double[] dQdF = new double[end - start + 1];
            double[] dPdV = new double[end - start + 1];
            // sum
            dPdF[0] = tempP[0];
            dQdV[0] = tempQ[0];
            // sum_plus
            dQdF[0] = tempQF[0];
            dPdV[0] = tempPV[0];
            for (int row = 1; row < dPdF.Length; row++)
            {
                dPdF[row] = dPdF[row-1] + tempP[row];
                dQdV[row] = dQdV[row - 1] + tempQ[row];
                dQdF[row] = dQdF[row - 1] + tempQF[row];
                dPdV[row] = dPdV[row - 1] + tempPV[row];
            }

            double divsor = Math.Sqrt(1 + kfactor * kfactor);
            for (int row = 0; row < DE_curve_temp.NRows; row++)
            {
                double sum = dPdF[row] + dQdV[row];
                double sum_plus = dQdF[row] + dPdV[row];
                DE_curve_temp[row][col] = (sum + sum_plus * kfactor) / divsor;
                DE_curveP[row][col] = dPdF[row] / divsor;
                DE_curveQ[row][col] = dQdV[row] / divsor;
                DE_curveQF[row][col] = dQdF[row] * kfactor / divsor;
                DE_curvePV[row][col] = dPdV[row] * kfactor / divsor;
            }
        }

        Gemstone.Numeric.Matrix<double> Amatrix = new Gemstone.Numeric.Matrix<double>(DE_time, 2);
        for (int row = 0; row < Amatrix.NRows; row++) Amatrix[row][1] = 1;

        DE_ranked = new Gemstone.Numeric.Matrix<double>(DE_curve_temp.NColumns, 7, 0);
        IEnumerable<Tuple<double, int>> sortOrder = Enumerable
            .Range(0, DE_ranked.NRows)
            .Select(ind => new Tuple<double, int>(GetSlopeCurve(Amatrix, DE_curve_temp.GetColumn(ind)), ind))
            .OrderByDescending(tuple => Math.Abs(tuple.Item1));
        double maxDECurve = Math.Abs(sortOrder.First().Item1);
        int index = 0;
        foreach(Tuple<double, int> tupe in sortOrder)
        {
            int row = tupe.Item2;
            double deCurve = tupe.Item1;
            DE_ranked[index][0] = row;
            DE_ranked[index][1] = deCurve / maxDECurve;
            DE_ranked[index][2] = GetSlopeCurve(Amatrix, DE_curveP.GetColumn(row)) / maxDECurve;
            DE_ranked[index][3] = GetSlopeCurve(Amatrix, DE_curveQ.GetColumn(row)) / maxDECurve;
            DE_ranked[index][4] = GetSlopeCurve(Amatrix, DE_curveQF.GetColumn(row)) / maxDECurve;
            DE_ranked[index][5] = GetSlopeCurve(Amatrix, DE_curvePV.GetColumn(row)) / maxDECurve;
            DE_ranked[index][6] = deCurve;
            index++;
        }
        DE_curve = DE_curve_temp;
    }

    private double GetSlopeCurve(Gemstone.Numeric.Matrix<double> timeMatrix, double[] vector)
    {
        Gemstone.Numeric.Matrix<double> paraMatrix = timeMatrix.GetLeastSquares(vector);
        return paraMatrix[0][0];
    }

    private double[] NumericalDerivative(double[] values, int nn)
    {
        (int start, int end) = GetStartEndNumericalDerivative(values.Length, nn);
        double[] derivative = new double[end - start + 1];
        for(int i = start; i <= end; i++)
        {
            double[] valueArray = new double[nn];
            for (int j = 0; j < nn; j++)
            {
                int indexDifference = (int) Math.Pow(2, j);
                double divisor = Math.Pow(2, j + 1);
                valueArray[j] = (values[i + indexDifference] - values[i - indexDifference]) / divisor;
            }
            while (valueArray.Length > 1)
            {
                double[] newValueArray = new double[valueArray.Length - 1];
                double divisor = Math.Pow(4, nn - newValueArray.Length) - 1;
                for (int j = 0; j < newValueArray.Length; j++)
                {
                    newValueArray[j] = valueArray[j] + (valueArray[j+1] - valueArray[j]) / divisor;
                }
                valueArray = newValueArray;
            }
            derivative[i - start] = valueArray[0];
        }
        return derivative;
    }

    private (int, int) GetStartEndNumericalDerivative(int length, int nn)
    {
        int start = (int)Math.Pow(2, nn - 1);
        return (start, length - start - 1);
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

