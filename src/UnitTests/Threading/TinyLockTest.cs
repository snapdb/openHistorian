//******************************************************************************************************
//  Stats.cs - Gbtc
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
//  12/19/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//  09/21/2023 - Lillian Gensolin
//       Converted code to .NET core.
//
//******************************************************************************************************

using System;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using SnapDB.Threading;

namespace openHistorian.UnitTests.Threading;

[TestFixture]
public class TinyLock_Test
{
    [Test]
    public void TestMonitor()
    {
        const int count = 100000000;
        Stopwatch sw = new();
        sw.Start();
        object obj = new();

        for (int x = 0; x < count; x++)
        {
            lock (obj) ;
            lock (obj) ;
            lock (obj) ;
            lock (obj) ;
            lock (obj) ;
            lock (obj) ;
            lock (obj) ;
            lock (obj) ;
            lock (obj) ;
            lock (obj) ;
        }
        sw.Stop();

        Console.WriteLine(count * 10.0 / sw.Elapsed.TotalSeconds / 1000000);
    }

    [Test]
    public void TestTinyLock_Lock()
    {
        TinyLock tl = new();
        const int count = 100000000;
        Stopwatch sw = new();
        sw.Start();

        for (int x = 0; x < count; x++)
        {
            using (tl.Lock()) ;
            using (tl.Lock()) ;
            using (tl.Lock()) ;
            using (tl.Lock()) ;
            using (tl.Lock()) ;
            using (tl.Lock()) ;
            using (tl.Lock()) ;
            using (tl.Lock()) ;
            using (tl.Lock()) ;
            using (tl.Lock()) ;

        }
        sw.Stop();

        Console.WriteLine(count * 10.0 / sw.Elapsed.TotalSeconds / 1000000);
    }

    private ManualResetEvent m_event;
    private TinyLock m_sync;
    private long m_value;
    private const long max = 100000000;

    [Test]
    public void TestContention()
    {
        m_value = 0;
        m_sync = new TinyLock();
        m_event = new ManualResetEvent(true);

        for (int x = 0; x < 16; x++)
            ThreadPool.QueueUserWorkItem(Adder);

        Thread.Sleep(100);
        m_event.Set();

        while (m_value < 16 * max)
        {
            Console.WriteLine(m_value);
            Thread.Sleep(1000);
        }

        Console.WriteLine(m_value);
    }

    public void Adder(object obj)
    {
        m_event.WaitOne();
        for (int x = 0; x < max; x++)
        {
            using (m_sync.Lock())
                m_value++;
        }
    }
}