
// - Command line arguments
using DataQualityMonitoring;
using Gemstone.Configuration;
using Gemstone.Diagnostics;
using Microsoft.Extensions.Configuration;

Settings settings = new()
{
    INIFile = ConfigurationOperation.Disabled,
    SQLite = ConfigurationOperation.ReadWrite
};

// Define component settings for application
DiagnosticsLogger.DefineSettings(settings);

// Bind settings to configuration sources
settings.Bind(new ConfigurationBuilder().ConfigureGemstoneDefaults(settings));

Console.WriteLine("Starting application...");

DEFComputationAdapter adapter = new DEFComputationAdapter();
adapter.LoadFile();

Console.WriteLine("Application Completed...");