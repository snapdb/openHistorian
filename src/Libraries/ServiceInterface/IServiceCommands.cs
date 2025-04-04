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

using Gemstone.PhasorProtocols;
using Gemstone.Timeseries.Adapters;

namespace ServiceInterface;

/// <summary>
/// Defines available service commands.
/// </summary>
public interface IServiceCommands
{
    /// <summary>
    /// Reloads the configuration for the openHistorian service.
    /// </summary>
    void ReloadConfig();

    /// <summary>
    /// Sends a command to a specific phasor connection instance.
    /// </summary>
    /// <param name="connectionID">ID of the connection for command operation.</param>
    /// <param name="command">Device command to send to the connection.</param>
    void SendCommand(Guid connectionID, DeviceCommand command);

    /// <summary>
    /// Gets the current status of the openHistorian.
    /// </summary>
    (string Status, string Type, string Description) GetCurrentStatus();


    /// <summary>
    /// Gets an adapter instance from active Iaon session by its runtime ID.
    /// </summary>
    /// <param name="runtimeID">Runtime ID of adapter to retrieve.</param>
    /// <returns>Adapter instance from Iaon session with specified <paramref name="runtimeID"/>.</returns>
    IAdapter GetActiveAdapterInstance(uint runtimeID);
}