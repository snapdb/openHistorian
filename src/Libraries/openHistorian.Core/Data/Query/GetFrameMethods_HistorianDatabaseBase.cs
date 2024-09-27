//******************************************************************************************************
//  GetFrameMethods.cs - Gbtc
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
//  08/07/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//
//  10/12/2023 - Lillian Gensolin
//       Converted code to .NET core.
//
//******************************************************************************************************

using openHistorian.Snap;
using SnapDB.Collections;
using SnapDB.Snap;
using SnapDB.Snap.Filters;
using SnapDB.Snap.Services.Reader;

namespace openHistorian.Data.Query;

/// <summary>
/// Queries a historian database for a set of signals.
/// </summary>
public static partial class GetFrameMethods
{
    #region [ Members ]

    private class EnumerableHelper
    {
        #region [ Members ]

        public bool IsValid;
        public ulong PointID;
        public HistorianValueStruct Value;
        private readonly IEnumerator<KeyValuePair<ulong, HistorianValueStruct>> m_enumerator;

        #endregion

        #region [ Constructors ]

        public EnumerableHelper(FrameData frame)
        {
            m_enumerator = frame.Points.GetEnumerator();
            IsValid = true;
            Read();
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Reads the data and ensures that it is valid by iterating through until it reaches an invalid item.
        /// </summary>
        public void Read()
        {
            if (IsValid && m_enumerator.MoveNext())
            {
                IsValid = true;
                PointID = m_enumerator.Current.Key;
                Value = m_enumerator.Current.Value;
            }

            else
            {
                IsValid = false;
            }
        }

        #endregion
    }

    #endregion

    #region [ Static ]

    /// <summary>
    /// Gets frames from the historian as individual frames.
    /// </summary>
    /// <param name="database">The database to use.</param>
    /// <param name="startTime">The starting time to be included in the request.</param>
    /// <param name="stopTime">The ending time to be included in the request.</param>
    /// <returns>The frames from the historian.</returns>
    public static SortedList<DateTime, FrameData> GetFrames(this IDatabaseReader<HistorianKey, HistorianValue> database, DateTime startTime, DateTime stopTime)
    {
        return database.GetFrames(SortedTreeEngineReaderOptions.Default, TimestampSeekFilter.CreateFromRange<HistorianKey>(startTime, stopTime), null);
    }

    /// <summary>
    /// Gets frames from the historian as individual frames.
    /// </summary>
    /// <param name="database">The database to use.</param>
    /// <param name="timestamps">The timestamp associated with each frame.</param>
    /// <param name="points">An array of point IDs for which frames are requested.</param>
    /// <returns>The frames from the historian.</returns>
    public static SortedList<DateTime, FrameData> GetFrames(this IDatabaseReader<HistorianKey, HistorianValue> database, SeekFilterBase<HistorianKey> timestamps, params ulong[] points)
    {
        return database.GetFrames(SortedTreeEngineReaderOptions.Default, timestamps, PointIDMatchFilter.CreateFromList<HistorianKey, HistorianValue>(points));
    }

    /// <summary>
    /// Gets frames from the historian as individual frames.
    /// </summary>
    /// <param name="database">The database to use.</param>
    /// <param name="startTime">The starting time to be included in the request.</param>
    /// <param name="stopTime">The ending time to be included in the request.</param>
    /// <param name="points">An array of point IDs for which frames are requested.</param>
    /// <returns>The frames from the historian.</returns>
    public static SortedList<DateTime, FrameData> GetFrames(this IDatabaseReader<HistorianKey, HistorianValue> database, DateTime startTime, DateTime stopTime, params ulong[] points)
    {
        return database.GetFrames(SortedTreeEngineReaderOptions.Default, TimestampSeekFilter.CreateFromRange<HistorianKey>(startTime, stopTime), PointIDMatchFilter.CreateFromList<HistorianKey, HistorianValue>(points));
    }

    /// <summary>
    /// Gets frames from the historian as individual frames.
    /// </summary>
    /// <param name="database">The database to use.</param>
    /// <param name="timestamps">The timestamps to query for.</param>
    /// <param name="points">The points to query.</param>
    /// <returns>The frames from the historian.</returns>
    public static SortedList<DateTime, FrameData> GetFrames(this IDatabaseReader<HistorianKey, HistorianValue> database, SeekFilterBase<HistorianKey> timestamps, MatchFilterBase<HistorianKey, HistorianValue> points)
    {
        return database.GetFrames(SortedTreeEngineReaderOptions.Default, timestamps, points);
    }

    /// <summary>
    /// Gets frames from the historian as individual frames.
    /// </summary>
    /// <param name="database">the database to use</param>
    /// <param name="timestamps">the timestamps to query for</param>
    /// <param name="points">the points to query</param>
    /// <param name="options">A list of query options</param>
    /// <returns>The frames from the historian.</returns>
    public static SortedList<DateTime, FrameData> GetFrames(this IDatabaseReader<HistorianKey, HistorianValue> database, SortedTreeEngineReaderOptions options, SeekFilterBase<HistorianKey> timestamps, MatchFilterBase<HistorianKey, HistorianValue> points)
    {
        return database.Read(options, timestamps, points).GetFrames();
    }

    /// <summary>
    /// Rounds the frame to the nearest level of specified tolerance.
    /// </summary>
    /// <param name="original">the frame to round</param>
    /// <param name="toleranceMilliseconds">the timespan in milliseconds.</param>
    /// <returns>A new frame that is rounded.</returns>
    public static SortedList<DateTime, FrameData> RoundToTolerance(this SortedList<DateTime, FrameData> original, int toleranceMilliseconds)
    {
        return original.RoundToTolerance(new TimeSpan(TimeSpan.TicksPerMillisecond * toleranceMilliseconds));
    }

    /// <summary>
    /// Rounds the frame to the nearest level of specified tolerance.
    /// </summary>
    /// <param name="original">The frame to round.</param>
    /// <param name="tolerance">The timespan to round on.</param>
    /// <returns>A new frame that is rounded.</returns>
    public static SortedList<DateTime, FrameData> RoundToTolerance(this SortedList<DateTime, FrameData> original, TimeSpan tolerance)
    {
        SortedList<DateTime, FrameData> results = new();

        SortedList<DateTime, List<FrameData>> buckets = new();

        foreach (KeyValuePair<DateTime, FrameData> items in original)
        {
            DateTime roundedDate = items.Key.Round(tolerance);
            if (!buckets.TryGetValue(roundedDate, out List<FrameData> frames))
            {
                frames = new List<FrameData>();
                buckets.Add(roundedDate, frames);
            }

            frames.Add(items.Value);
        }

        foreach (KeyValuePair<DateTime, List<FrameData>> bucket in buckets)
        {
            if (bucket.Value.Count == 1)
            {
                results.Add(bucket.Key, bucket.Value[0]);
            }
            else
            {
                int count = bucket.Value.Sum(x => x.Points.Count);
                List<ulong> keys = new(count);
                List<HistorianValueStruct> values = new(count);

                FrameData tempFrame = new();
                tempFrame.Points = new SortedList<ulong, HistorianValueStruct>();

                List<EnumerableHelper> allFrames = new();

                foreach (FrameData frame in bucket.Value)
                    allFrames.Add(new EnumerableHelper(frame));

                while (true)
                {
                    EnumerableHelper lowestKey = null;

                    foreach (EnumerableHelper item in allFrames)
                        lowestKey = Min(lowestKey, item);

                    if (lowestKey is null)
                        break;

                    keys.Add(lowestKey.PointID);
                    values.Add(lowestKey.Value);

                    //tempFrame.Points.Add(lowestKey.PointID, lowestKey.Value);
                    lowestKey.Read();
                }

                tempFrame.Points = SortedListFactory.Create(keys, values);
                results.Add(bucket.Key, tempFrame);
            }
        }

        return results;
    }

    private static DateTime Round(this DateTime original, TimeSpan tolerance)
    {
        long delta = original.Ticks % tolerance.Ticks;
        if (delta >= tolerance.Ticks >> 1)
            return new DateTime(original.Ticks - delta + tolerance.Ticks);

        return new DateTime(original.Ticks - delta);
    }

    private static EnumerableHelper Min(EnumerableHelper left, EnumerableHelper right)
    {
        if (left is null)
            return right;
        if (right is null)
            return left;
        if (!left.IsValid && !right.IsValid)
            return null;
        if (!left.IsValid)
            return right;
        if (!right.IsValid)
            return left;
        if (left.PointID < right.PointID)
            return left;
        return right;
    }

    #endregion
}