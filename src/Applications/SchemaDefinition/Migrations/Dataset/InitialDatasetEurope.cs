//******************************************************************************************************
//  InitialDatasetEurope.cs - Gbtc
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
using Gemstone.ComponentModel.DataAnnotations;
using Gemstone.Data.SchemaMigration;
using System.Collections.Generic;
using System.Data;

namespace SchemaDefinition.Migrations;

/// <summary>
/// The initial Location based Dateset for Europe for the openHistorian database.
/// </summary>
[SchemaMigration(author: "C. Lackner", branchNumber: 0, year: 2025, month: 02, day: 05)]
[Tags("Europe")]
public class InitialDatsetEU : Migration
{
    /// <inheritdoc/>
    public override void Down()
    {
       
    }

    /// <inheritdoc/>
    public override void Up()
    {
        Insert.IntoTable("Company")
            .Row(new { Acronym = "50Hertz", MapAcronym = "50Hertz", Name = "50Hertz Transmission", LoadOrder = 1 })
            .Row(new { Acronym = "Amprion", MapAcronym = "Amprion", Name = "Amprion", LoadOrder = 2 })
            .Row(new { Acronym = "APG", MapAcronym = "APG", Name = "Austrian Power Grid AG", LoadOrder = 3 })
            .Row(new { Acronym = "AST", MapAcronym = "AST", Name = "Augstsgrieguma tīkls", LoadOrder = 4 })
            .Row(new { Acronym = "ČEPS", MapAcronym = "ČEPS", Name = "ČEPS", LoadOrder = 5 })
            .Row(new { Acronym = "CGES", MapAcronym = "CGES", Name = "Crnogorski elektroprenosni sistem AD", LoadOrder = 6 })
            .Row(new { Acronym = "Creos", MapAcronym = "Creos", Name = "Creos Luxembourg", LoadOrder = 7 })
            .Row(new { Acronym = "Cyprus TSO", MapAcronym = "Cyprus TSO", Name = "Cyprus Transmission System Operator", LoadOrder = 8 })
            .Row(new { Acronym = "EirGrid", MapAcronym = "EirGrid", Name = "EirGrid", LoadOrder = 9 })
            .Row(new { Acronym = "Elering", MapAcronym = "Elering", Name = "Elering", LoadOrder = 10 })
            .Row(new { Acronym = "ELES", MapAcronym = "ELES", Name = "Elektro-Slovenija", LoadOrder = 11 })
            .Row(new { Acronym = "Elia", MapAcronym = "Elia", Name = "Elia Transmission Belgium", LoadOrder = 12 })
            .Row(new { Acronym = "EMS", MapAcronym = "EMS", Name = "Elektromreža Srbije", LoadOrder = 13 })
            .Row(new { Acronym = "ENS", MapAcronym = "ENS", Name = "Energie Netze Steiermark", LoadOrder = 14 })
            .Row(new { Acronym = "Energinet", MapAcronym = "Energinet", Name = "Energinet", LoadOrder = 15 })
            .Row(new { Acronym = "ESO", MapAcronym = "ESO", Name = "Electroenergien Sistemen Operator", LoadOrder = 16 })
            .Row(new { Acronym = "FEAS", MapAcronym = "FEAS", Name = "French Hosting Entity of ENTSO-E Awareness System", LoadOrder = 17 })
            .Row(new { Acronym = "Fingrid", MapAcronym = "Fingrid", Name = "Fingrid", LoadOrder = 18 })
            .Row(new { Acronym = "GEAS", MapAcronym = "GEAS", Name = "German Hosting Entity of ENTSO-E Awareness System", LoadOrder = 19 })
            .Row(new { Acronym = "HOPS", MapAcronym = "HOPS", Name = "Croatian Transmission System Operator", LoadOrder = 20 })
            .Row(new { Acronym = "Landsnet", MapAcronym = "Landsnet", Name = "Landsnet", LoadOrder = 21 })
            .Row(new { Acronym = "Litgrid", MapAcronym = "Litgrid", Name = "Litgrid", LoadOrder = 22 })
            .Row(new { Acronym = "MAVIR", MapAcronym = "MAVIR", Name = "Magyar Villamosenergia-ipari Átviteli Rendszerirányító ZRt.", LoadOrder = 23 })
            .Row(new { Acronym = "MEPSO", MapAcronym = "MEPSO", Name = "MEPSO", LoadOrder = 24 })
            .Row(new { Acronym = "NOS BiH", MapAcronym = "NOS BiH", Name = "BiH Independent System Operator", LoadOrder = 25 })
            .Row(new { Acronym = "OST", MapAcronym = "OST", Name = "Operatori I Sistemit te Transmetimit", LoadOrder = 26 })
            .Row(new { Acronym = "PSE", MapAcronym = "PSE", Name = "Polskie Sieci Elektroenergetyczne", LoadOrder = 27 })
            .Row(new { Acronym = "REE", MapAcronym = "REE", Name = "Red Eléctrica de España", LoadOrder = 28 })
            .Row(new { Acronym = "REN", MapAcronym = "REN", Name = "Redes Energéticas Nacionais", LoadOrder = 29 })
            .Row(new { Acronym = "RTE", MapAcronym = "RTE", Name = "Réseau de Transport d'Électricité", LoadOrder = 30 })
            .Row(new { Acronym = "SEPS", MapAcronym = "SEPS", Name = "Slovenská elektrizačná prenosová sústava", LoadOrder = 31 })
            .Row(new { Acronym = "SONI", MapAcronym = "SONI", Name = "System Operator for Northern Ireland", LoadOrder = 32 })
            .Row(new { Acronym = "SSE", MapAcronym = "SSE", Name = "Scottish and Southern Energy", LoadOrder = 33 })
            .Row(new { Acronym = "Statnett", MapAcronym = "Statnett", Name = "Statnett", LoadOrder = 34 })
            .Row(new { Acronym = "SVK", MapAcronym = "SVK", Name = "Svenska Kraftnät", LoadOrder = 35 })
            .Row(new { Acronym = "Swissgrid", MapAcronym = "Swissgrid", Name = "Swissgrid", LoadOrder = 36 })
            .Row(new { Acronym = "TEİAŞ", MapAcronym = "TEİAŞ", Name = "Turkish Electricity Transmission Corporation", LoadOrder = 37 })
            .Row(new { Acronym = "TERNA", MapAcronym = "TERNA", Name = "Terna", LoadOrder = 38 })
            .Row(new { Acronym = "TNG", MapAcronym = "TNG", Name = "Tennet TSO", LoadOrder = 39 })
            .Row(new { Acronym = "TE", MapAcronym = "TE", Name = "Transelectrica", LoadOrder = 40 })
            .Row(new { Acronym = "TTN", MapAcronym = "TTN", Name = "TenneT", LoadOrder = 41 })
            .Row(new { Acronym = "TUG", MapAcronym = "TUG", Name = "TU Graz", LoadOrder = 42 })
            .Row(new { Acronym = "Ukrenergo", MapAcronym = "Ukrenergo", Name = "Ukrenergo", LoadOrder = 43 })
            .Row(new { Acronym = "VUEN", MapAcronym = "VUEN", Name = "Vorarlberger Übertragungsnetz", LoadOrder = 44 });

        Insert.IntoTable("Interconnection")
            .Row(new { Acronym = "UCTE", Name = "Continental Europe", LoadOrder = 0 })
            .Row(new { Acronym = "Nordic", Name = "Nordic", LoadOrder = 1 })
            .Row(new { Acronym = "Baltic", Name = "Baltic", LoadOrder = 2 })
            .Row(new { Acronym = "UK", Name = "United Kingdom", LoadOrder = 3 })
            .Row(new { Acronym = "IE", Name = "Ireland", LoadOrder = 4 });

    }
}