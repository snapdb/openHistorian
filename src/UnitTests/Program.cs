//******************************************************************************************************
//  Program.cs - Gbtc
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

using openHistorian.UnitTests;
using System;

namespace openHistorian.PerformanceTests
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            //var m = new MeasureCompression();
            //m.Test();

            GCTime GCT = new GCTime();
            //GCT.Test();
            GCT.Test2();



            //var tl = new TinyLock_Test();
            //tl.TestTinyLock_Lock();
            //tl.TestMonitor();

            //var hl = new HalfLock_Test();
            //hl.TestTinyLock_Lock();
            Console.ReadLine();


            //var st = new ThreadContainerBase_Test();
            ////st.TestTimed();
            //st.Test();


            //var tree = new SortedTree256Test();
            //tree.SortedTree256Archive();
            //ReadPoints.TestReadPoints2();
            //ReadPoints.ReadAllPoints();
            //ReadPoints.TestReadFilteredPoints();

            //Console.ReadLine();
        }
    }
}