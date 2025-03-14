//******************************************************************************************************
//  ISupportConnectionTest.cs - Gbtc
//
//  Copyright © 2025, Grid Protection Alliance.  All Rights Reserved.
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
//  02/05/2025 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using Microsoft.AspNetCore.Mvc;

namespace ServiceInterface;

/// <summary>
/// Defines an interface for a class that supports connection testing.
/// </summary>
public interface ISupportConnectionTest
{
    // Note that attributes are not inherited by interface members, so implementations will need to reapply the
    // HttGet / Route attributes for each method in the interface that is part of the service contract. They are
    // added here simply so that you can copy and paste the declarations into your controller implementation.

    /// <summary>
    /// Opens a connection to a device using a connection string.
    /// </summary>
    /// <param name="connectionString">Connection string used to connect to device.</param>
    /// <param name="expiration">Expiration time for the connection, in minutes, if not accessed. Defaults to 1 minute if not provided.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>An <see cref="IActionResult"/> containing a token to be used for subsequent requests.</returns>
    [HttpGet, Route("Connect/{expiration:double?}")]
    Task<IActionResult> Connect(ConnectionRequest request, double? expiration, CancellationToken cancellationToken);

    /// <summary>
    /// Closes the connection to device associated with the provided token.
    /// </summary>
    /// <param name="token">Token associated with the device connection.</param>
    [HttpGet, Route("Close/{token}")]
    IActionResult Close(string token);

    /// <summary>
    /// Gets a stream of data from the device associated with the provided token over a web socket.
    /// </summary>
    /// <param name="token">Token associated with the device connection.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>Stream of data from the device.</returns>
    [HttpGet("DataStream/{token}")]
    Task GetDataStream(string token, CancellationToken cancellationToken);
}

public class ConnectionRequest
{
    public string ConnectionString { get; set; }
}