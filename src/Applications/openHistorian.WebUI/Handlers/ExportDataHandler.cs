//******************************************************************************************************
//  ExportDataHandler.ashx.cs - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
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
//  08/22/2016 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Runtime.Caching;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Gemstone;
using Gemstone.Collections.CollectionExtensions;
using Gemstone.COMTRADE;
using Gemstone.Configuration;
using Gemstone.Data;
using Gemstone.Data.Model;
using Gemstone.Diagnostics;
using Gemstone.IO;
using Gemstone.StringExtensions;
using Gemstone.Timeseries.Model;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Primitives;
using openHistorian.Adapters;
using openHistorian.Net;
using openHistorian.Snap;
using openHistorian.WebUI.Controllers;
using SnapDB.Snap;
using SnapDB.Snap.Filters;
using SnapDB.Snap.Services;
using SnapDB.Snap.Services.Reader;
using Measurement = Gemstone.Timeseries.Model.Measurement;
using EESignalType = Gemstone.Numeric.EE.SignalType;
using Schema = Gemstone.COMTRADE.Schema;
using SignalType = openHistorian.Model.SignalType;

// ReSharper disable LocalizableElement
// ReSharper disable once CheckNamespace
// ReSharper disable NotResolvedInText
// ReSharper disable AccessToDisposedClosure
namespace openHistorian;

/// <summary>
/// Handles downloading of exported historian data.
/// </summary>
public class ExportDataHandler
{
    #region [ Members ]

    // Nested Types
    private class ExportSettings
    {
        public long[] PointIDs = null!;
        public string StartTime = null!;
        public string EndTime = null!;
        public int FileFormat;
        public string FileName = null!;
        public string TimeFormat = null!;
        public string InstanceName = null!;
        public bool AlignTimestamps;
        public bool MissingAsNaN;
        public bool FillMissingTimestamps;
        public bool TimestampSnap;
        public bool UseCFF;
        public string FirstTimestampBasedOn = null!; // "frame-rate-starting-at-top-of-second" | "first-available-measurement" | "exact-start-time"
        public string ColumnHeaders = null!;
        public string FrameRateUnit = null!; // "frames-per-second" | "frames-per-minute" | "frames-per-hour"
        public double FrameRate;
        public double Tolerance;
        public long OperationHandle; // Operation handle from '/api/HistorianOperations/BeginDataExport' for tracking export operation state, or zero to skip operation tracking
        public string HeaderCacheID = null!; // Randomly generated ID, e.g., Guid string, to identify cached COMTRADE header data for download after export
    }

    private class PointMetadata
    {
        public ulong[] PointIDs = null!;
        public Measurement[] Measurements = null!;
        public string StationName = null!;
        public string DeviceID = null!;
        public string TargetDeviceName = null!;
        public int TargetQualityFlagsID;
    }

    private class HeaderData
    {
        public required string FileImage;
        private readonly string? m_fileName;

        public string FileName
        {
            get => m_fileName ?? throw new InvalidOperationException("FileName is not set.");
            init => m_fileName = $"{Path.GetFileNameWithoutExtension(value)}.cfg";
        }
    }

    // Constants
    private const string CsvContentType = "text/csv";
    private const string TextContentType = "text/plain";
    private const string BinaryContentType = "application/octet-stream";
    private const int TargetBufferSize = 524288;

    #endregion

    #region [ Constuctors ]

    /// <summary>
    /// Constructs a new instance of the <see cref="ExportDataHandler"/>.
    /// </summary>
    public ExportDataHandler(RequestDelegate _) {}

    #endregion

    #region [ Methods ]

    /// <summary>
    /// Invokes the <see cref="ExportDataHandler"/> to process an HTTP request.
    /// </summary>
    /// <param name="context">HTTP context to process.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task Invoke(HttpContext context)
    {
        return ProcessRequestAsync(context.Request, context.Response, context.User, context.RequestAborted);
    }

    /// <summary>
    /// Enables processing of HTTP web requests by a custom handler.
    /// </summary>
    /// <param name="request">HTTP request.</param>
    /// <param name="response">HTTP response.</param>
    /// <param name="securityPrincipal">Security principal of the user making the request.</param>
    /// <param name="cancellationToken">Propagates notification from client that operations should be canceled.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ProcessRequestAsync(HttpRequest request, HttpResponse response, ClaimsPrincipal securityPrincipal, CancellationToken cancellationToken)
    {
        NameValueCollection requestParameters = new Uri(request.GetEncodedUrl()).ParseQueryString();

        if (request.Method.Equals("Post", StringComparison.OrdinalIgnoreCase))
        {
            using StreamReader reader = new(request.Body);
            string content = await reader.ReadToEndAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(content))
                throw new InvalidOperationException("Cannot export data: no export settings JSON or point ID values were provided in the request body.");

            try
            {
                // Attempt to deserialize export settings from JSON content in request body
                ExportSettings? settings = JsonSerializer.Deserialize<ExportSettings>(content);
                
                if (settings == null)
                    throw new InvalidOperationException("Cannot export data: export settings could not be deserialized.");

                if (string.IsNullOrWhiteSpace(settings.FileName))
                    settings.FileName = "Export";

                settings.FileName = $"{settings.FileName}.{(settings.FileFormat < 0 ? "csv" : settings.UseCFF ? "cff" : "dat")}";

                response.Headers.ContentType = settings.FileFormat switch
                {
                    -1 => CsvContentType,   // CSV
                    0 => TextContentType,   // COMTRADE ASCII
                    _ => BinaryContentType  // COMTRADE Binary
                };

                response.Headers.ContentDisposition = new StringValues($"attachment; filename=\"{settings.FileName}\"");

                PointMetadata metadata;

                try
                {
                    metadata = CreatePointMetadata(securityPrincipal, settings.PointIDs.Select(id => (ulong)id).ToArray());
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Cannot export data: failed to parse \"PointIDs\" value: {ex.Message}", ex);
                }

                double frameRate = settings.FrameRateUnit switch 
                {
                    "frames-per-second" => settings.FrameRate,
                    "frames-per-minute" => settings.FrameRate / 60D,
                    "frames-per-hour" => settings.FrameRate / 3600D,
                    _ => settings.FrameRate
                };

                requestParameters["TimeFormat"] = settings.TimeFormat;
                requestParameters["StartTime"] = settings.StartTime;
                requestParameters["EndTime"] = settings.EndTime;
                requestParameters["FrameRate"] = $"{frameRate}";
                requestParameters["AlignTimestamps"] = settings.AlignTimestamps.ToString();
                requestParameters["MissingAsNaN"] = settings.MissingAsNaN.ToString();
                requestParameters["FillMissingTimestamps"] = settings.FillMissingTimestamps.ToString();
                requestParameters["InstanceName"] = settings.InstanceName;
                requestParameters["TimestampSnap"] = $"{settings.TimestampSnap}";
                requestParameters["Tolerance"] = $"{settings.Tolerance}";
                requestParameters["FirstTimestampBasedOn"] = settings.FirstTimestampBasedOn;
                requestParameters["ColumnHeaders"] = settings.ColumnHeaders;
                requestParameters["HeaderCacheID"] = settings.HeaderCacheID;

                if (settings.OperationHandle > 0L)
                    requestParameters["OperationHandle"] = $"{settings.OperationHandle}";

                await ExportToStreamAsync(settings.FileFormat, settings.UseCFF, settings.FileName, metadata, requestParameters, response.Body, cancellationToken);

            }
            catch (Exception ex)
            {
                Logger.SwallowException(ex, "Failed to process POST body request as 'ExportSettings' JSON, attempting to parse body as point ID list", nameof(ProcessRequestAsync));

                // Fall back to original point ID parsing operation if JSON deserialization fails which assumes that
                // initial post request is all point IDs to query, to be cached by key on server. This operation allows
                // for very large point ID selection posts that could otherwise exceed URI parameter string limits.
                ulong[] pointIDs = content.Split(',').Select(ulong.Parse).ToArray();
                Array.Sort(pointIDs);

                response.StatusCode = (int)HttpStatusCode.OK;
                await response.WriteAsync(CachePointMetadata(securityPrincipal, pointIDs), cancellationToken);
            }
        }
        else // Assuming GET request for other operations
        {
            if (requestParameters["ExportCachedHeader"]?.ParseBoolean() ?? false)
            {
                // Header export, e.g., cached CFG file
                HeaderData? header = GetCachedHeaderData(requestParameters["HeaderCacheID"]);

                if (header is not null)
                {
                    string fileName = requestParameters["FileName"] ?? header.FileName;

                    if (string.IsNullOrWhiteSpace(fileName))
                        fileName = "Export.cfg";

                    response.ContentType = TextContentType;
                    response.Headers.ContentDisposition = new StringValues($"attachment; filename=\"{fileName}\"");
                    
                    await response.WriteAsync(header.FileImage, cancellationToken);
                }
            }
            else
            {
                // Data export
                string? cacheIDParam = requestParameters["CacheID"];
                string? pointIDsParam = requestParameters["PointIDs"];
                PointMetadata? metadata;

                if (string.IsNullOrEmpty(cacheIDParam) && string.IsNullOrEmpty(pointIDsParam))
                    throw new ArgumentException("Cannot export data: no point ID values can be accessed. Neither the \"CacheID\" nor the \"PointIDs\" parameter was provided.");

                if (string.IsNullOrEmpty(pointIDsParam))
                {
                    metadata = GetCachedPointMetadata(cacheIDParam);

                    if (metadata is null)
                        throw new ArgumentNullException("CacheID", $"Cannot export data: failed to load cached point metadata referenced by \"CacheID\" parameter value \"{cacheIDParam}\".");
                }
                else
                {
                    try
                    {
                        ulong[] pointIDs = pointIDsParam.Split(',').Select(ulong.Parse).ToArray();
                        Array.Sort(pointIDs);
                        metadata = CreatePointMetadata(securityPrincipal, pointIDs);
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentNullException("PointIDs", $"Cannot export data: failed to parse \"PointIDs\" parameter value \"{pointIDsParam}\": {ex.Message}");
                    }
                }

                if (!int.TryParse(requestParameters["FileFormat"], out int fileFormat))
                    fileFormat = -1; // Default to CSV

                bool useCFF = requestParameters["UseCFF"]?.ParseBoolean() ?? false;
                string fileName = requestParameters["FileName"] ?? $"{metadata.TargetDeviceName}";

                if (string.IsNullOrWhiteSpace(fileName))
                    fileName = "Export";

                fileName = $"{fileName}.{(fileFormat < 0 ? "csv" : useCFF ? "cff" : "dat")}";

                response.Headers.ContentType = fileFormat switch
                {
                    -1 => CsvContentType,   // CSV
                    0 => TextContentType,   // COMTRADE ASCII
                    _ => BinaryContentType  // COMTRADE Binary
                };
                
                response.Headers.ContentDisposition = new StringValues($"attachment; filename=\"{fileName}\"");

                await ExportToStreamAsync(fileFormat, useCFF, fileName, metadata, requestParameters, response.Body, cancellationToken);
            }
        }
    }

    private static async Task ExportToStreamAsync(int fileFormat, bool useCFF, string fileName, PointMetadata metadata, NameValueCollection requestParameters, Stream responseStream, CancellationToken cancellationToken)
    {
        // See if operation state for this export can be found
        string? operationHandleParam = requestParameters["OperationHandle"];

        HistorianOperationState? operationState = null;
        Action? completeHistorianOperation = null;

        if (uint.TryParse(operationHandleParam, out uint operationHandle))
        {
            HistorianOperationsController hubClient = new();
            operationState = hubClient.GetHistorianOperationState(operationHandle);

            if (operationState.CancellationToken.IsCancelled)
            {
                operationState = null;
            }
            else
            {
                operationState.StartTime = DateTime.UtcNow.Ticks;
                operationState.TargetExportName = fileName;

                completeHistorianOperation = () =>
                {
                    operationState.Completed = !operationState.CancellationToken.IsCancelled;
                    operationState.StopTime = DateTime.UtcNow.Ticks;
                    hubClient.CancelHistorianOperation(operationHandle);
                };
            }
        }

        try
        {
            const double DefaultFrameRate = 30D;
            const int DefaultTimestampSnap = 0;
            const double DefaultTolerance = 0.5D;

            string dateTimeFormat = requestParameters["TimeFormat"] ?? TimeTagBase.DefaultFormat;
            string? startTimestampParam = requestParameters["StartTime"];
            string? endTimestampParam = requestParameters["EndTime"];
            string? frameRateParam = requestParameters["FrameRate"];
            string? alignTimestampsParam = requestParameters["AlignTimestamps"];
            string? missingAsNaNParam = requestParameters["MissingAsNaN"];
            string? fillMissingTimestampsParam = requestParameters["FillMissingTimestamps"];
            string? instanceName = requestParameters["InstanceName"];
            string? timestampSnapParam = requestParameters["TimestampSnap"];
            string? toleranceParam = requestParameters["Tolerance"]; // In milliseconds
            string? columnHeaders = requestParameters["ColumnHeaders"];

            // TODO: Implement support for "FirstTimestampBasedOn" parameter

            if (string.IsNullOrEmpty(startTimestampParam))
                throw new ArgumentNullException("StartTime", "Cannot export data: no \"StartTime\" parameter value was specified.");

            if (string.IsNullOrEmpty(endTimestampParam))
                throw new ArgumentNullException("EndTime", "Cannot export data: no \"EndTime\" parameter value was specified.");

            DateTime startTime, endTime;

            try
            {
                startTime = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(startTimestampParam)).UtcDateTime;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Cannot export data: failed to parse \"StartTime\" parameter value \"{startTimestampParam}\" with a format of \"{dateTimeFormat}\". Error message: {ex.Message}", "StartTime", ex);
            }

            try
            {
                endTime = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(endTimestampParam)).UtcDateTime;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Cannot export data: failed to parse \"EndTime\" parameter value \"{endTimestampParam}\" with a format of \"{dateTimeFormat}\". Error message: {ex.Message}", "EndTime", ex);
            }

            if (startTime > endTime)
                throw new ArgumentOutOfRangeException("StartTime", "Cannot export data: start time exceeds end time.");

            FileType? fileType = null;

            if (fileFormat > -1)
                fileType = (FileType)fileFormat;

            Dictionary<ulong, int> pointIDIndex = new(metadata.PointIDs.Length);
            byte[]? headers = GetHeaders(fileType, useCFF, metadata, pointIDIndex, startTime, columnHeaders, out Schema? schema);

            if (fileFormat > -1 && !useCFF && schema is null)
                throw new InvalidOperationException($"Cannot export data: failed to create schema for COMTRADE file format {fileType}.");

            if (!double.TryParse(frameRateParam, out double frameRate))
                frameRate = DefaultFrameRate;

            if (!int.TryParse(timestampSnapParam, out int timestampSnap))
                timestampSnap = DefaultTimestampSnap;

            if (!double.TryParse(toleranceParam, out double tolerance))
                tolerance = DefaultTolerance;

            int toleranceTicks = (int)Math.Ceiling(tolerance * Ticks.PerMillisecond);
            bool alignTimestamps = alignTimestampsParam?.ParseBoolean() ?? true;
            bool missingAsNaN = missingAsNaNParam?.ParseBoolean() ?? true;
            bool fillMissingTimestamps = alignTimestamps && (fillMissingTimestampsParam?.ParseBoolean() ?? false);

            if (string.IsNullOrEmpty(instanceName))
                instanceName = TrendValueAPI.DefaultInstanceName ?? "PPA";

            LocalOutputAdapter.Instances.TryGetValue(instanceName, out LocalOutputAdapter? adapter);
            HistorianServer? serverInstance = adapter?.Server;

            if (serverInstance is null)
                throw new InvalidOperationException($"Cannot export data: failed to access internal historian server instance \"{instanceName}\".");

            ManualResetEventSlim bufferReady = new(false);
            BlockAllocatedMemoryStream writeBuffer = new();
            bool[] readComplete = [false];

            Task readTask = ReadTask(fileType, schema, serverInstance, instanceName, metadata, pointIDIndex, startTime, endTime, writeBuffer, bufferReady, frameRate, missingAsNaN, timestampSnap, alignTimestamps, toleranceTicks, fillMissingTimestamps, dateTimeFormat, readComplete, operationState, cancellationToken);
            Task writeTask = WriteTask(responseStream, headers, writeBuffer, bufferReady, readComplete, operationState, completeHistorianOperation, cancellationToken);

            await Task.WhenAll(writeTask, readTask);

            if (fileFormat > -1 && !useCFF)
                CacheHeaderData(requestParameters["HeaderCacheID"], schema!.FileImage, fileName, operationState?.EndSampleCount ?? 0L);
        }
        catch (Exception ex)
        {
            if (operationState is not null)
            {
                operationState.Failed = true;
                operationState.FailedReason = ex.Message;
            }

            throw;
        }
    }

    private static Task ReadTask(FileType? fileType, Schema? schema, HistorianServer serverInstance, string instanceName, PointMetadata metadata, Dictionary<ulong, int> pointIDIndex, DateTime startTime, DateTime endTime, BlockAllocatedMemoryStream writeBuffer, ManualResetEventSlim bufferReady, double frameRate, bool missingAsNaN, int timestampSnap, bool alignTimestamps, int toleranceTicks, bool fillMissingTimestamps, string dateTimeFormat, bool[] readComplete, HistorianOperationState? operationState, CancellationToken cancellationToken)
    {
        return Task.Factory.StartNew(() =>
        {
            uint sample = 0U;

            try
            {
                using SnapClient connection = SnapClient.Connect(serverInstance.Host);
                BlockAllocatedMemoryStream readBuffer = new();
                StreamWriter readBufferWriter = new(readBuffer) { NewLine = Writer.CRLF };
                int valueCount = metadata.PointIDs.Length;

                if (fileType is not null && metadata.TargetQualityFlagsID > 0)
                    valueCount--;

                double[] values = new double[valueCount];

                for (int i = 0; i < values.Length; i++)
                    values[i] = double.NaN;

                ulong interval;

                if (Math.Abs(frameRate % 1) <= double.Epsilon * 100)
                {
                    Ticks[] subseconds = Ticks.SubsecondDistribution((int)frameRate);
                    interval = (ulong)(subseconds.Length > 1 ? subseconds[1].Value : Ticks.PerSecond);
                }
                else
                {
                    interval = (ulong)(Math.Floor(1.0d / frameRate) * Ticks.PerSecond);
                }

                ulong lastTimestamp = 0;

                // Write data pages
                SeekFilterBase<HistorianKey> timeFilter = TimestampSeekFilter.CreateFromRange<HistorianKey>(startTime, endTime);
                MatchFilterBase<HistorianKey, HistorianValue> pointFilter = PointIDMatchFilter.CreateFromList<HistorianKey, HistorianValue>(metadata.PointIDs);
                HistorianKey historianKey = new();
                HistorianValue historianValue = new();
                ushort fracSecValue = 0;

                // Write row values function
                void bufferValues(DateTime recordTimestamp)
                {
                    // Schema nullability check validated prior to this method call
                    switch (fileType)
                    {
                        case FileType.Ascii:
                            Writer.WriteNextRecordAscii(readBufferWriter, schema!, recordTimestamp, values, sample++, true, fracSecValue);
                            break;
                        case FileType.Binary:
                            Writer.WriteNextRecordBinary(readBuffer, schema!, recordTimestamp, values, sample++, true, fracSecValue);
                            break;
                        case FileType.Binary32:
                            Writer.WriteNextRecordBinary32(readBuffer, schema!, recordTimestamp, values, sample++, true, fracSecValue);
                            break;
                        case FileType.Float32:
                            Writer.WriteNextRecordFloat32(readBuffer, schema!, recordTimestamp, values, sample++, true, fracSecValue);
                            break;
                        case null:
                            readBufferWriter.Write($"{Environment.NewLine}{recordTimestamp.ToString(dateTimeFormat)},");
                            readBufferWriter.Write(missingAsNaN ? string.Join(",", values) : string.Join(",", values.Select(val => double.IsNaN(val) ? "" : $"{val}")));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null);
                    }

                    // Update progress based on time
                    if (operationState is not null)
                        operationState.Progress = recordTimestamp.Ticks - startTime.Ticks;

                    if (readBuffer.Length < TargetBufferSize)
                        return;

                    lock (writeBuffer)
                        readBuffer.WriteTo(writeBuffer);

                    readBuffer.Clear();
                    bufferReady.Set();
                }

                // Start stream reader for the provided time window and selected points
                using ClientDatabaseBase<HistorianKey, HistorianValue> database = connection.GetDatabase<HistorianKey, HistorianValue>(instanceName) ?? throw new InvalidOperationException($"Cannot export data: failed to access historian database instance \"{instanceName}\".");
                using TreeStream<HistorianKey, HistorianValue> stream = database.Read(SortedTreeEngineReaderOptions.Default, timeFilter, pointFilter);

                // Adjust timestamp to use first timestamp as base
                bool adjustTimeStamp = timestampSnap switch
                {
                    0 => false,
                    1 => true,
                    2 => false,
                    _ => true
                };

                long baseTime = timestampSnap switch
                {
                    0 => Ticks.RoundToSecondDistribution(startTime.Ticks, frameRate, startTime.Ticks - startTime.Ticks % Ticks.PerSecond),
                    _ => startTime.Ticks
                };

                while (stream.Read(historianKey, historianValue) && !cancellationToken.IsCancellationRequested && !(operationState?.CancellationToken.IsCancelled ?? false))
                {
                    ulong timestamp;

                    if (alignTimestamps)
                    {
                        if (adjustTimeStamp)
                        {
                            adjustTimeStamp = false;
                            baseTime = (long)historianKey.Timestamp;
                        }

                        // Make sure the timestamp is actually close enough to the distribution
                        Ticks ticks = Ticks.ToSecondDistribution((long)historianKey.Timestamp, frameRate, baseTime, toleranceTicks);
                        if (ticks == Ticks.MinValue)
                            continue;

                        timestamp = (ulong)ticks.Value;
                    }
                    else
                    {
                        timestamp = historianKey.Timestamp;
                    }

                    // Start a new row for each encountered new timestamp
                    if (timestamp != lastTimestamp)
                    {
                        if (lastTimestamp > 0UL)
                            bufferValues(new DateTime((long)lastTimestamp));

                        for (int i = 0; i < values.Length; i++)
                            values[i] = double.NaN;

                        if (fillMissingTimestamps && lastTimestamp > 0UL && timestamp > lastTimestamp)
                        {
                            ulong difference = timestamp - lastTimestamp;

                            if (difference > interval)
                            {
                                ulong interpolated = lastTimestamp;

                                for (ulong i = 1; i < difference / interval; i++)
                                {
                                    interpolated = (ulong)Ticks.RoundToSecondDistribution((long)(interpolated + interval), frameRate, startTime.Ticks).Value;
                                    bufferValues(new DateTime((long)interpolated, DateTimeKind.Utc));
                                }
                            }
                        }

                        lastTimestamp = timestamp;
                    }

                    // Save value to its column
                    if (pointIDIndex.TryGetValue(historianKey.PointID, out int index))
                        values[index] = historianValue.AsSingle;
                    else if (historianKey.PointID == (ulong)metadata.TargetQualityFlagsID)
                        fracSecValue = (ushort)historianValue.AsSingle;
                }

                if (lastTimestamp > 0UL)
                {
                    bufferValues(new DateTime((long)lastTimestamp));
                }
                else
                {
                    // No data queried, interpolate blank rows if requested
                    if (fillMissingTimestamps)
                    {
                        ulong difference = (ulong)(endTime.Ticks - startTime.Ticks);

                        if (difference > interval)
                        {
                            for (int i = 0; i < values.Length; i++)
                                values[i] = double.NaN;

                            ulong interpolated = (ulong)startTime.Ticks;

                            for (ulong i = 1; i < difference / interval; i++)
                            {
                                interpolated = (ulong)Ticks.RoundToSecondDistribution((long)(interpolated + interval), frameRate, startTime.Ticks).Value;
                                bufferValues(new DateTime((long)interpolated, DateTimeKind.Utc));
                            }
                        }
                    }
                }

                readBufferWriter.Flush();

                if (readBuffer.Length > 0)
                {
                    lock (writeBuffer)
                        readBuffer.WriteTo(writeBuffer);
                }

                if (operationState is not null)
                    operationState.Progress = operationState.Total;
            }
            catch (Exception ex)
            {
                if (operationState is not null)
                {
                    operationState.Failed = true;
                    operationState.FailedReason = ex.Message;
                }

                throw;
            }
            finally
            {
                if (operationState is not null && sample > 0U)
                    operationState.EndSampleCount = sample - 1U;

                readComplete[0] = true;
                bufferReady.Set();
            }
        }, cancellationToken);
    }

    private static Task WriteTask(Stream responseStream, byte[]? headers, BlockAllocatedMemoryStream writeBuffer, ManualResetEventSlim bufferReady, bool[] readComplete, HistorianOperationState? operationState, Action? completeHistorianOperation, CancellationToken cancellationToken)
    {
        return Task.Factory.StartNew(async () =>
        {
            long binaryByteCount = 0L;

            try
            {
                // Write headers, e.g., CSV header row or CFF schema
                if (headers is not null)
                    await responseStream.WriteAsync(headers, 0, headers.Length, cancellationToken);

                while ((writeBuffer.Length > 0 || !readComplete[0]) && !cancellationToken.IsCancellationRequested && !(operationState?.CancellationToken.IsCancelled ?? false))
                {
                    byte[] bytes;

                    bufferReady.Wait(cancellationToken);
                    bufferReady.Reset();

                    lock (writeBuffer)
                    {
                        bytes = writeBuffer.ToArray();
                        writeBuffer.Clear();
                    }

                    await responseStream.WriteAsync(bytes, 0, bytes.Length, cancellationToken);
                    binaryByteCount += bytes.Length;
                }

                // Flush stream
                await responseStream.FlushAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                if (operationState is not null)
                {
                    operationState.Failed = true;
                    operationState.FailedReason = ex.Message;
                }

                throw;
            }
            finally
            {
                if (operationState is not null)
                    operationState.BinaryByteCount = binaryByteCount;

                completeHistorianOperation?.Invoke();
            }
        }, cancellationToken);
    }

    #endregion

    #region [ Static ]

    // Static Fields
    //private static readonly string s_minimumRequiredRoles;
    private static readonly MemoryCache s_pointMetadataCache;
    private static readonly MemoryCache s_headerCache;

    // Static Constructor
    static ExportDataHandler()
    {
        s_pointMetadataCache = new MemoryCache($"{nameof(ExportDataHandler)}-{nameof(PointMetadata)}Cache");
        s_headerCache = new MemoryCache($"{nameof(ExportDataHandler)}-HeaderCache");
    }

    // Static Methods
    private static byte[]? GetHeaders(FileType? fileType, bool useCFF, PointMetadata metadata, Dictionary<ulong, int> pointIDIndex, DateTime startTime, string? columnHeaders, out Schema? schema)
    {
        using AdoDataConnection connection = new(Settings.Instance);
        TableOperations<Device> deviceTable = new(connection);
        Dictionary<int, Device?> deviceIDMap = new();
        byte[]? bytes = null;

        Device? lookupDevice(int? id)
        {
            return id is null ? null : deviceIDMap.GetOrAdd(id.Value, _ => deviceTable.QueryRecordWhere("ID = {0}", id));
        }

        if (fileType is null)
        {
            // Create CSV header
            StringBuilder headers;

            if (string.IsNullOrWhiteSpace(columnHeaders))
            {
                headers = new StringBuilder("\"Timestamp\"");

                if (metadata.Measurements.Length > 0)
                {
                    headers.Append(',');
                    headers.Append(string.Join(",", metadata.Measurements.Select(measurement => $"\"[{measurement.PointID}] {measurement.PointTag}\"")));
                }
            }
            else
            {
                headers = new StringBuilder(columnHeaders);
            }

            for (int i = 0; i < metadata.PointIDs.Length; i++)
                pointIDIndex.Add(metadata.PointIDs[i], i);

            bytes = new UTF8Encoding(false).GetBytes(headers.ToString());
            schema = null;
        }
        else
        {
            // Create COMTRADE header
            TableOperations<SignalType> signalTypeTable = new(connection);
            SignalType[] signalTypes = signalTypeTable.QueryRecords().ToArray()!;
            Dictionary<int, EESignalType> signalTypeAcronyms = signalTypes.ToDictionary(key => key.ID, value => Enum.TryParse(value.Acronym, true, out EESignalType signalType) ? signalType : EESignalType.NONE);
            Dictionary<int, string> signalTypeUnits = signalTypes.ToDictionary(key => key.ID, value => value.EngineeringUnits);
            string[] digitalSignalTypes = ["FLAG", "DIGI", "QUAL"];
            int[] digitalSignalTypeIDs = signalTypes.Where(signalType => digitalSignalTypes.Contains(signalType.Acronym)).Select(signalType => signalType.ID).ToArray();
            Measurement[] analogs = metadata.Measurements.Where(measurement => !digitalSignalTypeIDs.Contains(measurement.SignalTypeID)).ToArray();
            Measurement[] digitals = metadata.Measurements.Where(measurement => digitalSignalTypeIDs.Contains(measurement.SignalTypeID) && measurement.PointID != metadata.TargetQualityFlagsID).ToArray();
            Dictionary<int, Measurement> digitalIDMap = digitals.ToDictionary(key => key.PointID);
            Dictionary<ChannelMetadata, Measurement> analogChannelMeasurementMap = [];
            Dictionary<ChannelMetadata, Measurement> digitalChannelMeasurementMap = [];

            ChannelMetadata getAnalogChannel(Measurement analogMeasurement)
            {
                ChannelMetadata analogChannel = new()
                {
                    Name = analogMeasurement.PointTag,
                    SignalType = signalTypeAcronyms[analogMeasurement.SignalTypeID],
                    IsDigital = false,
                    Units = signalTypeUnits[analogMeasurement.SignalTypeID],
                    CircuitComponent = lookupDevice(analogMeasurement.DeviceID)?.Acronym
                };

                analogChannelMeasurementMap[analogChannel] = analogMeasurement;
                return analogChannel;
            }

            ChannelMetadata getDigitalChannel(Measurement digitalMeasurement)
            {
                EESignalType signalType = signalTypeAcronyms[digitalMeasurement.SignalTypeID];
                string? deviceAcronym = lookupDevice(digitalMeasurement.DeviceID)?.Acronym;

                ChannelMetadata digitalChannel = new()
                {
                    Name = signalType == EESignalType.FLAG ? deviceAcronym ?? digitalMeasurement.PointTag.Replace(":", "_") : digitalMeasurement.PointID.ToString(),
                    SignalType = signalType,
                    IsDigital = true,
                    CircuitComponent = deviceAcronym
                };

                digitalChannelMeasurementMap[digitalChannel] = digitalMeasurement;
                return digitalChannel;
            }

            // Create channel metadata
            List<ChannelMetadata> channels = new(analogs.Select(getAnalogChannel).Concat(digitals.Select(getDigitalChannel)));
            channels.Sort(ChannelMetadataSorter.Default);

            // Create schema
            schema = Writer.CreateSchema(channels, metadata.StationName, metadata.DeviceID, startTime.Ticks, Writer.MaxEndSample, 2013, fileType.Value, 1.0D, 0.0D, GlobalSettings.Default.NominalFrequency);

            // Load indexed digital labels
            Dictionary<int, string[]> digitalLabels = new();

            foreach (DigitalChannel digital in schema.DigitalChannels ?? [])
            {
                if (!int.TryParse(digital.Name, out int pointID) || !digitalIDMap.TryGetValue(pointID, out Measurement? digitalMeasurement))
                    continue;

                string? phaseID = digital.PhaseID?.Trim();

                if (string.IsNullOrEmpty(phaseID) || phaseID.Length < 2 || !int.TryParse(phaseID[1..], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int index))
                    continue;

                if (index < 0)
                    index = 0;

                if (index > 15)
                    index = 15;

                string[] labels = digitalLabels.GetOrAdd(pointID, _ => ParseDigitalLabels(digitalMeasurement.AlternateTag ?? ""));
                digital.Name = string.IsNullOrWhiteSpace(labels[index]) ? $"{digitalMeasurement.PointTag} ({index})" : labels[index];
            }

            if (useCFF)
            {
                // Create COMTRADE CFF header
                using BlockAllocatedMemoryStream stream = new();
                Writer.CreateCFFStream(stream, schema);
                bytes = stream.ToArray();
            }

            // Create properly ordered point ID index
            for (int i = 0; i < channels.Count; i++)
            {
                ChannelMetadata channel = channels[i];

                if (analogChannelMeasurementMap.TryGetValue(channel, out Measurement? analog))
                    pointIDIndex.Add((ulong)analog.PointID, i);
                else if (digitalChannelMeasurementMap.TryGetValue(channel, out Measurement? digital))
                    pointIDIndex.Add((ulong)digital.PointID, i);
            }
        }

        Debug.Assert(pointIDIndex.Count + (fileType is null || metadata.TargetQualityFlagsID == 0 ? 0 : 1) == metadata.PointIDs.Length);
        return bytes;
    }

    private static string[] ParseDigitalLabels(string digitalLabels)
    {
        string[] labels = new string[16];

        if (digitalLabels.Contains('|'))
        {
            string[] parts = digitalLabels.Split('|');

            for (int i = 0; i < parts.Length && i < 16; i++)
                labels[i] = parts[i].Trim();
        }
        else
        {
            int i = 0;

            for (int j = 0; j < digitalLabels.Length && i < 16; j += 16)
            {
                int length = 16;
                int remaining = digitalLabels.Length - j;

                if (remaining <= 0)
                    break;

                if (remaining < 16)
                    length = remaining;

                labels[i++] = digitalLabels.Substring(j, length).Trim();
            }
        }

        return labels;
    }

    private static PointMetadata CreatePointMetadata(ClaimsPrincipal? _, ulong[] pointIDs)
    {
        const int MaxSqlParams = 50;

        // TODO: Update check when security roles are implemented
        // Validate current user has access to requested data
        //if (!dataContext.UserIsInRole(securityPrincipal, s_minimumRequiredRoles))
        //    throw new SecurityException($"Cannot export data: access is denied for user \"{Thread.CurrentPrincipal.Identity?.Name ?? "Undefined"}\", minimum required roles = {s_minimumRequiredRoles.ToDelimitedString(", ")}.");

        PointMetadata metadata = new()
        {
            PointIDs = pointIDs,
            StationName = nameof(openHistorian),
        };

        using AdoDataConnection connection = new(Settings.Instance);
        TableOperations<Measurement> measurementTable = new(connection);
        List<Measurement> measurements = [];

        for (int i = 0; i < pointIDs.Length; i += MaxSqlParams)
        {
            object[] parameters = pointIDs.Skip(i).Take(MaxSqlParams).Select(id => (object)(int)id).ToArray();
            string parameterizedQueryString = $"PointID IN ({string.Join(",", parameters.Select((_, index) => $"{{{index}}}"))})";
            RecordRestriction pointIDRestriction = new(parameterizedQueryString, parameters);
            measurements.AddRange(measurementTable.QueryRecords(pointIDRestriction).Where(measurement => measurement is not null)!);
        }

        metadata.Measurements = measurements.ToArray();
        metadata.DeviceID = $"Export for {pointIDs.Length} measurements";

        try
        {
            int? firstDeviceID = metadata.Measurements.FirstOrDefault()?.DeviceID;

            // If all data is from a single device, can pick up device name and acronym for station name and device ID, respectively
            if (firstDeviceID is not null && metadata.Measurements.All(measurement => measurement.DeviceID == firstDeviceID))
            {
                TableOperations<Device> deviceTable = new(connection);
                Device? device = deviceTable.QueryRecordWhere("ID = {0}", firstDeviceID);

                if (device is not null)
                {
                    metadata.TargetDeviceName = device.Acronym;
                    metadata.StationName = device.Name;
                    metadata.DeviceID = $"{metadata.TargetDeviceName} (ID {device.ID})";

                    if (string.IsNullOrWhiteSpace(metadata.StationName))
                        metadata.StationName = metadata.TargetDeviceName;

                    Measurement? qualityFlags = measurementTable.QueryRecordWhere($"SignalReference = '{device.Acronym}-QF'");

                    if (qualityFlags?.PointID > 0 && measurements.Any(measurement => measurement.PointID == qualityFlags.PointID))
                        metadata.TargetQualityFlagsID = qualityFlags.PointID;
                }
            }
        }
        catch (Exception ex)
        {
            Logger.SwallowException(ex);
        }

        return metadata;
    }

    private static string CachePointMetadata(ClaimsPrincipal? securityPrincipal, ulong[] pointIDs)
    {
        string pointCacheID = Guid.NewGuid().ToString();
        s_pointMetadataCache.Add(pointCacheID, CreatePointMetadata(securityPrincipal, pointIDs), new CacheItemPolicy { SlidingExpiration = TimeSpan.FromSeconds(30.0D) });
        return pointCacheID;
    }

    private static PointMetadata? GetCachedPointMetadata(string? pointCacheID)
    {
        return string.IsNullOrEmpty(pointCacheID) ? null : s_pointMetadataCache.Get(pointCacheID) as PointMetadata;
    }

    private static void CacheHeaderData(string? headerCacheID, string fileImage, string fileName, long endSample)
    {
        if (string.IsNullOrEmpty(headerCacheID))
            return;

        // Update CFG end sample value
        string defaultEndSample = $"0,{Writer.MaxEndSample}";
        int endSampleIndex = fileImage.LastIndexOf(defaultEndSample, StringComparison.Ordinal);

        if (endSampleIndex > -1)
            fileImage = $"{fileImage[..endSampleIndex]}0,{endSample}{fileImage[(endSampleIndex + defaultEndSample.Length)..]}";

        s_headerCache.Add(headerCacheID, new HeaderData { FileImage = fileImage, FileName = fileName }, new CacheItemPolicy { SlidingExpiration = TimeSpan.FromSeconds(30.0D) });
    }

    private static HeaderData? GetCachedHeaderData(string? headerCacheID)
    {
        return string.IsNullOrEmpty(headerCacheID) ? null : s_headerCache.Get(headerCacheID) as HeaderData;
    }

    #endregion
}

/// <summary>
/// Extensions for <see cref="IApplicationBuilder"/> to add the <see cref="ExportDataHandler"/> middleware.
/// </summary>
public static class ExportDataHandlerExtensions
{
    /// <summary>
    /// Adds the <see cref="ExportDataHandler"/> middleware to the application's request pipeline.
    /// </summary>
    /// <param name="builder">The <see cref="IApplicationBuilder"/> instance to configure.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> instance with the middleware added.</returns>
    public static IApplicationBuilder UseExportDataHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExportDataHandler>();
    }
}