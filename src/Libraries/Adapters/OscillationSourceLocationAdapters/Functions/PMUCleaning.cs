
//******************************************************************************************************
//  PMUCleaning.cs - Gbtc
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
//  05/28/2025 - G. Santos
//       Generated original version of source code.
//******************************************************************************************************

using DataQualityMonitoring.Model;
using Gemstone.Numeric;
using Gemstone.Numeric.Analysis;
using Gemstone.Numeric.UnitExtensions;
using Gemstone.Units;

namespace DataQualityMonitoring.Functions;

public static class PMUCleaning
{
    public static DataStatus PMUDataCleaning(IEnumerable<ComplexNumber> V, IEnumerable<ComplexNumber> I, IEnumerable<double> f, int samplingFrequency, bool CleanOutlier)
    {
        DataStatus result = DataStatus.Success;

        DataStatus code;
        IEnumerable<double> Vm = PMU_Cleaning(new List<IEnumerable<double>>() { V.Select(p => p.Magnitude) }, 0.5, 100, 2, samplingFrequency, CleanOutlier, out code).First();

        if (code == DataStatus.BadData || result == DataStatus.BadData)
            result = DataStatus.BadData;

        IEnumerable<double> Va = PMU_Cleaning(new List<IEnumerable<double>>() { V.Select(p => p.Angle).Unwrap().Select(a => a.ToDegrees()) }, 0.5, 1, 0.05, samplingFrequency, CleanOutlier, out code).First();

        if (code == DataStatus.BadData || result == DataStatus.BadData)
            result = DataStatus.BadData;

        IEnumerable<double> Im = PMU_Cleaning(new List<IEnumerable<double>>() { I.Select(p => p.Magnitude) }, 0.5, 3, 0.05, samplingFrequency, CleanOutlier, out code).First();

        if (code == DataStatus.BadData || result == DataStatus.BadData)
            result = DataStatus.BadData;

        IEnumerable<double> Ia = PMU_Cleaning(new List<IEnumerable<double>>() { I.Select(p => p.Angle).Unwrap().Select(a => a.ToDegrees()) }, 0.5, 1, 0.05, samplingFrequency, CleanOutlier, out code).First();

        if (code == DataStatus.BadData || result == DataStatus.BadData)
            result = DataStatus.BadData;

        IEnumerable<double> Fbus = PMU_Cleaning(new List<IEnumerable<double>>() { f }, 0.5, 48, 0.00005, samplingFrequency, CleanOutlier, out code).First();

        if (code == DataStatus.BadData || result == DataStatus.BadData)
            result = DataStatus.BadData;

        V = Vm.Zip(Va, (m, a) => new ComplexNumber(Angle.FromDegrees(a), m));
        I = Im.Zip(Ia, (m, a) => new ComplexNumber(Angle.FromDegrees(a), m));
        f = Fbus;

        return result;
    }

    public static DataStatus PMUDataCleaning(
        ref IEnumerable<IEnumerable<double>> Vm,
        ref IEnumerable<IEnumerable<double>> Va,
        ref IEnumerable<IEnumerable<double>> Im,
        ref IEnumerable<IEnumerable<double>> Ia,
        ref IEnumerable<IEnumerable<double>> Fbus,
        int samplingFrequency,
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
        Vm = PMU_Cleaning(Vm, 0.5, 100, 2, samplingFrequency, CleanOutlier, out code);

        if (code == DataStatus.BadData || result == DataStatus.BadData)
            result = DataStatus.BadData;

        Va = PMU_Cleaning(Va, 0.5, 1, 0.05, samplingFrequency, CleanOutlier, out code);

        if (code == DataStatus.BadData || result == DataStatus.BadData)
            result = DataStatus.BadData;

        Im = PMU_Cleaning(Im, 0.5, 3, 0.05, samplingFrequency, CleanOutlier, out code);

        if (code == DataStatus.BadData || result == DataStatus.BadData)
            result = DataStatus.BadData;

        Ia = PMU_Cleaning(Ia, 0.5, 1, 0.05, samplingFrequency, CleanOutlier, out code);

        if (code == DataStatus.BadData || result == DataStatus.BadData)
            result = DataStatus.BadData;

        Fbus = PMU_Cleaning(Fbus, 0.5, 48, 0.00005, samplingFrequency, CleanOutlier, out code);

        if (code == DataStatus.BadData || result == DataStatus.BadData)
            result = DataStatus.BadData;

        return result;
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
    private static IEnumerable<IEnumerable<double>> PMU_Cleaning(IEnumerable<IEnumerable<double>> data, double TNaN, double epsilon1, double epsilon2, int samplingFrequncy, bool cleanOutlier, out DataStatus condition)
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

        cleanedData = cleanedData.Select((d) => (d.Count((v) => double.IsNaN(v)) > TNaN * nSamples ? d.Select((v) => 1e-8) : d));

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
            cleanedData = cleanedData.Select(d => DataCleaning.RemoveOutliers(d, 0.05));
        }

        // Handle Sitouation where First sample is NaN
        cleanedData = cleanedData.Select((d) => {
            if (!double.IsNaN(d.First()))
                return d;

            IEnumerable<Tuple<double, double>> inpData = d.Select((v, i) => new Tuple<double, double>(v, i)).Where((v) => !double.IsNaN(v.Item1)).Take(2 * samplingFrequncy);

            double a, b;
            CurveFit.LeastSquares(inpData.Select(v => v.Item1).ToArray(), inpData.Select(v => v.Item2).ToArray(), out a, out b);
            int n = d.TakeWhile((v) => double.IsNaN(v)).Count() - 1;

            return d.Select((v, i) => (i <= n ? a * i + b : v));
        });

        //Handle the situation when the last sample is NaN
        cleanedData = cleanedData.Select((d) => {
            if (!double.IsNaN(d.Last()))
                return d;

            IEnumerable<Tuple<double, double>> inpData = d.Reverse().Select((v, i) => new Tuple<double, double>(v, i)).Where((v) => !double.IsNaN(v.Item1)).Take(2 * samplingFrequncy);

            double a, b;
            CurveFit.LeastSquares(inpData.Select(v => v.Item1).ToArray(), inpData.Select(v => v.Item2).ToArray(), out a, out b);
            int n = d.TakeWhile((v) => double.IsNaN(v)).Count() - 1;

            return d.Select((v, i) => ((d.Count() - i - 1) <= n ? a * (d.Count() - i - 1) + b : v));
        });

        cleanedData = cleanedData.Select((d, i) =>
        {
            if (!d.Any(v => !double.IsNaN(v)))
                return d.Select((v) => 0.0001);

            return DataCleaning.InterpolateNaN(d);
        });

        return cleanedData;
    }

}