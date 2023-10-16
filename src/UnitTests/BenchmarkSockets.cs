//******************************************************************************************************
//  BenchmarkSockets.cs - Gbtc
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
//  10/16/2023 - Lillian Gensolin
//       Converted code to .NET core
//
//******************************************************************************************************

using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace openHistorian.UnitTests
{
    [TestFixture]
    public class BenchmarkSockets
    {
        private const int Loop = 10000;

        [Test]
        public void Run()
        {
            Thread read = new(Reader);
            read.IsBackground = true;

            TcpListener listen = new(IPAddress.Parse("127.0.0.1"), 36345);
            listen.Start();

            Thread.Sleep(100);
            read.Start();

            TcpClient client = listen.AcceptTcpClient();

            byte[] data = new byte[154600];
            Stopwatch sw = new();

            NetworkStream stream = client.GetStream();

            sw.Start();
            for (int x = 0; x < Loop; x++)
                stream.Write(data, 0, data.Length);
            sw.Stop();
            stream.Close();

            Console.WriteLine("Write: " + (Loop * data.Length / sw.Elapsed.TotalSeconds / 1000000).ToString());
            Thread.Sleep(1000);

            read.Join();
        }


        void Reader()
        {

            TcpClient client = new();
            client.Connect("127.0.0.1", 36345);

            Stopwatch sw = new();
            NetworkStream stream = client.GetStream();
            byte[] data = new byte[154600];
            sw.Start();
            while (stream.Read(data, 0, data.Length) > 0)
                ;
            sw.Stop();

            Console.WriteLine("Read: " + (Loop * data.Length / sw.Elapsed.TotalSeconds / 1000000).ToString());
        }
    }
}
