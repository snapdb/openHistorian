﻿//******************************************************************************************************
//  TsCombinedEncodingDefinition`1.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  02/21/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//
//  10/12/2023 - Lillian Gensolin
//       Converted code to .NET core.
//
//******************************************************************************************************

using Gemstone;
using openHistorian.Snap.Definitions;
using SnapDB.IO;
using SnapDB.Snap;
using SnapDB.Snap.Encoding;

namespace openHistorian.Snap.Encoding;

/// <summary>
/// Provides an encoding method for storing and retrieving pairs of HistorianKey and HistorianValue.
/// </summary>
public class HistorianFileEncoding : PairEncodingBase<HistorianKey, HistorianValue>
{
    #region [ Properties ]

    /// <summary>
    /// Does not contain an end of stream symbol.
    /// </summary>
    public override bool ContainsEndOfStreamSymbol => false;

    /// <summary>
    /// A type of <see cref="EncodingDefinition"/> unique identifier.
    /// </summary>
    public override EncodingDefinition EncodingMethod => HistorianFileEncodingDefinition.TypeGuid;

    /// <summary>
    /// If end of stream symbol is reached, throws a <see cref="NotSupportedException"/>.
    /// </summary>
    public override byte EndOfStreamSymbol => throw new NotSupportedException();

    /// <summary>
    /// Sets the maximum size for compression.
    /// </summary>
    public override int MaxCompressionSize => 54;

    /// <summary>
    /// The method uses the previous key.
    /// </summary>
    public override bool UsesPreviousKey => true;

    /// <summary>
    /// The method does NOT uses the previous value.
    /// </summary>
    public override bool UsesPreviousValue => false;

    #endregion

    #region [ Methods ]

    /// <summary>
    /// Encodes historical data.
    /// </summary>
    /// <param name="stream">The stream for writing the encoded data.</param>
    /// <param name="prevKey">The key prior to the current key to compare.</param>
    /// <param name="prevValue">The value prior to the current value to compare.</param>
    /// <param name="key">The current key to encode.</param>
    /// <param name="value">The current value to encode.</param>
    /// <returns>The size in bytes of the final encode.</returns>
    public override unsafe int Encode(byte* stream, HistorianKey prevKey, HistorianValue prevValue, HistorianKey key, HistorianValue value)
    {
        //ToDo: Make stage 1 still work on big endian processors.
        int size = 0;

        // Compression Stages:
        //   Stage 1: Big Positive Float. 
        //   Stage 2: Big Negative Float.
        //   Stage 3: Zero
        //   Stage 4: 32 bit
        //   Stage 5: Catch all

        if (key.Timestamp == prevKey.Timestamp && key.PointID > prevKey.PointID && key.PointID - prevKey.PointID <= 16 && key.EntryNumber == 0 && value.Value1 <= uint.MaxValue //must be a 32-bit value
            && value.Value2 == 0 && value.Value3 == 0)
        {
            uint deltaPointID = (uint)(key.PointID - prevKey.PointID);
            //Could match Stage 1, 2, 3, or 4

            //Check for Stage 3
            if (value.Value1 == 0 && deltaPointID <= 16)
            {
                //Stage 3: 28% of the time.
                stream[0] = (byte)(0xC0 | deltaPointID - 1);
                return 1;
            }

            //Check for Stage 1
            if (value.Value1 >> 28 == 4 && deltaPointID <= 8)
            {
                //Stage 1: 46% of the time
                //Big Positive Float

                //Must be stored big endian
                //ByteCode is 0DDDVVVV
                stream[0] = (byte)(value.Value1 >> 24 & 0xF | deltaPointID - 1 << 4);
                stream[1] = (byte)(value.Value1 >> 16);
                stream[2] = (byte)(value.Value1 >> 8);
                stream[3] = (byte)value.Value1;
                return 4;
            }

            //Check for stage 2
            if (value.Value1 >> 28 == 12 && deltaPointID <= 4)
            {
                //Must be stored big endian
                //ByteCode is 10DDVVVV
                stream[0] = (byte)(0x80 | value.Value1 >> 24 & 0xF | deltaPointID - 1 << 4);
                stream[1] = (byte)(value.Value1 >> 16);
                stream[2] = (byte)(value.Value1 >> 8);
                stream[3] = (byte)value.Value1;
                return 4;
            }

            //Check for stage 4
            //All conditions are in the logic statement that enters this block.
            //  deltaPointID <= 16
            stream[0] = (byte)(0xD0 | deltaPointID - 1);
            *(uint*)(stream + 1) = (uint)value.Value1;
            return 5;
        }

        //Stage 5: Catch All
        stream[0] = 0xE0;
        size = 1;
        if (key.Timestamp != prevKey.Timestamp)
        {
            stream[0] |= 0x10; //Set bit T
            Encoding7Bit.Write(stream, ref size, key.Timestamp - prevKey.Timestamp);
            Encoding7Bit.Write(stream, ref size, key.PointID);
        }
        else
        {
            Encoding7Bit.Write(stream, ref size, key.PointID - prevKey.PointID);
        }


        if (key.EntryNumber != 0)
        {
            stream[0] |= 0x08; //Set bit E
            Encoding7Bit.Write(stream, ref size, key.EntryNumber);
        }

        if (value.Value1 > uint.MaxValue)
        {
            stream[0] |= 0x04; //Set Bit V1
            *(ulong*)(stream + size) = value.Value1;
            size += 8;
        }
        else
        {
            *(uint*)(stream + size) = (uint)value.Value1;
            size += 4;
        }

        if (value.Value2 != 0)
        {
            stream[0] |= 0x02; // Set Bit V2
            Encoding7Bit.Write(stream, ref size, value.Value2);
        }

        if (value.Value3 != 0)
        {
            //ToDo: Special encoding of flag fields
            stream[0] |= 0x01; // Set Bit V3
            Encoding7Bit.Write(stream, ref size, value.Value3);
        }

        return size;
    }

    /// <summary>
    /// The Decode process.
    /// </summary>
    /// <param name="stream">The stream to decode.</param>
    /// <param name="prevKey">The key prior to the current key to compare.</param>
    /// <param name="prevValue">The value prior to the current value to compare.</param>
    /// <param name="key">The current key to decode.</param>
    /// <param name="value">The current value to decode.</param>
    /// <param name="isEndOfStream">The end of the stream returns <c>true</c>; otherwise, <c>false.</c></param>
    /// <returns>The decoded data.</returns>
    public override unsafe int Decode(byte* stream, HistorianKey prevKey, HistorianValue prevValue, HistorianKey key, HistorianValue value, out bool isEndOfStream)
    {
        isEndOfStream = false;
        int size = 0;
        uint code = stream[0];
        // Compression Stages:
        //   Stage 1: Big Positive Float. 
        //   Stage 2: Big Negative Float.
        //   Stage 3: Zero
        //   Stage 4: 32 bit
        //   Stage 5: Catch all

        if (code < 0x80)
        {
            // If stage 1 (50% success)
            key.Timestamp = prevKey.Timestamp;
            key.PointID = prevKey.PointID + 1 + (code >> 4 & 0x7);
            key.EntryNumber = 0;
            value.Value1 = 4u << 28 | (code & 0xF) << 24 | (uint)stream[1] << 16 | (uint)stream[2] << 8 | (uint)stream[3] << 0;
            value.Value2 = 0;
            value.Value3 = 0;
            return 4;
        }

        if (code < 0xC0)
        {
            //If stage 2 (16% success)
            key.Timestamp = prevKey.Timestamp;
            key.PointID = prevKey.PointID + 1 + (code >> 4 & 0x3);
            key.EntryNumber = 0;
            value.Value1 = 12u << 28 | (code & 0xF) << 24 | (uint)stream[1] << 16 | (uint)stream[2] << 8 | (uint)stream[3] << 0;
            value.Value2 = 0;
            value.Value3 = 0;
            return 4;
        }

        if (code < 0xD0)
        {
            //If stage 3 (28% success)
            key.Timestamp = prevKey.Timestamp;
            key.PointID = prevKey.PointID + 1 + (code & 0xF);
            key.EntryNumber = 0;
            value.Value1 = 0;
            value.Value2 = 0;
            value.Value3 = 0;
            return 1;
        }

        if (code < 0xE0)
        {
            //If stage 4 (3% success)
            key.Timestamp = prevKey.Timestamp;
            key.PointID = prevKey.PointID + 1 + (code & 0xF);
            key.EntryNumber = 0;
            value.Value1 = *(uint*)(stream + 1);
            value.Value2 = 0;
            value.Value3 = 0;
            return 5;
        }

        //Stage 5: 2%
        //Stage 5: Catch All
        size = 1;
        if ((code & 16) != 0) //T is set
        {
            key.Timestamp = prevKey.Timestamp + Encoding7Bit.ReadUInt64(stream, ref size);
            key.PointID = Encoding7Bit.ReadUInt64(stream, ref size);
        }
        else
        {
            key.Timestamp = prevKey.Timestamp;
            key.PointID = prevKey.PointID + Encoding7Bit.ReadUInt64(stream, ref size);
        }

        if ((code & 8) != 0) //E is set)
            key.EntryNumber = Encoding7Bit.ReadUInt64(stream, ref size);
        else
            key.EntryNumber = 0;

        if ((code & 4) != 0) //V1 is set)
        {
            value.Value1 = *(ulong*)(stream + size);
            size += 8;
        }
        else
        {
            value.Value1 = *(uint*)(stream + size);
            size += 4;
        }

        if ((code & 2) != 0) //V2 is set)
            value.Value2 = Encoding7Bit.ReadUInt64(stream, ref size);
        else
            value.Value2 = 0;

        if ((code & 1) != 0) //V3 is set)
            value.Value3 = Encoding7Bit.ReadUInt64(stream, ref size);
        else
            value.Value3 = 0;
        return size;
    }

    /// <summary>
    /// Encodes historical data and writes it to a <see cref="BinaryStreamBase"/>.
    /// </summary>
    /// <param name="stream">The BinaryStreamBase for writing the encoded data.</param>
    /// <param name="prevKey">The previous HistorianKey for comparison.</param>
    /// <param name="prevValue">The previous HistorianValue for comparison.</param>
    /// <param name="key">The current HistorianKey to encode.</param>
    /// <param name="value">The current HistorianValue to encode.</param>
    public override unsafe void Encode(BinaryStreamBase stream, HistorianKey prevKey, HistorianValue prevValue, HistorianKey key, HistorianValue value)
    {
        byte* ptr = stackalloc byte[MaxCompressionSize];
        int length = Encode(ptr, prevKey, prevValue, key, value);
        stream.Write(ptr, length);
    }

    /// <summary>
    /// Decodes the data from the <see cref="BinaryStreamBase"/>.
    /// </summary>
    /// <param name="stream">The <see cref="BinaryStreamBase"/> for decoded the encoded data.</param>
    /// <param name="prevKey">The previous HistorianKey to compare.</param>
    /// <param name="prevValue">The previous HistorianValue to compare.</param>
    /// <param name="key">The current key to decode.</param>
    /// <param name="value">The current value to decode.</param>
    /// <param name="isEndOfStream">If end of stream has been reached, returns <c>true</c>; else, <c>false</c>.</param>
    public override void Decode(BinaryStreamBase stream, HistorianKey prevKey, HistorianValue prevValue, HistorianKey key, HistorianValue value, out bool isEndOfStream)
    {
        isEndOfStream = false;
        uint code = stream.ReadUInt8();
        byte b1;
        byte b2;
        byte b3;
        //Compression Stages:
        //  Stage 1: Big Positive Float. 
        //  Stage 2: Big Negative Float.
        //  Stage 3: Zero
        //  Stage 4: 32 bit
        //  Stage 5: Catch all

        if (code < 0x80)
        {
            b1 = stream.ReadUInt8();
            b2 = stream.ReadUInt8();
            b3 = stream.ReadUInt8();

            //If stage 1 (50% success)
            key.Timestamp = prevKey.Timestamp;
            key.PointID = prevKey.PointID + 1 + (code >> 4 & 0x7);
            key.EntryNumber = 0;
            value.Value1 = 4u << 28 | (code & 0xF) << 24 | (uint)b1 << 16 | (uint)b2 << 8 | (uint)b3 << 0;
            value.Value2 = 0;
            value.Value3 = 0;
            return;
        }

        if (code < 0xC0)
        {
            b1 = stream.ReadUInt8();
            b2 = stream.ReadUInt8();
            b3 = stream.ReadUInt8();

            //If stage 2 (16% success)
            key.Timestamp = prevKey.Timestamp;
            key.PointID = prevKey.PointID + 1 + (code >> 4 & 0x3);
            key.EntryNumber = 0;
            value.Value1 = 12u << 28 | (code & 0xF) << 24 | (uint)b1 << 16 | (uint)b2 << 8 | (uint)b3 << 0;
            value.Value2 = 0;
            value.Value3 = 0;

            return;
        }

        if (code < 0xD0)
        {
            //If stage 3 (28% success)
            key.Timestamp = prevKey.Timestamp;
            key.PointID = prevKey.PointID + 1 + (code & 0xF);
            key.EntryNumber = 0;
            value.Value1 = 0;
            value.Value2 = 0;
            value.Value3 = 0;
            return;
        }

        if (code < 0xE0)
        {
            //If stage 4 (3% success)
            key.Timestamp = prevKey.Timestamp;
            key.PointID = prevKey.PointID + 1 + (code & 0xF);
            key.EntryNumber = 0;
            value.Value1 = stream.ReadUInt32();
            value.Value2 = 0;
            value.Value3 = 0;
            return;
        }

        //Stage 5: 2%
        //Stage 5: Catch All
        if ((code & 16) != 0) //T is set
        {
            key.Timestamp = prevKey.Timestamp + stream.Read7BitUInt64();
            key.PointID = stream.Read7BitUInt64();
        }
        else
        {
            key.Timestamp = prevKey.Timestamp;
            key.PointID = prevKey.PointID + stream.Read7BitUInt64();
        }


        if ((code & 8) != 0) //E is set)
            key.EntryNumber = stream.Read7BitUInt64();
        else
            key.EntryNumber = 0;

        if ((code & 4) != 0) //V1 is set)
            value.Value1 = stream.ReadUInt64();
        else
            value.Value1 = stream.ReadUInt32();

        if ((code & 2) != 0) //V2 is set)
            value.Value2 = stream.Read7BitUInt64();
        else
            value.Value2 = 0;

        if ((code & 1) != 0) //V3 is set)
            value.Value3 = stream.Read7BitUInt64();
        else
            value.Value3 = 0;
    }

    /// <summary>
    /// Clones a HistorianKey-HistorianValue pair.
    /// </summary>
    /// <returns>The clone of the pair.</returns>
    public override PairEncodingBase<HistorianKey, HistorianValue> Clone()
    {
        return this;
    }

    #endregion
}