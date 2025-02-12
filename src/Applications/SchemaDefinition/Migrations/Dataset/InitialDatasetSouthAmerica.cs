//******************************************************************************************************
//  InitialDatasetSouthAmerica.cs - Gbtc
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
/// The initial Location based Dateset for South America for the openHistorian database.
/// </summary>
[SchemaMigration(author: "C. Lackner", branchNumber: 0, year: 2025, month: 02, day: 12)]
[Tags("SouthAmerica", "Dataset")]
public class InitialDatsetUS : Migration
{
    /// <inheritdoc/>
    public override void Down()
    {

    }

    /// <inheritdoc/>
    public override void Up()
    {
        Insert.IntoTable("Company")
           
            .Row(new { Acronym = "ONS", MapAcronym = "ONS", Name = "Operador Nacional do Sistema Elétrico", LoadOrder = 1 })
            .Row(new { Acronym = "ELECTRONORTE", MapAcronym = "ELECTRONORTE", Name = "Centrais Elétricas Brasileiras S.A", LoadOrder = 2 })
            .Row(new { Acronym = "CELEO", MapAcronym = "CELEO", Name = "CELEO Brazilia", LoadOrder = 3 })
            .Row(new { Acronym = "CEMIG", MapAcronym = "CEMIG", Name = "Companhia Energética de Minas Gerais", LoadOrder = 4 })
            .Row(new { Acronym = "ELECTROSUL", MapAcronym = "ELECTROSUL", Name = "Eletrosul - Centrais Elétricas", LoadOrder = 5 })
            .Row(new { Acronym = "CHESF", MapAcronym = "CHESF", Name = "Eletrobras Chesf", LoadOrder = 6 })
            .Row(new { Acronym = "COPEL", MapAcronym = "COPEL", Name = "Companhia Paranaense de Energia", LoadOrder = 7 })
            .Row(new { Acronym = "CTEEP", MapAcronym = "CTEEP", Name = "Companhia de Transmissão de Energia Elétrica Paulista", LoadOrder = 8 })
            .Row(new { Acronym = "TAESA", MapAcronym = "TAESA", Name = "Transmissora Aliança de Energia Elétrica", LoadOrder = 9 })
            .Row(new { Acronym = "TBE", MapAcronym = "TBE", Name = "Transmissoras Brasileiras de Energia", LoadOrder = 10 })
            .Row(new { Acronym = "LXTE", MapAcronym = "LXTE", Name = "Linhas de Xingu Transmissora de Energia", LoadOrder = 11 })
            .Row(new { Acronym = "TME", MapAcronym = "TME", Name = "TotalEnergies", LoadOrder = 12 })
            .Row(new { Acronym = "ENGIE", MapAcronym = "ENGIE", Name = "Engie Brasil", LoadOrder = 13 })
            .Row(new { Acronym = "FURNAS", MapAcronym = "FURNAS", Name = "Eletrobras Furnas", LoadOrder = 14 })
            .Row(new { Acronym = "CPFL", MapAcronym = "CPFL", Name = "Companhia Paulista de Força e Luz", LoadOrder = 15 })
          
        Insert.IntoTable("Interconnection")
            .Row(new { Acronym = "SIN", Name = "Brazilian Interconnection", LoadOrder = 1 });
            .Row(new { Acronym = "Argentina", Name = "Argentine Interconnection System", LoadOrder = 2 });
            .Row(new { Acronym = "Chile", Name = "Chile National Electrical System", LoadOrder = 3 });
            .Row(new { Acronym = "SIEPAC", Name = "Central American Electrical Interconnection", LoadOrder = 4 });
          
    }
}