﻿//******************************************************************************************************
//  HistorianClient.cs - Gbtc
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
//  11/08/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//  10/12/2023 - Lillian Gensolin
//       Converted code to .NET core.
//
//******************************************************************************************************

using SnapDB.Snap.Services.Net;

namespace openHistorian.Core.Net;

/// <summary>
/// Connects to a socket based remote historian database collection.
/// </summary>
public class HistorianClient : SnapNetworkClient
{
    #region [ Constructors ]

    /// <summary>
    /// </summary>
    /// <param name="serverNameOrIp"></param>
    /// <param name="port"></param>
    /// <param name="integratedSecurity"></param>
    public HistorianClient(string serverNameOrIp, int port, bool integratedSecurity = false) : base(new SnapNetworkClientSettings
    {
        NetworkPort = port,
        ServerNameOrIp = serverNameOrIp,
        UseIntegratedSecurity = integratedSecurity
    })
    {
    }

    #endregion
}