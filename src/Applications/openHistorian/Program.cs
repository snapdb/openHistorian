using System.Diagnostics;
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

        // Bind settings to configuration sources
        settings.Bind(new ConfigurationBuilder()
            .ConfigureGemstoneDefaults(settings)
            .AddCommandLine(args, settings.SwitchMappings));

        dynamic alarmingSettings = settings["Alarming"];

        string appName = alarmingSettings.ApplicationName;
        double timeout = alarmingSettings.UserDataProtectionTimeout;
        double timeout2 = alarmingSettings.UserDataProtectionTimeout2;
        double timeout3 = alarmingSettings.Expression;
        string username = alarmingSettings.UserName;
        List<string> items = alarmingSettings.Items;
        HashSet<int> hashSet = alarmingSettings.HashSet;

        Debug.WriteLine(appName);
        Debug.WriteLine(timeout);
        Debug.WriteLine(timeout2);
        Debug.WriteLine(timeout3);
        Debug.WriteLine(username);
        Debug.WriteLine(string.Join(",", items));
        Debug.WriteLine(string.Join(",", hashSet.Select(item => item.ToString())));

        dynamic cryptoSettings = settings["CryptographyServices"];

        cryptoSettings.UserDataProtectionTimeout = 8.0D;

        alarmingSettings.UserDataProtectionTimeout = Eval.Null;
        timeout = alarmingSettings.UserDataProtectionTimeout;

        alarmingSettings.UserDataProtectionTimeout2 = Eval.Null;
        timeout2 = alarmingSettings.UserDataProtectionTimeout2;

        Debug.WriteLine(timeout);
        Debug.WriteLine(timeout2);

        Debug.Assert(Environment.GetEnvironmentVariable("USERNAME")!.Equals(username, StringComparison.OrdinalIgnoreCase), "Environmental variables do not match!");

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

    #if DEBUG
        Settings.Save(forceSave: true);
    #else
        Settings.Save();
    #endif  
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