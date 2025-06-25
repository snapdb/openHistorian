//******************************************************************************************************
//  InitialSchema.cs - Gbtc
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
//  01/16/2025 - Christoph Lackner
//       Generated original version of source code.
//
//******************************************************************************************************

using FluentMigrator;
using Gemstone.Data.SchemaMigration;
using System.Data;

namespace SchemaDefinition.Migrations;

/// <summary>
/// The initial schema for the openHistorian database.
/// </summary>
[SchemaMigration(author: "C. Lackner", branchNumber: 0, year: 2025, month: 06, day: 25)]
public class PhasorValueView : Migration
{
    /// <inheritdoc/>
    public override void Down()
    {
        Execute.DeleteView("PhasorValues");
        Delete.FromTable("ConfigurationEntity").Row(new { RunTimeName = "PhasorValues" });
    }

    /// <inheritdoc/>
    public override void Up()
    {
        this.AddView("PhasorValues", @"
                M.DeviceID AS DeviceID,
                M.PointTag AS MagnitudePointTag,
                A.PointTag AS AnglePointTag,
                M.ID AS MagnitudeID,
                A.ID AS AngleID,
                M.PhasorID AS PhasorID,
                M.SignalID AS MagnitudeSignalID,
                A.SignalID AS AngleSignalID,
                M.SignalReference AS MagnitudeSignalReference,
            `   A.SignalReference AS AngleSignalReference
                M.PhasorLabel AS Label,
                COALESCE(M.PhasorType, 'V') AS Type,
                COALESCE(M.Phase, '+') AS Phase,
                M.SourceIndex AS SourceIndex,
                M.BaseKV AS BaseKV,
                M.Longitude AS Longitude,
                M.Latitude AS Latitude,
                M.Company AS Company,
                MAX(M.UpdatedOn, A.UpdatedOn) AS UpdatedOn,
                M.ID AS ID,
                M.DeviceID AS DeviceID, 
                M.SignalID AS SignalID,
            FROM 
                ActiveMeasurement M LEFT JOIN ActiveMeasurement A ON 
                M.PhasorID = A.PhasorID AND A.SignalType LIKE '%PHA' AND M.SignalType LIKE '%PHM'
            WHERE M.PhasorID IS NOT NULL
        ");

        Insert.IntoTable("ConfigurationEntity")
            .Row(new { SourceName = "PhasorValues", RuntimeName = "PhasorValues", Description = "Defines Phasor definitions for a PDC ", LoadOrder = 19, Enabled = 1 });


    }
}