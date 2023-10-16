//******************************************************************************************************
//  GCTime.cs - Gbtc
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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace openHistorian.UnitTests;

[TestFixture]
public class GCTime
{
    private readonly List<AClass[]> m_objects = new();
    private readonly List<FinalizableClass[]> m_objects2 = new();

    [Test]
    public void Test()
    {
        for (int x = 0; x < 100; x++)
            AddItemsAndTime();
    }

    void AddItemsAndTime()
    {
        AClass[] array = new AClass[100000];
        for (int x = 0; x < array.Length; x++)
            array[x] = new AClass();

        m_objects.Add(array);
        GC.Collect();
        GC.WaitForPendingFinalizers();

        GC.Collect();
        GC.WaitForPendingFinalizers();

        GC.Collect();
        GC.WaitForPendingFinalizers();

        GC.Collect();
        GC.WaitForPendingFinalizers();

        Stopwatch sw = new();
        AClass swap = m_objects[0][0];
        m_objects[0][0] = m_objects[0][1];
        m_objects[0][1] = swap;
        m_objects[0][m_objects.Count] = null;

        sw.Start();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        sw.Stop();
        long memorySize = Process.GetCurrentProcess().VirtualMemorySize64;//GC.GetTotalMemory(false);
        Console.WriteLine("{0}00k items: {1}ms  {2}", m_objects.Count.ToString(), sw.Elapsed.TotalMilliseconds.ToString("0.00"), (memorySize / 1024.0 / 1024.0).ToString("0.0MB"));
    }

    private class AClass
    {
        public int Value = 1;
    }

    private class FinalizableClass
    {
        public readonly int Value = 1;

        ~FinalizableClass()
        {
            if (Value == int.MaxValue)
            {
                Marshal.FreeHGlobal(Marshal.AllocHGlobal(10));
            }
        }
    }

    [Test]
    public void Test2()
    {
        for (int x = 0; x < 100; x++)
            AddItemsAndTime2();
    }

    void AddItemsAndTime2()
    {
        FinalizableClass[] array = new FinalizableClass[100000];
        for (int x = 0; x < array.Length; x++)
            array[x] = new FinalizableClass();

        m_objects2.Add(array);
        GC.Collect();
        GC.WaitForPendingFinalizers();

        GC.Collect();
        GC.WaitForPendingFinalizers();

        GC.Collect();
        GC.WaitForPendingFinalizers();

        GC.Collect();
        GC.WaitForPendingFinalizers();

        Stopwatch sw = new();
        FinalizableClass swap = m_objects2[0][0];
        m_objects2[0][0] = m_objects2[0][1];
        m_objects2[0][1] = swap;
        m_objects2[0][m_objects2.Count] = null;

        sw.Start();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        sw.Stop();
        long memorySize = Process.GetCurrentProcess().VirtualMemorySize64;//GC.GetTotalMemory(false);
        //long memorySize = Process.GetCurrentProcess().PrivateMemorySize64;//GC.GetTotalMemory(false);
        Console.WriteLine("{0}00k items: {1}ms  {2}", m_objects2.Count.ToString(), sw.Elapsed.TotalMilliseconds.ToString("0.00"), (memorySize / 1024.0 / 1024.0).ToString("0.0MB"));
    }

}
