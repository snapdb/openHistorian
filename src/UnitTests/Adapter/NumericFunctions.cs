//******************************************************************************************************
//  WriteToCSV.cs - Gbtc
//
//  Copyright © 2023, Grid Protection Alliance.  All Rights Reserved.
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
//  10/20/2023 - Lillian Gensolin
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Gemstone.Data;
using Newtonsoft.Json.Linq;
using Gemstone.Numeric;
using NUnit.Framework;
using Gemstone.Numeric.Analysis;
using DataQualityMonitoring;
using Gemstone.Diagnostics;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.Extensions.Logging;
using Gemstone.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.DirectoryServices.ActiveDirectory;

namespace openHistorian.UnitTests;

[TestFixture]
internal class NumericFunctions
{
    public void Setup()
    {
        // Set up any necessary resources or configurations before running the tests
        // This method is called once before any tests in this fixture are run
    }

    [Test]
    public void VMD()
    {
        // Using Matlab Example
        double fs = 1e3;
        double[] t = Enumerable.Range(1, (int)fs).Select(v => 1+(double)(v-1) / fs).ToArray();
        double[] x = t.Select(v => Math.Cos(2 * Math.PI * 2 * v) + 2 * Math.Cos(2 * Math.PI * 10 * v) + 4 * Math.Cos(2 * Math.PI * 30 * v) + 0.01 * new Random().NextDouble()).ToArray();

        VariableModeDecomposition.vmd(x);
    }

    [Test]
    public void Test2()
    { 

        DEFComputationAdapter adapter = new DEFComputationAdapter();
        adapter.LoadFile();

    }

    internal static void ConfigureLogging(ILoggingBuilder builder)
    {
        builder.ClearProviders();
        builder.SetMinimumLevel(LogLevel.Information);

        builder.AddFilter("Microsoft", LogLevel.Warning);
        builder.AddFilter<DebugLoggerProvider>("", LogLevel.Debug);
        builder.AddFilter<DiagnosticsLoggerProvider>("", LogLevel.Trace);

        builder.AddConsole(options => options.LogToStandardErrorThreshold = LogLevel.Error);
        builder.AddDebug();

        // Add Gemstone diagnostics logging
        builder.AddGemstoneDiagnostics();
    }

}

[SetUpFixture]
public class TestLogging
{
    [OneTimeSetUp]
    public void Setup()
    {
       
        // - Command line arguments
        Settings settings = new()
        {
            INIFile = ConfigurationOperation.Disabled,
            SQLite = ConfigurationOperation.ReadWrite
        };

        // Define component settings for application
        DiagnosticsLogger.DefineSettings(settings);

        // Bind settings to configuration sources
        settings.Bind(new ConfigurationBuilder().ConfigureGemstoneDefaults(settings));
    }
}
