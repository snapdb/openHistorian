//******************************************************************************************************
//  GetSignalMethods.cs - Gbtc
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

using openHistorian.Core.Data.Types;
using openHistorian.Core.Snap;
using SnapDB.Snap;
using SnapDB.Snap.Filters;
using SnapDB.Snap.Services.Reader;

// ReSharper disable NotAccessedVariable
namespace openHistorian.Core.Data.Query;

/// <summary>
/// Queries a historian database for a set of signals.
/// </summary>
public static class GetSignalMethods
{
    #region [ Static ]

    /// <summary>
    /// Queries all of the signals at the given time.
    /// </summary>
    /// <param name="database">The database to use.</param>
    /// <param name="time">The time to query.</param>
    /// <returns>The signals in the database that correspond to the specified time.</returns>
    public static Dictionary<ulong, SignalDataBase> GetSignals(this IDatabaseReader<HistorianKey, HistorianValue> database, ulong time)
    {
        return database.GetSignals(time, time);
    }

    /// <summary>
    /// Queries all of the signals within a the provided time window [Inclusive]
    /// </summary>
    /// <param name="database">The database to use.</param>
    /// <param name="startTime">the lower bound of the time</param>
    /// <param name="endTime">the upper bound of the time. [Inclusive]</param>
    /// <returns></returns>
    public static Dictionary<ulong, SignalDataBase> GetSignals(this IDatabaseReader<HistorianKey, HistorianValue> database, ulong startTime, ulong endTime)
    {
        HistorianKey key = new();
        HistorianValue hvalue = new();
        Dictionary<ulong, SignalDataBase> results = new();

        TreeStream<HistorianKey, HistorianValue> stream = database.Read(startTime, endTime);
        ulong time, point, quality, value;
        while (stream.Read(key, hvalue))
        {
            time = key.Timestamp;
            point = key.PointID;
            quality = hvalue.Value3;
            value = hvalue.Value1;
            results.AddSignal(time, point, value);
        }

        foreach (SignalDataBase signal in results.Values)
            signal.Completed();

        return results;
    }

    /// <summary>
    /// Queries the provided signals within a the provided time window [Inclusive]
    /// </summary>
    /// <param name="database"></param>
    /// <param name="startTime">the lower bound of the time</param>
    /// <param name="endTime">the upper bound of the time. [Inclusive]</param>
    /// <param name="signals">an IEnumerable of all of the signals to query as part of the results set.</param>
    /// <returns></returns>
    public static Dictionary<ulong, SignalDataBase> GetSignals(this IDatabaseReader<HistorianKey, HistorianValue> database, ulong startTime, ulong endTime, IEnumerable<ulong> signals)
    {
        HistorianKey key = new();
        HistorianValue hvalue = new();
        Dictionary<ulong, SignalDataBase> results = signals.ToDictionary(x => x, x => (SignalDataBase)new SignalDataUnknown());

        TreeStream<HistorianKey, HistorianValue> stream = database.Read(startTime, endTime, signals);
        ulong time, point, quality, value;
        while (stream.Read(key, hvalue))
        {
            time = key.Timestamp;
            point = key.PointID;
            quality = hvalue.Value3;
            value = hvalue.Value1;
            results.AddSignalIfExists(time, point, value);
        }

        foreach (SignalDataBase signal in results.Values)
            signal.Completed();

        return results;
    }

    /// <summary>
    /// Queries the provided signals within a the provided time window [Inclusive]
    /// This method will strong type the signals, but all signals must be of the same type for this to work.
    /// </summary>
    /// <param name="database"></param>
    /// <param name="startTime">the lower bound of the time</param>
    /// <param name="endTime">the upper bound of the time. [Inclusive]</param>
    /// <param name="signals">an IEnumerable of all of the signals to query as part of the results set.</param>
    /// <param name="conversion">a single conversion method to use for all signals</param>
    /// <returns></returns>
    public static Dictionary<ulong, SignalDataBase> GetSignals(this IDatabaseReader<HistorianKey, HistorianValue> database, ulong startTime, ulong endTime, IEnumerable<ulong> signals, TypeBase conversion)
    {
        HistorianKey key = new();
        HistorianValue hvalue = new();
        Dictionary<ulong, SignalDataBase> results = signals.ToDictionary(x => x, x => (SignalDataBase)new SignalData(conversion));

        TreeStream<HistorianKey, HistorianValue> stream = database.Read(startTime, endTime, signals);
        ulong time, point, quality, value;
        while (stream.Read(key, hvalue))
        {
            time = key.Timestamp;
            point = key.PointID;
            quality = hvalue.Value3;
            value = hvalue.Value1;
            results.AddSignalIfExists(time, point, value);
        }

        foreach (SignalDataBase signal in results.Values)
            signal.Completed();

        return results;
    }

    /// <summary>
    /// Queries the provided signals within a the provided time window [Inclusive].
    /// With this method, the signals will be strong typed and therefore can be converted.
    /// </summary>
    /// <param name="database">The database to query.</param>
    /// <param name="startTime">The lower bound of the time.</param>
    /// <param name="endTime">The upper bound of the time [Inclusive].</param>
    /// <param name="signals">An IEnumerable of all of the signals to query as part of the results set.</param>
    /// <returns>The results of the query.</returns>
    public static Dictionary<ulong, SignalDataBase> GetSignals(this IDatabaseReader<HistorianKey, HistorianValue> database, ulong startTime, ulong endTime, IEnumerable<ISignalWithType> signals)
    {
        return database.GetSignals(TimestampSeekFilter.CreateFromRange<HistorianKey>(startTime, endTime), signals, SortedTreeEngineReaderOptions.Default);
    }

    /// <summary>
    /// Retrieves historian signals and their data from the database using specified timestamps, signals, and reader options.
    /// </summary>
    /// <param name="database">The database reader instance used for signal retrieval.</param>
    /// <param name="timestamps">The seek filter for specifying the timestamp range.</param>
    /// <param name="signals">An enumerable collection of signals with type information.</param>
    /// <param name="readerOptions">The reader options for accessing the database.</param>
    /// <returns>A dictionary containing signals (identified by their historian IDs) and corresponding signal data.</returns>
    public static Dictionary<ulong, SignalDataBase> GetSignals(this IDatabaseReader<HistorianKey, HistorianValue> database, SeekFilterBase<HistorianKey> timestamps, IEnumerable<ISignalWithType> signals, SortedTreeEngineReaderOptions readerOptions)
    {
        Dictionary<ulong, SignalDataBase> results = new();

        foreach (ISignalWithType pt in signals)
        {
            if (pt.HistorianId.HasValue)
                if (!results.ContainsKey(pt.HistorianId.Value))
                    results.Add(pt.HistorianId.Value, new SignalData(pt.Functions));
        }

        HistorianKey key = new();
        HistorianValue hvalue = new();
        MatchFilterBase<HistorianKey, HistorianValue> keyParser = PointIDMatchFilter.CreateFromList<HistorianKey, HistorianValue>(signals.Where(x => x.HistorianId.HasValue).Select(x => x.HistorianId.Value));
        TreeStream<HistorianKey, HistorianValue> stream = database.Read(readerOptions, timestamps, keyParser);
        ulong time, point, quality, value;
        while (stream.Read(key, hvalue))
        {
            time = key.Timestamp;
            point = key.PointID;
            quality = hvalue.Value3;
            value = hvalue.Value1;
            results.AddSignalIfExists(time, point, value);
        }

        foreach (SignalDataBase signal in results.Values)
            signal.Completed();
        return results;
    }

    /// <summary>
    /// Adds the following signal to the dictionary. If the signal is
    /// not part of the dictionary, it is added automatically.
    /// </summary>
    /// <param name="results">The results from the dictionary</param>
    /// <param name="time">The time associated with the signal.</param>
    /// <param name="point">The point associated with the signal.</param>
    /// <param name="value">The value associated with the signal.</param>
    private static void AddSignal(this Dictionary<ulong, SignalDataBase> results, ulong time, ulong point, ulong value)
    {
        if (!results.TryGetValue(point, out SignalDataBase signalData))
        {
            signalData = new SignalDataUnknown();
            results.Add(point, signalData);
        }

        signalData.AddDataRaw(time, value);
    }

    /// <summary>
    /// Adds the provided signal to the dictionary unless the signal is not
    /// already part of the dictionary.
    /// </summary>
    /// <param name="results">The results from the dictionary.</param>
    /// <param name="time">The time associated with the signal.</param>
    /// <param name="point">The point associated with the signal.</param>
    /// <param name="value">The value associated with the signal.</param>
    private static void AddSignalIfExists(this Dictionary<ulong, SignalDataBase> results, ulong time, ulong point, ulong value)
    {
        if (results.TryGetValue(point, out SignalDataBase signalData))
            signalData.AddDataRaw(time, value);
    }

    #endregion
}