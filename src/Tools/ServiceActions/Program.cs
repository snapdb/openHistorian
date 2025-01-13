//******************************************************************************************************
//  Program.cs - Gbtc
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
//  01/12/2025 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using Gemstone.Console;
using Gemstone.Diagnostics;

namespace ServiceActions;

internal class Program
{
    private static void Main()
    {
        Arguments args = new(Environment.CommandLine);

        string? serviceName = args["service"];

        if (!string.IsNullOrWhiteSpace(serviceName))
        {
            if (args.Exists("stop"))
            {
                StopService(serviceName);
                Environment.Exit(0);
            }
            else if (args.Exists("start"))
            {
                StartService(serviceName);
                Environment.Exit(0);
            }
            else if (args.Exists("restart"))
            {
                RestartService(serviceName);
                Environment.Exit(0);
            }
        }

        Console.Error.WriteLine("Example Usage: ServiceActions --restart --service=<ServiceName>");
        Environment.Exit(1);
    }

    private static void StopService(string serviceName)
    {
        // If not Windows, just exit
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.Error.WriteLine("ServiceActions is only supported on Windows operating systems.");
            Environment.Exit(1);
        }

        // Attempt to access service controller for the specified Windows service
        ServiceController? serviceController = ServiceController.GetServices().SingleOrDefault(svc => string.Compare(svc.ServiceName, serviceName, StringComparison.OrdinalIgnoreCase) == 0);

        if (serviceController is null)
        {
            Console.Error.WriteLine($"Failed to find the {serviceName} Windows service.");
            Environment.Exit(1);
        }

        try
        {
            if (serviceController.Status == ServiceControllerStatus.Running)
            {
                Console.WriteLine($"Attempting to stop the {serviceName} Windows service...");

                serviceController.Stop();

                // Wait forever for service to stop for 20 seconds
                serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(20.0D));

                Console.WriteLine(serviceController.Status == ServiceControllerStatus.Stopped ?
                    $"Successfully stopped the {serviceName} Windows service." :
                    $"Failed to stop the {serviceName} Windows service after trying for 20 seconds...");

                // Add an extra line for visual separation of service termination status
                Console.WriteLine();
            }
        }
        catch (Exception ex)
        {
            string errorMessage = $"Failed to stop the {serviceName} Windows service: {ex.Message}";
            Console.Error.WriteLine(errorMessage);
            Logger.SwallowException(ex, errorMessage);
            Environment.Exit(1);
        }

        // If the service failed to stop, or it is installed as stand-alone debug application, we try to forcibly stop any remaining running instances
        try
        {
            Process[] instances = Process.GetProcessesByName(serviceName);

            if (instances.Length > 0)
            {
                int total = 0;
                Console.WriteLine($"Attempting to stop running instances of the {serviceName}...");

                // Terminate all instances of service running on the local computer
                foreach (Process process in instances)
                {
                    process.Kill();
                    total++;
                }

                if (total > 0)
                    Console.WriteLine($"Stopped {total:N0} {serviceName} instance{(total > 1 ? "s" : "")}.");

                // Add an extra line for visual separation of process termination status
                Console.WriteLine();
            }
        }
        catch (Exception ex)
        {
            string errorMessage = $"Failed to terminate running instances of the {serviceName}: {ex.Message}";
            Console.Error.WriteLine(errorMessage);
            Logger.SwallowException(ex, errorMessage);
            Environment.Exit(1);
        }
    }

    private static void StartService(string serviceName)
    {
        // If not Windows, just return
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.Error.WriteLine("ServiceActions is only supported on Windows operating systems.");
            Environment.Exit(1);
        }

        // Attempt to access service controller for the specified Windows service
        ServiceController? serviceController = ServiceController.GetServices().SingleOrDefault(svc => string.Compare(svc.ServiceName, serviceName, StringComparison.OrdinalIgnoreCase) == 0);

        if (serviceController is null)
        {
            Console.Error.WriteLine($"Failed to find the {serviceName} Windows service.");
            Environment.Exit(1);
        }

        // Attempt to start Windows service...
        try
        {
            // Refresh state in case service process was forcibly stopped
            serviceController.Refresh();

            if (serviceController.Status != ServiceControllerStatus.Running)
                serviceController.Start();
        }
        catch (Exception ex)
        {
            string errorMessage = $"Failed to restart the {serviceName} Windows service: {ex.Message}";
            Console.Error.WriteLine(errorMessage);
            Logger.SwallowException(ex, errorMessage);
            Environment.Exit(1);
        }
    }

    private static void RestartService(string serviceName)
    {
        StopService(serviceName);
        StartService(serviceName);
    }
}
