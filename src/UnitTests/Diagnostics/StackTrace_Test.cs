//******************************************************************************************************
//  StackTrace_Test.cs - Gbtc
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

using Gemstone.Diagnostics;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Text;

namespace openHistorian.UnitTests.Diagnostics
{
    [TestFixture]
    public class StackTrace_Test
    {
        [Test]
        public void Test()
        {
            Console.WriteLine(Environment.StackTrace);

            RunMethod();
            DebugStopwatch sw = new DebugStopwatch();
            double time = sw.TimeEvent(() =>
            {
                for (int x = 0; x < 1000; x++)
                {
                    RunMethod3();
                }
            });
            Console.WriteLine(1000 / time);
        }

        void RunMethod()
        {
            string str = Environment.StackTrace;
            if (str is null)
                throw new Exception();
        }
        void RunMethod2()
        {
            StackTrace st = new StackTrace(true);
            StackFrame[] frames = st.GetFrames();

            StringBuilder sb = new StringBuilder();
            foreach (StackFrame frame in frames)
            {
                sb.AppendLine(frame.GetMethod().Name);
                sb.AppendLine(frame.GetMethod().Module.Assembly.FullName);
                sb.AppendLine(frame.GetFileName());
                sb.AppendLine(frame.GetFileLineNumber().ToString());
            }

            if (frames.Length == 0)
                throw new Exception();
        }
        void RunMethod3()
        {
            _ = new LogStackTrace();
        }


    }
}
