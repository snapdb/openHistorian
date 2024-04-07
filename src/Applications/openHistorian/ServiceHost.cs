using Gemstone;
using Gemstone.Configuration;
using Gemstone.Data;
using Gemstone.Diagnostics;
using Gemstone.IO;
using Gemstone.Security.Cryptography;
using openHistorian.WebUI;

namespace openHistorian;

internal sealed class ServiceHost : BackgroundService
{
    private readonly ILogger<ServiceHost> m_logger;

    public ServiceHost(ILogger<ServiceHost> logger)
    {
        m_logger = logger;

        LibraryEvents.SuppressedException += (sender, args) =>
        {
            if (args.ExceptionObject is Exception ex)
            {
                string message = ex.Message;
                m_logger.LogError(ex, "Suppressed Exception: {message}", message);
            }
            else
            {
                string message = args.ExceptionObject?.ToString() ?? "Unknown";
                m_logger.LogError("Suppressed Exception: {message}", message);
            }
        };
    }
    //m_logger.LogInformation("Example log information at: {time}", DateTimeOffset.Now);
    //m_logger.LogWarning("Example log warning at: {time}", DateTimeOffset.Now);
    //m_logger.LogError(ex, "{Message}", ex.Message);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using WebHosting hosting = new();
        WebServer? server = null;

        try
        {
            hosting.Initialize();
            server = hosting.BuildServer();

            await server.StartAsync().ContinueWith(task => m_logger.LogError(task.Exception, "Failed to start web server."), TaskContinuationOptions.OnlyOnFaulted);

            m_logger.LogInformation("Service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            // When the stopping token is canceled, for example, a call made from services.msc,
            // we shouldn't exit with a non-zero exit code. In other words, this is expected...
        }
        catch (Exception ex)
        {
            m_logger.LogError(ex, "{Message}", ex.Message);

            // Terminates this process and returns an exit code to the operating system.
            // This is required to avoid the 'BackgroundServiceExceptionBehavior', which
            // performs one of two scenarios:

            // 1. When set to "Ignore": will do nothing at all, errors cause zombie services.
            // 2. When set to "StopHost": will cleanly stop the host, and log errors.
            
            // In order for the Windows Service Management system to leverage configured
            // recovery options, we need to terminate the process with a non-zero exit code.
            Environment.Exit(1);
        }
        finally
        {
            if (server is not null)
                await server.StopAsync();
        }
    }

    /// <summary>
    /// Establishes default settings for the config file.
    /// </summary>
    public static void DefineSettings(Settings settings)
    {
        DiagnosticsLogger.DefineSettings(settings);
        AdoDataConnection.DefineSettings(settings);
        DataProtection.DefineSettings(settings);
        WebHosting.DefineSettings(settings);
        MultipleDestinationExporter.DefineSettings(settings, "HealthExporter");
    }
}
