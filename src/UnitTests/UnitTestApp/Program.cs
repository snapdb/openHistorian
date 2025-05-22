
// - Command line arguments
using DataQualityMonitoring;
using Gemstone.Configuration;
using Gemstone.Diagnostics;
using Gemstone.Timeseries;
using Gemstone.Timeseries.Model;
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
Task<Tuple<AlarmMeasurement, EventDetails>?> adapterTask = adapter.LoadFile();

DEFIdentificationAdapter idAdapter = new DEFIdentificationAdapter();
await adapterTask.ContinueWith(tupe => {
    EventDetails computationDetail = tupe.Result.Item2;
    if (computationDetail is null)
        Console.WriteLine("Computation adapter could not resolve event details.");
    else
        idAdapter.TestEventDetail(tupe.Result.Item2);
});

Console.WriteLine("Application Completed...");