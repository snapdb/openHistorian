﻿//******************************************************************************************************
//  SignalDataSingle.cs - Gbtc
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
//       Converted code to .NET core.
//
//******************************************************************************************************

using openHistorian.Data.Types;

namespace openHistorian.Data;

/// <summary>
/// Contains a series of Times and Values for an individual signal.
/// This class will store the value as a <see cref="float"/>.
/// </summary>
public class SignalDataSingle : SignalDataBase
{
    #region [ Members ]

    private readonly List<ulong> m_dateTime = [];

    private readonly List<float> m_values = [];

    #endregion

    #region [ Constructors ]

    /// <summary>
    /// The <see cref="SignalDataSingle"/> that creates an instance of <see cref="TypeSingle"/> as m_type.
    /// </summary>
    public SignalDataSingle()
    {
        Method = TypeSingle.Instance;
    }

    #endregion

    #region [ Properties ]

    /// <summary>
    /// Gets the number of values that are in the signal.
    /// </summary>
    public override int Count => m_values.Count;

    /// <summary>
    /// Provides the type conversion method for the base class to use.
    /// </summary>
    protected override TypeBase Method { get; }

    #endregion

    #region [ Methods ]

    /// <summary>
    /// Adds a value to the signal and converts it from a <see cref="float"/>
    /// into its native format.
    /// </summary>
    /// <param name="time">the time value to consider.</param>
    /// <param name="value">the value to convert.</param>
    public override void AddData(ulong time, float value)
    {
        if (IsComplete)
            throw new Exception("Signal has already been marked as complete");
        m_dateTime.Add(time);
        m_values.Add(value);
    }

    /// <summary>
    /// Gets a value from the signal with the provided index and automatically
    /// converts it to a <see cref="float"/>.
    /// </summary>
    /// <param name="index">The zero based index of the position.</param>
    /// <param name="time">An output field for the time.</param>
    /// <param name="value">An output field for the converted value.</param>
    public override void GetData(int index, out ulong time, out float value)
    {
        time = m_dateTime[index];
        value = m_values[index];
    }

    /// <summary>
    /// Adds a value to the signal in its raw 64-bit format.
    /// </summary>
    /// <param name="time">the time value to consider</param>
    /// <param name="value">the 64-bit value</param>
    public override unsafe void AddDataRaw(ulong time, ulong value)
    {
        if (IsComplete)
            throw new Exception("Signal has already been marked as complete");
        uint tmp = (uint)value;
        AddData(time, *(float*)&tmp);
    }

    /// <summary>
    /// Gets a value from the signal with the provided index in its
    /// raw 64-bit format.
    /// </summary>
    /// <param name="index">The zero based index of the position</param>
    /// <param name="time">An output field for the time</param>
    /// <param name="value">An output field for the raw 64-bit value</param>
    public override unsafe void GetDataRaw(int index, out ulong time, out ulong value)
    {
        GetData(index, out time, out float tmp);
        value = *(uint*)&tmp;
    }

    /// <summary>
    /// Gets the date stamp from the specified index.
    /// </summary>
    /// <param name="index">The index to retrieve the date from.</param>
    /// <returns>The date stamp from the signal stored at the specified index.</returns>
    public override ulong GetDate(int index)
    {
        return m_dateTime[index];
    }

    #endregion
}