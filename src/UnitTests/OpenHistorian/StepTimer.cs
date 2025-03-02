//******************************************************************************************************
//  StepTimer.cs - Gbtc
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

using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace openHistorian.UnitTests;

/// <summary>
/// Provides a simple utility for measuring and tracking time intervals using Stopwatches.
/// </summary>
public static class StepTimer
{
    #region [ Static ]

    private static readonly Dictionary<string, Stopwatch> AllStopwatches;

    /// <summary>
    /// Initializes the StepTimer class and the stopwatch dictionary.
    /// </summary>
    static StepTimer()
    {
        AllStopwatches = new Dictionary<string, Stopwatch>();
    }

    /// <summary>
    /// Starts a named Stopwatch or creates a new one if it doesn't exist.
    /// </summary>
    /// <param name="name">The name of the Stopwatch.</param>
    /// <returns>The started Stopwatch instance.</returns>
    public static Stopwatch Start(string name)
    {
        if (!AllStopwatches.ContainsKey(name))
            AllStopwatches.Add(name, new Stopwatch());
        Stopwatch sw = AllStopwatches[name];
        sw.Start();
        return sw;
    }

    /// <summary>
    /// Stops a given Stopwatch.
    /// </summary>
    /// <param name="sw">The Stopwatch to stop.</param>
    public static void Stop(Stopwatch sw)
    {
        sw.Stop();
    }

    /// <summary>
    /// Resets and clears all the Stopwatches stored in the dictionary.
    /// </summary>
    public static void Reset()
    {
        AllStopwatches.Clear();
    }

    /// <summary>
    /// Retrieves the results of all the recorded time intervals as a formatted string.
    /// </summary>
    /// <returns>A string containing time interval results with names.</returns>
    public static string GetResults()
    {
        StringBuilder sb = new();
        foreach (KeyValuePair<string, Stopwatch> kvp in AllStopwatches)
            sb.Append(kvp.Key + '\t' + kvp.Value.Elapsed.TotalMilliseconds);
        return sb.ToString();
    }

    #endregion
}