//******************************************************************************************************
//  MeasurementOpsController.cs - Gbtc
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
//  02/05/2025 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using Gemstone.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PhasorProtocolAdapters;
using ConfigSettings = Gemstone.Configuration.Settings;

namespace openHistorian.WebUI.Controllers;

/// <summary>
/// Controller for common measurement operations.
/// </summary>
[Route("api/MeasurementOps")]
[ApiController]
public class MeasurementOpsController : Controller
{
    private static string? s_companyAcronym;

    private static string ReadCompanyAcronymFromConfig()
    {
        try
        {
            dynamic section = ConfigSettings.Default[ConfigSettings.SystemSettingsCategory];

            string companyAcronym = section["CompanyAcronym", "GPA", "The acronym representing the company who owns the host system."];

            if (string.IsNullOrWhiteSpace(companyAcronym))
                companyAcronym = "GPA";

            return companyAcronym;
        }
        catch (Exception ex)
        {
            Logger.SwallowException(ex, "Failed to load company acronym from settings");
            return "GPA";
        }
    }

    /// <summary>
    /// Gets the company acronym for the host system.
    /// </summary>
    public static string CompanyAcronym => s_companyAcronym ??= ReadCompanyAcronymFromConfig();

    /// <summary>
    /// Gets the company acronym for the host system.
    /// </summary>
    [HttpGet]
    [Route("CompanyAcronym")]
    public string GetCompanyAcronym() => CompanyAcronym;

    /// <summary>
    /// Creates a new point tag name (based on defined tag naming expression) for a device.
    /// </summary>
    /// <param name="deviceAcronym">Device acronym name.</param>
    /// <param name="signalTypeAcronym">Signal type acronym name.</param>
    /// <returns>New point tag for device.</returns>
    [HttpGet]
    public string CreatePointTag(string deviceAcronym, string signalTypeAcronym)
    {
        return CommonPhasorServices.CreatePointTag(CompanyAcronym, deviceAcronym, null, signalTypeAcronym);
    }

    /// <summary>
    /// Creates a new point tag name (based on defined tag naming expression) for a device that has an index.
    /// </summary>
    /// <param name="deviceAcronym">Device acronym name.</param>
    /// <param name="signalTypeAcronym">Signal type acronym name.</param>
    /// <param name="signalIndex">Signal index.</param>
    /// <param name="label">Label for point tag.</param>
    /// <returns>New point tag for device.</returns>
    [HttpGet]
    public string CreateIndexedPointTag(string deviceAcronym, string signalTypeAcronym, int signalIndex, string label)
    {
        return CommonPhasorServices.CreatePointTag(CompanyAcronym, deviceAcronym, null, signalTypeAcronym, label, signalIndex);
    }

    /// <summary>
    /// Creates a new point tag name (based on defined tag naming expression) for a device that is for a phasor.
    /// </summary>
    /// <param name="deviceAcronym">Device acronym name.</param>
    /// <param name="signalTypeAcronym">Signal type acronym name.</param>
    /// <param name="phasorLabel">Phasor label.</param>
    /// <param name="phase">Phase of phasor.</param>
    /// <param name="signalIndex">Signal index.</param>
    /// <param name="baseKV">Target kV for phasor.</param>
    /// <returns>New point tag for device.</returns>
    [HttpGet]
    public string CreatePhasorPointTag(string deviceAcronym, string signalTypeAcronym, string phasorLabel, string phase, int signalIndex, int baseKV)
    {
        return CommonPhasorServices.CreatePointTag(CompanyAcronym, deviceAcronym, null, signalTypeAcronym, phasorLabel, signalIndex, string.IsNullOrWhiteSpace(phase) ? '_' : phase.Trim()[0], baseKV);
    }
}
