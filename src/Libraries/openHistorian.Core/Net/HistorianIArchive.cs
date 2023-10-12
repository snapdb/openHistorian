//******************************************************************************************************
//  HistorianIArchive.cs - Gbtc
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
//  07/26/2013 - J. Ritchie Carroll
//       Generated original version of source code.
//
//  10/12/2023 - Lillian Gensolin
//       Converted code to .NET core.
//
//******************************************************************************************************

using Gemstone;
using openHistorian.Snap;
using SnapDB;
using SnapDB.Snap;
using SnapDB.Snap.Services;
using System.Data;

namespace openHistorian.Net
{
    /// <summary>
    /// An <see cref="IArchive"/> wrapper around a SortedTreeStore.
    /// </summary>
    /// <remarks>
    /// This class implements the 1.0 historian <see cref="IArchive"/> to automatically bring in historian providers (e.g., web services).
    /// </remarks>
    public class HistorianIArchive 
    {
        #region [ Members ]

        // Events
        public event EventHandler MetadataUpdated;

        // Fields
        private readonly HistorianServer m_server;
        private readonly SnapClient m_client;
        private readonly ClientDatabaseBase<HistorianKey, HistorianValue> m_clientDatabase;

        #endregion


        #region [ Constructors ]

        public HistorianIArchive(HistorianServer server, string databaseName)
        {
            m_server = server;
            m_client = SnapClient.Connect(m_server.Host);
            m_clientDatabase = m_client.GetDatabase<HistorianKey, HistorianValue>(databaseName);
        }

        #endregion

        #region [ Properties ]

        public HistorianServer Server => m_server;

        public SnapClient Client => m_client;

        public ClientDatabaseBase<HistorianKey, HistorianValue> ClientDatabase => m_clientDatabase;

        #endregion
    }
}
