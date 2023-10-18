//******************************************************************************************************
//  HistorianQuery.cs - Gbtc
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
//  12/12/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//  10/12/2023 - Lillian Gensolin
//       Converted code to .NET core.
//
//******************************************************************************************************

using openHistorian.Core.Snap;
using SnapDB.Snap.Filters;
using SnapDB.Snap.Services;
using SnapDB.Snap.Services.Reader;

namespace openHistorian.Core.Data.Query;

/// <summary>
/// Represents a historian data query module.
/// </summary>
public class HistorianQuery
{
    #region [ Members ]

    private readonly SnapClient m_historian;
    private readonly int m_samplesPerSecond = 30;

    #endregion

    #region [ Constructors ]

    /// <summary>
    /// Initializes a new instance of the HistorianQuery class using server and port information.
    /// </summary>
    /// <param name="server">The server address for the historian.</param>
    /// <param name="port">The port number for the historian.</param>
    public HistorianQuery(string server, int port)
    {
    }

    /// <summary>
    /// Initializes a new instance of the HistorianQuery class using an existing SnapClient instance.
    /// </summary>
    /// <param name="historian">An existing SnapClient instance for historian connectivity.</param>
    public HistorianQuery(SnapClient historian)
    {
        m_historian = historian;
    }

    #endregion

    #region [ Methods ]

    /// <summary>
    /// Retrieves a query result from the historian for the specified time range and signals.
    /// </summary>
    /// <param name="startTime">The start time for the query.</param>
    /// <param name="endTime">The end time for the query.</param>
    /// <param name="zoomLevel">The zoom level for the query.</param>
    /// <param name="signals">A collection of signal calculations to apply to the query.</param>
    /// <returns>A dictionary of signals and their corresponding data as SignalDataBase.</returns>
    public IDictionary<Guid, SignalDataBase> GetQueryResult(DateTime startTime, DateTime endTime, int zoomLevel, IEnumerable<ISignalCalculation> signals)
    {
        using ClientDatabaseBase<HistorianKey, HistorianValue> db = m_historian.GetDatabase<HistorianKey, HistorianValue>("PPA");
        PeriodicScanner scanner = new(m_samplesPerSecond);
        SeekFilterBase<HistorianKey> timestamps = scanner.GetParser(startTime, endTime, 1500u);
        SortedTreeEngineReaderOptions options = new(TimeSpan.FromSeconds(1));
        IDictionary<Guid, SignalDataBase> results = db.GetSignalsWithCalculations(timestamps, signals, options);

        return results;
    }

    #endregion
}