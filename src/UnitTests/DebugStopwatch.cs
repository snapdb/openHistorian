//******************************************************************************************************
//  DebugStopwatch.cs - Gbtc
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
using System.Runtime;
using NUnit.Framework;

namespace openHistorian.UnitTests;

/// <summary>
/// Utility class for measuring and asserting execution time of methods or code blocks.
/// </summary>
public class DebugStopwatch
{
    #region [ Members ]

    private readonly Stopwatch sw;

    #endregion

    #region [ Constructors ]

    /// <summary>
    /// Initializes a new instance of the <see cref="DebugStopwatch"/> class.
    /// </summary>
    public DebugStopwatch()
    {
        GCSettings.LatencyMode = GCLatencyMode.Batch;
        sw = new Stopwatch();
    }

    #endregion

    #region [ Methods ]

    /// <summary>
    /// Forces a garbage collection and waits for pending finalizers.
    /// </summary>
    public void DoGC()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    /// <summary>
    /// Starts the stopwatch and optionally performs a garbage collection.
    /// </summary>
    /// <param name="skipCollection">True to skip garbage collection, false to collect before starting the stopwatch.</param>
    public void Start(bool skipCollection = false)
    {
        if (skipCollection)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        sw.Restart();
    }

    /// <summary>
    /// Stops the stopwatch and asserts that the elapsed time is within a specified maximum time.
    /// </summary>
    /// <param name="maximumTime">The maximum allowed time in milliseconds.</param>
    public void Stop(double maximumTime)
    {
        sw.Stop();
        Assert.IsTrue(sw.Elapsed.TotalMilliseconds <= maximumTime);
    }

    /// <summary>
    /// Stops the stopwatch and asserts that the elapsed time is within a specified range of time.
    /// </summary>
    /// <param name="minimumTime">The minimum allowed time in milliseconds.</param>
    /// <param name="maximumTime">The maximum allowed time in milliseconds.</param>
    public void Stop(double minimumTime, double maximumTime)
    {
        sw.Stop();
        Assert.IsTrue(sw.Elapsed.TotalMilliseconds >= minimumTime);
        Assert.IsTrue(sw.Elapsed.TotalMilliseconds <= maximumTime);
    }

    /// <summary>
    /// Measures the time taken to execute an action and returns the average execution time.
    /// </summary>
    /// <param name="function">The action to be executed and measured.</param>
    /// <returns>The average execution time in seconds.</returns>
    public double TimeEvent(Action function)
    {
        GC.Collect();
        function();
        int count = 0;
        sw.Reset();

        while (sw.Elapsed.TotalSeconds < .25)
        {
            sw.Start();
            function();
            sw.Stop();
            count++;
        }

        return sw.Elapsed.TotalSeconds / count;
    }

    /// <summary>
    /// Measures the time taken to execute an action multiple times and returns the median execution time.
    /// </summary>
    /// <param name="function">The action to be executed and measured.</param>
    /// <returns>The median execution time in seconds.</returns>
    public double TimeEventMedian(Action function)
    {
        List<double> values = new();
        GC.Collect();
        function();
        
        //int count = 0;
        
        Stopwatch swTotal = new();
        swTotal.Start();
        
        while (swTotal.Elapsed.TotalSeconds < 1 && values.Count < 100)
        {
            sw.Restart();
            function();
            sw.Stop();
            values.Add(sw.Elapsed.TotalSeconds);
        }

        return values[(values.Count - 1) >> 1];
    }

    #endregion
}