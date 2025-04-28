//******************************************************************************************************
//  GetPointStream.cs - Gbtc
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
//  10/19/2023 - Lillian Gensolin
//       Converted code to .NET core.
//
//******************************************************************************************************

using openHistorian.Data.Query;
using openHistorian.Snap;
using SnapDB.Collections;
using SnapDB.Snap;
using SnapDB.Snap.Filters;
using SnapDB.Snap.Services.Reader;

namespace openHistorian.Data.Query;

/// <summary>
/// A helper way to read data from a stream.
/// </summary>
[Obsolete("This will soon be removed")]
public class PointStream
    : TreeStream<HistorianKey, HistorianValue>
{
    private IDatabaseReader<HistorianKey, HistorianValue> m_reader;
    private readonly TreeStream<HistorianKey, HistorianValue> m_stream;

    /// <summary>
    /// Initializes a private database reader and a binary tree stream.
    /// </summary>
    /// <param name="reader">The reader for the tree stream.</param>
    /// <param name="stream">The binary tree stream.</param>
    public PointStream(IDatabaseReader<HistorianKey, HistorianValue> reader, TreeStream<HistorianKey, HistorianValue> stream)
    {
        m_stream = stream;
        m_reader = reader;
    }

    /// <summary>
    /// A new <see cref="HistorianKey"/> to be used as the current key.
    /// </summary>
    public HistorianKey CurrentKey = new();

    /// <summary>
    /// A new <see cref="HistorianValue"/> to be used as the current value.
    /// </summary>
    public HistorianValue CurrentValue = new();

    /// <summary>
    /// A boolean that returns whether or not a specified item is valid.
    /// </summary>
    public bool IsValid;

    /// <summary>
    /// A returns whether or not a key-pair value has been read.
    /// </summary>
    /// <returns><c>true</c> if the pair has been read, <c>false</c> if not.</returns>
    public bool Read()
    {
        return Read(CurrentKey, CurrentValue);
    }

    /// <summary>
    /// Moves to the next key-value pair to read.
    /// </summary>
    /// <param name="key">The key to read.</param>
    /// <param name="value">The value to read.</param>
    /// <returns><c>true</c> if the stream has been read and can advance, <c>false</c> if not read.</returns>
    protected override bool ReadNext(HistorianKey key, HistorianValue value)
    {
        if (m_stream.Read(CurrentKey, CurrentValue))
        {
            IsValid = true;
            return true;
        }
        else
        {
            IsValid = false;
            return false;
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with releasing or resetting resources.
    /// </summary>
    /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    /// <remarks>
    /// This method is used to clean up and release resources held by the object when it is disposed.
    /// </remarks>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            m_reader.Dispose();
            m_reader = null;
        }
    }
}

/// <summary>
/// Queries a historian database for a set of signals. 
/// </summary>
public static partial class GetPointStreamExtensionMethods
{


    ///// <summary>
    ///// Gets frames from the historian as individual frames.
    ///// </summary>
    ///// <param name="database">the database to use</param>
    ///// <returns></returns>
    //public static SortedList<DateTime, FrameData> GetFrames(this SortedTreeEngineBase<HistorianKey, HistorianValue> database, DateTime timestamp)
    //{
    //    return database.GetFrames(SortedTreeEngineReaderOptions.Default, TimestampFilter.CreateFromRange<HistorianKey>(timestamp, timestamp), PointIDFilter.CreateAllKeysValid<HistorianKey>(), null);
    //}

    /// <summary>
    /// Gets frames from the historian as individual frames.
    /// </summary>
    /// <param name="database">The database to use.</param>
    /// <param name="startTime">The starting time to be included in the query.</param>
    /// <param name="stopTime">The ending time to be included in the query.</param>
    /// <returns>The frames from the historian.</returns>
    public static PointStream GetPointStream(this IDatabaseReader<HistorianKey, HistorianValue> database, DateTime startTime, DateTime stopTime)
    {
        return database.GetPointStream(SortedTreeEngineReaderOptions.Default, TimestampSeekFilter.CreateFromRange<HistorianKey>(startTime, stopTime), null);
    }

    /// <summary>
    /// Gets frames from the historian as individual frames.
    /// </summary>
    /// <param name="database">the database to use</param>
    /// <param name="timestamps">The timestamps to query for.</param>
    /// <param name="points">The points to query for.</param>
    /// <returns>The frames from the historian.</returns>
    public static PointStream GetPointStream(this IDatabaseReader<HistorianKey, HistorianValue> database, SeekFilterBase<HistorianKey> timestamps, params ulong[] points)
    {
        return database.GetPointStream(SortedTreeEngineReaderOptions.Default, timestamps, PointIDMatchFilter.CreateFromList<HistorianKey, HistorianValue>(points));
    }


    /// <summary>
    /// Gets frames from the historian as individual frames.
    /// </summary>
    /// <param name="database">The database to use.</param>
    /// <param name="startTime">The starting time to be included in the queries.</param>
    /// <param name="stopTime">The ending time to be included in the queries.</param>
    /// <param name="points">The points to query for.</param>
    /// <returns>The frames from the historian.</returns>
    public static PointStream GetPointStream(this IDatabaseReader<HistorianKey, HistorianValue> database, DateTime startTime, DateTime stopTime, params ulong[] points)
    {
        return database.GetPointStream(SortedTreeEngineReaderOptions.Default, TimestampSeekFilter.CreateFromRange<HistorianKey>(startTime, stopTime), PointIDMatchFilter.CreateFromList<HistorianKey, HistorianValue>(points));
    }

    ///// <summary>
    ///// Gets frames from the historian as individual frames.
    ///// </summary>
    ///// <param name="database">the database to use</param>
    ///// <returns></returns>
    //public static SortedList<DateTime, FrameData> GetFrames(this SortedTreeEngineBase<HistorianKey, HistorianValue> database)
    //{
    //    return database.GetFrames(QueryFilterTimestamp.CreateAllKeysValid(), QueryFilterPointId.CreateAllKeysValid(), SortedTreeEngineReaderOptions.Default);
    //}

    ///// <summary>
    ///// Gets frames from the historian as individual frames.
    ///// </summary>
    ///// <param name="database">the database to use</param>
    ///// <param name="timestamps">the timestamps to query for</param>
    ///// <returns></returns>
    //public static SortedList<DateTime, FrameData> GetFrames(this SortedTreeEngineBase<HistorianKey, HistorianValue> database, QueryFilterTimestamp timestamps)
    //{
    //    return database.GetFrames(timestamps, QueryFilterPointId.CreateAllKeysValid(), SortedTreeEngineReaderOptions.Default);
    //}

    /// <summary>
    /// Gets frames from the historian as individual frames.
    /// </summary>
    /// <param name="database">the database to use</param>
    /// <param name="timestamps">the timestamps to query for</param>
    /// <param name="points">the points to query</param>
    /// <returns>The frames from the historian.</returns>
    public static PointStream GetPointStream(this IDatabaseReader<HistorianKey, HistorianValue> database, SeekFilterBase<HistorianKey> timestamps, MatchFilterBase<HistorianKey, HistorianValue> points)
    {
        return database.GetPointStream(SortedTreeEngineReaderOptions.Default, timestamps, points);
    }

    /// <summary>
    /// Gets frames from the historian as individual frames.
    /// </summary>
    /// <param name="database">the database to use</param>
    /// <param name="timestamps">the timestamps to query for</param>
    /// <param name="points">the points to query</param>
    /// <param name="options">A list of query options</param>
    /// <returns>The frames from the historian.</returns>
    public static PointStream GetPointStream(this IDatabaseReader<HistorianKey, HistorianValue> database,
        SortedTreeEngineReaderOptions options, SeekFilterBase<HistorianKey> timestamps, MatchFilterBase<HistorianKey, HistorianValue> points)
    {
        return new PointStream(database, database.Read(options, timestamps, points));
    }

    private class FrameDataConstructor
    {
        public readonly List<ulong> PointId = [];
        public readonly List<HistorianValueStruct> Values = [];

        /// <summary>
        /// Creates a new <see cref="FrameData"/> according to specified point IDs and values.
        /// </summary>
        /// <returns>The new <see cref="FrameData"/>.</returns>
        public FrameData ToFrameData()
        {
            return new FrameData(PointId, Values);
        }
    }

    /// <summary>
    /// Gets concentrated frames from the provided stream.
    /// </summary>
    /// <param name="stream">The database to use.</param>
    /// <returns>The frames from the historian.</returns>
    public static SortedList<DateTime, FrameData> GetPointStream(this TreeStream<HistorianKey, HistorianValue> stream)
    {
        HistorianKey key = new();
        HistorianValue value = new();

        SortedList<DateTime, FrameDataConstructor> results = new();
        ulong lastTime = ulong.MinValue;
        FrameDataConstructor lastFrame = null;
        while (stream.Read(key, value))
        {
            if (lastFrame is null || key.Timestamp != lastTime)
            {
                lastTime = key.Timestamp;
                DateTime timestamp = new((long)lastTime);

                if (!results.TryGetValue(timestamp, out lastFrame))
                {
                    lastFrame = new FrameDataConstructor();
                    results.Add(timestamp, lastFrame);
                }
            }
            lastFrame.PointId.Add(key.PointID);
            lastFrame.Values.Add(value.ToStruct());
        }
        List<FrameData> data = new(results.Count);
        data.AddRange(results.Values.Select(x => x.ToFrameData()));

        return SortedListFactory.Create(results.Keys, data);
    }
}