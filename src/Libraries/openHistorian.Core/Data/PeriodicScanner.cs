//******************************************************************************************************
//  PeriodicScanner.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  12/29/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//  10/11/2023 - Lillian Gensolin
//       Converted code to .NET core.
//
//******************************************************************************************************

using openHistorian.Core.Snap;
using SnapDB.Snap.Filters;

namespace openHistorian.Core.Data;

/// <summary>
/// Scanner class that parses for queries.
/// </summary>
public class PeriodicScanner
{
    #region [ Members ]

    private const decimal DecimalTicksPerDay = TimeSpan.TicksPerDay;

    private readonly List<long> m_downSampleRates;
    private readonly List<long> m_downSampleTicks;
    private readonly TimeSpan m_windowTolerance;

    #endregion

    #region [ Constructors ]

    /// <summary>
    /// Initializes a new instance of the PeriodicScanner class with the specified number of samples per second.
    /// The default sampling period is the number of ticks per second, divided by the number of samples per second, divided by four.
    /// </summary>
    /// <param name="samplesPerSecond">The number of samples per second for the scanner.</param>
    public PeriodicScanner(int samplesPerSecond) : this(samplesPerSecond, new TimeSpan(TimeSpan.TicksPerSecond / samplesPerSecond / 4))
    {
    }

    /// <summary>
    /// </summary>
    /// <param name="samplesPerSecond">The number of samples per second for the scanner.</param>
    /// <param name="windowTolerance">The acceptable margin on either side of the specified window.</param>
    public PeriodicScanner(int samplesPerSecond, TimeSpan windowTolerance)
    {
        m_windowTolerance = windowTolerance;
        m_downSampleRates = new List<long>();
        m_downSampleTicks = new List<long>();
        CalculateDownSampleRates(samplesPerSecond);
    }

    #endregion

    #region [ Methods ]

    /// <summary>
    /// Suggests the number of samples per day based on the specified time range and sample count.
    /// </summary>
    /// <param name="startTime">The start time of the time range.</param>
    /// <param name="endTime">The end time of the time range.</param>
    /// <param name="sampleCount">The desired sample count.</param>
    /// <returns>The suggested number of samples per day.</returns>
    public long SuggestSamplesPerDay(DateTime startTime, DateTime endTime, uint sampleCount)
    {
        double days = (endTime - startTime).TotalDays;

        long sampleRate = m_downSampleRates.FirstOrDefault(x => sampleCount <= x * days);
        if (sampleRate == 0)
            sampleRate = m_downSampleRates.Last();

        return sampleRate;
    }

    /// <summary>
    /// Gets a seek filter parser based on the specified time range and sample count.
    /// </summary>
    /// <param name="startTime">The start time of the time range.</param>
    /// <param name="endTime">The end time of the time range.</param>
    /// <param name="sampleCount">The desired sample count.</param>
    /// <returns>A seek filter parser for the specified time range and sample count.</returns>
    public SeekFilterBase<HistorianKey> GetParser(DateTime startTime, DateTime endTime, uint sampleCount)
    {
        return GetParser(startTime, endTime, (ulong)SuggestSamplesPerDay(startTime, endTime, sampleCount));
    }

    /// <summary>
    /// Gets a seek filter parser based on the specified time range and samples per day.
    /// </summary>
    /// <param name="startTime">The start time of the time range.</param>
    /// <param name="endTime">The end time of the time range.</param>
    /// <param name="samplesPerDay">The number of samples per day.</param>
    /// <returns>A seek filter parser for the specified time range and samples per day.</returns>
    public SeekFilterBase<HistorianKey> GetParser(DateTime startTime, DateTime endTime, ulong samplesPerDay)
    {
        long interval = (long)(TimeSpan.TicksPerDay / samplesPerDay);

        long startTime2 = RoundDownToNearestSample(startTime.Ticks, (long)samplesPerDay, interval);
        long endTime2 = RoundUpToNearestSample(endTime.Ticks, (long)samplesPerDay, interval);

        ulong bigInterval;

        uint count = 1;
        while (true)
        {
            if (samplesPerDay % count == 0)
                if (TimeSpan.TicksPerDay % (samplesPerDay / count) == 0)
                {
                    bigInterval = TimeSpan.TicksPerDay / (samplesPerDay / count);
                    break;
                }

            count++;
        }

        return TimestampSeekFilter.CreateFromIntervalData<HistorianKey>((ulong)startTime2, (ulong)endTime2, bigInterval, (ulong)interval, (ulong)m_windowTolerance.Ticks);
    }

    private void CalculateDownSampleRates(int samplesPerSecond)
    {
        m_downSampleRates.Add(1); //1 sample per day
        m_downSampleRates.Add(2); //2 sampes per day
        m_downSampleRates.Add(3);
        m_downSampleRates.Add(4);
        m_downSampleRates.Add(6);
        m_downSampleRates.Add(8);
        m_downSampleRates.Add(12);
        m_downSampleRates.Add(24); //1 sample per hour
        m_downSampleRates.Add(24 * 2);
        m_downSampleRates.Add(24 * 3);
        m_downSampleRates.Add(24 * 4);
        m_downSampleRates.Add(24 * 5);
        m_downSampleRates.Add(24 * 6);
        m_downSampleRates.Add(24 * 10);
        m_downSampleRates.Add(24 * 12);
        m_downSampleRates.Add(24 * 15);
        m_downSampleRates.Add(24 * 20);
        m_downSampleRates.Add(24 * 30);
        m_downSampleRates.Add(24 * 60); //1 sample per hour
        m_downSampleRates.Add(24 * 60 * 2);
        m_downSampleRates.Add(24 * 60 * 3);
        m_downSampleRates.Add(24 * 60 * 4);
        m_downSampleRates.Add(24 * 60 * 5);
        m_downSampleRates.Add(24 * 60 * 6);
        m_downSampleRates.Add(24 * 60 * 10);
        m_downSampleRates.Add(24 * 60 * 12);
        m_downSampleRates.Add(24 * 60 * 15);
        m_downSampleRates.Add(24 * 60 * 20);
        m_downSampleRates.Add(24 * 60 * 30);
        m_downSampleRates.Add(24 * 60 * 60); //1 sample per second


        List<int> factors = FactorNumber(samplesPerSecond);

        for (int x = 1; x < factors.Count; x++)
            m_downSampleRates.Add(24L * 60L * 60L * factors[x]);

        foreach (long rate in m_downSampleRates)
            m_downSampleTicks.Add(TimeSpan.TicksPerDay / rate);
    }

    #endregion

    #region [ Static ]

    private static long RoundDownToNearestSample(long startTime, long samplesPerDay, long interval)
    {
        if (interval * samplesPerDay == TimeSpan.TicksPerDay)
            return startTime - startTime % interval;

        //Not exact, but close enough.
        //ToDo: Consider the error if using double precision instead of decimal.
        long dateTicks = startTime - startTime % TimeSpan.TicksPerDay;
        long timeTicks = startTime - dateTicks; // timeticks cannot be more than 864 billion.

        decimal interval2 = DecimalTicksPerDay / samplesPerDay;
        decimal overBy = timeTicks % interval2;

        return dateTicks + timeTicks - (long)overBy;
    }

    private static long RoundUpToNearestSample(long startTime, long samplesPerDay, long interval)
    {
        if (interval * samplesPerDay == TimeSpan.TicksPerDay)
        {
            long delta = startTime % interval;
            if (delta == 0)
                return startTime;
            return startTime - delta + interval;
        }

        //Not exact, but close enough.
        //ToDo: Consider the error if using double precision instead of decimal.
        long dateTicks = startTime - startTime % TimeSpan.TicksPerDay;
        long timeTicks = startTime - dateTicks; //timeticks cannot be more than 864 billion.

        decimal interval2 = DecimalTicksPerDay / samplesPerDay;
        decimal overBy = timeTicks % interval2;

        if (overBy == 0)
            return dateTicks + timeTicks;
        return dateTicks + timeTicks - (long)overBy + interval;
    }

    private static List<int> FactorNumber(int number)
    {
        if (number < 1)
            throw new ArgumentOutOfRangeException(nameof(number), "Must be greather than or equal to 1");
        List<int> factors = new();
        for (int x = 1; x * x <= number; x++)
        {
            if (number % x == 0)
            {
                factors.Add(x);
                if (x * x != number)
                    factors.Add(number / x);
            }
        }

        factors.Sort();
        return factors;
    }

    #endregion
}