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
//       Converted code to .NET core.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace openHistorian.Core.UnitTests;

/// <summary>
/// Class to measure garbage collection (GC) and memory allocation times.
/// </summary>
[TestFixture]
public class GCTime
{
    #region [ Members ]

    private class AClass
    {
        #region [ Members ]

        public int Value = 1;

        #endregion
    }

    private class FinalizableClass
    {
        #region [ Members ]

        public readonly int Value = 1;

        #endregion

        #region [ Constructors ]

        ~FinalizableClass()
        {
            if (Value == int.MaxValue)
                Marshal.FreeHGlobal(Marshal.AllocHGlobal(10));
        }

        #endregion
    }

    private readonly List<AClass[]> m_objects = new();
    private readonly List<FinalizableClass[]> m_objects2 = new();

    #endregion

    #region [ Methods ]

    /// <summary>
    /// Runs a test to measure garbage collection and memory allocation times.
    /// </summary>
    [Test]
    public void Test()
    {
        for (int x = 0; x < 100; x++)
            AddItemsAndTime();
    }

    /// <summary>
    /// Runs a test to measure garbage collection and memory allocation times for finalizable classes.
    /// </summary>
    [Test]
    public void Test2()
    {
        for (int x = 0; x < 100; x++)
            AddItemsAndTime2();
    }

    /// <summary>
    /// Adds items to a list, triggers garbage collection, and measures the collection time.
    /// </summary>
    private void AddItemsAndTime()
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
        long memorySize = Process.GetCurrentProcess().VirtualMemorySize64; //GC.GetTotalMemory(false);
        Console.WriteLine("{0}00k items: {1}ms  {2}", m_objects.Count.ToString(), sw.Elapsed.TotalMilliseconds.ToString("0.00"), (memorySize / 1024.0 / 1024.0).ToString("0.0MB"));
    }

    /// <summary>
    /// Adds finalizable items to a list, triggers garbage collection, and measures the collection time.
    /// </summary>
    private void AddItemsAndTime2()
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
        long memorySize = Process.GetCurrentProcess().VirtualMemorySize64; //GC.GetTotalMemory(false);
        //long memorySize = Process.GetCurrentProcess().PrivateMemorySize64;//GC.GetTotalMemory(false);
        Console.WriteLine("{0}00k items: {1}ms  {2}", m_objects2.Count.ToString(), sw.Elapsed.TotalMilliseconds.ToString("0.00"), (memorySize / 1024.0 / 1024.0).ToString("0.0MB"));
    }

    #endregion
}