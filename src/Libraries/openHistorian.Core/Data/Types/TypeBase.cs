//******************************************************************************************************
//  TypeBase.cs - Gbtc
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
//  12/15/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//  10/12/2023 - Lillian Gensolin
//       Generated original version of source code.
//
//******************************************************************************************************

namespace openHistorian.Data.Types;

/// <summary>
/// This base class supports proper conversion of
/// each primitive type into a native format.
/// The native format is specified.
/// If not overloading individual properties, boxing will
/// occur each time that value is called.
/// </summary>
public abstract class TypeBase
{
    #region [ Methods ]

    /// <summary>
    /// Converts a raw ulong value to a double value using the GetValue method and assigns it to the 'value' out parameter.
    /// </summary>
    /// <param name="raw">The raw ulong value to be converted to a double.</param>
    /// <param name="value">The resulting double value after the conversion.</param>
    public virtual void ToValue(ulong raw, out double value)
    {
        value = GetValue(raw).ToDouble(null);
    }

    /// <summary>
    /// Converts a raw ulong value to a float value using the GetValue method and assigns it to the 'value' out parameter.
    /// </summary>
    /// <param name="raw">The raw ulong value to be converted to a float.</param>
    /// <param name="value">The resulting float value after the conversion.</param>
    public virtual void ToValue(ulong raw, out float value)
    {
        value = GetValue(raw).ToSingle(null);
    }

    /// <summary>
    /// Converts a raw ulong value to a long value using the GetValue method and assigns it to the 'value' out parameter.
    /// </summary>
    /// <param name="raw">The raw ulong value to be converted to a long.</param>
    /// <param name="value">The resulting long value after the conversion.</param>
    public virtual void ToValue(ulong raw, out long value)
    {
        value = GetValue(raw).ToInt64(null);
    }

    /// <summary>
    /// Converts a raw ulong value to a ulong value using the GetValue method and assigns it to the 'value' out parameter.
    /// </summary>
    /// <param name="raw">The raw ulong value to be converted to a ulong.</param>
    /// <param name="value">The resulting ulong value after the conversion.</param>
    public virtual void ToValue(ulong raw, out ulong value)
    {
        value = GetValue(raw).ToUInt64(null);
    }

    /// <summary>
    /// Converts a raw ulong value to a int value using the GetValue method and assigns it to the 'value' out parameter.
    /// </summary>
    /// <param name="raw">The raw ulong value to be converted to a int.</param>
    /// <param name="value">The resulting int value after the conversion.</param>
    public virtual void ToValue(ulong raw, out int value)
    {
        value = GetValue(raw).ToInt32(null);
    }

    /// <summary>
    /// Converts a raw ulong value to a uint value using the GetValue method and assigns it to the 'value' out parameter.
    /// </summary>
    /// <param name="raw">The raw ulong value to be converted to a uint.</param>
    /// <param name="value">The resulting uint value after the conversion.</param>
    public virtual void ToValue(ulong raw, out uint value)
    {
        value = GetValue(raw).ToUInt32(null);
    }

    /// <summary>
    /// Converts a raw ulong value to a short value using the GetValue method and assigns it to the 'value' out parameter.
    /// </summary>
    /// <param name="raw">The raw ulong value to be converted to a short.</param>
    /// <param name="value">The resulting short value after the conversion.</param>
    public virtual void ToValue(ulong raw, out short value)
    {
        value = GetValue(raw).ToInt16(null);
    }

    /// <summary>
    /// Converts a raw ulong value to a ushort value using the GetValue method and assigns it to the 'value' out parameter.
    /// </summary>
    /// <param name="raw">The raw ulong value to be converted to a ushort.</param>
    /// <param name="value">The resulting ushort value after the conversion.</param>
    public virtual void ToValue(ulong raw, out ushort value)
    {
        value = GetValue(raw).ToUInt16(null);
    }

    /// <summary>
    /// Converts a raw ulong value to a sbyte value using the GetValue method and assigns it to the 'value' out parameter.
    /// </summary>
    /// <param name="raw">The raw ulong value to be converted to a sbyte.</param>
    /// <param name="value">The resulting sbyte value after the conversion.</param>
    public virtual void ToValue(ulong raw, out sbyte value)
    {
        value = GetValue(raw).ToSByte(null);
    }

    /// <summary>
    /// Converts a raw ulong value to a byte value using the GetValue method and assigns it to the 'value' out parameter.
    /// </summary>
    /// <param name="raw">The raw ulong value to be converted to a byte.</param>
    /// <param name="value">The resulting byte value after the conversion.</param>
    public virtual void ToValue(ulong raw, out byte value)
    {
        value = GetValue(raw).ToByte(null);
    }

    /// <summary>
    /// Converts a raw ulong value to a bool value using the GetValue method and assigns it to the 'value' out parameter.
    /// </summary>
    /// <param name="raw">The raw ulong value to be converted to a bool.</param>
    /// <param name="value">The resulting bool value after the conversion.</param>
    public virtual void ToValue(ulong raw, out bool value)
    {
        value = GetValue(raw).ToBoolean(null);
    }

    /// <summary>
    /// Converts a ulong value to a double value using the ToValue method and returns the resulting double value.
    /// </summary>
    /// <param name="value">The ulong value to be converted to a double.</param>
    /// <returns>The double value obtained after the conversion.</returns>
    public double ToDouble(ulong value)
    {
        ToValue(value, out double tmp);
        return tmp;
    }

    /// <summary>
    /// Converts a ulong value to a float value using the ToValue method and returns the resulting float value.
    /// </summary>
    /// <param name="value">The ulong value to be converted to a float.</param>
    /// <returns>The float value obtained after the conversion.</returns>
    public float ToSingle(ulong value)
    {
        ToValue(value, out float tmp);
        return tmp;
    }

    /// <summary>
    /// Converts a ulong value to a long value using the ToValue method and returns the resulting long value.
    /// </summary>
    /// <param name="value">The ulong value to be converted to a long.</param>
    /// <returns>The long value obtained after the conversion.</returns>
    public long ToInt64(ulong value)
    {
        ToValue(value, out long tmp);
        return tmp;
    }

    /// <summary>
    /// Converts a ulong value to a UInt64 value using the ToValue method and returns the resulting UInt64 value.
    /// </summary>
    /// <param name="value">The ulong value to be converted to a UInt64.</param>
    /// <returns>The UInt64 value obtained after the conversion.</returns>
    public ulong ToUInt64(ulong value)
    {
        ToValue(value, out ulong tmp);
        return tmp;
    }

    /// <summary>
    /// Converts a ulong value to a Int32 value using the ToValue method and returns the resulting Int32 value.
    /// </summary>
    /// <param name="value">The ulong value to be converted to a Int32.</param>
    /// <returns>The Int32 value obtained after the conversion.</returns>
    public int ToInt32(ulong value)
    {
        ToValue(value, out int tmp);
        return tmp;
    }

    /// <summary>
    /// Converts a ulong value to a UInt32 value using the ToValue method and returns the resulting UInt32 value.
    /// </summary>
    /// <param name="value">The ulong value to be converted to a UInt32.</param>
    /// <returns>The UInt32 value obtained after the conversion.</returns>
    public uint ToUInt32(ulong value)
    {
        ToValue(value, out uint tmp);
        return tmp;
    }

    /// <summary>
    /// Converts a ulong value to a short value using the ToValue method and returns the resulting short value.
    /// </summary>
    /// <param name="value">The ulong value to be converted to a short.</param>
    /// <returns>The short value obtained after the conversion.</returns>
    public short ToInt16(ulong value)
    {
        ToValue(value, out short tmp);
        return tmp;
    }

    /// <summary>
    /// Converts a ulong value to a ushort value using the ToValue method and returns the resulting ushort value.
    /// </summary>
    /// <param name="value">The ulong value to be converted to a ushort.</param>
    /// <returns>The ushort value obtained after the conversion.</returns>
    public ushort ToUInt16(ulong value)
    {
        ToValue(value, out ushort tmp);
        return tmp;
    }

    /// <summary>
    /// Converts a ulong value to a sbyte value using the ToValue method and returns the resulting sbyte value.
    /// </summary>
    /// <param name="value">The ulong value to be converted to a sbyte.</param>
    /// <returns>The sbyte value obtained after the conversion.</returns>
    public sbyte ToSByte(ulong value)
    {
        ToValue(value, out sbyte tmp);
        return tmp;
    }

    /// <summary>
    /// Converts a ulong value to a byte value using the ToValue method and returns the resulting byte value.
    /// </summary>
    /// <param name="value">The ulong value to be converted to a byte.</param>
    /// <returns>The byte value obtained after the conversion.</returns>
    public byte ToByte(ulong value)
    {
        ToValue(value, out byte tmp);
        return tmp;
    }

    /// <summary>
    /// Converts a ulong value to a boolean value using the ToValue method and returns the resulting boolean value.
    /// </summary>
    /// <param name="value">The ulong value to be converted to a boolean.</param>
    /// <returns>The boolean value obtained after the conversion.</returns>
    public bool ToBoolean(ulong value)
    {
        ToValue(value, out bool tmp);
        return tmp;
    }

    /// <summary>
    /// Converts a double value to a raw ulong using the ToRaw method and returns the resulting raw ulong.
    /// </summary>
    /// <param name="value">The double value to be converted to a raw ulong.</param>
    /// <returns>The raw ulong value obtained after the conversion.</returns>
    public virtual ulong ToRaw(double value)
    {
        return ToRaw((IConvertible)value);
    }

    /// <summary>
    /// Converts a float value to a raw ulong using the ToRaw method and returns the resulting raw ulong.
    /// </summary>
    /// <param name="value">The float value to be converted to a raw ulong.</param>
    /// <returns>The raw ulong value obtained after the conversion.</returns>
    public virtual ulong ToRaw(float value)
    {
        return ToRaw((IConvertible)value);
    }

    /// <summary>
    /// Converts a long value to a raw ulong using the ToRaw method and returns the resulting raw ulong.
    /// </summary>
    /// <param name="value">The long value to be converted to a raw ulong.</param>
    /// <returns>The raw ulong value obtained after the conversion.</returns>
    public virtual ulong ToRaw(long value)
    {
        return ToRaw((IConvertible)value);
    }

    /// <summary>
    /// Converts a ulong value to a raw ulong using the ToRaw method and returns the resulting raw ulong.
    /// </summary>
    /// <param name="value">The ulong value to be converted to a raw ulong.</param>
    /// <returns>The raw ulong value obtained after the conversion.</returns>
    public virtual ulong ToRaw(ulong value)
    {
        return ToRaw((IConvertible)value);
    }

    /// <summary>
    /// Converts a int value to a raw ulong using the ToRaw method and returns the resulting raw ulong.
    /// </summary>
    /// <param name="value">The int value to be converted to a raw ulong.</param>
    /// <returns>The raw ulong value obtained after the conversion.</returns>
    public virtual ulong ToRaw(int value)
    {
        return ToRaw((IConvertible)value);
    }

    /// <summary>
    /// Converts a uint value to a raw ulong using the ToRaw method and returns the resulting raw ulong.
    /// </summary>
    /// <param name="value">The uint value to be converted to a raw ulong.</param>
    /// <returns>The raw ulong value obtained after the conversion.</returns>
    public virtual ulong ToRaw(uint value)
    {
        return ToRaw((IConvertible)value);
    }

    /// <summary>
    /// Converts a short value to a raw ulong using the ToRaw method and returns the resulting raw ulong.
    /// </summary>
    /// <param name="value">The short value to be converted to a raw ulong.</param>
    /// <returns>The raw ulong value obtained after the conversion.</returns>
    public virtual ulong ToRaw(short value)
    {
        return ToRaw((IConvertible)value);
    }

    /// <summary>
    /// Converts a ushort value to a raw ulong using the ToRaw method and returns the resulting raw ulong.
    /// </summary>
    /// <param name="value">The ushort value to be converted to a raw ulong.</param>
    /// <returns>The raw ulong value obtained after the conversion.</returns>
    public virtual ulong ToRaw(ushort value)
    {
        return ToRaw((IConvertible)value);
    }

    /// <summary>
    /// Converts a byte value to a raw ulong using the ToRaw method and returns the resulting raw ulong.
    /// </summary>
    /// <param name="value">The byte value to be converted to a raw ulong.</param>
    /// <returns>The raw ulong value obtained after the conversion.</returns>
    public virtual ulong ToRaw(byte value)
    {
        return ToRaw((IConvertible)value);
    }

    /// <summary>
    /// Converts a sbyte value to a raw ulong using the ToRaw method and returns the resulting raw ulong.
    /// </summary>
    /// <param name="value">The sbyte value to be converted to a raw ulong.</param>
    /// <returns>The raw ulong value obtained after the conversion.</returns>
    public virtual ulong ToRaw(sbyte value)
    {
        return ToRaw((IConvertible)value);
    }

    /// <summary>
    /// Converts a boolean value to a raw ulong using the ToRaw method and returns the resulting raw ulong.
    /// </summary>
    /// <param name="value">The boolean value to be converted to a raw ulong.</param>
    /// <returns>The raw ulong value obtained after the conversion.</returns>
    public virtual ulong ToRaw(bool value)
    {
        return ToRaw((IConvertible)value);
    }

    /// <summary>
    /// Converts an object implementing IConvertible to its corresponding ulong raw value.
    /// </summary>
    /// <param name="value">The IConvertible value to be converted to a ulong raw value.</param>
    /// <returns>The ulong raw value obtained after the conversion.</returns>
    protected abstract ulong ToRaw(IConvertible value);

    /// <summary>
    /// Retrieves the value represented by a ulong raw value and returns it as an object implementing IConvertible.
    /// </summary>
    /// <param name="value">The ulong raw value from which to obtain the value.</param>
    /// <returns>The IConvertible value obtained from the ulong raw value.</returns>
    protected abstract IConvertible GetValue(ulong value);

    #endregion
}