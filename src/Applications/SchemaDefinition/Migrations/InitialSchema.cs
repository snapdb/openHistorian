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
[SchemaMigration(author: "C. Lackner", branchNumber: 0, year: 2024, month: 07, day: 08)]
public class InitialSchema : Migration
{
    /// <inheritdoc/>
    public override void Down()
    {
        Delete.Table("Device");
        Delete.Table("Company");
        Delete.Table("Vendor");
        Delete.Table("VendorDevice");
        Delete.Table("Protocol");
        Delete.Table("SignalType");
        Delete.Table("Interconnection");
        Delete.Table("Measurement");
        Delete.Table("Phasor");
        Delete.Table("Historian");
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
            .WithColumn("CompanyID").AsInt32().Nullable().ForeignKey("Company", "ID")
            .WithColumn("HistorianID").AsInt32().Nullable()
            .WithColumn("AccessID").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("VendorDeviceID").AsInt32().Nullable().ForeignKey("VendorDevice", "ID")
            .WithColumn("ProtocolID").AsInt32().Nullable().ForeignKey("Protocol", "ID")
            .WithColumn("Longitude").AsDecimal(9, 6).Nullable()
            .WithColumn("Latitude").AsDecimal(9, 6).Nullable()
            .WithColumn("InterconnectionID").AsInt32().Nullable().ForeignKey("Interconnection", "ID")
            .WithColumn("ConnectionString").AsString().Nullable()
            .WithColumn("TimeZone").AsString(200).Nullable()
            .WithColumn("TimeAdjustmentTicks").AsInt16().NotNullable().WithDefaultValue(0)
            .WithColumn("ContactList").AsString().Nullable()
            .WithColumn("LoadOrder").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("Enabled").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("CreatedOn").AsDateTime().NotNullable().WithDefaultValue(DateTime.Now)
            .WithColumn("CreatedBy").AsString(200).NotNullable().WithDefaultValue("")
            .WithColumn("UpdatedOn").AsDateTime().NotNullable().WithDefaultValue(DateTime.Now)
            .WithColumn("UpdatedBy").AsString(200).NotNullable().WithDefaultValue("");
                 
        Create.UniqueConstraint("IX_Device_UniqueID").OnTable("Device").Column("UniqueID");
        Create.UniqueConstraint("IX_Device_UniqueAcronym").OnTable("Device").Columns("Acronym");

        // Company
        Create.Table("Company")
            .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
            .WithColumn("Acronym").AsString(200).NotNullable()
            .WithColumn("MapAcronym").AsString(10).NotNullable()
            .WithColumn("Name").AsString(200).NotNullable()
            .WithColumn("URL").AsString().Nullable()
            .WithColumn("LoadOrder").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("CreatedOn").AsDateTime().NotNullable().WithDefaultValue(DateTime.Now)
            .WithColumn("CreatedBy").AsString(200).NotNullable().WithDefaultValue("")
            .WithColumn("UpdatedOn").AsDateTime().NotNullable().WithDefaultValue(DateTime.Now)
            .WithColumn("UpdatedBy").AsString(200).NotNullable().WithDefaultValue("");

        // Vendor
        Create.Table("Vendor")
           .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
           .WithColumn("Acronym").AsString(200).NotNullable()
           .WithColumn("PhoneNumber").AsString(200).Nullable()
           .WithColumn("ContactEmail").AsString(200).Nullable()
           .WithColumn("Name").AsString(200).NotNullable()
           .WithColumn("URL").AsString().Nullable()
           .WithColumn("LoadOrder").AsInt32().NotNullable().WithDefaultValue(0)
           .WithColumn("CreatedOn").AsDateTime().NotNullable().WithDefaultValue(DateTime.Now)
           .WithColumn("CreatedBy").AsString(200).NotNullable().WithDefaultValue("")
           .WithColumn("UpdatedOn").AsDateTime().NotNullable().WithDefaultValue(DateTime.Now)
           .WithColumn("UpdatedBy").AsString(200).NotNullable().WithDefaultValue("");

        // VendorDevice
        Create.Table("VendorDevice")
           .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
           .WithColumn("VendorID").AsInt32().NotNullable().WithDefaultValue(10).ForeignKey("Vendor", "ID")
           .WithColumn("Name").AsString(200).NotNullable()
           .WithColumn("Description").AsString().Nullable()
           .WithColumn("URL").AsString().Nullable()
           .WithColumn("LoadOrder").AsInt32().NotNullable().WithDefaultValue(0)
           .WithColumn("CreatedOn").AsDateTime().NotNullable().WithDefaultValue(DateTime.Now)
           .WithColumn("CreatedBy").AsString(200).NotNullable().WithDefaultValue("")
           .WithColumn("UpdatedOn").AsDateTime().NotNullable().WithDefaultValue(DateTime.Now)
           .WithColumn("UpdatedBy").AsString(200).NotNullable().WithDefaultValue("");

        // Protocol
        Create.Table("Protocol")
           .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
           .WithColumn("Acronym").AsString(200).NotNullable()
           .WithColumn("Name").AsString(200).NotNullable()
           .WithColumn("Type").AsString(200).NotNullable().WithDefaultValue("Frame")
           .WithColumn("Category").AsString(200).NotNullable().WithDefaultValue("Phasor")
           .WithColumn("AssemblyName").AsString(1024).NotNullable().WithDefaultValue("PhasorProtocolAdapters.dll")
           .WithColumn("TypeName").AsString(200).NotNullable().WithDefaultValue("PhasorProtocolAdapters.PhasorMeasurementMapper")
           .WithColumn("CreatedOn").AsDateTime().NotNullable().WithDefaultValue(DateTime.Now)
           .WithColumn("CreatedBy").AsString(200).NotNullable().WithDefaultValue("")
           .WithColumn("UpdatedOn").AsDateTime().NotNullable().WithDefaultValue(DateTime.Now)
           .WithColumn("UpdatedBy").AsString(200).NotNullable().WithDefaultValue("")
           .WithColumn("LoadOrder").AsInt32().NotNullable().WithDefaultValue(0);

        //SignalType
        Create.Table("SignalType")
            .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
           .WithColumn("Name").AsString(200).NotNullable()
           .WithColumn("Acronym").AsString(4).NotNullable()
           .WithColumn("Suffix").AsString(2).NotNullable()
           .WithColumn("Abbreviation").AsString(2).NotNullable()
           .WithColumn("LongAcronym").AsString(200).NotNullable().WithDefaultValue("Undefined")
           .WithColumn("Source").AsString(10).NotNullable()
           .WithColumn("EngineeringUnits").AsString(10).Nullable()
           .WithColumn("CreatedOn").AsDateTime().NotNullable().WithDefaultValue(DateTime.Now)
           .WithColumn("CreatedBy").AsString(200).NotNullable().WithDefaultValue("")
           .WithColumn("UpdatedOn").AsDateTime().NotNullable().WithDefaultValue(DateTime.Now)
           .WithColumn("UpdatedBy").AsString(200).NotNullable().WithDefaultValue("")
           .WithColumn("LoadOrder").AsInt32().NotNullable().WithDefaultValue(0);

        // Interconnection
        Create.Table("Interconnection")
           .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
           .WithColumn("Acronym").AsString(200).NotNullable()
           .WithColumn("Name").AsString(200).NotNullable()
           .WithColumn("CreatedOn").AsDateTime().NotNullable().WithDefaultValue(DateTime.Now)
           .WithColumn("CreatedBy").AsString(200).NotNullable().WithDefaultValue("")
           .WithColumn("UpdatedOn").AsDateTime().NotNullable().WithDefaultValue(DateTime.Now)
           .WithColumn("UpdatedBy").AsString(200).NotNullable().WithDefaultValue("")
           .WithColumn("LoadOrder").AsInt32().NotNullable().WithDefaultValue(0);

        // Measurement
        Create.Table("Measurement")
           .WithColumn("PointID").AsInt32().NotNullable().PrimaryKey().Identity()
           .WithColumn("SignalID").AsString(36).Nullable()
           .WithColumn("HistorianID").AsInt32().Nullable().ForeignKey("Historian", "ID")
           .WithColumn("DeviceID").AsInt32().Nullable().ForeignKey("Device", "ID")
           .WithColumn("PointTag").AsString(200).NotNullable()
           .WithColumn("AlternateTag").AsString().Nullable()
           .WithColumn("SignalReference").AsString(200).NotNullable()
           .WithColumn("SignalTypeID").AsInt32().NotNullable().ForeignKey("SignalType", "ID")
           .WithColumn("PhasorSourceIndex").AsInt32().Nullable()
           .WithColumn("Adder").AsDouble().NotNullable().WithDefaultValue(0.0)
           .WithColumn("Multiplier").AsDouble().NotNullable().WithDefaultValue(1.0)
           .WithColumn("Description").AsString().Nullable()
           .WithColumn("Subscribed").AsBoolean().NotNullable().WithDefaultValue(false)
           .WithColumn("Internal").AsBoolean().NotNullable().WithDefaultValue(true)
           .WithColumn("Enabled").AsBoolean().NotNullable().WithDefaultValue(false)
           .WithColumn("CreatedOn").AsDateTime().NotNullable().WithDefaultValue(DateTime.Now)
           .WithColumn("CreatedBy").AsString(200).NotNullable().WithDefaultValue("")
           .WithColumn("UpdatedOn").AsDateTime().NotNullable().WithDefaultValue(DateTime.Now)
           .WithColumn("UpdatedBy").AsString(200).NotNullable().WithDefaultValue("");

        Create.Table("Phasor")
            .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
            .WithColumn("DeviceID").AsInt32().NotNullable().ForeignKey("Device", "ID").OnDelete(System.Data.Rule.Cascade)
            .WithColumn("Label").AsString(200).NotNullable()
            .WithColumn("Type").AsString(1).NotNullable().WithDefaultValue("V")
            .WithColumn("Phase").AsString(1).NotNullable().WithDefaultValue("+")
            .WithColumn("SourceIndex").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("PrimaryVoltageID").AsInt32().Nullable().ForeignKey("Phasor", "ID")
            .WithColumn("SecondaryVoltageID").AsInt32().Nullable().ForeignKey("Phasor", "ID")
            .WithColumn("BaseKV").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("CreatedOn").AsDateTime().NotNullable().WithDefaultValue(DateTime.Now)
            .WithColumn("CreatedBy").AsString(200).NotNullable().WithDefaultValue("")
            .WithColumn("UpdatedOn").AsDateTime().NotNullable().WithDefaultValue(DateTime.Now)
            .WithColumn("UpdatedBy").AsString(200).NotNullable().WithDefaultValue("");

        Create.UniqueConstraint("IX_Phasor_DeviceID_SourceIndex").OnTable("Phasor").Columns("DeviceID","SourceIndex");

        Create.Table("Historian")
            .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
            .WithColumn("Acronym").AsString(200).NotNullable()
            .WithColumn("Name").AsString(200).Nullable()
            .WithColumn("AssemblyName").AsString().Nullable()
            .WithColumn("TypeName").AsString().Nullable()
            .WithColumn("ConnectionString").AsString().Nullable()
            .WithColumn("IsLocal").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("Description").AsString().Nullable()
            .WithColumn("LoadOrder").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("Enabled").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("CreatedOn").AsDateTime().NotNullable().WithDefaultValue(DateTime.Now)
            .WithColumn("CreatedBy").AsString(200).NotNullable().WithDefaultValue("")
            .WithColumn("UpdatedOn").AsDateTime().NotNullable().WithDefaultValue(DateTime.Now)
            .WithColumn("UpdatedBy").AsString(200).NotNullable().WithDefaultValue("");
    }
}
