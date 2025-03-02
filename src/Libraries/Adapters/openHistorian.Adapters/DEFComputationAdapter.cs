
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
using Gemstone.ActionExtensions;
using Gemstone.Collections.CollectionExtensions;
using Gemstone.Data;
using Gemstone.Data.DataExtensions;
using Gemstone.Data.Model;
using Gemstone.Diagnostics;
using Gemstone.EnumExtensions;
using Gemstone.IO.Collections;
using Gemstone.Numeric.Analysis;
using Gemstone.Numeric.EE;
using Gemstone.StringExtensions;
using Gemstone.Threading.SynchronizedOperations;
using Gemstone.Timeseries;
using Gemstone.Timeseries.Adapters;
using GrafanaAdapters.Functions.BuiltIn;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto.Parameters;
using SnapDB.Snap;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics.Metrics;
using System.Diagnostics;
using System.Numerics;
using System.Reflection.Emit;
using System.Runtime.Caching;
using System.Runtime.Intrinsics.X86;
using System.Security.Claims;
using System.Text;

using static System.Collections.Specialized.BitVector32;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection.Metadata.Ecma335;
using Gemstone.Numeric;


namespace DataQualityMonitoring;

/// <summary>
/// Action adapter that generates alarm measurements based on alarm definitions from the database.
/// </summary>
[Description("DEF Computation: Computes the Dissipating Energy Flow")]

public class DEFComputationAdapter : FacileActionAdapterBase
{
    #region [ Members ]

    Dictionary<MeasurementKey, double[]> m_data;
    List<Line> m_lines = new List<Line>();

    Ticks[] m_timestamps;

    public class PhasorKey
    {
        public MeasurementKey Magnitude;
        public MeasurementKey Angle;

    }

    public class Line
    {
        public PhasorKey Current;
        public PhasorKey Voltage;
    }

    private enum DataCleaning { 
        Success = 0,
        BadData = 1
    }

    #endregion

    #region [ Constructors ]

    /// <summary>
    /// Creates a new instance of the <see cref="DEFComputationAdapter"/> class.
    /// </summary>
    public DEFComputationAdapter()
    {
       
    }

    #endregion

    #region [ Properties ]

    public double TimeMargin = 160; //GenSet(8)
    public double Cmin_w = 5; //GenSet(7)
    public double Cmin_l = 8; //GenSet(6)
    public double Cmin_h = 8; //GenSet(5)
    public double BandThreshold = 0.2; //GenSet(4)

    #endregion

    #region [ Methods ]

    private void ComputeDEF()
    {
        // This will need to come from Oscillation Detection
        Ticks Talarm = 0; 
        double freqAlarm = 0; // GenSet(23)
        double HysteresisAlarm = 0; //GenSet(24)

        double Twin = Cmin_w / freqAlarm;
        double Tth = (freqAlarm < BandThreshold? Cmin_l : Cmin_h)/freqAlarm;
        PhasorKey PhasorVAlarm = new PhasorKey();
        PhasorKey PhasorIAlarm = new PhasorKey();


        Ticks Tstart = Talarm - (long)(HysteresisAlarm + TimeMargin) * Ticks.PerSecond;
        Ticks Tend = Talarm + (long)(Tth - HysteresisAlarm + 3.0*Twin) * Ticks.PerSecond;

        // Get Alarm PMU Data
        IEnumerable<ComplexNumber> alarmCurrent = new List<ComplexNumber>();
        IEnumerable<ComplexNumber> alarmVoltage = new List<ComplexNumber>();

        // Remove spikes in Current
        double Imedian = m_lines.Select(l => m_data[l.Current.Magnitude].Where(d => !double.IsNaN(d)).FirstOrDefault()).Where(d => d != default(double)).Median().FirstOrDefault();
        alarmCurrent = alarmCurrent.Select(c => c.Magnitude > 100.0 * Imedian ? Complex.NaN : c).ToArray();

        // Clean Alarm PMU Data

        //remove any current values larger than 100X



        if (dataBad)
            return;

        //Calculate Pline 3 Phase MW Flow




        // Get Data From Queue
        if (!TrigggerCondition())
        return;

        //RemoveGarbagePMU
        //DataClean

        // RemoveTrend

        //IdentifySourceType

        // SelectKfactor - optional

        //  %====== Apply CPSD method for DE calculation ================
        /*  if GenSet(16) > 0
             % ---Remove trend in Vm,Va(not Fbus) signals
             [Vm11] = RemoveTrend(Vm, GenSet);
         Write_msg(ff{ 1}, [ff{ 16}
         '...Removed trend from Vm' ],GenSet(20),GenSet(20),0);
         [Va11] = RemoveTrend(Va, GenSet);
         Write_msg(ff{ 1}, [ff{ 16}
         '...Removed trend from Va ' ],GenSet(20),GenSet(20),0);
         % ---Apply CPSD method
         [DE_cpsd, Fcpsd] = CPSD(pmu.I2U, Pl11, Ql11, Vm11, Va11, GenSet, Vmagn);
         Write_msg(ff{ 1}, [ff{ 16}
         '...Completed CPSD analysis' ],GenSet(20),GenSet(20),0);
         clear Vm11 Va11 % Clean memory */

        // Run BandpassFilter

        /*
         *  %============ Apply CDEF method for DE calculation ==================
    if GenSet(16)==0 | GenSet(16)==2
        [DE_cdef,DE_time,DE_curve,DE_curveP,DE_curveQ,DE_curveQF,DE_curvePV] ...
                    = CDEF(pmu.I2U,Pl,Ql,Vm,Fbus,Va,GenSet,Vmagn);
        Write_msg(ff{1}, [ff{16} '...Completed DE calculation by CDEF method' ],GenSet(20),GenSet(20),0);
        %---- Reporting CDEF results
        Reporting_4(ff,GenSet,DE_cdef,DE_time,DE_curveP,DE_curveQ,...
            DE_curveQF,DE_curvePV,pmu);
    end
        */
         

        // Save Results in Table Form as appropriate
    }

    private bool  TrigggerCondition()
    {
        //SelTime.m Logic return true of Good Time INtervall Available and ALarm NOT False
        // Check DataConditions for all types of data

        return true;
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
    private Tuple<IEnumerable<IEnumerable<double>>,DataCleaning> PMU_Cleaning(IEnumerable<IEnumerable<double>> data, double TNaN, double epsilon1, double epsilon2, bool cleanOutlier)
    {
        DataCleaning condition = DataCleaning.Success;
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
                condition = DataCleaning.BadData;
                return new Tuple<IEnumerable<IEnumerable<double>>, DataCleaning>(data, condition);
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
            
            return d;

        });


        /*
            %% ---------------Handle the situation when the first sample is NaN
            % Replace the first NaN sample(and immedeatly following NaN) by linear extrapolation from 2secdata following NaN samples
            idx = find(isnan(In1(1,:))); % indices of variable with the first NaN sample
            if ~isempty(idx)
              for i = 1:numel(idx)
                  % replace NaN samples at the beginning by extrapolation of good data
                  id1 = find(~isnan(In1(:, idx(i)))); % indices of numbers at the beginning; after NaN
                  n_end = min([60 numel(id1)]); % number of available first 2 sec samples(at 30spf)
                  xx = id1(1:n_end); % x range of available samples
                  p = polyfit(xx, In1(xx, idx(i)), 1); % interpolation by 1st order function
                  xx = 1:1:id1(1) - 1; % samples to be extrapolated
                  In1(1:id1(1) - 1, idx(i)) = polyval(p, xx); % estimated values
              end
            end
            %% ---------------end of Handle the situation...


            %% ---------------Handle the situation when the last sample is NaN
            % Replace the last NaN sample(and immedeatly preceeding NaNs) by linear extrapolation from 2secdata preceeding NaN samples
            idx = find(isnan(In1(end,:))); % indices of variable with the first NaN sample
            if ~isempty(idx)
              for i = 1:numel(idx)
                  % replace NaN samples at the end by extrapolation of good data
                  id1 = find(~isnan(In1(:, idx(i)))); % indices of numbers; not NaNs
                  n_st = min([60 numel(id1)]); % number of available last 2 sec samples(at 30spf)
                  xx = id1(end - n_st + 1:end); % x range of available samples
                  p = polyfit(xx, In1(xx, idx(i)), 1); % interpolation by 1st order function
                  xx = id1(end) + 1:1:length(In1(:, 1)); % samples to be extrapolated
                  In1(id1(end) + 1:end, idx(i)) = polyval(p, xx); % estimated values
              end
            end
            %% ---------------end of Handle the situation...


            % Interpolate NaN
            In1 = interp1((1:Samples),In1(:, ~(sum(isnan(In1)) == Samples)) ,(1:Samples),'pchip');
                if Variables == 1; In1 = In1'; end


            Out = zeros(Samples, Variables);
            % restore initial size of matrix
            i1 = 0;
                for i = 1:Variables
                  if ind(i) == 1
                  % Out(:, i) = NaN;
                Out(:, i) = 0.0001;
              else
                    i1 = i1 + 1;
                Out(:, i) = In1(:, i1);
                end
              end
        end

        end */
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
        double median = NaNMedian(sampleData);
        double stdev = sampleData.StandardDeviation();

        double min = median - 6 * stdev;
        double max = median - 6 * stdev; ;


        return data.Select(v => {
            if (v < min || v > max)
                return double.NaN;
            return v;
        });
    }
        #endregion

    #region [ Static ]


    #endregion
    }

