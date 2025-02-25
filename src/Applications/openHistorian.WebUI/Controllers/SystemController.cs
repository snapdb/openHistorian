//******************************************************************************************************
//  SystemController.cs - Gbtc
//
//  Copyright © 2024, Grid Protection Alliance.  All Rights Reserved.
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
//  08/24/2024 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using Gemstone.Configuration;
using Gemstone.PhasorProtocols;
using Microsoft.AspNetCore.Mvc;
using openHistorian.Model;
using openHistorian.Utility;
using openHistorian.WebUI.Controllers.JsonModels;
using ServiceInterface;
using System.Diagnostics;
using static openHistorian.Utility.FailoverModule;

namespace openHistorian.WebUI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SystemController : Controller
{

    private readonly IServiceCommands m_serviceCommands = WebServer.ServiceCommands;

    [HttpGet, Route("reloadConfig")]
    public void ReloadConfig()
    {
        m_serviceCommands.ReloadConfig();
    }


    [HttpGet, Route("currentStatus")]
    public SystemStatus GetCurrentStatus()
    {
        (string status, ServiceStatus type, string description) = m_serviceCommands.GetCurrentStatus();
        
        return new SystemStatus
        {
            status = status, 
            type = type, 
            description = description
        };
    }

    [HttpPost, Route("sendCommand")]
    public void SendCommand(Guid connectionID, DeviceCommand command)
    {
        m_serviceCommands.SendCommand(connectionID, command);
    }

    /// <summary>
    /// Returns a true if this node should prevent the requesting node from starting up
    /// </summary>
    /// <returns> <see cref="true"/> if the other node should be prevented from starting up </returns>

    [HttpPost, Route("checkFailoverState")]
    public IActionResult checkFailoverStatus(FailoverRequest request)
    {

        if (!string.Equals(request.ClusterSecret, FailoverModule.ClusterSecret, StringComparison.InvariantCultureIgnoreCase))
            return Unauthorized();

        // Failover disablesS
        if (FailoverModule.SystemPriority == 0)
            return BadRequest("Failover is disabled on this System");
        
        FailoverLog log = new()
        {
            SystemName = request.SystemName,
            Priority = request.SystemPriotity,
            Timestamp = DateTime.UtcNow
        };

        FailoverResponse response = new()
        {
            SystemName = FailoverModule.SystemName,
            SystemPriority = FailoverModule.SystemPriority
        };

        if (request.SystemPriotity < FailoverModule.SystemPriority)
        {
            log.Message = $"Prevented startup of {log.SystemName} due to lower priority.";
            response.PreventStartup = true;
        }
        else if (request.SystemPriotity == FailoverModule.SystemPriority)
        {
            log.Message = "Node with matching priority started.";
            response.PreventStartup = false;
        }
        else
        {
            log.Message = $"Node with higher priority started. Shutting down {SystemName}";
            response.PreventStartup = false;
            Process.Start("ServiceActions.exe", $"--restart --service={FailoverModule.ServiceName}");

        }

        FailoverModule.LogMessage(log);

        return Ok(response);
    }

}
