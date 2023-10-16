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

namespace openHistorian.Data.Query
{
    /// <summary>
    /// 
    /// </summary>
    public class DataFillAdapter
        : IDisposable
    {
        /// <summary>
        /// The underlying point stream used for data retrieval.
        /// </summary>
#pragma warning disable CS0618 // Type or member is obsolete
        private readonly PointStream m_stream;
#pragma warning restore CS0618 // Type or member is obsolete

        /// <summary>
        /// Initializes a new instance of the DataFillAdapter class with the specified point stream.
        /// </summary>
        /// <param name="stream">The point stream used for data retrieval.</param>
        public DataFillAdapter(PointStream stream)
        {
            m_stream = stream;
            m_stream.Read();
        }

        /// <summary>
        /// Gets or sets the frame time of the <see cref="DataFillAdapter"/>.
        /// </summary>
        public DateTime FrameTime;

        /// <summary>
        /// Fills data frames using the specified callback.
        /// </summary>
        /// <param name="callback">The callback function needed to process the data frames.</param>
        /// <returns>
        /// <c>true</c> if data filling is successful; otherwise, <c>false</c> indicates the stream is invalid.
        /// </returns>
        public bool Fill(Action<ulong, HistorianValue> callback)
        {
            if (!m_stream.IsValid)
                return false;

            ulong timeStamp = m_stream.CurrentKey.Timestamp;
            FrameTime = m_stream.CurrentKey.TimestampAsDate;
            callback(m_stream.CurrentKey.PointID, m_stream.CurrentValue);

            while (true)
            {
                if (!m_stream.Read())
                {
                    Dispose();
                    return true; // End of stream
                }

                if (m_stream.CurrentKey.Timestamp == timeStamp)
                    callback(m_stream.CurrentKey.PointID, m_stream.CurrentValue);

                else
                    return true;
            }
        }

        /// <summary>
        /// Once completed, disposes of the <see cref="DataFillAdapter"/> and releases the associated resources.
        /// </summary>
        public void Dispose()
        {
            m_stream.Dispose();
        }
    }

    /// <summary>
    /// Queries a historian database for a set of signals. 
    /// </summary>
    public static partial class GetDataFillAdapterMethods
    {

        /// <summary>
        /// Gets concentrated frames from the provided stream
        /// </summary>
        /// <param name="stream">The database to use.</param>
        /// <returns>
        /// A <see cref="DataFillAdapter"/> for the provided point stream.
        /// </returns>
        public static DataFillAdapter GetFillAdapter(this PointStream stream)
        {
            return new DataFillAdapter(stream);
        }

    }
}