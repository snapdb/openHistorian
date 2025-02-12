﻿//******************************************************************************************************
//  TypeSingle.cs - Gbtc
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

namespace openHistorian.Data.Types;

/// <summary>
/// Method for converting data to and from a <see cref="float"/>.
/// </summary>
public unsafe class TypeSingle : TypeBase
{
    #region [ Constructors ]

    /// <summary>
    /// Must use the static instance.
    /// </summary>
    private TypeSingle()
    {
    }

    #endregion

    #region [ Methods ]

    /// <summary>
    /// Converts a convertible value to its raw form.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The raw value.</returns>
    protected override ulong ToRaw(IConvertible value)
    {
        float tmp = value.ToSingle(null);

        return *(uint*)&tmp;
    }

    /// <summary>
    /// Gets the value of the raw form data and converts it to a float.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The float value.</returns>
    protected override IConvertible GetValue(ulong value)
    {
        uint tmp = (uint)value;
        return *(float*)&tmp;
    }

    #endregion

    #region [ Static ]

    /// <summary>
    /// A readonly instance of TypeSingle.
    /// </summary>
    public static readonly TypeSingle Instance = new();

    #endregion
}