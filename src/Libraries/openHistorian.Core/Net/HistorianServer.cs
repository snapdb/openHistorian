//******************************************************************************************************
//  HistorianServer.cs - Gbtc
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
//  12/19/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//  07/24/2013 - J. Ritchie Carroll
//       Updated code to allow dynamic addition and removal of archive engines and associated sockets.
//
//  10/12/2023 - Lillian Gensolin
//       Converted code to .NET core.
//
//******************************************************************************************************

using SnapDB.Snap.Services;
using SnapDB.Snap.Services.Net;

namespace openHistorian.Core.Net;

/// <summary>
/// Represents a historian server instance that can be used to read and write time-series data.
/// </summary>
public class HistorianServer : IDisposable
{
    #region [ Constructors ]

    /// <summary>
    /// Creates a new instance of <see cref="HistorianServer"/>.
    /// </summary>
    public HistorianServer()
    {
        Host = new SnapServer();
    }

    /// <summary>
    /// Creates a new <see cref="HistorianServer"/> instance.
    /// </summary>
    /// <param name="port">The port number to be associated with the added database.</param>
    /// <param name="networkInterfaceIP">IP to be associated with the added database.</param>
    public HistorianServer(int? port, string? networkInterfaceIP = null)
    {
        ServerSettings server = new();

        if (port.HasValue || !string.IsNullOrWhiteSpace(networkInterfaceIP))
        {
            Settings = new SnapSocketListenerSettings
            {
                LocalTcpPort = port ?? SnapSocketListenerSettings.DefaultNetworkPort,
                LocalIPAddress = networkInterfaceIP,
                DefaultUserCanRead = true,
                DefaultUserCanWrite = true,
                DefaultUserIsAdmin = false
            };

            server.Listeners.Add(Settings);
        }

        // Maintain a member level list of all established archive database engines
        Host = new SnapServer(server);
    }

    /// <summary>
    /// Creates a new <see cref="HistorianServer"/> instance.
    /// </summary>
    /// <param name="settings">The socket listener settings for the historian.</param>
    public HistorianServer(SnapSocketListenerSettings settings)
    {
        ServerSettings server = new();

        Settings = settings;

        server.Listeners.Add(Settings);

        // Maintain a member level list of all established archive database engines
        Host = new SnapServer(server);
    }

    /// <summary>
    /// Adds a specified database to the historian..
    /// </summary>
    /// <param name="database">The database to add to the historian server.</param>
    /// <param name="port"><c>null</c>. The port number to be associated with the added database.</param>
    /// <param name="networkInterfaceIP"><c>null</c>. IP to be associated with the added database.</param>
    public HistorianServer(HistorianServerDatabaseConfig database, int? port = null, string? networkInterfaceIP = null) : this(port, networkInterfaceIP)
    {
        AddDatabase(database);
    }

    /// <summary>
    /// Adds a specified database to the historian..
    /// </summary>
    /// <param name="database">The database to add to the historian server.</param>
    /// <param name="settings">The socket listener settings for the historian.</param>
    public HistorianServer(HistorianServerDatabaseConfig database, SnapSocketListenerSettings settings) : this(settings)
    {
        AddDatabase(database);
    }

    #endregion

    #region [ Properties ]

    /// <summary>
    /// Gets the underlying host ending for the historian.
    /// </summary>
    public SnapServer Host { get; }

    /// <summary>
    /// Gets the SnapDB settings for the historian.
    /// </summary>
    public SnapSocketListenerSettings? Settings { get; }

    #endregion

    #region [ Methods ]

    /// <summary>
    /// Disposes of the new snap server initialized above.
    /// </summary>
    public void Dispose()
    {
        Host.Dispose();
    }

    /// <summary>
    /// Adds the supplied database to this server.
    /// </summary>
    /// <param name="database">The database to add to the server.</param>
    public void AddDatabase(HistorianServerDatabaseConfig database)
    {
        Host.AddDatabase(database);
    }

    /// <summary>
    /// Removes the supplied database from the historian.
    /// </summary>
    /// <param name="database">The database to remove from the server.</param>
    public void RemoveDatabase(string database)
    {
        Host.RemoveDatabase(database);
    }

    #endregion
}