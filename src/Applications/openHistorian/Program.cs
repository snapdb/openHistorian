#if RELEASE
using Microsoft.Extensions.Logging.EventLog;
#endif

using Gemstone.Threading;
using Gemstone.Timeseries;
using Newtonsoft.Json.Linq;
using openHistorian.Utility;

namespace openHistorian;

internal partial class Program
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
            DefineUserSpecificSettings(settings);

            // Bind settings to configuration sources
            settings.Bind(new ConfigurationBuilder()
                .ConfigureGemstoneDefaults(settings)
                .AddCommandLine(args, settings.SwitchMappings));

            GenerateUserDefaults(settings);

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

            SetupGrafanaHostingAdapter(settings);

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

    // TODO: This is not the best place for these settings, suggest moving to UI project or better location, then call from ServiceHost.DefineSettings
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

    private static void DefineUserSpecificSettings(Settings settings)
    {
        dynamic userSpecificSettings = settings["UserSettings"];
        userSpecificSettings.ConfigFile = ("UserDefaultSettings.json", "Defines a JSON file used to load the user specific settings if the user has not changed them.");
    }

    private static void GenerateUserDefaults(Settings settings)
    {
        dynamic userSpecificSettings = settings["UserSettings"];

        Environment.SpecialFolder specialFolder = Environment.SpecialFolder.CommonApplicationData;
        string appDataPath = Environment.GetFolderPath(specialFolder);
        string fullPath = Path.Combine(appDataPath, Common.ApplicationName, userSpecificSettings.ConfigFile);

        if (System.IO.File.Exists(fullPath))
            return;

        JObject defaults = new JObject();

        JObject general = new JObject();

        general.Add("HomePage", "System");
        general.Add("TimeFormat", "YYYY-MM-DDTHH:mm:ss");
        general.Add("ReloadInterval", 30);
        general.Add("DefaultDuration", 30);
        general.Add("DefaultDateTimeSetting", "startEnd");
        general.Add("ThemeID", 1);
        general.Add("UseAngleRef", false);
        general.Add("RefAnglePointTag", "");

        defaults.Add("General", general);

        JObject export = new JObject();

        export.Add("FrameRate", 30);
        export.Add("FrameRateUnit", "frames-per-second");
        export.Add("ColumnHeaderTemplate", "{PointTag}");

        defaults.Add("ExportData", export);

        JObject tutorials = new JObject();
        tutorials.Add("UseTutorials", true);

        defaults.Add("Tutorials", tutorials);

        JArray systemStatusWidgets = new JArray();

        var systemWdgets = new[] {
            new { Label = "Active Alarms", Enabled = true, ID = "Active Alarms" },
            new { Label = "Frequency Graph", Enabled = true, ID = "Frequency Graph" },
            new { Label = "Device Status Grid", Enabled = false, ID = "Device Status" },
            new { Label = "Device Status Map", Enabled = true, ID = "Status Map" },
            new { Label = "Frequency Map", Enabled = true, ID = "Frequency Map" },
            new { Label = "Phasor Chart", Enabled = false, ID = "Phasor Chart" },
            new { Label = "Failover Status", Enabled = false, ID = "Failover Status" }
        };

        foreach (var widget in systemWdgets)
        {
            JObject jsonWidget = new JObject();
            jsonWidget.Add("Label", widget.Label);
            jsonWidget.Add("Enabled", widget.Enabled);
            jsonWidget.Add("Setting", new JObject());
            jsonWidget.Add("ID", widget.ID);
            systemStatusWidgets.Add(jsonWidget);
        }

        defaults.Add("SystemStatusWidgets", systemStatusWidgets);

        JArray deviceCards = new JArray();

        var deviceCard = new[] {
            new { Label = "Settings", IsOpenOnLoad = true, Type = "GeneralSettings" },
            new { Label = "Measurement", IsOpenOnLoad = false, Type = "Measurement" },
            new { Label = "Measurement Data", IsOpenOnLoad = false, Type = "MeasurementData" },
            new { Label = "Phasor", IsOpenOnLoad = false, Type = "Phasor" },
            new { Label = "Phasor Data", IsOpenOnLoad = false, Type = "PhasorData" },
            new { Label = "Stats", IsOpenOnLoad = false, Type = "Stats" }
        };

        foreach (var card in deviceCard)
        {
            JObject jsonCard = new JObject();
            jsonCard.Add("Label", card.Label);
            jsonCard.Add("IsOpenOnLoad", card.IsOpenOnLoad);
            jsonCard.Add("Type", card.Type);
            deviceCards.Add(jsonCard);
        }

        defaults.Add("DeviceCards", deviceCards);

        JArray adapterCards = new JArray();

        var adapterCard = new[] {
            new { Label = "Settings", IsOpenOnLoad = true, Type = "Settings", Enabled = true },
            new { Label = "Status", IsOpenOnLoad = true, Type = "Status", Enabled = true },
            new { Label = "Connection String", IsOpenOnLoad = true, Type = "ConnectionString", Enabled = true },
            new { Label = "Input", IsOpenOnLoad = true, Type = "Input", Enabled = true },
            new { Label = "Output", IsOpenOnLoad = true, Type = "Output", Enabled = true },
        };

        foreach (var card in adapterCard)
        {
            JObject jsonCard = new JObject();
            jsonCard.Add("Label", card.Label);
            jsonCard.Add("IsOpenOnLoad", card.IsOpenOnLoad);
            jsonCard.Add("Type", card.Type);
            jsonCard.Add("Enabled", card.Enabled);
            adapterCards.Add(jsonCard);
        }

        defaults.Add("AdapterCards", adapterCards);

        File.WriteAllText(fullPath, defaults.ToString());

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