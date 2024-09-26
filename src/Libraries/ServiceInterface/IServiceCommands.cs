//******************************************************************************************************
//  IServiceCommands.cs - Gbtc
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

using Gemstone.EnumExtensions;
using Gemstone.PhasorProtocols;

namespace ServiceInterface;

/// <summary>
/// Defines available service commands.
/// </summary>
public interface IServiceCommands
{
    // Example service command methods to service a model / controller

    ///// <summary>
    ///// Gets the current status of the stream proxies.
    ///// </summary>
    ///// <returns>Array of stream proxy instances.</returns>
    //IEnumerable<IStreamProxyStatus> GetStreamProxyStatus();

    ///// <summary>
    ///// Gets map of connection state labels.
    ///// </summary>
    ///// <returns>Map of connection state labels.</returns>
    ///// <remarks>
    ///// This returns a map of connection states to a UI label that
    ///// can be used to display connection state descriptions.
    ///// </remarks>
    //Dictionary<int, string> GetConnectionStateLabels()
    //{
    //    return Enum.GetValues(typeof(ConnectionState))
    //        .Cast<ConnectionState>()
    //        .ToDictionary(state => (int)state, state => state.GetDescription());
    //}

    ///// <summary>
    ///// Gets the default connection parameters for a specific connection type.
    ///// </summary>
    ///// <param name="connection">Source connection.</param>
    ///// <returns>Collection of connection parameters.</returns>
    ///// <remarks>
    ///// Should be triggered by a change in phasor protocol type.
    ///// </remarks>
    //ConnectionParameter[] GetDefaultConnectionParameters(Connection connection)
    //{
    //    IConnectionParameters? parameters = Connection.GenerateConnectionParameters(connection);
    //    return parameters is null ? [] : Connection.ParseConnectionParameters(parameters);
    //}

    /// <summary>
    /// Sends a command to a specific phasor connection instance.
    /// </summary>
    /// <param name="connectionID">ID of the connection for command operation.</param>
    /// <param name="command">Device command to send to the connection.</param>
    void SendCommand(Guid connectionID, DeviceCommand command);
}