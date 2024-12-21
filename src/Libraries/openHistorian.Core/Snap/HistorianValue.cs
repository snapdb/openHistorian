//******************************************************************************************************
//  HistorianValue.cs - Gbtc
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
//  04/12/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//
//  10/12/2023 - Lillian Gensolin
//       Converted code to .NET core.
//
//******************************************************************************************************

using Gemstone;
using Gemstone.Timeseries;
using Gemstone.WordExtensions;
using SnapDB;
using SnapDB.IO;
using SnapDB.Snap;

namespace openHistorian.Snap;

/// <summary>
/// The standard value used in the OpenHistorian.
/// </summary>
public class HistorianValue : SnapTypeBase<HistorianValue>
{
    #region [ Members ]

    /// <summary>
    /// Value1 should be where the first 64 bits of the field is stored. For 32 bit values, use this field only.
    /// </summary>
    public ulong Value1;

    /// <summary>
    /// Should only be used if value cannot be entirely stored in Value1. Compression penalty occurs when using this field.
    /// </summary>
    public ulong Value2;

    /// <summary>
    /// Should contain any kind of digital data such as Quality. Compression penalty occurs when used for any other type of field.
    /// </summary>
    public ulong Value3;

    #endregion

    #region [ Properties ]

    /// <summary>
    /// Type casts the <see cref="Value1"/> as a single.
    /// </summary>
    public float AsSingle
    {
        get => BitConvert.ToSingle(Value1);
        set => Value1 = BitConvert.ToUInt64(value);
    }

    /// <summary>
    /// Type casts all values as an alarmed flag, an associated signal ID and status flags representing an alarm.
    /// </summary>
    /// <remarks>
    /// <see cref="Value1"/> and <see cref="Value2"/> are used to store the Guid-based signal ID.
    /// <see cref="Value3"/> is used to store the alarmed flag and the status flags.
    /// Alarmed flag is stored in the high 32-bits of <see cref="Value3"/> (bits 32-63) with a value of 0 or 1.
    /// Status flags are stored in the low 32-bits of <see cref="Value3"/> (bits 0-31) - per normal location.
    /// </remarks>
    public (bool alarmed, Guid signalID, MeasurementStateFlags flags) AsAlarm
    {
        get => (Value3.HighDoubleWord() > 0, BitConvert.ToGuid(Value1, Value2), (MeasurementStateFlags)Value3.LowDoubleWord());
        set
        {
            (Value1, Value2) = BitConvert.ToUInt64Pair(value.signalID);
            Value3 = Word.MakeQuadWord(value.alarmed ? 1U : 0U, (uint)value.flags);
        }
    }

    /// <summary>
    /// Type casts <see cref="Value1"/> and <see cref="Value2"/> into a 16 character string.
    /// </summary>
    public string AsString
    {
        get
        {
            byte[] data = new byte[16];
            BitConverter.GetBytes(Value1).CopyTo(data, 0);
            BitConverter.GetBytes(Value2).CopyTo(data, 8);
            return System.Text.Encoding.ASCII.GetString(data);
        }
        set
        {
            if (value.Length > 16)
                throw new OverflowException("String cannot be larger than 16 characters");

            byte[] data = new byte[16];
            System.Text.Encoding.ASCII.GetBytes(value).CopyTo(data, 0);
            Value1 = BitConverter.ToUInt64(data, 0);
            Value2 = BitConverter.ToUInt64(data, 8);
        }
    }

    /// <summary>
    /// The generic GUID type to use for encoding.
    /// </summary>
    public override Guid GenericTypeGuid =>
        // {24DDE7DC-67F9-42B6-A11B-E27C3E62D9EF}
        new(0x24dde7dc, 0x67f9, 0x42b6, 0xa1, 0x1b, 0xe2, 0x7c, 0x3e, 0x62, 0xd9, 0xef);

    /// <summary>
    /// The size of the encoded data.
    /// </summary>
    public override int Size => 24;

    #endregion

    #region [ Methods ]

    /// <summary>
    /// Copies the values to a specified destination.
    /// </summary>
    /// <param name="destination">The destination to send the copies to.</param>
    public override void CopyTo(HistorianValue destination)
    {
        destination.Value1 = Value1;
        destination.Value2 = Value2;
        destination.Value3 = Value3;
    }

    /// <summary>
    /// Compares values.
    /// </summary>
    /// <param name="other">The HistorianValue to compare to.</param>
    /// <returns>A number that indicates the relationship between the two values being compared.</returns>
    public override int CompareTo(HistorianValue? other)
    {
        if (other is null)
            return 1;

        if (Value1 < other.Value1)
            return -1;

        if (Value1 > other.Value1)
            return 1;
        
        if (Value2 < other.Value2)
            return -1;
        
        if (Value2 > other.Value2)
            return 1;
        
        if (Value3 < other.Value3)
            return -1;
        
        if (Value3 > other.Value3)
            return 1;

        return 0;
    }

    /// <summary>
    /// Sets the minimum for Value1, Value2, and Value3.
    /// </summary>
    public override void SetMin()
    {
        Value1 = 0;
        Value2 = 0;
        Value3 = 0;
    }

    /// <summary>
    /// Sets the maximum for Value1, Value2, and Value3.
    /// </summary>
    public override void SetMax()
    {
        Value1 = ulong.MaxValue;
        Value2 = ulong.MaxValue;
        Value3 = ulong.MaxValue;
    }

    /// <summary>
    /// Sets the value to the default values.
    /// </summary>
    public override void Clear()
    {
        Value1 = 0;
        Value2 = 0;
        Value3 = 0;
    }

    /// <summary>
    /// Reads the values from the stream.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    public override void Read(BinaryStreamBase stream)
    {
        Value1 = stream.ReadUInt64();
        Value2 = stream.ReadUInt64();
        Value3 = stream.ReadUInt64();
    }

    /// <summary>
    /// Writes the values.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    public override void Write(BinaryStreamBase stream)
    {
        stream.Write(Value1);
        stream.Write(Value2);
        stream.Write(Value3);
    }

    /// <summary>
    /// Clones this instance of the class.
    /// </summary>
    /// <returns>A clone of the <see cref="HistorianValue"/> class.</returns>
    public new HistorianValue Clone()
    {
        return new HistorianValue
        { 
            Value1 = Value1,
            Value2 = Value2,
            Value3 = Value3
        };
    }

    /// <summary>
    /// Creates a struct from this data.
    /// </summary>
    /// <returns>The new struct based on Value1, Value2, and Value3.</returns>
    public HistorianValueStruct ToStruct()
    {
        return new HistorianValueStruct
        {
            Value1 = Value1,
            Value2 = Value2,
            Value3 = Value3
        };
    }

    /// <summary>
    /// Reads the byte stream and assigns values to Value1, Value2, and Value3.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    public override unsafe void Read(byte* stream)
    {
        Value1 = *(ulong*)stream;
        Value2 = *(ulong*)(stream + 8);
        Value3 = *(ulong*)(stream + 16);
    }

    /// <summary>
    /// Writes the data for Value1, Value2, and Value3 to the stream.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    public override unsafe void Write(byte* stream)
    {
        *(ulong*)stream = Value1;
        *(ulong*)(stream + 8) = Value2;
        *(ulong*)(stream + 16) = Value3;
    }

    #endregion
}