﻿//******************************************************************************************************
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

using Gemstone.PhasorProtocols;
using Microsoft.AspNetCore.Mvc;
using openHistorian.WebUI.Controllers.JsonModels;
using ServiceInterface;

namespace openHistorian.WebUI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SystemController : Controller
{
    private readonly IServiceCommands m_serviceCommands = WebServer.ServiceCommands;


    [HttpGet, Route("currentStatus")]
    public SystemStatus GetCurrentStatus()
    {
        (string status, string type, string description) = m_serviceCommands.GetCurrentStatus();
        
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

    [HttpGet, Route("getEvents")]
    public IEnumerable<Event> GetEvents(DateTime startTime, DateTime endTime)
    {
        return
        [
            new Event
            {
                StartTime = DateTime.UtcNow - TimeSpan.FromMinutes(5.0D),
                EndTime = DateTime.UtcNow,
                PointTag = "SHELBY-AL2",
                Details = "Test Event",
                Type = "Test",
                ID = Guid.NewGuid()
            }
        ];
    }

    public void WriteEvent(Event evt)
    {
        // TODO: Write event to database with historian record
    }
}
