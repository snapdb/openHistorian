﻿//******************************************************************************************************
//  StreamWriterTest.cs - Gbtc
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
//       Convert code to .NET core.
//
//******************************************************************************************************

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using SnapDB;

namespace openHistorian.UnitTests;

[TestFixture]
internal class StreamWriterTest
{
    #region [ Members ]

    /// <summary>
    /// Implements methods to write to the stream.
    /// </summary>
    public class UltraStreamWriter
    {
        #region [ Members ]

        private const int FlushSize = 1024 - 40;
        private const int Size = 1024;
        private readonly char[] m_buffer;
        private int m_position;
        private readonly StreamWriter m_stream;
        private readonly string nl = Environment.NewLine;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the UltraStreamWriter class with the specified StreamWriter.
        /// </summary>
        /// <param name="stream">The underlying StreamWriter to write to.</param>
        public UltraStreamWriter(StreamWriter stream)
        {
            m_buffer = new char[Size];
            m_stream = stream;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Writes a character to the stream buffer.
        /// </summary>
        /// <param name="value">The character to write.</param>
        public void Write(char value)
        {
            if (m_position < FlushSize)
                Flush();
            m_buffer[m_position] = value;
        }

        /// <summary>
        /// Writes a float to the stream buffer.
        /// </summary>
        /// <param name="value">The float to write.</param>
        public void Write(float value)
        {
            if (m_position < FlushSize)
                Flush();
            m_position += value.WriteToChars(m_buffer, m_position);
        }

        /// <summary>
        /// Writes a new line to the stream buffer.
        /// </summary>
        public void WriteLine()
        {
            if (m_position < FlushSize)
                Flush();
            if (nl.Length == 2)
            {
                m_buffer[m_position] = nl[0];
                m_buffer[m_position + 1] = nl[1];
                m_position += 2;
            }
            else
            {
                m_buffer[m_position] = nl[0];
                m_position += 2;
            }
        }

        /// <summary>
        /// Flushes the stream buffer to the underlying stream writer.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Flush()
        {
            if (m_position > 0)
                m_stream.Write(m_buffer, 0, m_position);
            m_position = 0;
        }

        #endregion
    }

    private const float FloatToConvert = 2263.1234f;

    private const uint IntToConvert = 2214352634u;

    #endregion

    #region [ Methods ]

    /// <summary>
    /// Tests the original writing process using a StreamWriter with direct writing of float values to a file.
    /// </summary>
    [Test]
    public void TestOrig()
    {
        using StreamWriter csvStream = new("C:\\Temp\\file.csv");
        Stopwatch sw = new();
        sw.Start();
        for (float x = 0.216421654f; x < 2000000; x++)
        {
            csvStream.Write(x);
            csvStream.Write(',');
        }

        sw.Stop();
        Console.WriteLine(sw.Elapsed.TotalSeconds);
    }

    /// <summary>
    /// Tests an optimized approach that uses the formatting functionality of the StreamWriter for writing float values to a file.
    /// </summary>
    [Test]
    public void TestOpt1()
    {
        using StreamWriter csvStream = new("C:\\temp\\file.csv");
        IFormatProvider format = csvStream.FormatProvider;
        NumberFormatInfo info = NumberFormatInfo.GetInstance(format);

        Stopwatch sw = new();
        sw.Start();
        for (float x = 0.216421654f; x < 2000000; x++)
        {
            csvStream.Write(x.ToString(format)); //Number.FormatSingle(x, null, info));
            csvStream.Write(',');
        }

        sw.Stop();
        Console.WriteLine(sw.Elapsed.TotalSeconds);
    }


    /// <summary>
    /// Tests an optimized approach using a custom UltraStreamWriter for writing float values to a file.
    /// </summary>
    [Test]
    public void TestOpt3()
    {
        using StreamWriter csvStream = new("C:\\temp\\file.csv");
        UltraStreamWriter usw = new(csvStream);
        IFormatProvider format = csvStream.FormatProvider;
        NumberFormatInfo info = NumberFormatInfo.GetInstance(format);

        Stopwatch sw = new();
        sw.Start();
        for (float x = 0.216421654f; x < 2000000; x++)
        {
            usw.Write(x);
            usw.Write(',');
        }

        usw.Flush();
        sw.Stop();
        Console.WriteLine(sw.Elapsed.TotalSeconds);
    }

    [Test]
    public void TestWriteInt32()
    {
        char[] data = new char[30];

        for (int x = 0; x < 5000000; x++)
            IntToConvert.WriteToChars(data, 0);

        Stopwatch sw = new();
        sw.Start();

        for (int x = 0; x < 50000000; x++)
            IntToConvert.WriteToChars(data, 0);

        sw.Stop();
        Console.WriteLine(sw.Elapsed.TotalSeconds);
    }

    /// <summary>
    /// Tests the writing of an integer value to a character array using the WriteToChars method.
    /// </summary>
    [Test]
    public void TestWriteInt322()
    {
        char[] data = new char[30];

        for (int x = 0; x < 5000000; x++)
            IntToConvert.WriteToChars2(data, 0);

        Stopwatch sw = new();
        sw.Start();

        for (int x = 0; x < 50000000; x++)
            IntToConvert.WriteToChars2(data, 0);

        sw.Stop();
        Console.WriteLine(sw.Elapsed.TotalSeconds);
    }

    /// <summary>
    /// Tests the original approach of converting a float value to a string multiple times using the ToString method.
    /// </summary>
    [Test]
    public void TestWriteOrig()
    {
        _ = new char[300];

        for (int x = 0; x < 500000; x++)
            FloatToConvert.ToString();

        Stopwatch sw = new();
        sw.Start();

        for (int x = 0; x < 5000000; x++)
            FloatToConvert.ToString();

        sw.Stop();
        Console.WriteLine(sw.Elapsed.TotalSeconds / 5000000.0 * 1000000000.0);
    }

    /// <summary>
    /// Tests the writing of float values to a character array using the WriteToChars method and measures the performance.
    /// </summary>
    [Test]
    public void TestWriteFloat2()
    {
        char[] data = new char[300];

        for (int x = 0; x < 5000000; x++)
            FloatToConvert.WriteToChars(data, 0);

        Stopwatch sw = new();
        sw.Start();

        for (int x = 0; x < 50000000; x++)
            FloatToConvert.WriteToChars(data, 0);

        sw.Stop();
        Console.WriteLine(sw.Elapsed.TotalSeconds / 50000000.0 * 1000000000.0);
    }

    /// <summary>
    /// Tests the consistency of converting float values to strings using different formats and compares the results.
    /// </summary>
    //[Test]
    public void TestWriteFloatConsistency()
    {
        char[] data = new char[300];
        CompareFloats(0, data);
        CompareFloats(12345678e-0f, data);
        CompareFloats(1234567e-0f, data);
        CompareFloats(1234567e-1f, data);
        CompareFloats(1234567e-2f, data);
        CompareFloats(1234567e-3f, data);
        CompareFloats(1234567e-4f, data);
        CompareFloats(1234567e-5f, data);
        CompareFloats(1234567e-6f, data);
        CompareFloats(1234567e-7f, data);
        CompareFloats(1234567e-8f, data);
        CompareFloats(1234567e-9f, data);
        CompareFloats(1234567e-10f, data);
        CompareFloats(1234567e-11f, data);

        CompareFloats(12345600e-0f, data);
        CompareFloats(1234560e-0f, data);
        CompareFloats(1234560e-1f, data);
        CompareFloats(1234560e-2f, data);
        CompareFloats(1234560e-3f, data);
        CompareFloats(1234560e-4f, data);
        CompareFloats(1234560e-5f, data);
        CompareFloats(1234560e-6f, data);
        CompareFloats(1234560e-7f, data);
        CompareFloats(1234560e-8f, data);
        CompareFloats(1234560e-9f, data);
        CompareFloats(1234560e-10f, data);
        CompareFloats(1234560e-11f, data);
        CompareFloats(12345600e-0f, data);

        CompareFloats(12345605e-0f, data);
        CompareFloats(12345605e-1f, data);
        CompareFloats(12345605e-2f, data);
        CompareFloats(12345605e-3f, data);
        CompareFloats(12345605e-4f, data);
        CompareFloats(12345605e-5f, data);
        CompareFloats(12345605e-6f, data);
        CompareFloats(12345605e-7f, data);
        CompareFloats(12345605e-8f, data);
        CompareFloats(12345605e-9f, data);
        CompareFloats(12345605e-10f, data);
        CompareFloats(12345605e-11f, data);
    }

    /// <summary>
    /// Displays floats and other values to a default string format.
    /// </summary>
    [Test]
    public void DisplayDefaultFormat()
    {
        Console.WriteLine(9999999f.ToString());
        Console.WriteLine(9999998.5f.ToString());

        Console.WriteLine(12345605e-1f.ToString());
        Console.WriteLine(12345615e-1f.ToString());
        Console.WriteLine(12345625e-1f.ToString());
        Console.WriteLine(12345635e-1f.ToString());
        Console.WriteLine(12345645e-1f.ToString());
        Console.WriteLine(12345655e-1f.ToString());
        Console.WriteLine(12345665e-1f.ToString());
        Console.WriteLine(12345675e-1f.ToString());
        Console.WriteLine(12345685e-1f.ToString());
        Console.WriteLine(12345695e-1f.ToString());

        Console.WriteLine(7234567890123456789e-0f.ToString());
        Console.WriteLine(7234567890123456789e-1f.ToString());
        Console.WriteLine(7234567890123456789e-2f.ToString());
        Console.WriteLine(7234567890123456789e-3f.ToString());
        Console.WriteLine(7234567890123456789e-4f.ToString());
        Console.WriteLine(7234567890123456789e-5f.ToString());
        Console.WriteLine(7234567890123456789e-6f.ToString());
        Console.WriteLine(7234567890123456789e-7f.ToString());
        Console.WriteLine(7234567890123456789e-8f.ToString());
        Console.WriteLine(7234567890123456789e-9f.ToString());
        Console.WriteLine(7234567890123456789e-10f.ToString());
        Console.WriteLine(7234567890123456789e-11f.ToString());
        Console.WriteLine(7234567890123456789e-12f.ToString());
        Console.WriteLine(7234567890123456789e-13f.ToString());
        Console.WriteLine(7234567890123456789e-14f.ToString());
        Console.WriteLine(7234567890123456789e-15f.ToString());
        Console.WriteLine(7234567890123456789e-16f.ToString());
        Console.WriteLine(7234567890123456789e-17f.ToString());
        Console.WriteLine(7234567890123456789e-18f.ToString());
        Console.WriteLine(7234567890123456789e-19f.ToString());
        Console.WriteLine(7234567890123456789e-20f.ToString());
        Console.WriteLine(7234567890123456789e-21f.ToString());
        Console.WriteLine(7234567890123456789e-22f.ToString());
        Console.WriteLine(7234567890123456789e-23f.ToString());
        Console.WriteLine(7234567890123456789e-24f.ToString());
        Console.WriteLine(7234567890123456789e-25f.ToString());
        Console.WriteLine(7234567890123456789e-26f.ToString());
        Console.WriteLine(7234567890123456789e-27f.ToString());
        Console.WriteLine(7234567890123456789e-28f.ToString());
        Console.WriteLine(7234567890123456789e-29f.ToString());
        Console.WriteLine((-1502345222199E-07F).ToString());
    }

    /// <summary>
    /// Creates a method that enables a float comparison.
    /// </summary>
    /// <param name="value">The float value to be written to a character array.</param>
    /// <param name="data">The character array to be written to.</param>
    /// <exception cref="Exception">Thrown if lengths are not equal or if the indexes do not align.</exception>
    private void CompareFloats(float value, char[] data)
    {
        int len = value.WriteToChars(data, 0);
        string str = value.ToString();
        if (len != str.Length)
        {
            value.WriteToChars(data, 0);
            throw new Exception();
        }

        for (int x = 0; x < len; x++)
        {
            if (str[x] != data[x])
            {
                value.WriteToChars(data, 0);
                throw new Exception();
            }
        }

        Console.WriteLine(str);
    }

    #endregion
}