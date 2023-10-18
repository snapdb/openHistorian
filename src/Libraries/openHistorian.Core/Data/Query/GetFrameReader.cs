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

using openHistorian.Core.Snap;
using openHistorian.Data.Query;

namespace openHistorian.Core.Data.Query;

/// <summary>
/// Represents a frame reader for querying historian data.
/// </summary>
public class FrameReader
    : IDisposable
{
    private readonly PointStream m_stream;

    /// <summary>
    /// Initializes a new instance of the FrameReader class.
    /// </summary>
    /// <param name="stream">The point stream for querying historian data.</param>
    public FrameReader(PointStream stream)
    {
        m_stream = stream;
        Frame = new SortedList<ulong, HistorianValueStruct>();
        m_stream.Read();
    }

    /// <summary>
    /// The timestamp associated with the current  frame.
    /// </summary>
    public DateTime FrameTime;


    /// <summary>
    /// Frame data containing point IDs and historian values.
    /// </summary>
    public SortedList<ulong, HistorianValueStruct> Frame;

    /// <summary>
    /// Reads the next frame from the historian data stream.
    /// </summary>
    /// <returns>True if there is another frame, false if the end of the stream is reached.</returns>
    public bool Read()
    {
        if (!m_stream.IsValid)
            return false;

        Frame.Clear();
        Frame.Add(m_stream.CurrentKey.PointID, m_stream.CurrentValue.ToStruct());
        FrameTime = m_stream.CurrentKey.TimestampAsDate;

        while (true)
        {
            if (!m_stream.Read())
            {
                Dispose();
                return true; //End of stream
            }

            if (m_stream.CurrentKey.TimestampAsDate == FrameTime)
            {
                //try
                //{
                Frame.Add(m_stream.CurrentKey.PointID, m_stream.CurrentValue.ToStruct());
                //}
                //catch (Exception ex)
                //{
                //    ex = ex;
                //}
            }
            else
            {
                return true;
            }
        }
    }

    /// <summary>
    /// Disposes of the stream.
    /// </summary>
    public void Dispose()
    {
        m_stream.Dispose();
    }
}

/// <summary>
/// Queries a historian database for a set of signals. 
/// </summary>
public static partial class GetFrameReaderMethods
{

    /// <summary>
    /// Gets concentrated frames from the provided stream.
    /// </summary>
    /// <param name="stream">The database to use.</param>
    /// <returns>The concentrated frames.</returns>
    public static FrameReader GetFrameReader(this PointStream stream)
    {
        return new FrameReader(stream);
    }

}