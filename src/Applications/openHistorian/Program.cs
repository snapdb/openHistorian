using Gemstone.Configuration;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.Extensions.Logging.EventLog;

namespace openHistorian;

internal class Program
{
    private static void Main(string[] args)
    {
        //SQLitePCL.Batteries.Init();

        Settings settings = new();

        IConfiguration configuration = new ConfigurationBuilder()
            .ConfigureGemstoneDefaults(settings.ConfigureAppSettings, useINI: true)
            .AddCommandLine(args)
            .Build();

        settings.Initialize(configuration);
        configuration.Bind(settings);

        HostApplicationBuilder application = Host.CreateApplicationBuilder(args);

        ConfigureLogging(application.Logging);

        application.Services.AddWindowsService(options =>
        {
            options.ServiceName = nameof(openHistorian);
        });

        application.Services.AddHostedService<ServiceHost>();

        IHost host = application.Build();
        host.Run();
    }

    private static void ConfigureLogging(ILoggingBuilder builder)
    {
        builder.ClearProviders();
        builder.SetMinimumLevel(LogLevel.Information);
        builder.AddFilter("Microsoft", LogLevel.Warning);
        builder.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Error);
        builder.AddFilter<DebugLoggerProvider>("", LogLevel.Debug);

        if (OperatingSystem.IsWindows())
            builder.AddFilter<EventLogLoggerProvider>("Application", LogLevel.Warning);

        builder.AddConsole(options => options.LogToStandardErrorThreshold = LogLevel.Error);
        builder.AddDebug();

    #if RELEASE
        if (OperatingSystem.IsWindows())
            builder.AddEventLog();
    #endif
    }
}