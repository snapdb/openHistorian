﻿//******************************************************************************************************
//  HistorianInputQueue.cs - Gbtc
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
//  01/04/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//
//  10/12/2023 - Lillian Gensolin
//       Converted code to .NET core.
//
//******************************************************************************************************

using Gemstone;
using Gemstone.Threading;
using openHistorian.Snap;
using SnapDB;
using SnapDB.Snap;
using SnapDB.Snap.Services;

namespace openHistorian.Queues;

/// <summary>
/// Serves as a local queue for getting data into a remote historian.
/// This queue will isolate the input from the volatility of a
/// remote historian. Data is also kept in this buffer until it has been committed
/// to the disk subsystem.
/// </summary>
public class HistorianInputQueue : IDisposable
{
    #region [ Members ]

    private class StreamPoints : TreeStream<HistorianKey, HistorianValue>
    {
        #region [ Members ]

        private int m_count;
        private readonly int m_maxPoints;
        private readonly IsolatedQueue<PointData> m_measurements;

        #endregion

        #region [ Constructors ]

        public StreamPoints(IsolatedQueue<PointData> measurements, int maxPointsPerStream)
        {
            m_measurements = measurements;
            m_maxPoints = maxPointsPerStream;
        }

        #endregion

        #region [ Properties ]

        public bool QuitOnPointCount => m_count >= m_maxPoints;

        #endregion

        #region [ Methods ]

        public void Reset()
        {
            m_count = 0;
            SetEos(false);
        }

        protected override void EndOfStreamReached()
        {
            SetEos(true);
        }

        protected override bool ReadNext(HistorianKey key, HistorianValue value)
        {
            if (m_count < m_maxPoints && m_measurements.TryDequeue(out PointData data))
            {
                key.Timestamp = data.Key1;
                key.PointID = data.Key2;
                value.Value3 = data.Value1;
                value.Value1 = data.Value2;
                m_count++;
                return true;
            }

            key.Timestamp = 0;
            key.PointID = 0;
            value.Value3 = 0;
            value.Value1 = 0;
            return false;
        }

        #endregion
    }

    private struct PointData
    {
        public ulong Key1;
        public ulong Key2;
        public ulong Value1;
        public ulong Value2;

        public bool Load(TreeStream<HistorianKey, HistorianValue> stream)
        {
            HistorianKey key = new();
            HistorianValue value = new();
            if (stream.Read(key, value))
            {
                Key1 = key.Timestamp;
                Key2 = key.PointID;
                Value1 = value.Value3;
                Value2 = value.Value1;

                return true;
            }

            return false;
        }
    }

    private readonly IsolatedQueue<PointData> m_blocks;

    private ClientDatabaseBase<HistorianKey, HistorianValue> m_database;

    private readonly Func<ClientDatabaseBase<HistorianKey, HistorianValue>> m_getDatabase;

    private readonly StreamPoints m_pointStream;

    private readonly Lock m_syncWrite;

    private readonly ScheduledTask m_worker;

    #endregion

    #region [ Constructors ]

    /// <summary>
    /// Initializes a new instance of the HistorianInputQueue with the specified function to get a database.
    /// </summary>
    /// <param name="getDatabase">A function that returns a ClientDatabaseBase of HistorianKey and HistorianValue.</param>
    public HistorianInputQueue(Func<ClientDatabaseBase<HistorianKey, HistorianValue>> getDatabase)
    {
        m_syncWrite = new Lock();
        m_blocks = new IsolatedQueue<PointData>();
        m_pointStream = new StreamPoints(m_blocks, 1000);
        m_getDatabase = getDatabase;
        m_worker = new ScheduledTask(ThreadingMode.DedicatedForeground);
        m_worker.Running += WorkerDoWork;
    }

    #endregion

    #region [ Properties ]

    /// <summary>
    /// Gets queue size.
    /// </summary>
    public long Size => m_blocks is null ? 0L : m_blocks.Count;

    #endregion

    #region [ Methods ]

    /// <summary>
    /// </summary>
    public void Dispose()
    {
        m_worker.Dispose();
    }

    /// <summary>
    /// Provides a thread safe way to enqueue points.
    /// While points are streaming all other writes are blocked. Therefore,
    /// this point stream should be high speed.
    /// </summary>
    /// <param name="stream">The high speed point stream.</param>
    public void Enqueue(TreeStream<HistorianKey, HistorianValue> stream)
    {
        lock (m_syncWrite)
        {
            PointData data = default;
            while (data.Load(stream))
                m_blocks.Enqueue(data);
        }

        m_worker.Start();
    }

    /// <summary>
    /// Adds point data to the queue.
    /// </summary>
    /// <param name="key">The key associated with the point being added.</param>
    /// <param name="value">The value associated with the point being added.</param>
    public void Enqueue(HistorianKey key, HistorianValue value)
    {
        lock (m_syncWrite)
        {
            PointData data = new()
            {
                Key1 = key.Timestamp,
                Key2 = key.PointID,
                Value1 = value.Value3,
                Value2 = value.Value1
            };
            m_blocks.Enqueue(data);
        }

        m_worker.Start();
    }

    private void WorkerDoWork(object sender, EventArgs<ScheduledTaskRunningReason> eventArgs)
    {
        m_pointStream.Reset();

        try
        {
            if (m_database is null)
                m_database = m_getDatabase();
            m_database.Write(m_pointStream);
        }
        catch (Exception)
        {
            m_database = null;
            m_worker.Start(1000);
            return;
        }

        if (m_pointStream.QuitOnPointCount)
            m_worker.Start();
        else
            m_worker.Start(1000);
    }

    #endregion
}