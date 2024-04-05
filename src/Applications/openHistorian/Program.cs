using System.Diagnostics;
using Gemstone.Configuration;
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

        HostApplicationBuilder application = Host.CreateApplicationBuilder(args);

        ConfigureLogging(application.Logging);

        application.Services.AddWindowsService(options =>
        {
            options.ServiceName = nameof(openHistorian);
        });

        application.Services.AddHostedService<ServiceHost>();

        IHost host = application.Build();
        host.Run();

        settings.Save(true);
    }

    internal static void ConfigureLogging(ILoggingBuilder builder)
    {
        builder.ClearProviders();
        builder.SetMinimumLevel(LogLevel.Information);

        builder.AddFilter("Microsoft", LogLevel.Warning);
        builder.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Error);
        builder.AddFilter<DebugLoggerProvider>("", LogLevel.Debug);

        builder.AddConsole(options => options.LogToStandardErrorThreshold = LogLevel.Error);
        builder.AddDebug();

    #if RELEASE
        if (OperatingSystem.IsWindows())
        {
            builder.AddFilter<EventLogLoggerProvider>("Application", LogLevel.Warning);
            builder.AddEventLog();
        }
    #endif
    }
}