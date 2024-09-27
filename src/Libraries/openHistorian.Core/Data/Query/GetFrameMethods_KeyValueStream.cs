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

namespace openHistorian.Data.Query;

/// <summary>
/// Represents a container for frame data, including sorted points and values.
/// </summary>
public class FrameData
{
    #region [ Members ]

    /// <summary>
    /// Points within the <see cref="FrameData"/>.
    /// </summary>
    public SortedList<ulong, HistorianValueStruct> Points;

    #endregion

    #region [ Constructors ]

    /// <summary>
    /// Initializes a new instance of the FrameData class with an empty sorted list for points and values.
    /// </summary>
    public FrameData()
    {
        Points = new SortedList<ulong, HistorianValueStruct>();
    }

    /// <summary>
    /// Initializes a new instance of the FrameData class with a sorted list of points and their associated values.
    /// </summary>
    /// <param name="pointId">A list of point IDs to be included in the frame.</param>
    /// <param name="values">A list of historian values associated with the points.</param>
    public FrameData(List<ulong> pointId, List<HistorianValueStruct> values)
    {
        Points = SortedListFactory.Create(pointId, values);
    }

    #endregion
}

/// <summary>
/// Provides extension methods for querying a historian database for sets of signals.
/// </summary>
public static partial class GetFrameMethods
{
    #region [ Members ]

    /// <summary>
    /// Helper class for constructing FrameData instances by accumulating point IDs and values.
    /// </summary>
    public class FrameDataConstructor
    {
        #region [ Members ]

        /// <summary>
        /// Gets a list of point IDs to be included in the frame.
        /// </summary>
        public readonly List<ulong> PointID = new();

        /// <summary>
        /// Gets a list of historian values associated with the points.
        /// </summary>
        public readonly List<HistorianValueStruct> Values = new();

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Creates a new FrameData instance from the accumulated point IDs and values.
        /// </summary>
        /// <returns>A FrameData instance based on the accumulated data.</returns>
        public FrameData ToFrameData()
        {
            return new FrameData(PointID, Values);
        }

        #endregion
    }

    #endregion

    #region [ Static ]

    /// <summary>
    /// Retrieves concentrated frames from the provided stream and organizes them by timestamp.
    /// </summary>
    /// <param name="stream">The database stream to use for data retrieval.</param>
    /// <returns>A sorted list of frame data organized by timestamps.</returns>
    public static SortedList<DateTime, FrameData> GetFrames(this TreeStream<HistorianKey, HistorianValue> stream)
    {
        SortedList<DateTime, FrameDataConstructor> results = new();
        ulong lastTime = ulong.MinValue;
        FrameDataConstructor lastFrame = null;
        HistorianKey key = new();
        HistorianValue value = new();

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