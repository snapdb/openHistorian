//******************************************************************************************************
//  BinaryStreamBenchmark.cs - Gbtc
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
using SnapDB.IO.Unmanaged;
using System;

namespace openHistorian.UnitTests.IO;

[TestFixture]
public unsafe class BinaryStreamBenchmark
{
    [Test]
    public void Test7Bit1()
    {
        byte[] data = new byte[4096 * 5];
        fixed (byte* lp = data)
        {
            using BinaryStreamPointerWrapper bs = new(lp, data.Length);
            DebugStopwatch sw = new();
            double time = sw.TimeEventMedian(() =>
            {
                for (int repeat = 0; repeat < 1000; repeat++)
                {
                    bs.Position = 0;
                    for (int x = 0; x < 1000; x++)
                    {
                        bs.Write7Bit(1u);
                        bs.Write7Bit(1u);
                        bs.Write7Bit(1u);
                        bs.Write7Bit(1u);
                    }
                }

            });
            Console.WriteLine(4 * 1000 * 1000 / time / 1000 / 1000);
        }
    }
    [Test]
    public void Test7Bit2()
    {
        byte[] data = new byte[4096 * 5];
        fixed (byte* lp = data)
        {
            using BinaryStreamPointerWrapper bs = new(lp, data.Length);
            DebugStopwatch sw = new();
            double time = sw.TimeEventMedian(() =>
            {
                for (int repeat = 0; repeat < 1000; repeat++)
                {
                    bs.Position = 0;
                    for (int x = 0; x < 1000; x++)
                    {
                        bs.Write7Bit(128u);
                        bs.Write7Bit(128u);
                        bs.Write7Bit(128u);
                        bs.Write7Bit(128u);
                    }
                }

            });
            Console.WriteLine(4 * 1000 * 1000 / time / 1000 / 1000);
        }
    }

    [Test]
    public void Test7Bit3()
    {
        byte[] data = new byte[4096 * 5];
        fixed (byte* lp = data)
        {
            using BinaryStreamPointerWrapper bs = new(lp, data.Length);
            DebugStopwatch sw = new();
            double time = sw.TimeEventMedian(() =>
            {
                for (int repeat = 0; repeat < 1000; repeat++)
                {
                    bs.Position = 0;
                    for (int x = 0; x < 1000; x++)
                    {
                        bs.Write7Bit(128u * 128u);
                        bs.Write7Bit(128u * 128u);
                        bs.Write7Bit(128u * 128u);
                        bs.Write7Bit(128u * 128u);
                    }
                }

            });
            Console.WriteLine(4 * 1000 * 1000 / time / 1000 / 1000);
        }
    }

    [Test]
    public void Test7Bit4()
    {
        byte[] data = new byte[4096 * 5];
        fixed (byte* lp = data)
        {
            using BinaryStreamPointerWrapper bs = new(lp, data.Length);
            DebugStopwatch sw = new();
            double time = sw.TimeEventMedian(() =>
            {
                for (int repeat = 0; repeat < 1000; repeat++)
                {
                    bs.Position = 0;
                    for (int x = 0; x < 1000; x++)
                    {
                        bs.Write7Bit(128u * 128u * 128u);
                        bs.Write7Bit(128u * 128u * 128u);
                        bs.Write7Bit(128u * 128u * 128u);
                        bs.Write7Bit(128u * 128u * 128u);
                    }
                }

            });
            Console.WriteLine(4 * 1000 * 1000 / time / 1000 / 1000);
        }
    }

    [Test]
    public void Test7Bit5()
    {
        byte[] data = new byte[4096 * 6];
        fixed (byte* lp = data)
        {
            using BinaryStreamPointerWrapper bs = new(lp, data.Length);
            DebugStopwatch sw = new();
            double time = sw.TimeEventMedian(() =>
            {
                for (int repeat = 0; repeat < 1000; repeat++)
                {
                    bs.Position = 0;
                    for (int x = 0; x < 1000; x++)
                    {
                        bs.Write7Bit(uint.MaxValue);
                        bs.Write7Bit(uint.MaxValue);
                        bs.Write7Bit(uint.MaxValue);
                        bs.Write7Bit(uint.MaxValue);
                    }
                }

            });
            Console.WriteLine(4 * 1000 * 1000 / time / 1000 / 1000);
        }
    }


    [Test]
    public void TestWriteByte()
    {
        byte[] data = new byte[4096];
        fixed (byte* lp = data)
        {
            using BinaryStreamPointerWrapper bs = new(lp, data.Length);
            DebugStopwatch sw = new();
            double time = sw.TimeEventMedian(() =>
            {
                for (int repeat = 0; repeat < 1000; repeat++)
                {
                    bs.Position = 0;
                    for (int x = 0; x < 1000; x++)
                    {
                        bs.Write((sbyte)x);
                        bs.Write((sbyte)x);
                        bs.Write((sbyte)x);
                        bs.Write((sbyte)x);
                    }
                }

            });

            Console.WriteLine(4 * 1000 * 1000 / time / 1000 / 1000);
        }
    }

    [Test]
    public void TestWriteShort()
    {
        byte[] data = new byte[4096 * 2];
        fixed (byte* lp = data)
        {
            using BinaryStreamPointerWrapper bs = new(lp, data.Length);
            DebugStopwatch sw = new();
            double time = sw.TimeEventMedian(() =>
            {
                for (int repeat = 0; repeat < 1000; repeat++)
                {
                    bs.Position = 0;
                    for (int x = 0; x < 1000; x++)
                    {
                        bs.Write((short)x);
                        bs.Write((short)x);
                        bs.Write((short)x);
                        bs.Write((short)x);
                    }
                }

            });

            Console.WriteLine(4 * 1000 * 1000 / time / 1000 / 1000);
        }
    }

    [Test]
    public void TestWriteInt()
    {
        byte[] data = new byte[4096 * 4];
        fixed (byte* lp = data)
        {
            using BinaryStreamPointerWrapper bs = new(lp, data.Length);
            DebugStopwatch sw = new();
            double time = sw.TimeEventMedian(() =>
            {
                for (int repeat = 0; repeat < 1000; repeat++)
                {
                    bs.Position = 0;
                    for (int x = 0; x < 1000; x++)
                    {
                        bs.Write(x);
                        bs.Write(x);
                        bs.Write(x);
                        bs.Write(x);
                    }
                }

            });

            Console.WriteLine(4 * 1000 * 1000 / time / 1000 / 1000);
        }
    }

    [Test]
    public void TestWriteLong()
    {
        byte[] data = new byte[4096 * 8];
        fixed (byte* lp = data)
        {
            using BinaryStreamPointerWrapper bs = new(lp, data.Length);
            DebugStopwatch sw = new();
            double time = sw.TimeEventMedian(() =>
            {
                for (int repeat = 0; repeat < 1000; repeat++)
                {
                    bs.Position = 0;
                    for (int x = 0; x < 1000; x++)
                    {
                        bs.Write((long)x);
                        bs.Write((long)x);
                        bs.Write((long)x);
                        bs.Write((long)x);
                    }
                }

            });

            Console.WriteLine(4 * 1000 * 1000 / time / 1000 / 1000);
        }
    }

}
