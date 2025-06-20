//******************************************************************************************************
//  InitializeGrafanaAdapter.cs - Gbtc
//
//  Copyright © 2025, Grid Protection Alliance.  All Rights Reserved.
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
//  05/01/2025 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using Gemstone.Data.Model;
using Gemstone.StringExtensions;
using openHistorian.Model;
using openHistorian.WebUI.Controllers;

namespace openHistorian;

internal partial class Program
{
    private static void SetupGrafanaHostingAdapter(Settings settings)
    {
        try
        {
            const string GrafanaProcessAdapterName = "GRAFANA!PROCESS";
            const string DefaultGrafanaServerPath = GrafanaAuthProxyController.DefaultServerPath;
            const string DefaultSettingsCategory = GrafanaAuthProxyController.DefaultSettingsCategory;

            //const string GrafanaAdminRoleName = GrafanaAuthProxyController.GrafanaAdminRoleName;
            //const string GrafanaAdminRoleDescription = "Grafana Administrator Role";

            // Access needed settings from specified categories in configuration file
            dynamic grafanaHosting = settings[DefaultSettingsCategory];
            
            // Make sure needed settings exist
            grafanaHosting.ServerPath =(DefaultGrafanaServerPath, "Defines the path to the Grafana server to host - set to empty string to disable hosting.");

            // Get settings as currently defined in configuration file
            string grafanaServerPath = grafanaHosting["ServerPath"];

            // Only enable adapter if file path to configured Grafana server executable is accessible
            bool enabled = File.Exists(FilePath.GetAbsolutePath(grafanaServerPath));

            // Open database connection as defined in configuration file "systemSettings" category
            using AdoDataConnection connection = new(settings);

            // Make sure Grafana process adapter exists
            TableOperations<CustomActionAdapter> actionAdapterTable = new(connection);
            CustomActionAdapter actionAdapter = actionAdapterTable.QueryRecordWhere("AdapterName = {0}", GrafanaProcessAdapterName) ?? actionAdapterTable.NewRecord()!;

            // Update record fields
            actionAdapter.AdapterName = GrafanaProcessAdapterName;
            actionAdapter.AssemblyName = "FileAdapters.dll";
            actionAdapter.TypeName = "FileAdapters.ProcessLauncher";
            actionAdapter.Enabled = enabled;

            // Define default adapter connection string if none is defined
            if (string.IsNullOrWhiteSpace(actionAdapter.ConnectionString))
                actionAdapter.ConnectionString =
                    $"FileName={DefaultGrafanaServerPath}; " +
                    "WorkingDirectory=Grafana; " +
                    "ForceKillOnDispose=True; " +
                    "ProcessOutputAsLogMessages=True; " +
                    "LogMessageTextExpression={(?<=.*msg\\s*\\=\\s*\\\")[^\\\"]*(?=\\\")|(?<=.*file\\s*\\=\\s*\\\")[^\\\"]*(?=\\\")|(?<=.*file\\s*\\=\\s*)[^\\s]*(?=s|$)|(?<=.*path\\s*\\=\\s*\\\")[^\\\"]*(?=\\\")|(?<=.*path\\s*\\=\\s*)[^\\s]*(?=s|$)|(?<=.*error\\s*\\=\\s*\\\")[^\\\"]*(?=\\\")|(?<=.*reason\\s*\\=\\s*\\\")[^\\\"]*(?=\\\")|(?<=.*id\\s*\\=\\s*\\\")[^\\\"]*(?=\\\")|(?<=.*version\\s*\\=\\s*)[^\\s]*(?=\\s|$)|(?<=.*dbtype\\s*\\=\\s*)[^\\s]*(?=\\s|$)|(?<=.*)commit\\s*\\=\\s*[^\\s]*(?=\\s|$)|(?<=.*)compiled\\s*\\=\\s*[^\\s]*(?=\\s|$)|(?<=.*)address\\s*\\=\\s*[^\\s]*(?=\\s|$)|(?<=.*)protocol\\s*\\=\\s*[^\\s]*(?=\\s|$)|(?<=.*)subUrl\\s*\\=\\s*[^\\s]*(?=\\s|$)|(?<=.*)code\\s*\\=\\s*[^\\s]*(?=\\s|$)|(?<=.*name\\s*\\=\\s*)[^\\s]*(?=\\s|$)}; " +
                    "LogMessageLevelExpression={(?<=.*lvl\\s*\\=\\s*)[^\\s]*(?=\\s|$)}; " +
                    "LogMessageLevelMappings={info=Info; warn=Waning; error=Error; critical=Critical; debug=Debug}";

            // Preserve connection string on existing records except for Grafana server executable path that comes from configuration file
            Dictionary<string, string> connectionSettings = actionAdapter.ConnectionString.ParseKeyValuePairs();
            connectionSettings["FileName"] = grafanaServerPath;
            actionAdapter.ConnectionString = connectionSettings.JoinKeyValuePairs();

            // Save record updates
            actionAdapterTable.AddNewOrUpdateRecord(actionAdapter);

            //// Make sure Grafana admin role exists
            //TableOperations<ApplicationRole> applicationRoleTable = new(connection);
            //ApplicationRole applicationRole = applicationRoleTable.QueryRecordWhere("Name = {0} AND NodeID = {1}", GrafanaAdminRoleName, nodeID);

            //if (applicationRole is null)
            //{
            //    applicationRole = applicationRoleTable.NewRecord();
            //    applicationRole.Name = GrafanaAdminRoleName;
            //    applicationRole.Description = GrafanaAdminRoleDescription;
            //    applicationRoleTable.AddNewRecord(applicationRole);
            //}
        }
        catch (Exception ex)
        {
            Logger.SwallowException(ex, nameof(SetupGrafanaHostingAdapter), "Failed to setup Grafana hosting adapter");
        }
    }
}
