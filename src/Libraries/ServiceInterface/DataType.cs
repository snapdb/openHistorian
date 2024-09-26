//******************************************************************************************************
//  DataType.cs - Gbtc
//
//  Copyright © 2024, Grid Protection Alliance.  All Rights Reserved.
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
//  07/28/2024 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

namespace ServiceInterface;

/// <summary>
/// Defines the basic data types for connection parameters.
/// </summary>
/// <remarks>
/// Enumeration is intended to provide UI with basic data type constraints
/// for possible encountered connection parameters.
/// </remarks>
public enum DataType
{
    /// <summary>
    /// Represents a <see cref="String"/> data type.
    /// </summary>
    String,
    /// <summary>
    /// Represents a <see cref="Int16"/> data type.
    /// </summary>
    Int16,
    /// <summary>
    /// Represents a <see cref="UInt16"/> data type.
    /// </summary>
    UInt16,
    /// <summary>
    /// Represents a <see cref="Int32"/> data type.
    /// </summary>
    Int32,
    /// <summary>
    /// Represents a <see cref="UInt32"/> data type.
    /// </summary>
    UInt32,
    /// <summary>
    /// Represents a <see cref="Int64"/> data type.
    /// </summary>
    Int64,
    /// <summary>
    /// Represents a <see cref="UInt64"/> data type.
    /// </summary>
    UInt64,
    /// <summary>
    /// Represents a <see cref="Single"/> data type.
    /// </summary>
    Single,
    /// <summary>
    /// Represents a <see cref="Double"/> data type.
    /// </summary>
    Double,
    /// <summary>
    /// Represents a <see cref="DateTime"/> data type.
    /// </summary>
    DateTime,
    /// <summary>
    /// Represents a <see cref="Boolean"/> data type.
    /// </summary>
    Boolean,
    /// <summary>
    /// Represents an <see cref="Enum"/> data type.
    /// </summary>
    Enum
}