//******************************************************************************************************
//  SocketBenchmark.cs - Gbtc
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
using openHistorian.Net;
using openHistorian.Snap;
using openHistorian.Snap.Definitions;
using openHistorian.UnitTests;
using SnapDB.Snap;
using SnapDB.Snap.Services;
using SnapDB.Snap.Services.Reader;
using SnapDB.Snap.Storage;
using SnapDB.Snap.Tree;
using System;
using System.IO;

namespace SampleCode.openHistorian.Server.dll;

[TestFixture]
public class SocketBenchmark
{
    [Test]
    public void CreateScadaDatabase()
    {
        throw new NotImplementedException();

        //Array.ForEach(Directory.GetFiles(@"c:\temp\Scada\", "*.d2", SearchOption.AllDirectories), File.Delete);

        //HistorianDatabaseInstance db = new HistorianDatabaseInstance();
        //db.IsNetworkHosted = false;
        //db.InMemoryArchive = false;
        //db.Paths = new[] { @"c:\temp\Scada\" };

        //HistorianKey key = new HistorianKey();
        //HistorianValue value = new HistorianValue();

        //using (HistorianServer server = new HistorianServer(db))
        //{
        //    ServerDatabaseBase database = server.GetDefaultDatabase();

        //    for (ulong x = 0; x < 10000000; x++)
        //    {
        //        key.Timestamp = x;
        //        database.Write(key, value);
        //    }

        //    database.HardCommit();
        //}

        //Console.WriteLine("KeyMethodsBase calls");
        //for (int x = 0; x < 23; x++)
        //{
        //    Console.WriteLine(TreeKeyMethodsBase<HistorianKey>.CallMethods[x] + "\t" + ((TreeKeyMethodsBase<HistorianKey>.Method)(x)).ToString());
        //}
        //Console.WriteLine("ValueMethodsBase calls");
        //for (int x = 0; x < 5; x++)
        //{
        //    Console.WriteLine(TreeValueMethodsBase<HistorianValue>.CallMethods[x] + "\t" + ((TreeValueMethodsBase<HistorianValue>.Method)(x)).ToString());
        //}
    }

    [Test]
    public void BenchmarkWriteSpeed()
    {
        DebugStopwatch sw = new DebugStopwatch();

        double time;
        double count = 0;

        using (SortedTreeFile file = SortedTreeFile.CreateInMemory())
        {
            SortedTreeTable<HistorianKey, HistorianValue> table = file.OpenOrCreateTable<HistorianKey, HistorianValue>(HistorianFileEncodingDefinition.TypeGuid);
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();

            time = sw.TimeEvent(() =>
            {
                //TreeKeyMethodsBase<HistorianKey>.ClearStats();
                //TreeValueMethodsBase<HistorianKey>.ClearStats();
                count = 0;
                using (SortedTreeTableEditor<HistorianKey, HistorianValue> scan = table.BeginEdit())
                {
                    for (uint x = 0; x < 10000000; x++)
                    {
                        key.PointID = x;
                        scan.AddPoint(key, value);
                        count++;
                    }
                    scan.Rollback();
                }
            });
        }

        Console.WriteLine((count / 1000000 / time).ToString() + " Million PPS");

        //Console.WriteLine("KeyMethodsBase calls");
        //for (int x = 0; x < 23; x++)
        //{
        //    Console.WriteLine(TreeKeyMethodsBase<HistorianKey>.CallMethods[x] + "\t" + ((TreeKeyMethodsBase<HistorianKey>.Method)(x)).ToString());
        //}
        //Console.WriteLine("ValueMethodsBase calls");
        //for (int x = 0; x < 5; x++)
        //{
        //    Console.WriteLine(TreeValueMethodsBase<HistorianValue>.CallMethods[x] + "\t" + ((TreeValueMethodsBase<HistorianValue>.Method)(x)).ToString());
        //}
    }

    [Test]
    public void TestReadData()
    {
        HistorianKey key = new HistorianKey();
        HistorianValue value = new HistorianValue();

        HistorianServerDatabaseConfig settings = new HistorianServerDatabaseConfig("PPA", @"c:\temp\Scada\", true);
        using (HistorianServer server = new HistorianServer(settings))
        {
            double count = 0;

            DebugStopwatch sw = new DebugStopwatch();
            double time = sw.TimeEvent(() =>
            {
                count = 0;
                using (HistorianClient client = new HistorianClient("127.0.0.1", 12345))
                using (ClientDatabaseBase<HistorianKey, HistorianValue> database = client.GetDatabase<HistorianKey, HistorianValue>(String.Empty))
                {
                    //IHistorianDatabase<HistorianKey, HistorianValue> database = server.GetDefaultDatabase();//.GetDatabase();
                    //TreeStream<HistorianKey, HistorianValue> stream = reader.Read(0, ulong.MaxValue, new ulong[] { 2 });
                    TreeStream<HistorianKey, HistorianValue> stream = database.Read(0, ulong.MaxValue);
                    while (stream.Read(key, value))
                    {
                        count++;
                    }
                }
            });

            Console.WriteLine((count / 1000000 / time).ToString() + " Million PPS");
        }

        //Console.WriteLine("KeyMethodsBase calls");
        //for (int x = 0; x < 23; x++)
        //{
        //    Console.WriteLine(TreeKeyMethodsBase<HistorianKey>.CallMethods[x] + "\t" + ((TreeKeyMethodsBase<HistorianKey>.Method)(x)).ToString());
        //}
        //Console.WriteLine("ValueMethodsBase calls");
        //for (int x = 0; x < 5; x++)
        //{
        //    Console.WriteLine(TreeValueMethodsBase<HistorianValue>.CallMethods[x] + "\t" + ((TreeValueMethodsBase<HistorianValue>.Method)(x)).ToString());
        //}
        //for (int x = 0; x < 15; x++)
        //{
        //    Console.WriteLine(BinaryStreamBase.CallMethods[x] + "\t" + ((BinaryStreamBase.Method)(x)).ToString());
        //}
    }

    [Test]
    public void TestReadDataFromArchive()
    {

        DebugStopwatch sw = new DebugStopwatch();
        HistorianKey key = new HistorianKey();
        HistorianValue value = new HistorianValue();

        string path = Directory.GetFiles(@"c:\temp\Scada\", "*.d2")[0];
        double time;
        double count = 0;

        using (SortedTreeFile file = SortedTreeFile.OpenFile(path, true))
        {
            SortedTreeTable<HistorianKey, HistorianValue> table = file.OpenTable<HistorianKey, HistorianValue>();

            time = sw.TimeEvent(() =>
            {
                count = 0;
                using (SortedTreeTableReadSnapshot<HistorianKey, HistorianValue> scan = table.BeginRead())
                {
                    SortedTreeScannerBase<HistorianKey, HistorianValue> t = scan.GetTreeScanner();
                    t.SeekToStart();
                    while (t.Read(key, value))
                    {
                        count++;
                    }
                }
            });
        }
        Console.WriteLine((count / 1000000 / time).ToString() + " Million PPS");

        //Console.WriteLine("KeyMethodsBase calls");
        //for (int x = 0; x < 23; x++)
        //{
        //    Console.WriteLine(TreeKeyMethodsBase<HistorianKey>.CallMethods[x] + "\t" + ((TreeKeyMethodsBase<HistorianKey>.Method)(x)).ToString());
        //}
        //Console.WriteLine("ValueMethodsBase calls");
        //for (int x = 0; x < 5; x++)
        //{
        //    Console.WriteLine(TreeValueMethodsBase<HistorianValue>.CallMethods[x] + "\t" + ((TreeValueMethodsBase<HistorianValue>.Method)(x)).ToString());
        //}
    }
}