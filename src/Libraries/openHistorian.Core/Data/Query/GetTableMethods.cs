//******************************************************************************************************
//  GetTableMethods.cs - Gbtc
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
using SnapDB.Snap.Services;
using System.Data;

namespace openHistorian.Core.Data.Query;

/// <summary>
/// Represents a collection of signals that are delinearized for querying historian data.
/// </summary>
public interface IDelinearizedSignals
{
    /// <summary>
    /// Gets the string form column headers corresponding to the delinearized signals.
    /// </summary>
    IList<string> ColumnHeaders
    {
        get;
    }

    /// <summary>
    /// Gets the key-value pairs of the delinearized signals.
    /// </summary>
    KeyValuePair<object, IList<ISignalWithType>> ColumnGroups
    {
        get;
    }
}

/// <summary>
/// Creates a relationship between the signal associated with a name and the signal associated with a type.
/// </summary>
public interface ISignalWithName : ISignalWithType
{
    /// <summary>
    /// The name for the signal of a certain type.
    /// </summary>
    string TagName
    {
        get;
    }
}

/// <summary>
/// Provides extension methods for retrieving historian data as a DataTable.
/// </summary>
public static class GetTableMethods
{
    /// <summary>
    /// Gets the DataTable from the historian database using delinearized signals
    /// </summary>
    /// <param name="database">The database reader instance used for data retrieval.</param>
    /// <param name="start">The starting timestamp of the data range.</param>
    /// <param name="stop">The ending timestamp of the data range.</param>
    /// <param name="signals">The delinearized signals to retrieve the data for.</param>
    /// <returns>A DataTable containing historian data based on the provided delinearized signals.</returns>
    public static DataTable GetTable(this ClientDatabaseBase<HistorianKey, HistorianValue> database, ulong start, ulong stop, IDelinearizedSignals signals)
    {
        return null;
    }

    /// <summary>
    /// Gets a DataTable from the historian database using a list of signals with names.
    /// </summary>
    /// <param name="database">The database reader instance used for data retrieval.</param>
    /// <param name="start">The start timestamp for the data range.</param>
    /// <param name="stop">The stop timestamp for the data range.</param>
    /// <param name="columns">A list of signals with names to retrieve data for.</param>
    /// <returns>A DataTable containing historian data based on the provided signals with names.</returns>
    /// <exception cref="Exception">Thrown if columns do not exist in the historian, thus causing the function not to work.</exception>
    public static DataTable GetTable(this ClientDatabaseBase<HistorianKey, HistorianValue> database, ulong start, ulong stop, IList<ISignalWithName> columns)
    {
        if (columns.Any((x) => !x.HistorianId.HasValue))
            throw new Exception("All columns must be contained in the historian for this function to work.");

        Dictionary<ulong, SignalDataBase> results = database.GetSignals(start, stop, columns);
        int[] columnPosition = new int[columns.Count];
        object[] rowValues = new object[columns.Count + 1];
        SignalDataBase[] signals = new SignalDataBase[columns.Count];

        DataTable table = new();
        table.Columns.Add("Time", typeof(DateTime));
        foreach (ISignalWithName signal in columns)
            table.Columns.Add(signal.TagName, typeof(double));

        for (int x = 0; x < columns.Count; x++)
            signals[x] = results[columns[x].HistorianId.Value];

        while (true)
        {
            ulong minDate = ulong.MaxValue;
            for (int x = 0; x < columns.Count; x++)
            {
                SignalDataBase signal = signals[x];
                if (signal.Count < columnPosition[x])
                    minDate = Math.Min(minDate, signals[x].GetDate(columnPosition[x]));
            }

            rowValues[0] = null;
            for (int x = 0; x < columns.Count; x++)
            {
                SignalDataBase signal = signals[x];
                if (signal.Count < columnPosition[x] && minDate == signals[x].GetDate(columnPosition[x]))
                {
                    signals[x].GetData(columnPosition[x], out ulong date, out double value);
                    rowValues[x + 1] = value;
                    columnPosition[x]++;
                }
                else
                {
                    rowValues[x + 1] = null;
                }
            }

            if (minDate == ulong.MaxValue && rowValues.All((x) => x is null))
                return table;

            rowValues[0] = minDate;

            table.Rows.Add(rowValues);
        }
    }
}