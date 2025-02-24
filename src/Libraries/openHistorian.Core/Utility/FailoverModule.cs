//******************************************************************************************************
//  FailoverModule.cs - Gbtc
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
//
//******************************************************************************************************


using Gemstone.Configuration;
using System.Net.Http;
using System.Net;
using SnapDB.Snap.Tree;
using System.Net.Http.Json;
using System.Text;
using Gemstone.Data.Model;
using Gemstone.Data;
using static openHistorian.Utility.FailoverModule;

namespace openHistorian.Utility;

/// <summary>
/// openHistorian Failover Logic
/// </summary>
public static class FailoverModule
{
    #region [ Members ]

    private const string FailoverSettingsCategory = "Failover";

    private const int DefaultPriority = 0;
    private const double DefaultFailoverLogRetention = 0.01666666;
    private static readonly string[] DefaultNodes = new string[] { "localhost:8280", "localhost:8280" };
    private const string DefaultClusterSecret = "GPAClusterSecret";
    private const int DefaultRetryInterval = 1000;

    public class FailoverResponse
    {
        public string SystemName { get; set; }
        public int SystemPriority { get; set; }
        public bool PreventStartup { get; set; }
    }

    public class FailoverRequest
    {
        public string SystemName { get; set; }
        public int SystemPriotity { get; set; }
        public string ClusterSecret { get; set; }
    }

    public class FailoverLog
    {
        [PrimaryKey(true)]
        public int ID { get; set; }
        public string SystemName { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public int Priority { get; set; }
    }
      
    #endregion [ Members ]

    #region [ Static ]

    private static HttpClient s_httpClient = new();
    public static string SystemName
    {
        get
        {
            try
            {
                dynamic section = Gemstone.Configuration.Settings.Default[Settings.SystemSettingsCategory];
                return section.SystemName;
            }
            catch (Exception ex)
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
                dynamic section = Gemstone.Configuration.Settings.Default[FailoverSettingsCategory];
                return section.Priority;
            }
            catch (Exception ex)
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
                dynamic section = Gemstone.Configuration.Settings.Default[FailoverSettingsCategory];
                return section.ClusterSecret;
            }
            catch (Exception ex)
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
                dynamic section = Gemstone.Configuration.Settings.Default[FailoverSettingsCategory];
                return section.LogRetention;
            }
            catch (Exception ex)
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
                dynamic section = Gemstone.Configuration.Settings.Default[FailoverSettingsCategory];
                return section.Nodes;
            }
            catch (Exception ex)
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
                dynamic section = Gemstone.Configuration.Settings.Default[FailoverSettingsCategory];
                return section.RetryInterval;
            }
            catch (Exception ex)
            {
                return DefaultRetryInterval;
            }
        }
    }

    /// <summary>
    /// Defines settings section for Failover module
    /// </summary>
    /// <param name="settings"></param>
    public static void DefineSettings(Settings settings)
    {
        dynamic failoverSettings = settings[FailoverSettingsCategory];

        failoverSettings.Priority = (DefaultPriority, "Defines the priority of this node. Nodes with lower priority take precendece over higher priority. a value of 0 means Failover mode is disabled");
        failoverSettings.LogRetention = (DefaultFailoverLogRetention, "Defines the time in hours, during which logs from other failover nodes are kept in the database.");
        failoverSettings.Nodes = (DefaultNodes, "Defines the list of urls for other nodes in the cluster.");
        failoverSettings.ClusterSecret = (DefaultClusterSecret, "Defines the cluster secret used to identify and validate other nodes.");
        failoverSettings.RetryInterval = (DefaultRetryInterval, "Defines the interval in ms after which the system will try to startup again.");

    }

    /// <summary>
    /// Function that prevents startup if another node with Higher Priroity is found.
    /// </summary>
    public static void PreventStartup()
    {
        Func<string, bool> CheckNode = (endpoint) =>
        {

            try
            {
                using HttpRequestMessage request = new();

                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(endpoint);

                request.Content = new StringContent(new FailoverRequest()
                {
                    ClusterSecret = ClusterSecret,
                    SystemName = SystemName,
                    SystemPriotity = SystemPriority
                }.ToString(), Encoding.UTF8, "application/json");


                using HttpResponseMessage response = s_httpClient
                    .SendAsync(request)
                    .GetAwaiter()
                    .GetResult();

                if (response is null)
                    return false;

                
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    FailoverResponse failoverResponse = response.Content.ReadFromJsonAsync<FailoverResponse>().GetAwaiter().GetResult();
                    LogMessage(new FailoverLog()
                    {
                        SystemName = failoverResponse.SystemName,
                        Message = failoverResponse.PreventStartup ? $"Prevented startup of {SystemName} due to lower priority." : (
                        failoverResponse.SystemPriority < SystemPriority? $"Node with higher priority started. Shutting down {failoverResponse.SystemName}" : "Node with matching priority started"),
                        Priority = failoverResponse.SystemPriority
                    });
                    return failoverResponse.PreventStartup;
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    LogMessage(new FailoverLog()
                    {
                        SystemName = SystemName,
                        Message = $"Failed to reach {endpoint} due to authentication failure",
                        Priority = SystemPriority
                    });
                    return false;
                }

                LogMessage(new FailoverLog()
                {
                    SystemName = SystemName,
                    Message = $"Failed to reach {endpoint} endpoint responded with status: {response.StatusCode}",
                    Priority = SystemPriority
                });
                return false;
               
            }
            catch (Exception ex)
            {
                LogMessage(new FailoverLog()
                {
                    SystemName = SystemName,
                    Message = $"Failed to reach {endpoint} with error message: {ex.Message}",
                    Priority = SystemPriority
                });
                return false;
            }
        };


        // Prevent Startup if Failover is on AND Priority active
        if (SystemPriority <= 0)
            return;

        Console.WriteLine("Failover mode is active. Checking for other instances");

        if (Nodes.Length < 1)
        {
            Console.WriteLine("No Failover nodes to check are specified");
            return;

        }

        while (true)
        {
            bool preventStartup = false;
            foreach (string node in Nodes)
            {
                Console.WriteLine($"Failover logic connecting to {node}");

                string endpoint = $"http://{node}/api/System/checkFailoverState";
                preventStartup = preventStartup || CheckNode(endpoint);
            }

            if (!preventStartup)
            {
                Console.WriteLine($"Failover logic starting up this node");
                return;
            }

            Console.WriteLine($"Failover logic preventing startup");
            Thread.Sleep(RetryInterval);
        }



    }

    /// <summary>
    /// Saves the <see cref="FailoverLog"/> to the Database and removes old records.
    /// </summary>
    /// <param name="log">The <see cref="FailoverLog"/> to be saved.</param>
    public static void LogMessage(FailoverLog log)
    {
        StringBuilder deleteQuery = new StringBuilder();
        List<object> deleteParameters = new List<object>();

        using (AdoDataConnection connection = new AdoDataConnection(Gemstone.Configuration.Settings.Instance))
        {
            log.Timestamp = DateTime.UtcNow;
            new TableOperations<FailoverLog>(connection).AddNewOrUpdateRecord(log);
            connection.ExecuteNonQuery("DELETE FROM FailoverLog WHERE Timestamp < {0} AND SystemName = {1} ",
                log.Timestamp.AddHours(-LogRetention), log.SystemName);
        }
    }

    #endregion

}