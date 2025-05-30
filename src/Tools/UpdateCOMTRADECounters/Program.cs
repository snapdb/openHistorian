﻿//******************************************************************************************************
//  Program.cs - Gbtc
//
//  Copyright © 2021, Grid Protection Alliance.  All Rights Reserved.
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
//  05/17/2021 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using GSF;
using GSF.Console;
using GSF.Identity;
using GSF.Interop;
using GSF.IO;
using Microsoft.Win32;

// ReSharper disable LocalizableElement
namespace UpdateCOMTRADECounters
{
    // This application loads needed dependencies from embedded resources so
    // application will run from a single downloaded standalone executable
    internal static class Program
    {
        public const string TargetExeFileName = nameof(UpdateCOMTRADECounters) + ".exe";
        public const string UriScheme = "comtrade-update-counter";
        public const string FriendlyName = "COMTRADE Update Counter";

        // Chromium based browsers share a policy structure
        private const string ChromePolicies = "SOFTWARE\\Policies\\Google\\Chrome";
        private const string EdgePolicies = "SOFTWARE\\Policies\\Microsoft\\Edge";
        private const string FirefoxPolicies = "SOFTWARE\\Policies\\Mozilla\\Firefox";
        private const string AutoLaunchProtocolPolicy = "AutoLaunchProtocolsFromOrigins";
        private const string ExemptFileTypeWarningPolicy = "ExemptFileTypeDownloadWarnings";

        private static Assembly s_currentAssembly;
        private static Dictionary<string, Assembly> s_assemblyCache;

        public static Assembly CurrentAssembly => s_currentAssembly ??= typeof(Program).Assembly;

        private static Dictionary<string, Assembly> AssemblyCache => s_assemblyCache ??= new Dictionary<string, Assembly>();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            // Hook into assembly resolve event so assemblies can be loaded from embedded resources
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssemblyFromResource;

            // Load needed assemblies from embedded resources in dependency order
            AppDomain.CurrentDomain.Load("Antlr3.Runtime");
            AppDomain.CurrentDomain.Load("ExpressionEvaluator");
            AppDomain.CurrentDomain.Load("GSF.Core");
            AppDomain.CurrentDomain.Load("GSF.Communication");
            AppDomain.CurrentDomain.Load("GSF.Net");
            AppDomain.CurrentDomain.Load("GSF.Security");
            AppDomain.CurrentDomain.Load("GSF.ServiceProcess");
            AppDomain.CurrentDomain.Load("GSF.TimeSeries");
            AppDomain.CurrentDomain.Load("GSF.PhasorProtocols");
            AppDomain.CurrentDomain.Load("Newtonsoft.Json");
            AppDomain.CurrentDomain.Load("GSF.COMTRADE");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string commandLine = Environment.CommandLine.ToLowerInvariant();

            RegisterUriScheme(commandLine);

            if (commandLine.Contains("-registeronly") || commandLine.Contains("-install") || commandLine.Contains("-showsetup"))
                Environment.Exit(0);

            Application.Run(new Main());
        }

        private static void RegisterUriScheme(string commandLine)
        {
            bool silent = commandLine.Contains("-silent");

            try
            {
                bool elevated = UserAccountControl.IsCurrentProcessElevated || (!UserAccountControl.IsUacEnabled && UserAccountControl.IsUserAdmin);
                bool targetAllUsers = commandLine.Contains("-allusers");

                // -AllUsers flag requires admin elevation
                if (targetAllUsers && !elevated)
                    throw new InvalidOperationException("Cannot target all users without process elevation. Try running process as administrator.");

                bool unregister = commandLine.Contains("-unregister");
                bool forceUpdate = commandLine.Contains("-forceupdate");
                bool showSetupDialog = commandLine.Contains("-showsetup");

                string allUsersApplicationFolder = ShellHelpers.GetCommonApplicationDataFolder();
                string allUsersApplicationFilePath = Path.Combine(allUsersApplicationFolder, TargetExeFileName);
                string localUserApplicationFolder = ShellHelpers.GetUserApplicationDataFolder();
                string localUserApplicationFilePath = Path.Combine(localUserApplicationFolder, TargetExeFileName);

                if (unregister || forceUpdate)
                {
                    if (targetAllUsers)
                    {
                        DeleteRegistration(Registry.LocalMachine, silent, commandLine);

                        try
                        {
                            // Try to remove all users installation
                            File.Delete(allUsersApplicationFilePath);
                        }
                        catch
                        {
                            // ignored
                        }
                    }

                    DeleteRegistration(Registry.CurrentUser, silent || !elevated, commandLine);

                    try
                    {
                        // Try to remove local user installation
                        File.Delete(localUserApplicationFilePath);
                    }
                    catch
                    {
                        // ignored
                    }

                    if (unregister)
                        Environment.Exit(0);
                }

                // Check if tool is properly registered and installed
                using RegistryKey allUsersKey = Registry.LocalMachine.OpenSubKey($"SOFTWARE\\Classes\\{UriScheme}");
                using RegistryKey localUserKey = Registry.CurrentUser.OpenSubKey($"SOFTWARE\\Classes\\{UriScheme}");

                if ((File.Exists(allUsersApplicationFilePath) && allUsersKey is not null || File.Exists(localUserApplicationFilePath) && localUserKey is not null) && !forceUpdate && !showSetupDialog)
                    return;

                if (!commandLine.Contains("-registeronly") && !commandLine.Contains("-install") || showSetupDialog)
                {
                    // Show install dialog when tool is not properly registered or installed
                    Application.Run(new Install());
                    return;
                }

                // Handle URI protocol registration
                try
                {
                    string currentExeFilePath = Process.GetCurrentProcess().MainModule?.FileName ?? CurrentAssembly.Location;
                    string targetApplicationFilePath = targetAllUsers ? allUsersApplicationFilePath : localUserApplicationFilePath;
                    string targetApplicationFolder = targetAllUsers ? allUsersApplicationFolder : localUserApplicationFolder;

                    try
                    {
                        // Make sure target application folder exists
                        if (!Directory.Exists(targetApplicationFolder))
                            Directory.CreateDirectory(targetApplicationFolder);

                        // Copy executable to application data folder as primary install location
                        if (!File.Exists(targetApplicationFilePath))
                            File.Copy(currentExeFilePath, targetApplicationFilePath);
                    }
                    catch
                    {
                        targetApplicationFilePath = currentExeFilePath;
                    }

                    if (allUsersKey is null && (localUserKey is null || targetAllUsers))
                    {
                        RegistryKey rootKey = targetAllUsers ?
                            Registry.LocalMachine :
                            Registry.CurrentUser;

                        using RegistryKey uriSchemeKey = rootKey.CreateSubKey($"SOFTWARE\\Classes\\{UriScheme}");

                        if (uriSchemeKey is null)
                            return;

                        uriSchemeKey.SetValue("", $"URL:{FriendlyName}");
                        uriSchemeKey.SetValue("URL Protocol", "");

                        using RegistryKey defaultIcon = uriSchemeKey.CreateSubKey("DefaultIcon");
                        defaultIcon?.SetValue("", $"{targetApplicationFilePath},1");

                        using RegistryKey commandKey = uriSchemeKey.CreateSubKey(@"shell\open\command");
                        commandKey?.SetValue("", $"\"{targetApplicationFilePath}\" \"%1\"");
                    }
                }
                catch (Exception ex)
                {
                    if (!silent)
                        MessageBox.Show($"Failed to register URI scheme \"{UriScheme}\" for {(targetAllUsers ? "all users" : "current user")}: {ex.Message}", "URI Scheme Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (commandLine.Contains("-nopolicies"))
                    return;

                // Handle browser policy registration
                try
                {
                    if (targetAllUsers)
                    {
                        RegisterBrowserPolicies(Registry.LocalMachine, commandLine);
                    }
                    else if (elevated)
                    {
                        RegisterBrowserPolicies(Registry.CurrentUser, commandLine);
                    }
                }
                catch (Exception ex)
                {
                    if (!silent)
                        MessageBox.Show($"Failed to register browser policies for {(targetAllUsers ? "all users" : "current user")}: {ex.Message}", "Browser Policy Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                if (!silent)
                    MessageBox.Show($"Failed during URI scheme registration setup: {ex.Message}", "URI Scheme Registration Setup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void DeleteRegistration(RegistryKey keyRoot, bool silent, string commandLine)
        {
            try
            {
                // Remove registration first - non-admin local user action is allowed
                DeleteRegistryKey(keyRoot, $"SOFTWARE\\Classes\\{UriScheme}");
            }
            catch (Exception ex)
            {
                if (!silent)
                    MessageBox.Show($"Failed to unregister URI scheme \"{UriScheme}\" for {(keyRoot == Registry.LocalMachine ? "all users" : "current user")}: {ex.Message}", "URI Scheme Registration Removal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                // Remove registered browser policies, i.e., only those that were registered by this application
                UnregisterBrowserPolicies(keyRoot, commandLine);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            // Only completely remove existing browser policies if requested by command line parameter
            if (!commandLine.Contains("-removepolicies"))
                return;

            if (!silent && MessageBox.Show($"Are you sure you want to remove all browser policies for {(keyRoot == Registry.LocalMachine ? "all users" : "current user")}?", "Browser Policy Removal Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                // Removing the following registry keys may require elevation
                DeleteRegistryKey(keyRoot, ChromePolicies);
                DeleteRegistryKey(keyRoot, EdgePolicies);
                DeleteRegistryKey(keyRoot, FirefoxPolicies);
            }
            catch (Exception ex)
            {
                if (!silent)
                    MessageBox.Show($"Failed to delete browser policies for {(keyRoot == Registry.LocalMachine ? "all users" : "current user")}: {ex.Message}", "Browser Policy Removal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void DeleteRegistryKey(RegistryKey key, string name)
        {
            RegistryKey subKey = key.OpenSubKey(name, true);

            if (subKey is null)
                return;

            string[] keyNames = subKey.GetSubKeyNames();

            foreach (string keyName in keyNames)
                DeleteRegistryKey(subKey, keyName);

            key.DeleteSubKey(name);
        }

        private static void RegisterBrowserPolicies(RegistryKey keyRoot, string commandLine)
        {
            void applyAutoLaunchProtocolPolicy(RegistryKey policyKey, string allowedOrigin, string protocol)
            {
                string originPolicies = policyKey.GetValue(AutoLaunchProtocolPolicy)?.ToString();

                if (string.IsNullOrEmpty(originPolicies))
                    originPolicies = "[]";

                policyKey.SetValue(AutoLaunchProtocolPolicy,
                    JsonHelpers.InjectProtocolOrigins(originPolicies, allowedOrigin, protocol));
            }


            void applyFileTypeWarningExemptionPolicy(RegistryKey policyKey, string fileType, string domain)
            {
                string filesTypeDomainExceptions = policyKey.GetValue(ExemptFileTypeWarningPolicy)?.ToString();

                if (string.IsNullOrEmpty(filesTypeDomainExceptions))
                    filesTypeDomainExceptions = "[]";

                policyKey.SetValue(ExemptFileTypeWarningPolicy,
                    JsonHelpers.InjectExemptDomainFilesTypes(filesTypeDomainExceptions, fileType, domain));
            }

            void applyPolicies(RegistryKey policyKey)
            {
                if (policyKey is null)
                    return;

                string targetUri = $"*:{GetTargetWebPort()}";

                if (!commandLine.Contains("-noautolaunch"))
                {
                    applyAutoLaunchProtocolPolicy(policyKey, $"http://{targetUri}", UriScheme);
                    applyAutoLaunchProtocolPolicy(policyKey, $"https://{targetUri}", UriScheme);
                }

                if (!commandLine.Contains("-nocfgexemption"))
                    applyFileTypeWarningExemptionPolicy(policyKey, "cfg", targetUri);
            }

            // Apply Chrome policy
            using (RegistryKey chromeKey = keyRoot.CreateSubKey(ChromePolicies, true) ?? keyRoot.OpenSubKey(ChromePolicies, true))
                applyPolicies(chromeKey);

            // Apply Edge policy
            using (RegistryKey edgeKey = keyRoot.CreateSubKey(EdgePolicies, true) ?? keyRoot.OpenSubKey(EdgePolicies, true))
                applyPolicies(edgeKey);

            // Apply Firefox policy
            using (RegistryKey firefoxKey = keyRoot.CreateSubKey(FirefoxPolicies, true) ?? keyRoot.OpenSubKey(FirefoxPolicies, true))
                applyPolicies(firefoxKey);
        }

        private static void UnregisterBrowserPolicies(RegistryKey keyRoot, string commandLine)
        {
            void removeAutoLaunchProtocolPolicy(RegistryKey policyKey, string allowedOrigin, string protocol)
            {
                string originPolicies = policyKey.GetValue(AutoLaunchProtocolPolicy)?.ToString();

                if (string.IsNullOrEmpty(originPolicies))
                    return; // Nothing to remove

                string updatedPolicies = JsonHelpers.RemoveProtocolOrigins(originPolicies, allowedOrigin, protocol);

                // If removing our entry results in an empty array, delete the key entirely
                if (updatedPolicies == "[]")
                    policyKey.DeleteValue(AutoLaunchProtocolPolicy, false);
                else
                    policyKey.SetValue(AutoLaunchProtocolPolicy, updatedPolicies);
            }

            void removeFileTypeWarningExemptionPolicy(RegistryKey policyKey, string fileType, string domain)
            {
                string filesTypeDomainExceptions = policyKey.GetValue(ExemptFileTypeWarningPolicy)?.ToString();

                if (string.IsNullOrEmpty(filesTypeDomainExceptions))
                    return; // Nothing to remove

                string updatedExceptions = JsonHelpers.RemoveExemptDomainFilesTypes(filesTypeDomainExceptions, fileType, domain);

                // If removing our entry results in an empty array, delete the key entirely
                if (updatedExceptions == "[]")
                    policyKey.DeleteValue(ExemptFileTypeWarningPolicy, false);
                else
                    policyKey.SetValue(ExemptFileTypeWarningPolicy, updatedExceptions);
            }

            void removePolicies(RegistryKey policyKey)
            {
                if (policyKey is null)
                    return;

                string targetUri = $"*:{GetTargetWebPort()}";

                if (!commandLine.Contains("-noautolaunch"))
                {
                    removeAutoLaunchProtocolPolicy(policyKey, $"http://{targetUri}", UriScheme);
                    removeAutoLaunchProtocolPolicy(policyKey, $"https://{targetUri}", UriScheme);
                }

                if (!commandLine.Contains("-nocfgexemption"))
                    removeFileTypeWarningExemptionPolicy(policyKey, "cfg", targetUri);
            }

            // Remove Chrome policy
            using (RegistryKey chromeKey = keyRoot.OpenSubKey(ChromePolicies, true)) 
                removePolicies(chromeKey);

            // Remove Edge policy
            using (RegistryKey edgeKey = keyRoot.OpenSubKey(EdgePolicies, true)) 
                removePolicies(edgeKey);

            // Remove Firefox policy
            using (RegistryKey firefoxKey = keyRoot.OpenSubKey(FirefoxPolicies, true)) 
                removePolicies(firefoxKey);
        }

        private static int GetTargetWebPort()
        {
            const string SourceApp = "openHistorian";
            const int DefaultPort = 8180;

            try
            {
                bool tryParseUrl(string url, out string callback)
                {
                    callback = null;

                    try
                    {
                        string query = new Uri(url).Query;

                        while (query.Length > 1 && query[0] == '?')
                            query = query.Length == 1 ? string.Empty : query.Substring(1);

                        if (string.IsNullOrEmpty(query))
                            return false;

                        Dictionary<string, string> parameters = query.ParseKeyValuePairs('&');

                        if (parameters.TryGetValue("callback", out callback))
                            callback = Uri.UnescapeDataString(callback);

                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }

                bool tryParseClipboard(out string callback)
                {
                    callback = null;
                    return Clipboard.ContainsText() && tryParseUrl(Clipboard.GetText(), out callback);
                }

                bool tryParseCommandLine(out string callback)
                {
                    callback = null;
                    string[] args = Arguments.ToArgs(Environment.CommandLine);
                    return args.Length > 1 && tryParseUrl(args[1], out callback);
                }

                // Check for callback parameter in protocol URL first - best option for typical use case
                if (!tryParseClipboard(out string hostUrl) && !tryParseCommandLine(out hostUrl))
                {
                    // Fall back on looking for openHistorian installation - useful during installation
                    string configFile = FilePath.AddPathSuffix(ShellHelpers.GetCommonApplicationDataFolder()) + SourceApp + "\\settings.ini";

                    // Return default value if config file cannot be found
                    if (string.IsNullOrEmpty(configFile) || !File.Exists(configFile))
                        return DefaultPort;

                    // Load web host URL that includes listening port from target config file
                    IniFile settings = new(configFile);

                    string hostURLs = settings["WebHosting", "HostURLs"];

                    if (string.IsNullOrWhiteSpace(hostURLs))
                        return DefaultPort;

                    string[] parts = hostURLs.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    
                    foreach (string part in parts)
                    {
                        string uri = part.Trim();

                        if (Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                        {
                            hostUrl = uri;
                            break;
                        }
                    }
                }

                return string.IsNullOrEmpty(hostUrl) ? DefaultPort :
                    new Uri(hostUrl.Replace("//+:", "//localhost:")).Port;
            }
            catch
            {
                return DefaultPort;
            }
        }

        private static Assembly ResolveAssemblyFromResource(object sender, ResolveEventArgs e)
        {
            string shortName = e.Name.Split(',')[0].Trim();

            if (AssemblyCache.TryGetValue(shortName, out Assembly resourceAssembly))
                return resourceAssembly;

            // Loop through all the resources in the current assembly
            foreach (string name in CurrentAssembly.GetManifestResourceNames())
            {
                // See if the embedded resource name matches the assembly it is trying to load
                if (!string.Equals(Path.GetFileNameWithoutExtension(name), $"{nameof(UpdateCOMTRADECounters)}.{shortName}", StringComparison.OrdinalIgnoreCase))
                    continue;

                // If so, load embedded resource assembly into a binary buffer
                Stream resourceStream = CurrentAssembly.GetManifestResourceStream(name);

                if (resourceStream is null)
                    break;

                byte[] buffer = new byte[resourceStream.Length];

                // ReSharper disable once MustUseReturnValue
                resourceStream.Read(buffer, 0, (int)resourceStream.Length);
                resourceStream.Close();

                // Load assembly from binary buffer
                resourceAssembly = Assembly.Load(buffer);

                // Add assembly to the cache
                AssemblyCache.Add(shortName, resourceAssembly);
                break;
            }

            return resourceAssembly;
        }
    }
}
