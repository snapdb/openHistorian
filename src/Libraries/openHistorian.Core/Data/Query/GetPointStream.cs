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
//  10/12/2023 - Lillian Gensolin
//       Converted code to .NET core.
//
//******************************************************************************************************

using openHistorian.Core.Snap;
using SnapDB.Collections;
using SnapDB.Snap;
using SnapDB.Snap.Filters;
using SnapDB.Snap.Services.Reader;

namespace openHistorian.Core.Data.Query;

/// <summary>
/// A helper way to read data from a stream.
/// </summary>
[Obsolete("This will soon be removed")]
public class PointStream : TreeStream<HistorianKey, HistorianValue>
{
    #region [ Members ]

    /// <summary>
    /// Creates a new historian key to be used as the current key.
    /// </summary>
    public HistorianKey CurrentKey = new();

    /// <summary>
    /// Creates a new historian value to be used as the current value.
    /// </summary>
    public HistorianValue CurrentValue = new();

    /// <summary>
    /// Checks to see if something is valid.
    /// </summary>
    public bool IsValid;

    private IDatabaseReader<HistorianKey, HistorianValue> m_reader;
    private readonly TreeStream<HistorianKey, HistorianValue> m_stream;

    #endregion

    #region [ Constructors ]

    /// <summary>
    /// Creates a new point stream.
    /// </summary>
    /// <param name="reader">The reader, which will be called m_reader for this instance, to use.</param>
    /// <param name="stream">The stream, which will be called m_stream for this instance, to use.</param>
    public PointStream(IDatabaseReader<HistorianKey, HistorianValue> reader, TreeStream<HistorianKey, HistorianValue> stream)
    {
        m_stream = stream;
        m_reader = reader;
    }

    #endregion

    #region [ Methods ]

    /// <summary>
    /// Disposes of the reader.
    /// </summary>
    /// <param name="disposing">If <c>true</c>, disposes of the m_reader and sets its default value to null; otherwise, <c>false</c> does nothing.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            m_reader.Dispose();
            m_reader = null;
        }
    }

    /// <summary>
    /// Checks to see if the current key and value are read.
    /// </summary>
    /// <returns><c>true</c> if read has occurred; otherwise, false.</returns>
    public bool Read()
    {
        return Read(CurrentKey, CurrentValue);
    }

    /// <summary>
    /// Reads the next key and value.
    /// </summary>
    /// <param name="key">The key to read.</param>
    /// <param name="value">The value to read.</param>
    /// <returns><c>true</c> if the current key and value are being read; otherwise, <c>false</c>.</returns>
    protected override bool ReadNext(HistorianKey key, HistorianValue value)
    {
        if (m_stream.Read(CurrentKey, CurrentValue))
        {
            IsValid = true;
            return IsValid;
        }

        IsValid = false;
        return IsValid;
    }

    #endregion
}

/// <summary>
/// Queries a historian database for a set of signals.
/// </summary>
public static class GetPointStreamExtensionMethods
{
    #region [ Members ]

    /// <summary>
    /// Creates lists for point IDs and value .
    /// </summary>
    private class FrameDataConstructor
    {
        if (disposing)
        {
            m_reader.Dispose();
            m_reader = null;
        }
    }
}

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
    /// <param name="database">The database to use.</param>
    /// <param name="timestamps">The timestamps associated with the frames.</param>
    /// <param name="points">An array of point IDs for which frames are requested.</param>
    /// <returns></returns>
    public static PointStream GetPointStream(this IDatabaseReader<HistorianKey, HistorianValue> database, SeekFilterBase<HistorianKey> timestamps, params ulong[] points)
    {
        return database.GetPointStream(SortedTreeEngineReaderOptions.Default, timestamps, PointIDMatchFilter.CreateFromList<HistorianKey, HistorianValue>(points));
    }

    /// <summary>
    /// Gets frames from the historian as individual frames.
    /// </summary>
    /// <param name="database">The database to use.</param>
    /// <returns>The frames from the historian.</returns>
    public static PointStream GetPointStream(this IDatabaseReader<HistorianKey, HistorianValue> database, DateTime startTime, DateTime stopTime, params ulong[] points)
    {
        return database.GetPointStream(SortedTreeEngineReaderOptions.Default, TimestampSeekFilter.CreateFromRange<HistorianKey>(startTime, stopTime), PointIDMatchFilter.CreateFromList<HistorianKey, HistorianValue>(points));
    }

    /// <summary>
    /// Gets frames from the historian as individual frames.
    /// </summary>
    /// <param name="database">The database to use.</param>
    /// <param name="timestamps">The timestamps to query for.</param>
    /// <param name="points">The points to query.</param>
    /// <returns>The frames from the historian.</returns>
    public static PointStream GetPointStream(this IDatabaseReader<HistorianKey, HistorianValue> database, SeekFilterBase<HistorianKey> timestamps, MatchFilterBase<HistorianKey, HistorianValue> points)
    {
        return database.GetPointStream(SortedTreeEngineReaderOptions.Default, timestamps, points);
    }

    /// <summary>
    /// Gets frames from the historian as individual frames.
    /// </summary>
    /// <param name="database">The database to use.</param>
    /// <param name="timestamps">The timestamps to query for.</param>
    /// <param name="points">The points to query.</param>
    /// <param name="options">A list of query options.</param>
    /// <returns>The frames from the historian</returns>
    public static PointStream GetPointStream(this IDatabaseReader<HistorianKey, HistorianValue> database, SortedTreeEngineReaderOptions options, SeekFilterBase<HistorianKey> timestamps, MatchFilterBase<HistorianKey, HistorianValue> points)
    {
        return new PointStream(database, database.Read(options, timestamps, points));
    }

    /// <summary>
    /// Gets concentrated frames from the provided stream
    /// </summary>
    /// <param name="stream">The database to use</param>
    /// <returns>The concentrated frames from the historian.</returns>
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

            lastFrame.PointID.Add(key.PointID);
            lastFrame.Values.Add(value.ToStruct());
        }

        List<FrameData> data = new(results.Count);
        data.AddRange(results.Values.Select(x => x.ToFrameData()));

        return SortedListFactory.Create(results.Keys, data);
    }

    #endregion
}