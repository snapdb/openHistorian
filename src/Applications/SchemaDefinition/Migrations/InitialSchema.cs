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

namespace SchemaDefinition.Migrations;

/// <summary>
/// The initial schema for the openHistorian database.
/// </summary>
[SchemaMigration(author: "C. Lackner", branchNumber: 0, year: 2025, month: 01, day: 16)]
public class InitialSchema : Migration
{
    /// <inheritdoc/>
    public override void Down()
    {
        //MetaData Information
        Delete.Table("Company");
        Delete.Table("Vendor");
        Delete.Table("VendorDevice");
        Delete.Table("SignalType");
        Delete.Table("Interconnection");

        //Device related
        Delete.Table("Device");
        Delete.Table("Measurement");
        Delete.Table("Phasor");
        Delete.Table("OutputStream");
        Delete.Table("OutputStreamDevice");
        Delete.Table("OutputStreamMeasurement");
        Delete.Table("OutputStreamDeviceAnalog");
        Delete.Table("OutputStreamDeviceDigital");
        Delete.Table("OutputStreamDevicePhasor");
        Delete.Table("Statistic");

        Delete.Table("Subscriber");
        Delete.Table("SubscriberMeasurement");
        Delete.Table("MeasurementGroup");
        Delete.Table("SubscriberMeasurementGroup");
        Delete.Table("MeasurementGroupMeasurement");

        //Security related
        Delete.Table("AccessLog");
        Delete.Table("UserAccount");
        Delete.Table("SecurityGroup");
        Delete.Table("ApplicationRole");
        Delete.Table("ApplicationRoleSecurityGroup");
        Delete.Table("ApplicationRoleUserAccount");
        Delete.Table("SecurityGroupUserAccount"); 

        //System related
        Delete.Table("Historian");
        Delete.Table("ErrorLog");
        Delete.Table("Runtime");
        Delete.Table("ConfigurationEntity");

        //Adapter related
        Delete.Table("CustomActionAdapter");
        Delete.Table("CustomInputAdapter");
        Delete.Table("CustomFilterAdapter");
        Delete.Table("CustomOutputAdapter");

        /// Related to Grafana Device Status
        Delete.Table("AlarmState");
        Delete.Table("AlarmDevice");
        Delete.Table("DataAvailability");

    }

    /// <inheritdoc/>
    public override void Up()
    {
        //MetaData Information
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
        
        Create.Table("Interconnection")
           .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
           .WithColumn("Acronym").AsString(200).NotNullable()
           .WithColumn("Name").AsString(200).NotNullable()
           .WithColumn("CreatedOn").AsDateTime().NotNullable().WithDefaultValue(DateTime.Now)
           .WithColumn("CreatedBy").AsString(200).NotNullable().WithDefaultValue("")
           .WithColumn("UpdatedOn").AsDateTime().NotNullable().WithDefaultValue(DateTime.Now)
           .WithColumn("UpdatedBy").AsString(200).NotNullable().WithDefaultValue("")
           .WithColumn("LoadOrder").AsInt32().NotNullable().WithDefaultValue(0);

        //Device related
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
            .WithColumn("MeasuredLines").AsInt32().Nullable()
            .WithColumn("LoadOrder").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("Enabled").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("CreatedOn").AsDateTime().NotNullable().WithDefaultValue(DateTime.Now)
            .WithColumn("CreatedBy").AsString(200).NotNullable().WithDefaultValue("")
            .WithColumn("UpdatedOn").AsDateTime().NotNullable().WithDefaultValue(DateTime.Now)
            .WithColumn("UpdatedBy").AsString(200).NotNullable().WithDefaultValue("");
                 
        Create.IndexForTable("Device").Column("UniqueID");
        Create.UniqueConstraint("IX_Device_UniqueAcronym").OnTable("Device").Columns("Acronym");

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

        Create.Table("OutputStream")
            .WithColums("ID").AsInt32().NotNullable().PrimaryKey().Identity()
            .WithColumn("Acronym").AsString(200).NotNullable()
            .WithColumn("Name").AsString(200).Nullable()
            .WithColumn("Type").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("ConnectionString").AsString().Nullable()
            .WithColumn("DataChannel").AsString().Nullable()
            .WithColumn("CommandChannel").AsString().Nullable()
            .WithColumn("IDCode").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("AutoPublishConfigFrame").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("AutoStartDataChannel").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("NominalFrequency").AsInt32().NotNullable().WithDefaultValue(60)
            .WithColumn("FramesPerSecond").AsInt32().NotNullable().WithDefaultValue(30)
            .WithColumn("LagTime").AsDecimal().NotNullable().WithDefaultValue(3.0)
            .WithColumn("LeadTime").AsDecimal().NotNullable().WithDefaultValue(1.0)
            .WithColumn("UseLocalClockAsRealTime").AsBoolean().NotNullable().WithDefaultValue(0)
            .WithColumn("AllowSortsByArrival").AsBoolean().NotNullable().WithDefaultValue(1)
            .WithColumn("IgnoreBadTimeStamps").AsBoolean().NotNullable().WithDefaultValue(0)
            .WithColumn("TimeResolution").AsInt32().NotNullable().WithDefaultValue(330000)
            .WithColumn("AllowPreemptivePublishing").AsBoolean().NotNullable().WithDefaultValue(1)
            .WithColumn("PerformTimeReasonabilityCheck").AsBoolean().NotNullable().WithDefaultValue(1)
            .WithColumn("DownsamplingMethod").AsString(15).NotNullable().WithDefaultValue("LastReceived")
            .WithColumn("DataFormat".AsString(15).NotNullable().WithDefaultValue("FloatingPoint")
            .WithColumn("CoordinateFormat").AsString(15).NotNullable().WithDefaultValue("Polar")
            .WithColumn("CurrentScalingValue").AsInt32().NotNullable().WithDefaultValue(2423)
            .WithColumn("VoltageScalingValue").AsInt32().NotNullable().WithDefaultValue(2725785)
            .WithColumn("AnalogScalingValue").AsInt32().NotNullable().WithDefaultValue(1373291)
            .WithColumn("DigitalMaskValue").AsInt32().NotNullable().WithDefaultValue(-65536)
            .WithColumn("LoadOrder").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("Enabled").AsBoolean().NotNullable().WithDefaultValue(0)
            .WithColumn("CreatedOn").AsDateTime().NotNullable().WithDefaultValue("")
            .WithColumn("CreatedBy").AsString(200).NotNullable().WithDefaultValue("")
            .WithColumn("UpdatedOn").AsDateTime().NotNullable().WithDefaultValue("")
            .WithColumn("UpdatedBy").AsString(200).NotNullable().WithDefaultValue("")

        Create.UniqueConstraint("IX_OutputStream_Acronym").OnTable("OutputStream").Columns("Acronym");

        Create.Table("OutputStreamDevice")
            .WithColumn("AdapterID").AsInt32().NotNullable()
            .WithColumn("ID").AsInt32().PrimaryKey().Identity().NotNullable()
            .WithColumn("IDCode").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("Acronym").AsString(200).NotNullable()
            .WithColumn("BpaAcronym").AsString(4).Nullable()
            .WithColumn("Name").AsString(200).NotNullable()
            .WithColumn("PhasorDataFormat").AsString(15).Nullable()
            .WithColumn("FrequencyDataFormat").AsString(15).Nullable()
            .WithColumn("AnalogDataFormat").AsString(15).Nullable()
            .WithColumn("CoordinateFormat").AsString(15).Nullable()
            .WithColumn("LoadOrder").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("Enabled").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("CreatedOn").AsDateTime().NotNullable().WithDefaultValue("")
            .WithColumn("CreatedBy").AsString(200).NotNullable().WithDefaultValue("")
            .WithColumn("UpdatedOn").AsDateTime().NotNullable().WithDefaultValue("")
            .WithColumn("UpdatedBy").AsString(200).NotNullable().WithDefaultValue("");

    CONSTRAINT FK_OutputStreamDevice_OutputStream FOREIGN KEY(AdapterID) REFERENCES OutputStream (ID) ON DELETE CASCADE


        Create.Table("OutputStreamMeasurement")
            .WithColumn("AdapterID").AsInt32().NotNullable()
            .WithColumn("ID").AsInt32().PrimaryKey().Identity().NotNullable()
            .WithColumn("HistorianID").AsInt32().Nullable()
            .WithColumn("PointID").AsInt32().NotNullable()
            .WithColumn("SignalReference").AsString(200).NotNullable()
            .WithColumn("CreatedOn").AsDateTime().NotNullable().WithDefaultValue("")
            .WithColumn("CreatedBy").AsString(200).NotNullable().WithDefaultValue("")
            .WithColumn("UpdatedOn").AsDateTime().NotNullable().WithDefaultValue("")
            .WithColumn("UpdatedBy").AsString(200).NotNullable().WithDefaultValue("");


    CONSTRAINT FK_OutputStreamMeasurement_Historian FOREIGN KEY(HistorianID) REFERENCES Historian (ID),
    CONSTRAINT FK_OutputStreamMeasurement_Measurement FOREIGN KEY(PointID) REFERENCES Measurement (PointID) ON DELETE CASCADE,
    CONSTRAINT FK_OutputStreamMeasurement_OutputStream FOREIGN KEY(AdapterID) REFERENCES OutputStream (ID) ON DELETE CASCADE


        Create.Table("OutputStreamDeviceAnalog")
            .WithColumn("OutputStreamDeviceID").AsInt32().NotNullable()
            .WithColumn("ID").AsInt32().PrimaryKey().Identity().NotNullable()
            .WithColumn("Label").AsString(200).NotNullable()
            .WithColumn("Type").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("ScalingValue").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("LoadOrder").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("CreatedOn").AsDateTime().NotNullable().WithDefaultValue("GETDATE()")
            .WithColumn("CreatedBy").AsString(200).NotNullable().WithDefaultValue("")
            .WithColumn("UpdatedOn").AsDateTime().NotNullable().WithDefaultValue("GETDATE()")
            .WithColumn("UpdatedBy").AsString(200).NotNullable().WithDefaultValue("");

    CONSTRAINT FK_OutputStreamDeviceAnalog_OutputStreamDevice FOREIGN KEY(OutputStreamDeviceID) REFERENCES OutputStreamDevice (ID) ON DELETE CASCADE

        Create.Table("OutputStreamDeviceDigital")
            .WithColumn("OutputStreamDeviceID").AsInt32().NotNullable()
            .WithColumn("ID").AsInt32().PrimaryKey().Identity().NotNullable()
            .WithColumn("Label").AsString().NotNullable()
            .WithColumn("MaskValue").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("LoadOrder").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("CreatedOn").AsDateTime().NotNullable().WithDefaultValue("GETDATE()")
            .WithColumn("CreatedBy").AsString(200).NotNullable().WithDefaultValue("")
            .WithColumn("UpdatedOn").AsDateTime().NotNullable().WithDefaultValue("GETDATE()")
            .WithColumn("UpdatedBy").AsString(200).NotNullable().WithDefaultValue("");

    CONSTRAINT FK_OutputStreamDeviceDigital_OutputStreamDevice FOREIGN KEY(OutputStreamDeviceID) REFERENCES OutputStreamDevice (ID) ON DELETE CASCADE



        Create.Table("OutputStreamDevicePhasor")
            .WithColumn("OutputStreamDeviceID").AsInt32().NotNullable()
            .WithColumn("ID").AsInt32().PrimaryKey().Identity().NotNullable()
            .WithColumn("Label").AsString(200).NotNullable()
            .WithColumn("Type").AsString(1).NotNullable().WithDefaultValue("V")
            .WithColumn("Phase").AsString(1).NotNullable().WithDefaultValue("+")
            .WithColumn("ScalingValue").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("LoadOrder").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("CreatedOn").AsDateTime().NotNullable().WithDefaultValue("GETDATE()")
            .WithColumn("CreatedBy").AsString(200).NotNullable().WithDefaultValue("")
            .WithColumn("UpdatedOn").AsDateTime().NotNullable().WithDefaultValue("GETDATE()")
            .WithColumn("UpdatedBy").AsString(200).NotNullable().WithDefaultValue("");

    CONSTRAINT FK_OutputStreamDevicePhasor_OutputStreamDevice FOREIGN KEY(OutputStreamDeviceID) REFERENCES OutputStreamDevice (ID) ON DELETE CASCADE

        Create.Table("Statistic")
            .WithColumn("ID").AsInt32().PrimaryKey().Identity().NotNullable()
            .WithColumn("Source").AsString(20).NotNullable()
            .WithColumn("SignalIndex").AsInt32().NotNullable()
            .WithColumn("Name").AsString(200).NotNullable()
            .WithColumn("Description").AsString().Nullable()
            .WithColumn("AssemblyName").AsString().NotNullable()
            .WithColumn("TypeName").AsString().NotNullable()
            .WithColumn("MethodName").AsString(200).NotNullable()
            .WithColumn("Arguments").AsString().Nullable()
            .WithColumn("Enabled").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("DataType").AsString(200).Nullable()
            .WithColumn("DisplayFormat").AsString().Nullable()
            .WithColumn("IsConnectedState").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("LoadOrder").AsInt32().NotNullable().WithDefaultValue(0);

    CONSTRAINT IX_Statistic_Source_SignalIndex UNIQUE (Source ASC, SignalIndex ASC)

        Create.Table("Subscriber")
            .WithColumn("ID").AsString(36).NotNullable().WithDefaultValue("")
            .WithColumn("Acronym").AsString(200).NotNullable()
            .WithColumn("Name").AsString(200).Nullable()
            .WithColumn("SharedSecret").AsString(200).Nullable()
            .WithColumn("AuthKey").AsString(int.MaxValue).Nullable()
            .WithColumn("ValidIPAddresses").AsString(int.MaxValue).Nullable()
            .WithColumn("RemoteCertificateFile").AsString(500).Nullable()
            .WithColumn("ValidPolicyErrors").AsString(200).Nullable()
            .WithColumn("ValidChainFlags").AsString(500).Nullable()
            .WithColumn("AccessControlFilter").AsString(int.MaxValue).Nullable()
            .WithColumn("Enabled").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("CreatedOn").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
            .WithColumn("CreatedBy").AsString(200).NotNullable().WithDefaultValue("")
            .WithColumn("UpdatedOn").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
            .WithColumn("UpdatedBy").AsString(200).NotNullable().WithDefaultValue("")
            .PrimaryKey("PK_Subscriber").OnColumn("ID")
            .Unique("IX_Subscriber_Acronym").OnColumn("Acronym");

        Create.Table("SubscriberMeasurement")
            .WithColumn("SubscriberID").AsString(36).NotNullable()
            .WithColumn("SignalID").AsString(36).NotNullable()
            .WithColumn("Allowed").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("CreatedOn").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
            .WithColumn("CreatedBy").AsString(200).NotNullable().WithDefaultValue("")
            .WithColumn("UpdatedOn").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
            .WithColumn("UpdatedBy").AsString(200).NotNullable().WithDefaultValue("")
            .PrimaryKey("PK_SubscriberMeasurement").OnColumns("SubscriberID", "SignalID")
            .ForeignKey("FK_SubscriberMeasurement_Measurement", "SignalID").ReferencedTable("Measurement").OnDelete(Rule.Cascade).OnUpdate(Rule.Cascade)
            .ForeignKey("FK_SubscriberMeasurement_Subscriber", "SubscriberID").ReferencedTable("Subscriber").OnDelete(Rule.Cascade).OnUpdate(Rule.Cascade);

        Create.Table("MeasurementGroup")
            .WithColumn("ID").AsInt32().PrimaryKey().Identity()
            .WithColumn("Name").AsString(200).NotNullable()
            .WithColumn("Description").AsString(int.MaxValue).Nullable()
            .WithColumn("FilterExpression").AsString(int.MaxValue).Nullable()
            .WithColumn("CreatedOn").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
            .WithColumn("CreatedBy").AsString(200).NotNullable().WithDefaultValue("")
            .WithColumn("UpdatedOn").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
            .WithColumn("UpdatedBy").AsString(200).NotNullable().WithDefaultValue("");

        Create.Table("SubscriberMeasurementGroup")
            .WithColumn("SubscriberID").AsString(36).NotNullable()
            .WithColumn("MeasurementGroupID").AsInt32().NotNullable()
            .WithColumn("Allowed").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("CreatedOn").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
            .WithColumn("CreatedBy").AsString(200).NotNullable().WithDefaultValue("")
            .WithColumn("UpdatedOn").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
            .WithColumn("UpdatedBy").AsString(200).NotNullable().WithDefaultValue("")
            .PrimaryKey("PK_SubscriberMeasurementGroup").OnColumns("SubscriberID", "MeasurementGroupID")
            .ForeignKey("FK_SubscriberMeasurementGroup_Subscriber", "SubscriberID").ReferencedTable("Subscriber").OnDelete(Rule.Cascade).OnUpdate(Rule.Cascade)
            .ForeignKey("FK_SubscriberMeasurementGroup_MeasurementGroup", "MeasurementGroupID").ReferencedTable("MeasurementGroup").OnDelete(Rule.Cascade).OnUpdate(Rule.Cascade);

        Create.Table("MeasurementGroupMeasurement")
            .WithColumn("MeasurementGroupID").AsInt32().NotNullable()
            .WithColumn("SignalID").AsString(36).NotNullable()
            .WithColumn("CreatedOn").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
            .WithColumn("CreatedBy").AsString(200).NotNullable().WithDefaultValue("")
            .WithColumn("UpdatedOn").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
            .WithColumn("UpdatedBy").AsString(200).NotNullable().WithDefaultValue("")
            .PrimaryKey("PK_MeasurementGroupMeasurement").OnColumns("MeasurementGroupID", "SignalID")
            .ForeignKey("FK_MeasurementGroupMeasurement_Measurement", "SignalID").ReferencedTable("Measurement").OnDelete(Rule.Cascade).OnUpdate(Rule.Cascade)
            .ForeignKey("FK_MeasurementGroupMeasurement_MeasurementGroup", "MeasurementGroupID").ReferencedTable("MeasurementGroup").OnDelete(Rule.Cascade).OnUpdate(Rule.Cascade);
    
        //Security related
        Create.Table("AccessLog")
            .WithColumn("ID").AsInt32().PrimaryKey().Identity()
            .WithColumn("UserName").AsString(200).NotNullable()
            .WithColumn("AccessGranted").AsBoolean().NotNullable()
            .WithColumn("CreatedOn").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime);

        Create.Table("UserAccount")
            .WithColumn("ID").AsString(36).NotNullable().WithDefaultValue("")
            .WithColumn("Name").AsString(200).NotNullable()
            .WithColumn("Password").AsString(200).Nullable()
            .WithColumn("FirstName").AsString(200).Nullable()
            .WithColumn("LastName").AsString(200).Nullable()
            .WithColumn("Phone").AsString(200).Nullable()
            .WithColumn("Email").AsString(200).Nullable()
            .WithColumn("LockedOut").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("UseADAuthentication").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("ChangePasswordOn").AsDateTime().Nullable()
            .WithColumn("CreatedOn").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
            .WithColumn("CreatedBy").AsString(200).NotNullable().WithDefaultValue("")
            .WithColumn("UpdatedOn").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
            .WithColumn("UpdatedBy").AsString(200).NotNullable().WithDefaultValue("")
            .PrimaryKey("PK_UserAccount").OnColumn("ID")
            .Unique("IX_UserAccount").OnColumn("Name");

        Create.Table("SecurityGroup")
            .WithColumn("ID").AsString(36).NotNullable().WithDefaultValue("")
            .WithColumn("Name").AsString(200).NotNullable()
            .WithColumn("Description").AsString(int.MaxValue).Nullable()
            .WithColumn("CreatedOn").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
            .WithColumn("CreatedBy").AsString(200).NotNullable().WithDefaultValue("")
            .WithColumn("UpdatedOn").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
            .WithColumn("UpdatedBy").AsString(200).NotNullable().WithDefaultValue("")
            .PrimaryKey("PK_SecurityGroup").OnColumn("ID")
            .Unique("IX_SecurityGroup").OnColumn("Name");

        Create.Table("ApplicationRole")
            .WithColumn("ID").AsString(36).NotNullable().WithDefaultValue("")
            .WithColumn("Name").AsString(200).NotNullable()
            .WithColumn("Description").AsString(int.MaxValue).Nullable()
            .WithColumn("CreatedOn").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
            .WithColumn("CreatedBy").AsString(200).NotNullable().WithDefaultValue("Admin")
            .WithColumn("UpdatedOn").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime)
            .WithColumn("UpdatedBy").AsString(200).NotNullable().WithDefaultValue("Admin")
            .PrimaryKey("PK_ApplicationRole").OnColumn("ID")
            .Unique("IX_ApplicationRole").OnColumn("Name");

        Create.Table("ApplicationRoleSecurityGroup")
            .WithColumn("ApplicationRoleID").AsString(36).NotNullable()
            .WithColumn("SecurityGroupID").AsString(36).NotNullable()
            .ForeignKey("FK_applicationrolesecuritygroup_applicationrole", "ApplicationRoleID").ReferencedTable("ApplicationRole").OnDelete(Rule.Cascade).OnUpdate(Rule.Cascade)
            .ForeignKey("FK_applicationrolesecuritygroup_securitygroup", "SecurityGroupID").ReferencedTable("SecurityGroup").OnDelete(Rule.Cascade).OnUpdate(Rule.Cascade);

        Create.Table("ApplicationRoleUserAccount")
            .WithColumn("ApplicationRoleID").AsString(36).NotNullable()
            .WithColumn("UserAccountID").AsString(36).NotNullable()
            .ForeignKey("FK_applicationroleuseraccount_useraccount", "UserAccountID").ReferencedTable("UserAccount").OnDelete(Rule.Cascade).OnUpdate(Rule.Cascade)
            .ForeignKey("FK_applicationroleuseraccount_applicationrole", "ApplicationRoleID").ReferencedTable("ApplicationRole").OnDelete(Rule.Cascade).OnUpdate(Rule.Cascade);

        Create.Table("SecurityGroupUserAccount")
            .WithColumn("SecurityGroupID").AsString(36).NotNullable()
            .WithColumn("UserAccountID").AsString(36).NotNullable()
            .ForeignKey("FK_securitygroupuseraccount_useraccount", "UserAccountID").ReferencedTable("UserAccount").OnDelete(Rule.Cascade).OnUpdate(Rule.Cascade)
            .ForeignKey("FK_securitygroupuseraccount_securitygroup", "SecurityGroupID").ReferencedTable("SecurityGroup").OnDelete(Rule.Cascade).OnUpdate(Rule.Cascade);




        // Measurement
       

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
