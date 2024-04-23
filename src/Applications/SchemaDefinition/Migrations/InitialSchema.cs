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
//  04/22/2024 - Christoph Lackner
//       Generated original version of source code.
//
//******************************************************************************************************

using FluentMigrator;
using Gemstone.Data.SchemaMigration;

namespace SchemaDefinition.Migrations;

/// <summary>
/// The initial schema for the openHistorian database.
/// </summary>
[SchemaMigration(author: "C. Lackner", branchNumber: 0, year: 2024, month: 04, day: 22)]
public class InitialSchema : Migration
{
    /// <inheritdoc/>
    public override void Down()
    {
        Delete.Table("Device");
    }

    /// <inheritdoc/>
    public override void Up()
    {
        Create.Table("Device")
            .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
            .WithColumn("ParentID").AsInt32().Nullable().ForeignKey("Device", "ID")
            .WithColumn("UniqueID").AsString().Nullable()
            .WithColumn("Acronym").AsString(200).NotNullable()
            .WithColumn("Name").AsString(200).Nullable()
            .WithColumn("OriginalSource").AsString(200).Nullable()
            .WithColumn("IsConcentrator").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("CompanyID").AsInt32().Nullable()
            .WithColumn("HistorianID").AsInt32().Nullable()
            .WithColumn("AccessID").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("VendorDeviceID").AsInt32().Nullable()
            .WithColumn("ProtocolID").AsInt32().Nullable()
            .WithColumn("Longitude").AsDecimal(9, 6).Nullable()
            .WithColumn("Latitude").AsDecimal(9, 6).Nullable()
            .WithColumn("InterconnectionID").AsInt32().Nullable()
            .WithColumn("ConnectionString").AsString().Nullable()
            .WithColumn("TimeZone").AsString(200).Nullable()
            .WithColumn("LoadOrder").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("Enabled").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("CreatedOn").AsDateTime().NotNullable().WithDefaultValue(DateTime.Now)
            .WithColumn("CreatedBy").AsString(200).NotNullable().WithDefaultValue("")
            .WithColumn("UpdatedOn").AsDateTime().NotNullable().WithDefaultValue(DateTime.Now)
            .WithColumn("UpdatedBy").AsString(200).NotNullable().WithDefaultValue("");
                 
        Create.UniqueConstraint("IX_Device_UniqueID").OnTable("Device").Column("UniqueID");
        Create.UniqueConstraint("IX_Device_NodeID_Acronym").OnTable("Device").Columns("Acronym");
    }
}