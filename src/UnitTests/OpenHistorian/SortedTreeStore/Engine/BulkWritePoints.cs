﻿//******************************************************************************************************
//  BulkWritePoints.cs - Gbtc
//
//  Copyright © 2023, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
//  file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  10/13/2023 - Lillian Gensolin
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Gemstone.Diagnostics;
using NUnit.Framework;
using openHistorian.Net;
using openHistorian.Snap;
using SnapDB;
using SnapDB.IO.Unmanaged;
using SnapDB.Snap;
using SnapDB.Snap.Services;

namespace openHistorian.UnitTests.SortedTreeStore.Engine;

[TestFixture]
public class BulkWritePoints
{
    #region [ Members ]

    private const int PointsToArchive = 100000000;
    private volatile int PointCount;
    private SortedList<double, int> PointSamples;
    private volatile bool Quit;

    #endregion

    #region [ Methods ]

    /// <summary>
    /// Verifies the database by reading and checking archived points.
    /// </summary>
    /// <exception cref="Exception">Thrown if files are corrupt, missing, or in excess.</exception>
    //[Test]
    public void VerifyDB()
    {
        //Logger.ReportToConsole(VerboseLevel.All ^ VerboseLevel.DebugLow);
        //Logger.ConsoleSubscriber.AddIgnored(Logger.LookupType("SnapDB.SortedTreeStore"));
        Globals.MemoryPool.SetMaximumBufferSize(1000 * 1024 * 1024);
        Globals.MemoryPool.SetTargetUtilizationLevel(TargetUtilizationLevels.Low);

        HistorianServerDatabaseConfig settings = new("DB", "c:\\temp\\benchmark\\", true);
        using SnapServer engine = new(settings);
        using SnapClient client = SnapClient.Connect(engine);
        using ClientDatabaseBase<HistorianKey, HistorianValue> db = client.GetDatabase<HistorianKey, HistorianValue>("DB");
        using TreeStream<HistorianKey, HistorianValue> scan = db.Read(null, null, null);
        HistorianKey key = new();
        HistorianValue value = new();

        Stopwatch sw = new();
        sw.Start();

        for (int x = 0; x < PointsToArchive; x++)
        {
            if (!scan.Read(key, value))
                throw new Exception("Missing points");
            if (key.PointID != (ulong)x)
                throw new Exception("Corrupt");
            if (key.Timestamp != 0)
                throw new Exception("Corrupt");
            if (key.EntryNumber != 0)
                throw new Exception("Corrupt");
            if (value.Value1 != 0)
                throw new Exception("Corrupt");
            if (value.Value1 != 0)
                throw new Exception("Corrupt");
            if (value.Value1 != 0)
                throw new Exception("Corrupt");
        }

        double totalTime = sw.Elapsed.TotalSeconds;
        Console.WriteLine("Completed read test in {0:#,##0.00} seconds at {1:#,##0.00} points per second", totalTime, PointsToArchive / totalTime);

        if (scan.Read(key, value))
            throw new Exception("too many points");
    }

    //[Test]
    //public void TestWriteSpeedSocket()
    //{
    //    Thread th = new Thread(WriteSpeed);
    //    th.IsBackground = true;
    //    th.Start();

    //    Quit = false;
    //    foreach (var file in Directory.GetFiles("c:\\temp\\benchmark\\"))
    //        File.Delete(file);

    //    PointCount = 0;
    //    var collection = new Server();
    //    using (var engine = new ServerDatabase<HistorianKey, HistorianValue>("DB", WriterMode.OnDisk, CreateHistorianCompressionTs.TypeGuid, "c:\\temp\\benchmark\\"))
    //    using (var socket = new SocketListener(13141, collection))
    //    {
    //        collection.Add(engine);

    //        var options = new RemoteClientOptions();
    //        options.serverNameOrIP = "127.0.0.1";
    //        options.NetworkPort = 13141;

    //        using (var client = new RemoteClient(options))
    //        using (var db = client.GetDatabase<HistorianKey, HistorianValue>("DB"))
    //        {
    //            db.SetEncodingMode(CreateHistorianCompressedStream.TypeGuid);

    //            engine.ProcessException += engine_Exception;
    //            Thread.Sleep(100);
    //            var key = new HistorianKey();
    //            var value = new HistorianValue();

    //            using (var writer = db.StartBulkWriting())
    //            {
    //                for (int x = 0; x < 100000000; x++)
    //                {
    //                    key.PointID = (ulong)x;
    //                    PointCount = x;
    //                    writer.Write(key, value);
    //                }
    //            }

    //            Quit = true;
    //            th.Join();
    //        }
    //    }

    //    Console.WriteLine("Time (sec)\tPoints");
    //    foreach (var kvp in PointSamples)
    //    {
    //        Console.WriteLine(kvp.Key.ToString() + "\t" + kvp.Value.ToString());
    //    }
    //}

    /// <summary>
    /// Tests the speed of writing data points to the database.
    /// </summary>
    [Test]
    public void TestWriteSpeed()
    {
        //Logger.ReportToConsole(VerboseLevel.All ^ VerboseLevel.DebugLow);
        //Logger.SetLoggingPath("c:\\temp\\");

        Globals.MemoryPool.SetMaximumBufferSize(4000 * 1024 * 1024L);

        //Thread th = new Thread(WriteSpeed);
        //th.IsBackground = true;
        //th.Start();

        //Quit = false;
        foreach (string file in Directory.GetFiles("c:\\temp\\benchmark\\", "*.*", SearchOption.AllDirectories))
            File.Delete(file);

        //PointCount = 0;

        HistorianServerDatabaseConfig settings = new("DB", "c:\\temp\\benchmark\\", true);

        using (SnapServer engine = new(settings))
        using (SnapClient client = SnapClient.Connect(engine))
        using (ClientDatabaseBase<HistorianKey, HistorianValue> db = client.GetDatabase<HistorianKey, HistorianValue>("DB"))
        {
            Thread.Sleep(100);
            HistorianKey key = new();
            HistorianValue value = new();

            Stopwatch sw = new();
            sw.Start();

            for (int x = 0; x < PointsToArchive; x++)
            {
                key.PointID = (ulong)x;
                //PointCount = x;
                db.Write(key, value);
            }

            double totalTime = sw.Elapsed.TotalSeconds;
            Console.WriteLine("Completed write test in {0:#,##0.00} seconds at {1:#,##0.00} points per second", totalTime, PointsToArchive / totalTime);
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
        Thread.Sleep(100);
    }

    /// <summary>
    /// Tests the write speed.
    /// </summary>
    [Test]
    public void TestWriteSpeedRandom()
    {
        Logger.Console.Verbose = VerboseLevel.All;

        Random r = new(1);
        Thread th = new(WriteSpeed) { IsBackground = true };
        th.Start();

        Quit = false;
        foreach (string file in Directory.GetFiles("c:\\temp\\benchmark\\"))
            File.Delete(file);

        PointCount = 0;

        HistorianServerDatabaseConfig settings = new("DB", "c:\\temp\\benchmark\\", true);

        using (SnapServer engine = new(settings))
        using (SnapClient client = SnapClient.Connect(engine))
        using (ClientDatabaseBase<HistorianKey, HistorianValue> db = client.GetDatabase<HistorianKey, HistorianValue>("DB"))
        {
            Thread.Sleep(100);
            HistorianKey key = new();
            HistorianValue value = new();
            for (int x = 0; x < 10000000; x++)
            {
                key.Timestamp = (ulong)r.Next();
                key.PointID = (ulong)x;
                PointCount = x;
                db.Write(key, value);
            }
        }

        Quit = true;
        th.Join();
        Console.WriteLine("Time (sec)\tPoints");
        foreach (KeyValuePair<double, int> kvp in PointSamples)
            Console.WriteLine(kvp.Key + "\t" + kvp.Value);
    }

    /// <summary>
    /// Tests the rollover functionality of the database by writing a large number of data points.
    /// </summary>
    [Test]
    public void TestRollover()
    {
        Logger.Console.Verbose = VerboseLevel.All;

        Globals.MemoryPool.SetMaximumBufferSize(4000 * 1024 * 1024L);

        //foreach (string file in Directory.GetFiles("c:\\temp\\Test\\", "*.*", SearchOption.AllDirectories))
        //    File.Delete(file);

        PointCount = 0;

        //HistorianServerDatabaseConfig settings = new("DB", "c:\\temp\\Test\\Main\\", true);
        //settings.FinalWritePaths.Add("c:\\temp\\Test\\Rollover\\");

        ulong time = (ulong)DateTime.Now.Ticks;

        //using (SnapServer engine = new(settings))
        //using (SnapClient client = SnapClient.Connect(engine))
        //using (ClientDatabaseBase<HistorianKey, HistorianValue> db = client.GetDatabase<HistorianKey, HistorianValue>("DB"))
        //{
        //    Thread.Sleep(100);
        //    HistorianKey key = new();
        //    HistorianValue value = new();
        //    for (int x = 0; x < 100000000; x++)
        //    {
        //        if (x % 100 == 0)
        //            Thread.Sleep(10);
        //        key.Timestamp = time;
        //        time += TimeSpan.TicksPerMinute;
        //        db.Write(key, value);
        //    }
        //}

        GC.Collect();
        GC.WaitForPendingFinalizers();
        Thread.Sleep(100);
    }

    /// <summary>
    /// Stores the write speed.
    /// </summary>
    private void WriteSpeed()
    {
        Stopwatch sw = new();
        sw.Start();
        PointSamples = new SortedList<double, int>();

        while (!Quit)
        {
            double elapsed = sw.Elapsed.TotalSeconds;
            PointSamples.Add(elapsed, PointCount);
            int sleepTime = (int)(elapsed * 1000) % 100;

            sleepTime = 100 - sleepTime;
            if (sleepTime < 50)
                sleepTime += 100;
            Thread.Sleep(sleepTime);
        }
    }

    #endregion
}