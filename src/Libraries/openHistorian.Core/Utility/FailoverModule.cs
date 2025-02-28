//******************************************************************************************************
//  FailOverModule.cs - Gbtc
//
//  Copyright © 2025, Grid Protection Alliance.  All Rights Reserved.
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
//  02/24/2025 - C. Lackner
//       Generated original version of source code. 
//
//******************************************************************************************************
// ReSharper disable InconsistentNaming
#pragma warning disable CS1591 // TODO: FIx missing XML comments

using System.Net;
using System.Net.Http.Json;
using System.Text;
using Gemstone.Configuration;
using Gemstone.Data.Model;
using Gemstone.Data;

namespace openHistorian.Utility;

/// <summary>
/// Defines openHistorian fail over Logic.
/// </summary>
public static class FailOverModule
{
    #region [ Members ]

    private const string FailOverSettingsCategory = "Failover";

    private const int DefaultPriority = 0;
    private const double DefaultFailOverLogRetention = 0.01666666;
    private static readonly string[] DefaultNodes = ["localhost:8280", "localhost:8280"];
    private const string DefaultClusterSecret = "GPAClusterSecret";
    private const int DefaultRetryInterval = 1000;

    public class FailOverResponse
    {
        public string? SystemName { get; set; }
        public int SystemPriority { get; set; }
        public bool PreventStartup { get; set; }
    }

    public class FailOverRequest
    {
        public string? SystemName { get; set; }
        public int SystemPriority { get; set; }
        public string? ClusterSecret { get; set; }
    }

    public class FailOverLog
    {
        [PrimaryKey(true)]
        public int ID { get; set; }
        public string? SystemName { get; set; }
        public string? Message { get; set; }
        public DateTime Timestamp { get; set; }
        public int Priority { get; set; }
    }
      
    #endregion [ Members ]

    #region [ Static ]

    private static readonly HttpClient s_httpClient = new();

    public static string SystemName
    {
        get
        {
            try
            {
                dynamic section = Settings.Default[Settings.SystemSettingsCategory];
                return section.SystemName;
            }
            catch
            {
                return "openHistorian";
            }
        }
    }

    public static int SystemPriority
    {
        get
        {
            try
            {
                dynamic section = Settings.Default[FailOverSettingsCategory];
                return section.Priority;
            }
            catch
            {
                return DefaultPriority;
            }
        }
    }

    public static string ClusterSecret
    {
        get
        {
            try
            {
                dynamic section = Settings.Default[FailOverSettingsCategory];
                return section.ClusterSecret;
            }
            catch
            {
                return DefaultClusterSecret;
            }
        }
    }

    public static double LogRetention
    {
        get
        {
            try
            {
                dynamic section = Settings.Default[FailOverSettingsCategory];
                return section.LogRetention;
            }
            catch
            {
                return 0.01666666D;
            }
        }
    }

    public static string ServiceName => "openHistorian";

    public static string[] Nodes
    {
        get
        {
            try
            {
                dynamic section = Settings.Default[FailOverSettingsCategory];
                return section.Nodes;
            }
            catch
            {
                return DefaultNodes;
            }
        }
    }

    public static int RetryInterval
    {
        get
        {
            try
            {
                dynamic section = Settings.Default[FailOverSettingsCategory];
                return section.RetryInterval;
            }
            catch
            {
                return DefaultRetryInterval;
            }
        }
    }

    /// <summary>
    /// Defines settings section for fail over module
    /// </summary>
    /// <param name="settings"></param>
    public static void DefineSettings(Settings settings)
    {
        dynamic failOverSettings = settings[FailOverSettingsCategory];

        failOverSettings.Priority = (DefaultPriority, "Defines the priority of this node. Nodes with lower priority take precedence over higher priority. a value of 0 means fail over mode is disabled");
        failOverSettings.LogRetention = (DefaultFailOverLogRetention, "Defines the time in hours, during which logs from other fail over nodes are kept in the database.");
        failOverSettings.Nodes = (DefaultNodes, "Defines the list of urls for other nodes in the cluster.");
        failOverSettings.ClusterSecret = (DefaultClusterSecret, "Defines the cluster secret used to identify and validate other nodes.");
        failOverSettings.RetryInterval = (DefaultRetryInterval, "Defines the interval in ms after which the system will try to startup again.");
    }

    /// <summary>
    /// Function that prevents startup if another node with higher priority is found.
    /// </summary>
    public static void PreventStartup()
    {
        bool checkNode(string endpoint)
        {
            try
            {
                using HttpRequestMessage request = new();

                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(endpoint);

                request.Content = new StringContent(new FailOverRequest()
                {
                    ClusterSecret = ClusterSecret,
                    SystemName = SystemName,
                    SystemPriority = SystemPriority
                }.ToString()!, Encoding.UTF8, "application/json");

                using HttpResponseMessage response = s_httpClient.SendAsync(request).GetAwaiter().GetResult();

                FailOverResponse? failOverResponse = response.Content.ReadFromJsonAsync<FailOverResponse>().GetAwaiter().GetResult();

                if (failOverResponse is not null)
                {
                    LogMessage(new FailOverLog()
                    {
                        SystemName = failOverResponse.SystemName,
                        Message = failOverResponse.PreventStartup ? $"Prevented startup of {SystemName} due to lower priority." : (failOverResponse.SystemPriority < SystemPriority ? $"Node with higher priority started. Shutting down {failOverResponse.SystemName}" : "Node with matching priority started"),
                        Priority = failOverResponse.SystemPriority
                    });
                    return failOverResponse.PreventStartup;
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    LogMessage(new FailOverLog()
                    {
                        SystemName = SystemName,
                        Message = $"Failed to reach {endpoint} due to authentication failure",
                        Priority = SystemPriority
                    });
                    return false;
                }

                LogMessage(new FailOverLog()
                {
                    SystemName = SystemName,
                    Message = $"Failed to reach {endpoint} endpoint responded with status: {response.StatusCode}",
                    Priority = SystemPriority
                });
                return false;
            }
            catch (Exception ex)
            {
                LogMessage(new FailOverLog()
                {
                    SystemName = SystemName,
                    Message = $"Failed to reach {endpoint} with error message: {ex.Message}",
                    Priority = SystemPriority
                });
                return false;
            }
        }

        // Prevent startup if fail over is on and priority active
        if (SystemPriority <= 0)
            return;

        Console.WriteLine("Fail over mode is active. Checking for other instances");

        if (Nodes.Length < 1)
        {
            Console.WriteLine("No fail over nodes to check are specified");
            return;

        }

        while (true)
        {
            bool preventStartup = false;
            
            foreach (string node in Nodes)
            {
                Console.WriteLine($"Fail over logic connecting to {node}");

                string endpoint = $"http://{node}/api/System/checkFailOverState";
                preventStartup = preventStartup || checkNode(endpoint);
            }

            if (!preventStartup)
            {
                Console.WriteLine("Fail over logic starting up this node");
                return;
            }

            Console.WriteLine("Fail over logic preventing startup");
            Thread.Sleep(RetryInterval);
        }
    }

    /// <summary>
    /// Saves the <see cref="FailOverLog"/> to the Database and removes old records.
    /// </summary>
    /// <param name="log">The <see cref="FailOverLog"/> to be saved.</param>
    public static void LogMessage(FailOverLog log)
    {
        using AdoDataConnection connection = new(Settings.Instance);
        log.Timestamp = DateTime.UtcNow;
        new TableOperations<FailOverLog>(connection).AddNewOrUpdateRecord(log);
        
        connection.ExecuteNonQuery("DELETE FROM FailOverLog WHERE Timestamp < {0} AND SystemName = {1} ",
            log.Timestamp.AddHours(-LogRetention), log.SystemName);
    }

    #endregion
}