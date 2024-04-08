using Gemstone.Configuration;
using Gemstone.Diagnostics;
using Microsoft.Extensions.Logging.Debug;

#if RELEASE
using Microsoft.Extensions.Logging.EventLog;
#endif

namespace openHistorian;

internal class Program
{
    private static void Main(string[] args)
    {
        // Define settings for the service. Note that the Gemstone defaults
        // for handling INI and SQLite configuration are defined in a hierarchy
        // where the configuration settings are loaded are in the following
        // priority order, from lowest to hightest:
        // - INI file (defaults.ini) - Machine Level, %programdata% folder
        // - INI file (settings.ini) - Machine Level, %programdata% folder
        // - SQLite database (settings.db) - User Level, %appdata% folder (not used by service)
        // - Environment variables - Machine Level
        // - Environment variables - User Level
        // - Command line arguments
        Settings settings = new()
        {
            INIFile = ConfigurationOperation.ReadWrite,
            SQLite = ConfigurationOperation.Disabled
        };

        // Define settings for service components
        ServiceHost.DefineSettings(settings);

        // Bind settings to configuration sources
        settings.Bind(new ConfigurationBuilder()
            .ConfigureGemstoneDefaults(settings)
            .AddCommandLine(args, settings.SwitchMappings));

        HostApplicationBuilderSettings appSettings = new()
        {
            Args = args,
            ApplicationName = nameof(openHistorian),
            DisableDefaults = true,
        };

        HostApplicationBuilder application = new(appSettings);

        application.Services.AddWindowsService(options =>
        {
            options.ServiceName = appSettings.ApplicationName;
        });

        application.Services.AddHostedService<ServiceHost>();

        ConfigureLogging(application.Logging);

        IHost host = application.Build();
        host.Run();

        settings.Save(true, true);
    }

    internal static void ConfigureLogging(ILoggingBuilder builder)
    {
        builder.ClearProviders();
        builder.SetMinimumLevel(LogLevel.Information);

        builder.AddFilter("Microsoft", LogLevel.Warning);
        builder.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Error);
        builder.AddFilter<DebugLoggerProvider>("", LogLevel.Debug);
        builder.AddFilter<DiagnosticsLoggerProvider>("", LogLevel.Trace);

        builder.AddConsole(options => options.LogToStandardErrorThreshold = LogLevel.Error);
        builder.AddDebug();

        // Add Gemstone diagnostics logging
        builder.AddGemstoneDiagnostics();

    #if RELEASE
        if (OperatingSystem.IsWindows())
        {
            builder.AddFilter<EventLogLoggerProvider>("Application", LogLevel.Warning);
            builder.AddEventLog();
        }
    #endif
    }
}