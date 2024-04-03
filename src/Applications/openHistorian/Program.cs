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
        Settings settings = new()
        {
            INIFile = ConfigurationOperation.ReadWrite,
            SQLite = ConfigurationOperation.Disabled
        };

        // Define settings for known components
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