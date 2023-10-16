//******************************************************************************************************
//  PlotSpeed.cs - Gbtc
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
using openHistorian.UnitTests;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using PlotSurface2D = openHistorian.PerformanceTests.NPlot;
using ST = openHistorian.PerformanceTests.NPlot;

namespace openHistorian.PerformanceTests.NPlot
{
    [TestFixture]
    public class PlotSpeed
    {
        [Test]
        public void RefreshSpeed()
        {
            List<double> xVal = new();
            List<double> yVal = new();

            for (int x = 0; x < 100000; x++)
            {
                xVal.Add(x);
                yVal.Add(1 - x);
            }
            Stopwatch sw = new();
            Stopwatch sw2 = new();
            LinePlot p1 = new LinePlot(yVal, xVal);

            PlotSurface2D plot = new PlotSurface2D(640, 480);

            sw.Start();

            plot.Add(p1);
            plot.Add(p1);
            plot.Add(p1);
            plot.Add(p1);
            plot.Add(p1);
            plot.Add(p1);
            plot.Add(p1);
            plot.Add(p1);
            plot.Add(p1);
            plot.Add(p1);

            sw2.Start();
            plot.Refresh();
            sw2.Stop();
            sw.Stop();

            Console.WriteLine(sw2.Elapsed.TotalSeconds.ToString() + " seconds to refresh");
            Console.WriteLine(sw.Elapsed.TotalSeconds.ToString() + " seconds To add and refresh");
        }

        [Test]
        public void RefreshSpeedTest()
        {
            ST.Reset();

            DebugStopwatch sw = new();
            double time = sw.TimeEvent(RefreshSpeed);
            Console.WriteLine(time.ToString() + " seconds to on average");

            Console.WriteLine(ST.GetResultsPercent());
        }
    }
}