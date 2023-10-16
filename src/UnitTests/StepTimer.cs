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

public static class StepTimer
{
    private static readonly Dictionary<string, Stopwatch> AllStopwatches;

    static StepTimer()
    {
        AllStopwatches = new Dictionary<string, Stopwatch>();
    }

    public static Stopwatch Start(string name)
    {
        if (!AllStopwatches.ContainsKey(name))
        {
            AllStopwatches.Add(name, new Stopwatch());
        }
        Stopwatch sw = AllStopwatches[name];
        sw.Start();
        return sw;
    }

    public static void Stop(Stopwatch sw)
    {
        sw.Stop();
    }

    public static void Reset()
    {
        AllStopwatches.Clear();
    }

    public static string GetResults()
    {
        StringBuilder sb = new();
        foreach (KeyValuePair<string, Stopwatch> kvp in AllStopwatches)
        {
            sb.Append(kvp.Key + '\t' + kvp.Value.Elapsed.TotalMilliseconds.ToString());
        }
        return sb.ToString();
    }
}