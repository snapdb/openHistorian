//  ServiceHost.cs - Gbtc
//
//  Copyright © 2025, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
//  file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  03/13/2024 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System.Data;
using Gemstone.PhasorProtocols;
using Gemstone.Timeseries;
using Gemstone.Timeseries.Adapters;
using Gemstone.Timeseries.Data;
using openHistorian.Utility;
using Gemstone.Timeseries.Adapters;
using openHistorian.WebUI;
using ServiceInterface;

namespace openHistorian;

internal sealed class ServiceHost : ServiceHostBase, IServiceCommands
{
    private readonly ILogger<ServiceHost> m_logger;

    public ServiceHost(ILogger<ServiceHost> logger) : base(logger)
    {
        m_logger = logger;

        LibraryEvents.SuppressedException += (_, args) =>
        {
            if (args.ExceptionObject is Exception ex)
            {
                string message = ex.Message;
                m_logger.LogError(ex, "Suppressed Exception: {message}", message);
            }
            else
            {
                string message = args.ExceptionObject.ToString() ?? "Unknown";
                m_logger.LogError("Suppressed Exception: {message}", message);
            }
        };

    #if DEBUG
        SettingsSection section = Settings.Instance[Settings.SystemSettingsCategory];
        m_logger.LogInformation("Service host config source: {connectionString}", section["ConnectionString"]);
    #endif
    }

    //m_logger.LogInformation("Example log information at: {time}", DateTimeOffset.Now);
    //m_logger.LogWarning("Example log warning at: {time}", DateTimeOffset.Now);
    //m_logger.LogError(ex, "{message}", ex.Message);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using WebHosting webHosting = new();
        WebServer? webServer = null;

        try
        {
            webHosting.Initialize();
            webServer = webHosting.BuildServer(m_logger, this);

            // Start the web server in a separate long-running task
            await Task.Factory.StartNew(async () => 
            {
                await webServer.StartAsync()
                    .ContinueWith(task => m_logger.LogError(task.Exception, "Failed to start web server."), TaskContinuationOptions.OnlyOnFaulted);
            },
            TaskCreationOptions.LongRunning);

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
            m_logger.LogError(ex, "{message}", ex.Message);

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
            // Shut down the web server
            if (webServer is not null)
                await webServer.StopAsync();
        }
    }

    /// <inheritdoc />
    public void SendCommand(Guid connectionID, DeviceCommand command)
    {
        // TODO: Implement phasor protocol command send
    }

    /// <inheritdoc />
    public IAdapter GetActiveAdapterInstance(uint runtimeID)
    {
        AllAdaptersCollection allAdapters = AllAdapters ?? throw new NullReferenceException("No adapters are currently defined");

        if (!allAdapters.TryGetAnyAdapterByID(runtimeID, out IAdapter? adapter, out _))
            throw new InvalidOperationException($"Failed to find adapter with runtime ID {runtimeID}");

        return adapter ?? throw new NullReferenceException("Adapter instance is not initialized");
    }

    /// <inheritdoc />
    public (string Status, ServiceStatus Type, string Description) GetCurrentStatus()
    {
        // WARNING / NORMAL / ERROR
        return ("ONLINE", ServiceStatus.Normal, "openHistorian service is running normal.");
    }

    private void GetStatsForCurrentStatus()
    {
        // "SignalReference LIKE '%SYSTEM-ST16'" // [0] System CPU Usage
        // "SignalReference LIKE '%SYSTEM-ST20'" // [1] System Memory Usage
        // "SignalReference LIKE '%SYSTEM-ST24'" // [2] System Time Deviation
        // "SignalReference LIKE '%SYSTEM-ST25'" // [3] Primary Disk Usage
    }


    /// <summary>
    /// Establishes default settings for the config file.
    /// </summary>
    public static void DefineSettings(Settings settings)
    {
        using (Logger.SuppressFirstChanceExceptionLogMessages())
        {
            DiagnosticsLogger.DefineSettings(settings);
            WebHosting.DefineSettings(settings);
            ServiceHostBase.DefineSettings(settings);
            FailOverModule.DefineSettings(settings);
        }
    }
}
