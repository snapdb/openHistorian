//******************************************************************************************************
//  ConcurrentReading.cs - Gbtc
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

using NUnit.Framework;
using openHistorian.Core.Snap;
using SnapDB.Snap;
using SnapDB.Snap.Services;
using SnapDB.Snap.Services.Reader;
using System;
using System.Diagnostics;
using System.Threading;
using openHistorian.Core.Net;

namespace openHistorian.UnitTests;

/// <summary>
/// This class contains methods for testing concurrent reading of historian data.
/// </summary>
[TestFixture]
public class ConcurrentReading
{
    const int PointsToRead = 10000000;
    int ReaderNumber;
    int ThreadNumber;
    volatile bool StopReading;

    /// <summary>
    /// Test concurrent scanning of points using multiple threads.
    /// </summary>
    [Test]
    public void ScanAllPoints()
    {
        Stats.Clear();
        long points;

        StopReading = false;

        HistorianServerDatabaseConfig settings = new("PPA", @"C:\Program Files\openHistorian\Archive\", true);
        using (HistorianServer server = new(settings))
        {
            Thread.Sleep(1000);

            for (int x = 1; x < 30; x++)
            {
                StartScanner();
                Thread.Sleep(1000);
                if (x == 1)
                    Thread.Sleep(5000);
                Interlocked.Exchange(ref Stats.PointsReturned, 0);
                Thread.Sleep(1000);
                long v = Interlocked.Read(ref Stats.PointsReturned);
                Console.WriteLine("Clients: " + x.ToString() + " points " + v.ToString());
            }

            StopReading = true;
            Thread.Sleep(2000);
        }
        Thread.Sleep(2000);

    }

    /// <summary>
    /// Test concurrent reading of points using multiple threads.
    /// </summary>
    [Test]
    public void SendAllPoints()
    {
        Stats.Clear();
        long points;

        StopReading = false;

        HistorianServerDatabaseConfig settings = new("PPA", @"C:\Program Files\openHistorian\Archive\", true);
        using (HistorianServer server = new(settings))
        {
            Thread.Sleep(1000);

            for (int x = 1; x < 30; x++)
            {
                StartReader();
                Thread.Sleep(1000);
                if (x == 1)
                    Thread.Sleep(5000);
                Interlocked.Exchange(ref Stats.PointsReturned, 0);
                Thread.Sleep(1000);
                long v = Interlocked.Read(ref Stats.PointsReturned);
                Console.WriteLine("Clients: " + x.ToString() + " points " + v.ToString());
            }

            StopReading = true;
            Thread.Sleep(2000);
        }
        Thread.Sleep(2000);

    }

    /// <summary>
    /// Starts a scanner thread for concurrent scanning of historian data.
    /// </summary>
    void StartScanner()
    {
        Thread th = new(ScannerThread);
        th.IsBackground = true;
        th.Start();
    }

    /// <summary>
    /// Runs a scanner thread for concurrent scanning of historian data.
    /// </summary>
    void ScannerThread()
    {
        int threadId = Interlocked.Increment(ref ThreadNumber);
        try
        {

            //DateTime start = DateTime.FromBinary(Convert.ToDateTime("2/1/2014").Date.Ticks + Convert.ToDateTime("6:00:00PM").TimeOfDay.Ticks).ToUniversalTime();
            while (!StopReading)
            {

                Stopwatch sw = new();
                using HistorianClient client = new("127.0.0.1", 12345);
                using ClientDatabaseBase<HistorianKey, HistorianValue> database = client.GetDatabase<HistorianKey, HistorianValue>(String.Empty);
                HistorianKey key = new();
                HistorianValue value = new();

                sw.Start();
                TreeStream<HistorianKey, HistorianValue> scan = database.Read(0, ulong.MaxValue, new ulong[] { 65, 953, 5562 });
                while (scan.Read(key, value))
                    ;
                sw.Stop();

                //Console.WriteLine("Thread: " + threadId.ToString() + " " + "Run Number: " + myId.ToString() + " " + (pointCount / sw.Elapsed.TotalSeconds / 1000000).ToString());
            }
        }
        catch (Exception)
        {
            //Console.WriteLine(ex.ToString());
        }
        Console.WriteLine("Thread: " + threadId.ToString() + " Quit");
    }

    /// <summary>
    /// Starts a reader thread for concurrent reading of historian data.
    /// </summary>
    void StartReader()
    {
        Thread th = new(ReaderThread);
        th.IsBackground = true;
        th.Start();
    }

    /// <summary>
    /// Runs a reader thread for concurrent reading of historian data.
    /// </summary>
    void ReaderThread()
    {
        int threadId = Interlocked.Increment(ref ThreadNumber);
        try
        {

            DateTime start = DateTime.FromBinary(Convert.ToDateTime("2/1/2014").Date.Ticks + Convert.ToDateTime("6:00:00PM").TimeOfDay.Ticks).ToUniversalTime();
            while (!StopReading)
            {

                int myId = Interlocked.Increment(ref ReaderNumber);
                Stopwatch sw = new();
                int pointCount = 0;
                using HistorianClient client = new("127.0.0.1", 12345);
                using ClientDatabaseBase<HistorianKey, HistorianValue> database = client.GetDatabase<HistorianKey, HistorianValue>(String.Empty);
                HistorianKey key = new();
                HistorianValue value = new();

                sw.Start();
                TreeStream<HistorianKey, HistorianValue> scan = database.Read((ulong)start.Ticks, ulong.MaxValue);//, new ulong[] { 65, 953, 5562 });
                while (scan.Read(key, value) && pointCount < PointsToRead)
                    pointCount++;
                sw.Stop();

                //Console.WriteLine("Thread: " + threadId.ToString() + " " + "Run Number: " + myId.ToString() + " " + (pointCount / sw.Elapsed.TotalSeconds / 1000000).ToString());
            }
        }
        catch (Exception)
        {
            //Console.WriteLine(ex.ToString());
        }
        Console.WriteLine("Thread: " + threadId.ToString() + " Quit");
    }


}