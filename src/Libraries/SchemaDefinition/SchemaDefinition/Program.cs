
using FluentMigrator.Runner;
using Gemstone.Configuration;
using Gemstone.Data;
using Gemstone.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using SchemaDefinition.Migrations;

namespace SchemaDefinition
{
    class Program
    {

        private static ILogger<Program> m_logger;

        static void Main(string[] args)
        {
            
            Settings settings = new()
            {
                INIFile = ConfigurationOperation.ReadWrite,
                SQLite = ConfigurationOperation.Disabled
            };

            // Define settings for service components
            Program.DefineSettings(settings);

            // Bind settings to configuration sources
            settings.Bind(new ConfigurationBuilder()
                .ConfigureGemstoneDefaults(settings)
                .AddCommandLine(args, settings.SwitchMappings));

            using var loggerFactory = LoggerFactory.Create(ConfigureLogging);
            m_logger = loggerFactory.CreateLogger<Program>();

            using (ServiceProvider serviceProvider = CreateServices())
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                UpdateDatabase(scope.ServiceProvider);
            }
        }

        /// <summary>
        /// Configure the dependency injection services
        /// </summary>
        private static ServiceProvider CreateServices()
        {
            dynamic systemSettings = Settings.Default.System;

            string connectionString = systemSettings.ConnectionString;

            return new ServiceCollection()
                // Add common FluentMigrator services
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    // Add SQLite support to FluentMigrator
                    .AddSQLite()
                    .AddSqlServer()
                    // Set the connection string
                    .WithGlobalConnectionString(connectionString)
                    // Define the assembly containing the migrations
                    .ScanIn(typeof(InitialSchema).Assembly).For.Migrations())
                // Enable logging to console in the FluentMigrator way
                .AddLogging(ConfigureLogging)
                // Build the service provider
                .BuildServiceProvider(false);
        }

        /// <summary>
        /// Update the database
        /// </summary>
        private static void UpdateDatabase(IServiceProvider serviceProvider)
        {
            // Instantiate the runner
            IMigrationRunner runner = serviceProvider.GetRequiredService<IMigrationRunner>();

            if (!runner.HasMigrationsToApplyUp())
                m_logger.LogInformation("Database version newer or equal to current Schema");
            if (runner.HasMigrationsToApplyUp())
                m_logger.LogInformation("Updating database...");

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
            }
        }

        /// <summary>
        /// Configure Gemstone Logging
        /// </summary>
        /// <param name="builder"></param>
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
}