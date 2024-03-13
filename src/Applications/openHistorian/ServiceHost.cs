using openHistorian.WebUI;

namespace openHistorian;

public sealed class ServiceHost : BackgroundService
{
    private readonly ILogger<ServiceHost> m_logger;

    public ServiceHost(ILogger<ServiceHost> logger)
    {
        m_logger = logger;

        //m_logger.LogInformation("Example log information at: {time}", DateTimeOffset.Now);
        //m_logger.LogWarning("Example log warning at: {time}", DateTimeOffset.Now);
        //m_logger.LogError(ex, "{Message}", ex.Message);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            WebServer server = WebHosting.BuildServer();

            await server.StartAsync(Settings.Instance.HostURLs.Split(','))
                .ContinueWith(task => m_logger.LogError(task.Exception, "Failed to run admin server."), TaskContinuationOptions.OnlyOnFaulted);

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
    }
}
