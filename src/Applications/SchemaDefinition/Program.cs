//******************************************************************************************************
//  Program.cs - Gbtc
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
//  04/22/2024 - Christoph Lackner
//       Generated original version of source code.
//
//******************************************************************************************************

using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Logging;
using Gemstone.Configuration;
using Gemstone.Data;
using Gemstone.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.Extensions.Options;
using SchemaDefinition.Migrations;

namespace SchemaDefinition;

internal class Program
{
    private static ILogger<Program>? s_logger;

    private static void Main(string[] args)
    {
        Settings settings = new()
        {
            INIFile = ConfigurationOperation.ReadOnly,
            SQLite = ConfigurationOperation.Disabled,
        };

        DefineSettings(settings);
        
        settings.Bind(new ConfigurationBuilder().AddIniFile(GetOHIniFilePath()).AddCommandLine(args));


        using ILoggerFactory loggerFactory = LoggerFactory.Create(ConfigureLogging);
        s_logger = loggerFactory.CreateLogger<Program>();


        using ServiceProvider serviceProvider = CreateServices(settings);
        using IServiceScope scope = serviceProvider.CreateScope();

        UpdateDatabase(scope.ServiceProvider);
    }

    /// <summary>
    /// Configure the dependency injection services
    /// </summary>
    private static ServiceProvider CreateServices(Settings settings)
    {
        dynamic x = settings;
        string fileName = $"openHistorian_Upgrade_{DateTime.Now.ToString("yyyyMMdd")}.sql";

        IServiceCollection serviceCollection = new ServiceCollection()
            // Add common FluentMigrator services
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => {
                rb.UseAdoConnectionDatabase(settings);
                dynamic x = settings;
                if (x.Migration.GenerateScript)
                    rb.AsGlobalPreview();
                // Define the assembly containing the migrations
                rb.ScanIn(typeof(InitialSchema).Assembly).For.Migrations();
            })
            // Enable logging to console in the FluentMigrator way
            .AddLogging(ConfigureLogging)
            .Configure<RunnerOptions>(opt => {
                dynamic x = settings;
                if (x.Migration.IncludeDataset)
                    opt.Tags = new string[] { "Dataset", x.Migration.Locale };
            });

            if (x.Migration.GenerateScript)
                serviceCollection.AddSingleton<ILoggerProvider, LogFileFluentMigratorLoggerProvider>();

            serviceCollection.Configure<LogFileFluentMigratorLoggerOptions>(opt => {
                    opt.OutputFileName = fileName;
                    opt.OutputGoBetweenStatements = true;
                    opt.ShowSql = true;
                });
            // Build the service provider
            
        return serviceCollection.BuildServiceProvider(false);

    }

    /// <summary>
    /// Update the database
    /// </summary>
    private static void UpdateDatabase(IServiceProvider serviceProvider)
    {
        // Instantiate the runner
        IMigrationRunner runner = serviceProvider.GetRequiredService<IMigrationRunner>();
      
        if (!runner.HasMigrationsToApplyUp())
            s_logger?.LogInformation("Database version newer or equal to current schema.");

        if (runner.HasMigrationsToApplyUp())
            s_logger?.LogInformation("Updating database...");

        // Execute the migrations
        runner.MigrateUp();
    }

    /// <summary>
    /// Establishes default settings for the config file.
    /// </summary>
    public static void DefineSettings(Settings settings)
    {
        using (Logger.SuppressFirstChanceExceptionLogMessages())
        {
            DiagnosticsLogger.DefineSettings(settings);
            AdoDataConnection.DefineSettings(settings);
            DefineMigrationSettings(settings);
        }
    }

    /// <summary>
    /// Gets the ini file path for openHistorian
    /// </summary>
    private static string GetOHIniFilePath()
    {
        Environment.SpecialFolder specialFolder = Environment.SpecialFolder.CommonApplicationData;
        string appDataPath = Environment.GetFolderPath(specialFolder);
        return Path.Combine(appDataPath, "openHistorian", "settings.ini");
    }

    /// <summary>
    /// Configure Gemstone Logging
    /// </summary>
    /// <param name="builder">Builder to configure logging.</param>
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

    internal static void DefineMigrationSettings(Settings settings)
    {
        dynamic migrationSettings = settings["Migration"];
        migrationSettings.IncludeDataset = (true, "Determines whether the initial dataset should be added.");
        migrationSettings.Locale = ("NorthAmerica", "Determines which set of default settings should be added.");
        migrationSettings.GenerateScript = (false, "Defines whether a script should be generated instead of applying migrations directly.");
    }
}