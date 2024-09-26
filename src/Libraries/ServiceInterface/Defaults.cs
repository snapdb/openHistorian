//******************************************************************************************************
//  Defaults.cs - Gbtc
//
//  Copyright © 2024, Grid Protection Alliance.  All Rights Reserved.
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
//  07/25/2024 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using Gemstone.Configuration;
using Gemstone.Diagnostics;

namespace ServiceInterface;

/// <summary>
/// Defines default values for StreamSplitter library.
/// </summary>
public static class Defaults
{
    /// <summary>
    /// Default configuration file name.
    /// </summary>
    public const string DefaultConfigurationFileName = "ProxyConnections.xml";

    /// <summary>
    /// Default configuration backup file count.
    /// </summary>
    public const int DefaultConfigurationBackups = 5;

    /// <summary>
    /// Default maximum queue size for socket send operations.
    /// </summary>
    public const int DefaultMaxQueueSize = 10000;

    /// <summary>
    /// Default socket error reporting interval, in seconds.
    /// </summary>
    public const double DefaultSocketErrorReportingInterval = 10.0D;

    /// <summary>
    /// Default data monitor interval, in milliseconds.
    /// </summary>
    public const double DefaultDataMonitorInterval = 10000.0D;

    /// <summary>
    /// Default status history mapping file name.
    /// </summary>
    public const string DefaultStatusHistoryMappingFileName = "StatusHistory.bin";

    /// <summary>
    /// Default status reporting interval, in minutes.
    /// </summary>
    public const double DefaultStatusReportingInterval = 1.0D;

    /// <summary>
    /// Gets setting from the specified <paramref name="settings"/> object using the specified <paramref name="name"/>,
    /// returning <paramref name="defaultValue"/> if the setting is not found or an exception occurs.
    /// </summary>
    /// <typeparam name="T">Type of setting value.</typeparam>
    /// <param name="settings">Source settings object.</param>
    /// <param name="name">Name of setting to retrieve.</param>
    /// <param name="defaultValue">Default value to return if setting is not found or an exception occurs.</param>
    /// <param name="category">Category of setting to retrieve.</param>
    /// <returns>Specified setting from the specified <paramref name="settings"/> object.</returns>
    public static T GetSetting<T>(this Settings settings, string name, T defaultValue, string category = Settings.SystemSettingsCategory)
    {
        try
        {
            return (T)settings[category][name]!;
        }
        catch (Exception ex)
        {
            Logger.SwallowException(ex);
            return defaultValue;
        }
    }
}