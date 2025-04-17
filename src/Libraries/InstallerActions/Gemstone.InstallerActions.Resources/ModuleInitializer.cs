//******************************************************************************************************
//  ModuleInitializer.cs - Gbtc
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
//  04/14/2025 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using GSF.Diagnostics;

namespace Gemstone.InstallerActions;

/// <summary>
/// Module initializer for loading assemblies from embedded resources.
/// </summary>
/// <remarks>
/// This code is run before any other code in the assembly is executed.
/// Assemblies need only to be loaded in application domain to be used.
/// module initialization handled using NuGet package that injects
/// needed code into the assembly at build time -- assembly not needed
/// for deployment.
/// </remarks>
internal static class ModuleInitializer
{
    // This is the namespace prefix used to identify embedded resources in the assembly
    private const string SourceNamespace = $"{nameof(Gemstone)}.{nameof(InstallerActions)}.";

    private static Assembly s_currentAssembly;
    private static Dictionary<string, Assembly> s_assemblyCache;

    private static Assembly CurrentAssembly => s_currentAssembly ??= typeof(ModuleInitializer).Assembly;

    private static Dictionary<string, Assembly> AssemblyCache => s_assemblyCache ??= new Dictionary<string, Assembly>();

    internal static void Run()
    {
        const string EventName = nameof(ModuleInitializer);

        LogPublisher log = Logger.CreatePublisher(typeof(ModuleInitializer), MessageClass.Framework);

        try
        {
            // Hook into assembly resolve event so assemblies can be loaded from embedded resources
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssemblyFromResource;

            // Load needed assemblies from embedded resources in dependency order
            AppDomain.CurrentDomain.Load("Antlr3.Runtime");
            AppDomain.CurrentDomain.Load("ExpressionEvaluator");
            AppDomain.CurrentDomain.Load("GSF.Core");
            AppDomain.CurrentDomain.Load("GSF.Communication");
            AppDomain.CurrentDomain.Load("GSF.Security");
            AppDomain.CurrentDomain.Load("GSF.ServiceProcess");

            log.Publish(MessageLevel.Info, EventName, $"Embedded resource assembly load complete, {AssemblyCache.Count:N0} assemblies loaded.");
        }
        catch (Exception ex)
        {
            log.Publish(MessageLevel.Error, EventName, $"Embedded resource assembly load failed: {ex.Message}", exception: ex);
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
            if (!string.Equals(Path.GetFileNameWithoutExtension(name), $"{SourceNamespace}.{shortName}", StringComparison.OrdinalIgnoreCase))
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