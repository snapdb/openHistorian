//******************************************************************************************************
//  HistorianOperationsController.cs - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
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
//  06/07/2016 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System.Collections.Concurrent;
using System.Globalization;
using Gemstone;
using Gemstone.ActionExtensions;
using Microsoft.AspNetCore.Mvc;
using openHistorian.Adapters;
using openHistorian.Model;
using openHistorian.Net;
using openHistorian.Snap;
using SnapDB.Snap.Services;
using CancellationToken = Gemstone.Threading.Cancellation.CancellationToken;
using Random = Gemstone.Security.Cryptography.Random;

namespace openHistorian.WebUI.Controllers;

/// <summary>
/// Defines enumeration of supported timestamp representations.
/// </summary>
public enum TimestampType
{
    /// <summary>
    /// Timestamps are in Ticks.
    /// </summary>
    Ticks,
    /// <summary>
    /// Timestamps are in Unix milliseconds.
    /// </summary>
    UnixMilliseconds,
    /// <summary>
    /// Timestamps are in Unix seconds.
    /// </summary>
    UnixSeconds
}

/// <summary>
/// Represents the current operational state for a given historian operation, e.g., read or write.
/// </summary>
public class HistorianOperationState
{
    private long m_stopTime;

    /// <summary>
    /// Gets or sets the cancellation token for a historian operation.
    /// </summary>
    public CancellationToken CancellationToken { get; } = new();

    /// <summary>
    /// Gets or sets associated operation handle.
    /// </summary>
    public uint OperationHandle { get; set; }

    /// <summary>
    /// Gets or sets progress that represents number of completed operations.
    /// </summary>
    public long Progress { get; set; }

    /// <summary>
    /// Gets or sets total number of historian operations to be completed.
    /// </summary>
    public long Total { get; set; }

    /// <summary>
    /// Gets or sets flag that indicates if write operation has successfully completed.
    /// </summary>
    public bool Completed { get; set; }

    /// <summary>
    /// Gets or sets flag that indicates if write operation has failed.
    /// </summary>
    public bool Failed { get; set; }

    /// <summary>
    /// Gets or sets failure reason message when <see cref="Failed"/> is <c>true</c>.
    /// </summary>
    public string? FailedReason { get; set; }

    /// <summary>
    /// Gets or sets the start time, in Ticks, of the operation.
    /// </summary>
    public long StartTime { get; set; }

    /// <summary>
    /// Gets or sets the stop time, in Ticks, of the operation.
    /// </summary>
    public long StopTime
    {
        get => m_stopTime > 0 ? m_stopTime : DateTime.UtcNow.Ticks;
        set => m_stopTime = value;
    }

    /// <summary>
    /// Gets the calculated operation rate, in operations per second.
    /// </summary>
    public long OperationRate => (long)(Progress / new Ticks(StopTime - StartTime).ToSeconds());

    /// <summary>
    /// Gets the estimated remaining import time as an elapsed time string.
    /// </summary>
    public string RemainingTimeEstimate => Ticks.FromSeconds((Total - Progress) / (double)OperationRate).ToElapsedTimeString(0);

    /// <summary>
    /// Gets total operation time as an elapsed time string.
    /// </summary>
    public string TotalOperationTime => new Ticks(StopTime - StartTime).ToElapsedTimeString(2);

    /// <summary>
    /// Gets or sets target export name.
    /// </summary>
    public string? TargetExportName { get; set; }

    /// <summary>
    /// Gets or sets end sample count.
    /// </summary>
    public long EndSampleCount { get; set; }

    /// <summary>
    /// Gets or sets binary byte count.
    /// </summary>
    public long BinaryByteCount { get; set; }
}

/// <summary>
/// Represents a client instance of a SignalR Hub for historian data operations.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class HistorianOperationsController : Controller
{
    #region [ Methods ]

    /// <summary>
    /// Gets loaded historian adapter instance names.
    /// </summary>
    /// <returns>Historian adapter instance names.</returns>
    [HttpGet]
    public IEnumerable<string> GetInstanceNames()
    {
        return TrendValueAPI.GetInstanceNames();
    }

    /// <summary>
    /// Generates a new cache ID that can be used to identify a specific operation or data set.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public string GenerateCacheID()
    {
        return Guid.NewGuid().ToString();
    }

    /// <summary>
    /// Begins a new data export operation for the specified time range using the provided date time format.
    /// </summary>
    /// <param name="startTime">Start time as Unix timestamp/></param>   
    /// <param name="endTime">End time as Unix timestamp/></param>
    /// <returns>New operational state handle.</returns>
    [HttpGet, Route("BeginDataExport")]
    public uint BeginDataExport(string startTime, string endTime)
    {
        long startTicks, endTicks;

        try
        {
            startTicks = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(startTime, CultureInfo.InvariantCulture)).Ticks;
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Cannot export data: failed to parse \"{nameof(startTime)}\" parameter value \"{startTime}\": {ex.Message}", nameof(startTime), ex);
        }

        try
        {
           endTicks = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(endTime, CultureInfo.InvariantCulture)).Ticks;
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Cannot export data: failed to parse \"{nameof(endTime)}\" parameter value \"{endTime}\": {ex.Message}", nameof(endTime), ex);
        }

        if (startTicks > endTicks)
            throw new ArgumentOutOfRangeException(nameof(startTime), "Cannot export data: start time exceeds end time.");

        return BeginHistorianRead(endTicks - startTicks);
    }

    /// <summary>
    /// Begins a new historian read operation.
    /// </summary>
    /// <param name="totalValues">Total values or timespan to read, if known in advance.</param>
    /// <returns>New operational state handle.</returns>
    [HttpGet]
    public uint BeginHistorianRead(long totalValues = 0)
    {
        return AddNewHistorianOperationState(new HistorianOperationState { Total = totalValues });
    }

    /// <summary>
    /// Begins a new historian write operation.
    /// </summary>
    /// <param name="instanceName">Historian instance name.</param>
    /// <param name="values">Enumeration of <see cref="TrendValue"/> instances to write.</param>
    /// <param name="totalValues">Total values or timespan to write, if known in advance.</param>
    /// <param name="timestampType">Type of timestamps.</param>
    /// <returns>New operational state handle.</returns>
    [HttpPost]
    public uint BeginHistorianWrite(string instanceName, [FromBody] IEnumerable<TrendValue> values, long totalValues = 0, TimestampType timestampType = TimestampType.UnixMilliseconds)
    {
        HistorianOperationState operationState = new() { Total = totalValues };
        uint operationHandle = AddNewHistorianOperationState(operationState);

        new Thread(() =>
        {
            operationState.StartTime = DateTime.UtcNow.Ticks;

            try
            {
                SnapServer? server = GetServer(instanceName)?.Host;

                if (server is null)
                    throw new InvalidOperationException($"Server is null for instance [{instanceName}].");

                using SnapClient connection = SnapClient.Connect(server);
                using ClientDatabaseBase<HistorianKey, HistorianValue>? database = connection.GetDatabase<HistorianKey, HistorianValue>(instanceName);

                if (database is null)
                    throw new InvalidOperationException($"Database is null for instance [{instanceName}].");

                HistorianKey key = new();
                HistorianValue value = new();

                foreach (TrendValue trendValue in values)
                {
                    key.PointID = (ulong)trendValue.ID;
                    key.Timestamp = timestampType switch
                    {
                        TimestampType.Ticks => (ulong)trendValue.Timestamp,
                        TimestampType.UnixSeconds => (ulong)trendValue.Timestamp * 10000000UL + 621355968000000000UL,
                        TimestampType.UnixMilliseconds => (ulong)trendValue.Timestamp * 10000UL + 621355968000000000UL,
                        _ => key.Timestamp
                    };

                    value.AsSingle = (float)trendValue.Value;

                    database.Write(key, value);
                    operationState.Progress++;

                    if (operationState.CancellationToken.IsCancelled)
                        break;
                }

                operationState.Completed = !operationState.CancellationToken.IsCancelled;
            }
            catch (Exception ex)
            {
                operationState.Failed = true;
                operationState.FailedReason = ex.Message;
            }

            // Schedule operation handle to be removed
            CancelHistorianOperation(operationHandle);

            operationState.StopTime = DateTime.UtcNow.Ticks;
        })
        {
            IsBackground = true
        }
        .Start();

        return operationHandle;
    }

    /// <summary>
    /// Gets current historian operation state for specified handle.
    /// </summary>
    /// <param name="operationHandle">Handle to historian operation state.</param>
    /// <returns>Current historian write operation state.</returns>
    [HttpGet, Route("GetHistorianOperationState")]
    public HistorianOperationState GetHistorianOperationState(uint operationHandle)
    {
        if (s_historianOperationStates.TryGetValue(operationHandle, out HistorianOperationState? operationState))
            return operationState;

        // Returned a cancelled operation state if operation handle was not found
        operationState = new HistorianOperationState();
        operationState.CancellationToken.Cancel();

        return operationState;
    }

    /// <summary>
    /// Cancels a historian operation.
    /// </summary>
    /// <param name="operationHandle">Handle to historian operation state.</param>
    /// <returns><c>true</c> if operation was successfully terminated; otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// Any <see cref="HistorianOperationState"/> associated with the <paramref name="operationHandle"/>
    /// will remain available for query from the <see cref="GetHistorianOperationState"/> for 30 seconds after
    /// successful cancellation of the operation.
    /// </remarks>
    [HttpGet]
    public bool CancelHistorianOperation(uint operationHandle)
    {
        if (!s_historianOperationStates.TryGetValue(operationHandle, out HistorianOperationState? operationState))
            return false;

        // Immediately signal for operation cancellation
        operationState.CancellationToken.Cancel();

        // Schedule operation handle to be removed after about 30 seconds
        Action action = () => s_historianOperationStates.TryRemove(operationHandle, out operationState);
        action.DelayAndExecute(30000);

        return true;
    }

    /// <summary>
    /// Cancel all historian operations and release resources.
    /// </summary>
    // TODO: Make method admin only
    [HttpGet]
    public void CancelAllHistorianOperations()
    {
        HistorianOperationState[] operationStates = s_historianOperationStates.Values.ToArray();
        s_historianOperationStates.Clear();

        foreach (HistorianOperationState operationState in operationStates)
            operationState.CancellationToken.Cancel();
    }

    ///// <summary>
    ///// Read historian data from server. Function is not reentrant will cancel any previous read operation.
    ///// </summary>
    ///// <param name="instanceName">Historian instance name.</param>
    ///// <param name="startTime">Start time of query.</param>
    ///// <param name="stopTime">Stop time of query.</param>
    ///// <param name="measurementIDs">Measurement IDs to query - or <c>null</c> for all available points.</param>
    ///// <param name="resolution">Resolution for data query.</param>
    ///// <param name="seriesLimit">Maximum number of points per series.</param>
    ///// <param name="forceLimit">Flag that determines if series limit should be strictly enforced.</param>
    ///// <param name="timestampType">Type of timestamps.</param>
    ///// <returns>Enumeration of <see cref="TrendValue"/> instances read for time range.</returns>
    //// Update: Modify to be async and reentrant and allow multiple read operations to be in progress at the same time
    //public IEnumerable<TrendValue> GetHistorianData(string instanceName, DateTime startTime, DateTime stopTime, ulong[] measurementIDs, Resolution resolution, int seriesLimit, bool forceLimit, TimestampType timestampType = TimestampType.UnixMilliseconds)
    //{
    //    // Cancel any running operation
    //    CancellationToken cancellationToken = new();
    //    Interlocked.Exchange(ref m_readCancellationToken, cancellationToken)?.Cancel();

    //    SnapServer? server = GetServer(instanceName)?.Host;
    //    IEnumerable<TrendValue> values = TrendValueAPI.GetHistorianData(server, instanceName, startTime, stopTime, measurementIDs, resolution, seriesLimit, forceLimit, cancellationToken);

    //    return timestampType switch
    //    {
    //        TimestampType.Ticks => values.Select(value =>
    //        {
    //            value.Timestamp = value.Timestamp * 10000.0D + 621355968000000000.0D;
    //            return value;
    //        }),
    //        TimestampType.UnixSeconds => values.Select(value =>
    //        {
    //            value.Timestamp /= 1000.0D;
    //            return value;
    //        }),
    //        _ => values
    //    };
    //}

    #endregion

    #region [ Static ]

    private static readonly ConcurrentDictionary<uint, HistorianOperationState> s_historianOperationStates;

    /// <summary>
    /// Creates a new <see cref="HistorianOperationsController"/>.
    /// </summary>
    static HistorianOperationsController()
    {
        s_historianOperationStates = new ConcurrentDictionary<uint, HistorianOperationState>();
    }

    private static uint AddNewHistorianOperationState(HistorianOperationState operationState)
    {
        uint operationHandle = Random.UInt32;

        while (!s_historianOperationStates.TryAdd(operationHandle, operationState))
            operationHandle = Random.UInt32;

        operationState.OperationHandle = operationHandle;

        return operationHandle;
    }

    private static HistorianServer? GetServer(string instanceName)
    {
        return LocalOutputAdapter.Instances.TryGetValue(instanceName, out LocalOutputAdapter? historianAdapter) ? historianAdapter.Server : null;
    }

    #endregion
}