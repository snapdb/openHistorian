//******************************************************************************************************
//  IStreamProxyStatus.cs - Gbtc
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
//  07/27/2024 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System.ComponentModel;

namespace ServiceInterface;

/// <summary>
/// Defines connection state enumeration.
/// </summary>
[Serializable]
public enum ConnectionState
{
    /// <summary>
    /// Disabled state - gray.
    /// </summary>
    [Description("Disabled")]
    Disabled,
    /// <summary>
    /// Disconnected state - red.
    /// </summary>
    [Description("Disconnected")]
    Disconnected,
    /// <summary>
    /// Connected with no data state - yellow.
    /// </summary>
    [Description("Connected (No Data)")]
    ConnectedNoData,
    /// <summary>
    /// Connected normally state - green.
    /// </summary>
    [Description("Connected")]
    Connected
}

/// <summary>
/// Defines the status of a stream proxy.
/// </summary>
public interface IStreamProxyStatus
{
    /// <summary>
    /// Gets the ID of the associated stream proxy.
    /// </summary>
    Guid ID { get; }

    /// <summary>
    /// Gets current connection state for a stream proxy.
    /// </summary>
    ConnectionState ConnectionState { get; }

    /// <summary>
    /// Gets recent status messages for a stream proxy.
    /// </summary>
    string RecentStatusMessages { get; }

    /// <summary>
    /// Gets the name of the associated stream proxy.
    /// </summary>
    string Name { get; }
}