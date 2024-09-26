//******************************************************************************************************
//  ConnectionParameter.cs - Gbtc
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
/// Represents a connection parameter.
/// </summary>
/// <remarks>
/// Class is intended to provide UI with a structured representation of custom
/// protocol connection parameters.
/// </remarks>
public class ConnectionParameter
{
    /// <summary>
    /// Gets the name of the parameter.
    /// </summary>
    public string Name { get; init; } = default!;

    /// <summary>
    /// Gets the category of the parameter.
    /// </summary>
    /// <remarks>
    /// Category is used for UI grouping of parameters.
    /// </remarks>
    public string Category { get; init; } = default!;

    /// <summary>
    /// Gets the description of the parameter.
    /// </summary>
    public string Description { get; init; } = default!;

    /// <summary>
    /// Gets the basic type of the parameter.
    /// </summary>
    public DataType DataType { get; init; }

    /// <summary>
    /// Gets the available values, e.g., when <see cref="DataType"/> is "Enum".
    /// </summary>
    public string[] AvailableValues { get; init; } = [];

    /// <summary>
    /// Gets the default value of the parameter.
    /// </summary>
    public string DefaultValue { get; init; } = default!;

    /// <summary>
    /// Gets or sets the parameter value.
    /// </summary>
    public string Value { get; set; } = default!;
}