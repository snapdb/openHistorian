//******************************************************************************************************
//  ReadPoints.cs - Gbtc
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
//       Converted code to .NET core.
//
//******************************************************************************************************

using NUnit.Framework;
using openHistorian.Core.Net;
using openHistorian.Core.Snap;
using SnapDB.Snap;
using SnapDB.Snap.Services;
using SnapDB.Snap.Services.Reader;
using System;
using System.Diagnostics;

namespace openHistorian.UnitTests;

[TestFixture]
public class ReadPoints
{
    [Test]
    public void ReadFrames()
    {
        //Stopwatch sw = new Stopwatch();
        //int pointCount = 0;
        //HistorianDatabaseInstance db = new HistorianDatabaseInstance();
        //db.InMemoryArchive = true;
        //db.ConnectionString = "port=12345";
        //db.Paths = new[] { @"C:\Program Files\openHistorian\Archive\" };

        //using (HistorianServer server = new HistorianServer(db))
        //{
        //    SortedTreeClientOptions clientOptions = new SortedTreeClientOptions();
        //    clientOptions.NetworkPort = 12345;
        //    clientOptions.ServerNameOrIp = "127.0.0.1";

        //    using (var client = new HistorianClient(clientOptions))
        //    {
        //        var database = server.GetDefaultDatabase();

        //        using (var frameReader = database.GetPointStream(DateTime.MinValue, DateTime.MaxValue).GetFrameReader())
        //        {
        //            while (frameReader.Read())
        //                ;
        //        }


        //        sw.Start();
        //        using (var frameReader = database.GetPointStream(DateTime.MinValue, DateTime.MaxValue).GetFrameReader())
        //        {
        //            while (frameReader.Read())
        //                ;
        //        }
        //        sw.Stop();
        //    }
        //}
        //Console.WriteLine(pointCount);
        //Console.WriteLine(sw.Elapsed.TotalSeconds.ToString());

    }

    //[Test]
    public static void ReadAllPoints()
    {
        Stopwatch sw = new();
        int pointCount = 0;

        HistorianServerDatabaseConfig settings = new("PPA", @"C:\Temp\", true);
        using (HistorianServer server = new(settings))
        {
            DateTime start = DateTime.FromBinary(Convert.ToDateTime("2/1/2014").Date.Ticks + Convert.ToDateTime("6:00:00PM").TimeOfDay.Ticks).ToUniversalTime();

            using HistorianClient client = new("127.0.0.1", 12345);
            using ClientDatabaseBase<HistorianKey, HistorianValue> database = client.GetDatabase<HistorianKey, HistorianValue>(string.Empty);
            HistorianKey key = new();
            HistorianValue value = new();

            sw.Start();
            TreeStream<HistorianKey, HistorianValue> scan = database.Read((ulong)start.Ticks, ulong.MaxValue);
            while (scan.Read(key, value) && pointCount < 10000000)
                pointCount++;
            sw.Stop();

            //sw.Start();
            //using (var frameReader = database.GetPointStream(DateTime.MinValue, DateTime.MaxValue))
            //{
            //    while (frameReader.Read())
            //        ;
            //}
            //sw.Stop();
        }

        Console.WriteLine(pointCount);
        Console.WriteLine(sw.Elapsed.TotalSeconds.ToString());
        Console.WriteLine((pointCount / sw.Elapsed.TotalSeconds / 1000000).ToString());

    }

    [Test]
    public static void ReadAllPointsServer()
    {
    }

    //[Test]
    public void TestReadPoints()
    {
        Stopwatch sw = new();
        int pointCount = 0;

        HistorianKey key = new();
        HistorianValue value = new();

        HistorianServerDatabaseConfig settings = new("PPA", @"C:\Program Files\openHistorian\Archive\", true);

        using (HistorianServer server = new(settings))
        {
            using HistorianClient client = new("127.0.0.1", 12345);
            using ClientDatabaseBase<HistorianKey, HistorianValue> database = client.GetDatabase<HistorianKey, HistorianValue>(string.Empty);

            TreeStream<HistorianKey, HistorianValue> stream = database.Read(0, (ulong)DateTime.MaxValue.Ticks, new ulong[] { 1 });
            while (stream.Read(key, value))
                ;

            sw.Start();
            stream = database.Read(0, (ulong)DateTime.MaxValue.Ticks, new ulong[] { 65, 953, 5562 });

            while (stream.Read(key, value))
                pointCount++;

            sw.Stop();
        }

        Console.WriteLine(pointCount);
        Console.WriteLine(sw.Elapsed.TotalSeconds.ToString());
    }

    //[Test]
    public static void TestReadFilteredPoints()
    {
        throw new NotImplementedException();
    }

    //[Test]
    public void TestReadFilteredPointsAll()
    {
        throw new NotImplementedException();

        //HistorianKey key = new HistorianKey();
        //HistorianValue value = new HistorianValue();
        //Stopwatch sw = new Stopwatch();
        //int pointCount = 0;
        //HistorianDatabaseInstance db = new HistorianDatabaseInstance();
        //db.InMemoryArchive = true;
        //db.ConnectionString = "port=12345";
        //db.Paths = new[] { @"C:\Program Files\openHistorian\Archive\" };

        //var lst = new List<ulong>();
        //for (uint x = 0; x < 6000; x++)
        //{
        //    lst.Add(x);
        //}



        //using (HistorianServer server = new HistorianServer(db))
        //{
        //    SortedTreeClientOptions clientOptions = new SortedTreeClientOptions();
        //    clientOptions.NetworkPort = 12345;
        //    clientOptions.ServerNameOrIp = "127.0.0.1";

        //    //using (var client = new HistorianClient(clientOptions))
        //    //{
        //    var database = server.GetDefaultDatabase();
        //    var stream = database.Read(0, (ulong)DateTime.MaxValue.Ticks, new ulong[] { 1 });
        //    while (stream.Read(key, value))
        //        ;

        //    sw.Start();
        //    stream = database.Read(0, (ulong)DateTime.MaxValue.Ticks, lst);
        //    while (stream.Read(key, value))
        //        pointCount++;

        //    sw.Stop();
        //    //}
        //}
        //Console.WriteLine(pointCount);
        //Console.WriteLine(sw.Elapsed.TotalSeconds.ToString());
        //Console.WriteLine((140107816 / sw.Elapsed.TotalSeconds / 1000000).ToString());
    }


    public static void TestReadPoints2()
    {
        int pointCount = 0;
        HistorianKey key = new();
        HistorianValue value = new();
        HistorianServerDatabaseConfig settings = new("PPA", @"C:\Program Files\openHistorian\Archive\", true);
        using HistorianServer server = new(settings);
        Stopwatch sw = new();
        sw.Start();
        using (HistorianClient client = new("127.0.0.1", 12345))
        using (ClientDatabaseBase<HistorianKey, HistorianValue> database = client.GetDatabase<HistorianKey, HistorianValue>(string.Empty))
        {

            TreeStream<HistorianKey, HistorianValue> stream = database.Read(0, (ulong)DateTime.MaxValue.Ticks, new ulong[] { 65, 953, 5562 });
            while (stream.Read(key, value))
                pointCount++;

        }

        sw.Stop();
        //MessageBox.Show(sw.Elapsed.TotalSeconds.ToString());
    }
}