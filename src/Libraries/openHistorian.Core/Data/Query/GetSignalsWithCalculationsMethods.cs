//******************************************************************************************************
//  GetSignalsWithCalculationsMethods.cs - Gbtc
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
//******************************************************************************************************

using openHistorian.Core.Snap;
using SnapDB.Snap.Filters;
using SnapDB.Snap.Services;
using SnapDB.Snap.Services.Reader;

namespace openHistorian.Core.Data.Query;

/// <summary>
/// Provides extension methods for querying historian signals with calculations.
/// </summary>
public static class GetSignalsWithCalculationsMethods
{
    /// <summary>
    /// Gets historian signals with calculations within a specified time range.
    /// </summary>
    /// <param name="database">The database reader instance used for signal retrieval.</param>
    /// <param name="startTime">The start time of the query range.</param>
    /// <param name="endTime">The end time of the query range.</param>
    /// <param name="signals">An enumerable collection of signal calculations to apply to the query.</param>
    /// <returns>A dictionary of signals and their corresponding signal data.</returns>
    public static IDictionary<Guid, SignalDataBase> GetSignalsWithCalculations(this ClientDatabaseBase<HistorianKey, HistorianValue> database, ulong startTime, ulong endTime, IEnumerable<ISignalCalculation> signals)
    {
        return database.GetSignalsWithCalculations(TimestampSeekFilter.CreateFromRange<HistorianKey>(startTime, endTime), signals, SortedTreeEngineReaderOptions.Default);
    }

    /// <summary>
    /// Gets historian signals with calculations using specified timestamps, signals, and reader options.
    /// </summary>
    /// <param name="database">The database reader instance used for signal retrieval.</param>
    /// <param name="timestamps">The seek filter for specifying the timestamp range.</param>
    /// <param name="signals">An enumerable collection of signal calculations to apply to the query.</param>
    /// <param name="readerOptions">The reader options for accessing the database.</param>
    /// <returns>A dictionary of signals and their corresponding signal data after applying calculations.</returns>
    public static IDictionary<Guid, SignalDataBase> GetSignalsWithCalculations(this ClientDatabaseBase<HistorianKey, HistorianValue> database, SeekFilterBase<HistorianKey> timestamps, IEnumerable<ISignalCalculation> signals, SortedTreeEngineReaderOptions readerOptions)
    {
        Dictionary<ulong, SignalDataBase> queryResults = database.GetSignals(timestamps, signals, readerOptions);

        Dictionary<Guid, SignalDataBase> calculatedResults = new();
        foreach (ISignalCalculation signal in signals)
        {
            if (signal.HistorianId.HasValue)
                calculatedResults.Add(signal.SignalId, queryResults[signal.HistorianId.Value]);

            else
                calculatedResults.Add(signal.SignalId, new SignalData(signal.Functions));
        }

        foreach (ISignalCalculation signal in signals)
        {
            signal.Calculate(calculatedResults);
        }

        return calculatedResults;
    }
}