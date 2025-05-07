#if RELEASE
using Microsoft.Extensions.Logging.EventLog;
#endif

using Gemstone.Threading;
using Gemstone.Timeseries;
using openHistorian.Utility;

namespace openHistorian;

internal class Program
{
    private static void Main(string[] args)
    {
        try
        {
            ShutdownHandler.Initialize();

            // Define settings for the service. Note that the Gemstone defaults
            // for handling INI and SQLite configuration are defined in a hierarchy
            // where the configuration settings are loaded are in the following
            // priority order, from lowest to highest:
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
            DefineGeoSpatialDisplaySettings(settings);

            // Bind settings to configuration sources
            settings.Bind(new ConfigurationBuilder()
                .ConfigureGemstoneDefaults(settings)
                .AddCommandLine(args, settings.SwitchMappings));

            FailoverModule.PreventStartup();

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

            ServiceHostBase.ServiceName = appSettings.ApplicationName;

            application.Services.AddHostedService<ServiceHost>();

            ConfigureLogging(application.Logging);

            IHost host = application.Build();
            host.Run();

        #if DEBUG
            Settings.Save(forceSave: true);
        #else
            Settings.Save();
        #endif
        }
        finally
        {
            ShutdownHandler.InitiateSafeShutdown();
        }
    }

    private static void DefineGeoSpatialDisplaySettings(Settings settings)
    {
        dynamic section = settings["GeospatialDisplayServer"];

        section.Url = ("https://tile.openstreetmap.org/{z}/{x}/{y}.png", "Defines the url for the Tile server used for maps in the user interfaces.");
        section.Attribution = ("&copy; <a href=\"http://www.openstreetmap.org/copyright\">OpenStreetMap</a> <br>' + '&copy; <a href=\"http://cartodb.com/attributions\">CartoDB</a>", "Defines the attribution displayed on maps in the user interfaces.");
        section.Subdomains = ("abcd", "Defines the subdomains used for the Tile server used to display maps in the user interfaces.");
        section.MaxZoom = (19, "Defines the maximum zoom level on the maps in the user interfaces.");
        section.MinZoom = (1, "Defines the minimum zoom level on the maps in the user interfaces.");
        section.DefaultZoom = (5, "Defines the default zoom level on the maps in the user interfaces.");
        section.DefaultLatitude = (35.0458, "Defines the default center latitude on the maps in the user interfaces.");
        section.DefaultLongitude = (-85.3094, "Defines the default center longitude on the maps in the user interfaces.");

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