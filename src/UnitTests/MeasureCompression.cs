//******************************************************************************************************
//  MeasureCompression.cs - Gbtc
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

using NUnit.Framework;
using openHistorian.Snap;
using SnapDB.Snap;
using SnapDB.Snap.Storage;
using SnapDB.Snap.Tree;
using System;
using System.Text;
using openHistorian.Core.Snap;

namespace openHistorian.UnitTests;

/// <summary>
/// Class for measuring compression and analyzing data from a historian storage file.
/// </summary>
[TestFixture]
public class MeasureCompression
{
    /// <summary>
    /// Test method to read data from a historian storage file.
    /// </summary>
    [Test]
    public void Test()
    {
        HistorianKey key = new();
        HistorianValue value = new();
        //using SortedTreeFile file = SortedTreeFile.OpenFile(@"C:\Unison\GPA\Codeplex\openHistorian\Main\Build\Output\Release\Applications\openHistorian\Archive\635293583194231435-Stage2-0ef36dcc-4264-498f-b194-01b2043a9231.d2", true);
        //using SortedTreeTable<HistorianKey, HistorianValue> table = file.OpenTable<HistorianKey, HistorianValue>();
        //using SortedTreeTableReadSnapshot<HistorianKey, HistorianValue> reader = table.BeginRead();
        //using SortedTreeScannerBase<HistorianKey, HistorianValue> scan = reader.GetTreeScanner();
        //scan.SeekToStart();

        //while (scan.Read(key, value));
    }

    /// <summary>
    /// Gets and analyzes bit distribution for historian keys.
    /// </summary>
    [Test]
    public void GetBits()
    {
        HistorianKey key = new();
        HistorianValue value = new();
        StringBuilder sb = new();
        sb.AppendLine("Higher Bits, Bucket Number, Count, FloatValue");
        //using (SortedTreeFile file = SortedTreeFile.OpenFile(@"C:\Archive\635184227258021940-Stage2-8b835d6a-8299-45bb-9624-d4a470e4abe1.d2", true))
        //using (SortedTreeTable<HistorianKey, HistorianValue> table = file.OpenTable<HistorianKey, HistorianValue>())
        //using (SortedTreeTableReadSnapshot<HistorianKey, HistorianValue> reader = table.BeginRead())
        //using (SortedTreeScannerBase<HistorianKey, HistorianValue> scan = reader.GetTreeScanner())
        //{
        //    int count = 0;
        //    scan.SeekToStart();
        //    while (scan.Read(key, value))
        //        count++;

        //    for (int x = 1; x < 24; x++)
        //    {
        //        scan.SeekToStart();
        //        int[] bucket = MeasureBits(scan, x);
        //        Write(sb, bucket, x, count);
        //    }
        //}

        Console.WriteLine(sb.ToString());
    }

    /// <summary>
    /// Measures bit distribution for historian keys.
    /// </summary>
    /// <param name="stream">A stream of historian data.</param>
    /// <param name="higherBits">The number of higher bits to consider.</param>
    /// <returns>An array representing bit distribution.</returns>
    public int[] MeasureBits(TreeStream<HistorianKey, HistorianValue> stream, int higherBits)
    {
        HistorianKey hkey = new();
        HistorianValue hvalue = new();
        int[] bucket = new int[1 << higherBits];
        int shiftBits = 32 - higherBits;
        while (stream.Read(hkey, hvalue))
        {
            uint value = (uint)hvalue.Value1 >> shiftBits;
            bucket[value]++;
        }
        return bucket;
    }

    /// <summary>
    /// Writes bit distribution details to a string builder.
    /// </summary>
    /// <param name="sb">The string builder to write to.</param>
    /// <param name="buckets">An array representing bit distribution.</param>
    /// <param name="higherBits">The number of higher bits considered.</param>
    /// <param name="count">The total number of items.</param>
    public unsafe void Write(StringBuilder sb, int[] buckets, int higherBits, int count)
    {
        int shift = 32 - higherBits;
        for (uint x = 0; x < buckets.Length; x++)
        {
            uint value = x << shift;
            float valuef = *(float*)&value;
            double percent = buckets[x] / (double)count * 100.0;
            if (percent > 0.01)
                sb.AppendLine(higherBits.ToString() + "," + x.ToString() + "," + (buckets[x] / (double)count * 100.0).ToString("0.00") + "," + valuef.ToString());
        }
    }

    /// <summary>
    /// Analyzes and reports the difference in point IDs within historian data.
    /// </summary>
    [Test]
    public void GetDifference()
    {
        StringBuilder sb = new();
        sb.AppendLine("Bucket Number, Count");
        //using (SortedTreeFile file = SortedTreeFile.OpenFile(@"C:\Unison\GPA\Codeplex\openHistorian\Main\Build\Output\Release\Applications\openHistorian\Archive\635293583194231435-Stage2-0ef36dcc-4264-498f-b194-01b2043a9231.d2", true))
        //using (SortedTreeTable<HistorianKey, HistorianValue> table = file.OpenTable<HistorianKey, HistorianValue>())
        //using (SortedTreeTableReadSnapshot<HistorianKey, HistorianValue> reader = table.BeginRead())
        //using (SortedTreeScannerBase<HistorianKey, HistorianValue> scan = reader.GetTreeScanner())
        //{

        //    HistorianKey key1 = new();
        //    HistorianKey key2 = new();
        //    HistorianValue value = new();

        //    int count = 0;
        //    scan.SeekToStart();
        //    while (scan.Read(key1, value))
        //        count++;

        //    int[] bucket = new int[130];
        //    scan.SeekToStart();

        //    while (true)
        //    {
        //        if (!scan.Read(key1, value))
        //            break;

        //        if (key1.Timestamp == key2.Timestamp)
        //        {
        //            int diff = Math.Abs((int)(key1.PointID - key2.PointID));
        //            diff = Math.Min(129, diff);
        //            bucket[diff]++;
        //        }

        //        if (!scan.Read(key2, value))
        //            break;

        //        if (key1.Timestamp == key2.Timestamp)
        //        {
        //            int diff = Math.Abs((int)(key1.PointID - key2.PointID));
        //            diff = Math.Min(129, diff);
        //            bucket[diff]++;
        //        }
        //    }

        //    for (uint x = 0; x < bucket.Length; x++)
        //    {
        //        sb.AppendLine(x.ToString() + "," + (bucket[x] / (double)count * 100.0).ToString("0.00"));
        //    }
        //}
        Console.WriteLine(sb.ToString());
    }
}