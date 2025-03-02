//******************************************************************************************************
//  ScheduledTask.cs - Gbtc
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

using System;
using System.Diagnostics;
using System.Threading;
using Gemstone;
using Gemstone.Threading;
using NUnit.Framework;

namespace openHistorian.UnitTests.Threading;

[TestFixture]
internal class ScheduledTaskTest
{
    #region [ Members ]

    private int m_doWorkCount;

    #endregion

    #region [ Methods ]

    /// <summary>
    /// Test method that measures execution performance with different threading modes.
    /// </summary>
    [Test]
    public void Test()
    {
        Test(ThreadingMode.DedicatedBackground);
        Test(ThreadingMode.DedicatedForeground);
        Test(ThreadingMode.ThreadPool);
    }

    /// <summary>
    /// Test method that measures timed execution performance with different threading modes.
    /// </summary>
    [Test]
    public void TestTimed()
    {
        TestTimed(ThreadingMode.DedicatedBackground);
        TestTimed(ThreadingMode.DedicatedForeground);
        TestTimed(ThreadingMode.ThreadPool);
    }

    /// <summary>
    /// Test method that measures concurrent execution performance with different threading modes.
    /// </summary>
    [Test]
    public void TestConcurrent()
    {
        TestConcurrent(ThreadingMode.DedicatedBackground);
        TestConcurrent(ThreadingMode.DedicatedForeground);
        TestConcurrent(ThreadingMode.ThreadPool);
    }

    private void Test(ThreadingMode mode)
    {
        const int Count = 1000000000;
        Stopwatch sw = new();
        m_doWorkCount = 0;
        using (ScheduledTask work = new(mode))
        {
            work.Running += work_DoWork;

            sw.Start();
            for (int x = 0; x < 1000; x++)
                work.Start();

            sw.Stop();
        }

        m_doWorkCount = 0;
        sw.Reset();

        using (ScheduledTask work = new(mode))
        {
            work.Running += work_DoWork;

            sw.Start();
            for (int x = 0; x < Count; x++)
                work.Start();

            sw.Stop();
        }

        Console.WriteLine(mode.ToString());
        Console.WriteLine(" Fire Event Count: " + m_doWorkCount);
        Console.WriteLine("  Fire Event Rate: " + (m_doWorkCount / sw.Elapsed.TotalSeconds / 1000000).ToString("0.00"));
        Console.WriteLine(" Total Calls Time: " + sw.Elapsed.TotalMilliseconds.ToString("0.0") + "ms");
        Console.WriteLine(" Total Calls Rate: " + (Count / sw.Elapsed.TotalSeconds / 1000000).ToString("0.00"));
        Console.WriteLine();
    }

    private void TestTimed(ThreadingMode mode)
    {
        const int Count = 1000000000;
        Stopwatch sw = new();
        m_doWorkCount = 0;
        using (ScheduledTask work = new(mode))
        {
            work.Running += work_DoWork;

            sw.Start();
            for (int x = 0; x < 1000; x++)
            {
                work.Start(1);
                work.Start();
            }

            sw.Stop();
        }

        m_doWorkCount = 0;
        sw.Reset();

        using (ScheduledTask work = new(mode))
        {
            work.Running += work_DoWork;

            sw.Start();
            for (int x = 0; x < Count; x++)
            {
                work.Start(1000);
                work.Start();
            }

            sw.Stop();
        }

        Console.WriteLine(mode.ToString());
        Console.WriteLine(" Fire Event Count: " + m_doWorkCount);
        Console.WriteLine("  Fire Event Rate: " + (m_doWorkCount / sw.Elapsed.TotalSeconds / 1000000).ToString("0.00"));
        Console.WriteLine(" Total Calls Time: " + sw.Elapsed.TotalMilliseconds.ToString("0.0") + "ms");
        Console.WriteLine(" Total Calls Rate: " + (Count / sw.Elapsed.TotalSeconds / 1000000).ToString("0.00"));
        Console.WriteLine();
    }

    private void TestConcurrent(ThreadingMode mode)
    {
        int workCount;

        const int Count = 100000000;
        Stopwatch sw = new();
        m_doWorkCount = 0;
        using (ScheduledTask work = new(mode))
        {
            work.Running += work_DoWork;

            sw.Start();
            for (int x = 0; x < 1000; x++)
                work.Start();

            sw.Stop();
        }

        m_doWorkCount = 0;
        sw.Reset();

        using (ScheduledTask work = new(mode))
        {
            work.Running += work_DoWork;


            sw.Start();
            ThreadPool.QueueUserWorkItem(BlastStartMethod, work);
            ThreadPool.QueueUserWorkItem(BlastStartMethod, work);

            for (int x = 0; x < Count; x++)
                work.Start();
            workCount = m_doWorkCount;
            sw.Stop();
            Thread.Sleep(100);
        }

        Console.WriteLine(mode.ToString());
        Console.WriteLine(" Fire Event Count: " + workCount);
        Console.WriteLine("  Fire Event Rate: " + (workCount / sw.Elapsed.TotalSeconds / 1000000).ToString("0.00"));
        Console.WriteLine(" Total Calls Time: " + sw.Elapsed.TotalMilliseconds.ToString("0.0") + "ms");
        Console.WriteLine(" Total Calls Rate: " + (Count / sw.Elapsed.TotalSeconds / 1000000).ToString("0.00"));
        Console.WriteLine();
    }

    private void BlastStartMethod(object obj)
    {
        try
        {
            ScheduledTask task = (ScheduledTask)obj;
            const int Count = 100000000;
            for (int x = 0; x < Count; x++)
                task.Start();
        }
        catch (Exception)
        {
        }
    }


    private void work_DoWork(object sender, EventArgs<ScheduledTaskRunningReason> eventArgs)
    {
        m_doWorkCount++;
    }

    #endregion
}