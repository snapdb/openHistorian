//******************************************************************************************************
//  InitialDatasetNorthAmerica.cs - Gbtc
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
//  02/05/2025 - Christoph Lackner
//       Generated original version of source code.
//
//******************************************************************************************************

using FluentMigrator;
using Gemstone.Data.SchemaMigration;

namespace SchemaDefinition.Migrations;

/// <summary>
/// The initial Location based Dateset for North America for the openHistorian database.
/// </summary>
[SchemaMigration(author: "C. Lackner", branchNumber: 0, year: 2025, month: 02, day: 06)]
[Tags("NorthAmerica", "Dataset")]
public class InitialDatsetNA : Migration
{
    /// <inheritdoc/>
    public override void Down()
    {

    }

    /// <inheritdoc/>
    public override void Up()
    {
        Insert.IntoTable("Company")
            .Row(new { Acronym= "AEP", MapAcronym = "AEP", Name = "American Electric Power", LoadOrder = 1 })
            .Row(new { Acronym = "AGP", MapAcronym = "AGP", Name = "Allegheny Power", LoadOrder = 2 })
            .Row(new { Acronym = "AMR", MapAcronym = "AMR", Name = "Ameren", LoadOrder = 3 })
            .Row(new { Acronym = "ATC", MapAcronym = "ATC", Name = "American Transmission Company", LoadOrder = 4 })
            .Row(new { Acronym = "BCH", MapAcronym = "BCH", Name = "British Columbia Hydro", LoadOrder = 5 })
            .Row(new { Acronym = "BE", MapAcronym = "BE", Name = "NSTAR Electric", LoadOrder = 6 })
            .Row(new { Acronym = "BH", MapAcronym = "BH", Name = "Bangor Hydro-Electric Company", LoadOrder = 7 })
            .Row(new { Acronym = "BPA", MapAcronym = "BPA", Name = "Bonneville Power Administration", LoadOrder = 8 })
            .Row(new { Acronym = "CONED", MapAcronym = "CED", Name = "ConEdison", LoadOrder = 9 })
            .Row(new { Acronym = "CX", MapAcronym = "CX", Name = "Northeast Utilities Services Company", LoadOrder = 10 })
            .Row(new { Acronym = "DOM", MapAcronym = "DOM", Name = "Dominion", LoadOrder = 11 })
            .Row(new { Acronym = "DUKE", MapAcronym = "DUK", Name = "Duke Power", LoadOrder = 12 })
            .Row(new { Acronym = "ENT", MapAcronym = "ENT", Name = "Entergy Services Inc", LoadOrder = 13 })
            .Row(new { Acronym = "EXE", MapAcronym = "EXE", Name = "Exelon Energy", LoadOrder = 14 })
            .Row(new { Acronym = "FEN", MapAcronym = "FEN", Name = "First Energy", LoadOrder = 15 })
            .Row(new { Acronym = "FPL", MapAcronym = "FPL", Name = "Florida Power & Light Company", LoadOrder = 16 })
            .Row(new { Acronym = "HEC", MapAcronym = "HEC", Name = "Hawaiian Electric Company", LoadOrder = 17 })
            .Row(new { Acronym = "HQC", MapAcronym = "HQC", Name = "Hydro Quebec", LoadOrder = 18 })
            .Row(new { Acronym = "IPTO", MapAcronym = "IPTO", Name = "Independent Power Transmission Operator", LoadOrder = 19 })
            .Row(new { Acronym = "ITC", MapAcronym = "ITC", Name = "International Transmission Company", LoadOrder = 20 })
            .Row(new { Acronym = "LAWP", MapAcronym = "LWP", Name = "Los Angeles Dept of Water and Power", LoadOrder = 21 })
            .Row(new { Acronym = "LIPA", MapAcronym = "LPA", Name = "Long Island Power Authority", LoadOrder = 22 })
            .Row(new { Acronym = "MAM", MapAcronym = "MAM", Name = "MidAmerican Power", LoadOrder = 23 })
            .Row(new { Acronym = "MDA", MapAcronym = "MDA", Name = "Ameritech", LoadOrder = 24 })
            .Row(new { Acronym = "MDK", MapAcronym = "MDK", Name = "Montana-Dakota ", LoadOrder = 25 })
            .Row(new { Acronym = "ME", MapAcronym = "ME", Name = "Central Maine Power Company", LoadOrder = 26 })
            .Row(new { Acronym = "METC", MapAcronym = "MTC", Name = "Michigan Electric Transmission Co.", LoadOrder = 27 })
            .Row(new { Acronym = "MISO", MapAcronym = "MSO", Name = "Midwest ISO", LoadOrder = 28 })
            .Row(new { Acronym = "MPC", MapAcronym = "MPC", Name = "Minnkota Power Collective", LoadOrder = 29 })
            .Row(new { Acronym = "MTB", MapAcronym = "MTB", Name = "Manitoba Hydro", LoadOrder = 30 })
            .Row(new { Acronym = "NE", MapAcronym = "NE", Name = "National Grid USA", LoadOrder = 31 })
            .Row(new { Acronym = "NEISO", MapAcronym = "NEI", Name = "New England ISO", LoadOrder = 32 })
            .Row(new { Acronym = "NH", MapAcronym = "NH", Name = "Public Service Company of New Hampshire", LoadOrder = 33 })
            .Row(new { Acronym = "NOJA", MapAcronym = "NOJA", Name = "NOJA Power Switchgear", LoadOrder = 34 })
            .Row(new { Acronym = "NYPA", MapAcronym = "NYP", Name = "New York Power Authority", LoadOrder = 35 })
            .Row(new { Acronym = "OGE", MapAcronym = "OGE", Name = "Oklahoma Gas & Electric", LoadOrder = 36 })
            .Row(new { Acronym = "PGE", MapAcronym = "PGE", Name = "Pacific Gas and Electric", LoadOrder = 38 })
            .Row(new { Acronym = "PJM", MapAcronym = "PJM", Name = "PJM Interconnection", LoadOrder = 39 })
            .Row(new { Acronym = "PPL", MapAcronym = "PPL", Name = "PPL Electric Utilities", LoadOrder = 40 })
            .Row(new { Acronym = "SCE", MapAcronym = "SCE", Name = "Southern California Edison", LoadOrder = 41 })
            .Row(new { Acronym = "SDGE", MapAcronym = "SDGE", Name = "San Diego Gas & Electric", LoadOrder = 42 })
            .Row(new { Acronym = "SOCO", MapAcronym = "SOC", Name = "Southern Company", LoadOrder = 43 })
            .Row(new { Acronym = "SPP", MapAcronym = "SPP", Name = "Southwest Power Pool", LoadOrder = 44 })
            .Row(new { Acronym = "SWT", MapAcronym = "SWT", Name = "Southwest (APS and SRP)", LoadOrder = 45 })
            .Row(new { Acronym = "TVA", MapAcronym = "TVA", Name = "Tennessee Valley Authority", LoadOrder = 46 })
            .Row(new { Acronym = "UI", MapAcronym = "UI", Name = "United Illuminating Company", LoadOrder = 47 })
            .Row(new { Acronym = "UTK", MapAcronym = "UTK", Name = "University of Tennessee, Knoxville", LoadOrder = 48 })
            .Row(new { Acronym = "VE", MapAcronym = "VE", Name = "Vermont Electric Company", LoadOrder = 49 })
            .Row(new { Acronym = "WAPA", MapAcronym = "WPA", Name = "Western Area Power Administration", LoadOrder = 50 });

        Insert.IntoTable("Interconnection")
            .Row(new { Acronym = "Eastern", Name = "Eastern Interconnection", LoadOrder = 0 })
            .Row(new { Acronym = "Western", Name = "Western Interconnection", LoadOrder = 1 })
            .Row(new { Acronym = "ERCOT", Name = "Texas Interconnection", LoadOrder = 2 })
            .Row(new { Acronym = "Quebec", Name = "Quebec Interconnection", LoadOrder = 3 })
            .Row(new { Acronym = "Alaskan", Name = "Alaskan Interconnection", LoadOrder = 4 })
            .Row(new { Acronym = "Hawaii", Name = "Islands of Hawaii", LoadOrder = 5 });
    }
}