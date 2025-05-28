
//******************************************************************************************************
//  InterpolateNaN.cs - Gbtc
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

using Gemstone.Numeric.Analysis;
using Gemstone.Numeric.Interpolation;

namespace DataQualityMonitoring.Functions;

public static class DataCleaning
{
    /// <summary>
    /// Note this is skipping the Interpolate step for now.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="threshold"></param>
    /// <returns></returns>
    public static IEnumerable<double> RemoveOutliers(IEnumerable<double> data, double threshold = 0.05, IEnumerable<int>? ind = null)
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

    /// <summary>
    /// use PCHip to interpolate data in NaN Locations
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static IEnumerable<double> InterpolateNaN(IEnumerable<double> data)
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
}