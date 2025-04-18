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
        Delete.Table("DataOperation");
        Delete.Table("FailoverLog");
        Delete.Table("Theme");

        //Adapter related
        Delete.Table("CustomActionAdapter");
        Delete.Table("CustomInputAdapter");
        Delete.Table("CustomFilterAdapter");
        Delete.Table("CustomOutputAdapter");

        /// Related to Grafana Device Status
        Delete.Table("DeviceState");
        Delete.Table("DeviceStatus");
        Delete.Table("DataAvailability");

        // Event Related
        Delete.Table("Alarm");
        Delete.Table("EventDetails");

        // Views
        Execute.DeleteView("RuntimeOutputStreamMeasurement");
        Execute.DeleteView("RuntimeHistorian");
        Execute.DeleteView("RuntimeDevice");
        Execute.DeleteView("RuntimeCustomOutputAdapter");
        Execute.DeleteView("RuntimeInputStreamDevice");
        Execute.DeleteView("RuntimeCustomInputAdapter");
        Execute.DeleteView("RuntimeCustomFilterAdapter");
        Execute.DeleteView("RuntimeOutputStreamDevice");
        Execute.DeleteView("RuntimeOutputStream");
        Execute.DeleteView("RuntimeCustomActionAdapter");
        Execute.DeleteView("ActiveMeasurement");
        Execute.DeleteView("RuntimeStatistic");
        Execute.DeleteView("IaonOutputAdapter");
        Execute.DeleteView("IaonInputAdapter");
        Execute.DeleteView("IaonActionAdapter");
        Execute.DeleteView("IaonFilterAdapter");
        Execute.DeleteView("CurrentAlarmState");
        Execute.DeleteView("IaonTreeView");
        Execute.DeleteView("DeviceStatusView");
        Execute.DeleteView("NEW_GUID");
        Execute.DeleteView("FailoverNodeView");

    }

    /// <inheritdoc/>
    public override void Up()
    {
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
          .WithCreatedBy();

        //MetaData Information
        Create.Table("Company")
            .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
            .WithColumn("Acronym").AsString(200).NotNullable()
            .WithColumn("MapAcronym").AsString(10).NotNullable()
            .WithColumn("Name").AsString(200).NotNullable()
            .WithColumn("URL").AsString().Nullable()
            .WithColumn("LoadOrder").AsInt32().NotNullable().WithDefaultValue(0)
            .WithCreatedBy();

        Create.Table("Vendor")
            .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
            .WithColumn("Acronym").AsString(200).NotNullable()
            .WithColumn("PhoneNumber").AsString(200).Nullable()
            .WithColumn("ContactEmail").AsString(200).Nullable()
            .WithColumn("Name").AsString(200).NotNullable()
            .WithColumn("URL").AsString().Nullable()
            .WithColumn("LoadOrder").AsInt32().NotNullable().WithDefaultValue(0)
            .WithCreatedBy();

        Create.Table("VendorDevice")
            .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
            .WithColumn("VendorID").AsInt32().NotNullable().WithDefaultValue(10).ForeignKey("Vendor", "ID")
            .WithColumn("Name").AsString(200).NotNullable()
            .WithColumn("Description").AsString().Nullable()
            .WithColumn("URL").AsString().Nullable()
            .WithColumn("LoadOrder").AsInt32().NotNullable().WithDefaultValue(0)
            .WithCreatedBy();

        Create.Table("SignalType")
            .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
            .WithColumn("Name").AsString(200).NotNullable()
            .WithColumn("Acronym").AsString(4).NotNullable()
            .WithColumn("Suffix").AsString(2).NotNullable()
            .WithColumn("Abbreviation").AsString(2).NotNullable()
            .WithColumn("LongAcronym").AsString(200).NotNullable().WithDefaultValue("Undefined")
            .WithColumn("Source").AsString(10).NotNullable()
            .WithColumn("EngineeringUnits").AsString(10).Nullable()
            .WithColumn("LoadOrder").AsInt32().NotNullable().WithDefaultValue(0)
            .WithCreatedBy();

        Create.Table("Interconnection")
           .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
           .WithColumn("Acronym").AsString(200).NotNullable()
           .WithColumn("Name").AsString(200).NotNullable()
           .WithColumn("LoadOrder").AsInt32().NotNullable().WithDefaultValue(0)
           .WithCreatedBy();

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
            .WithColumn("Longitude").AsDecimal(9, 6).Nullable()
            .WithColumn("Latitude").AsDecimal(9, 6).Nullable()
            .WithColumn("InterconnectionID").AsInt32().Nullable().ForeignKey("Interconnection", "ID")
            .WithColumn("ConnectionString").AsString().Nullable()
            .WithColumn("Description").AsString().Nullable()
            .WithColumn("TimeZone").AsString(200).Nullable()
            .WithColumn("TimeAdjustmentTicks").AsInt16().NotNullable().WithDefaultValue(0)
            .WithColumn("ContactList").AsString().Nullable()
            .WithColumn("LoadOrder").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("Enabled").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("Internal").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("Local").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("Subscribed").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithCreatedBy();

        Create.UniqueConstraint("IX_Device_UniqueID").OnTable("Device").Column("UniqueID");
        Create.Index("IX_Device_Acronym").OnTable("Device").OnColumn("Acronym").Ascending();

         Create.Table("Measurement")
            .WithColumn("PointID").AsInt32().NotNullable().PrimaryKey().Identity()
            .WithColumn("SignalID").AsString(36).Nullable().Unique()
            .WithColumn("HistorianID").AsInt32().Nullable().ForeignKey("Historian", "ID")
            .WithColumn("DeviceID").AsInt32().Nullable().ForeignKey("Device", "ID")
            .WithColumn("PointTag").AsString(200).NotNullable()
            .WithColumn("AlternateTag").AsString().Nullable()
            .WithColumn("AlternateTag2").AsString().Nullable()
            .WithColumn("AlternateTag3").AsString().Nullable()
            .WithColumn("SignalReference").AsString(200).NotNullable()
            .WithColumn("SignalTypeID").AsInt32().NotNullable().ForeignKey("SignalType", "ID")
            .WithColumn("PhasorSourceIndex").AsInt32().Nullable()
            .WithColumn("Adder").AsDouble().NotNullable().WithDefaultValue(0.0)
            .WithColumn("Multiplier").AsDouble().NotNullable().WithDefaultValue(1.0)
            .WithColumn("Description").AsString().Nullable()
            .WithColumn("Subscribed").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("Internal").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("Enabled").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("Manual").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("Label").AsString().Nullable()
            .WithCreatedBy();

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
            .WithCreatedBy();

        Create.UniqueConstraint("IX_Phasor_DeviceID_SourceIndex").OnTable("Phasor").Columns("DeviceID","SourceIndex");

        Create.Table("OutputStream")
            .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
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
            .WithColumn("DataFormat").AsString(15).NotNullable().WithDefaultValue("FloatingPoint")
            .WithColumn("CoordinateFormat").AsString(15).NotNullable().WithDefaultValue("Polar")
            .WithColumn("CurrentScalingValue").AsInt32().NotNullable().WithDefaultValue(2423)
            .WithColumn("VoltageScalingValue").AsInt32().NotNullable().WithDefaultValue(2725785)
            .WithColumn("AnalogScalingValue").AsInt32().NotNullable().WithDefaultValue(1373291)
            .WithColumn("DigitalMaskValue").AsInt32().NotNullable().WithDefaultValue(-65536)
            .WithColumn("LoadOrder").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("Enabled").AsBoolean().NotNullable().WithDefaultValue(0)
            .WithCreatedBy();

        Create.UniqueConstraint("IX_OutputStream_Acronym").OnTable("OutputStream").Columns("Acronym");

        Create.Table("OutputStreamDevice")
            .WithColumn("AdapterID").AsInt32().NotNullable().ForeignKey("OutputStream","ID").OnDelete(System.Data.Rule.Cascade)
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
            .WithCreatedBy();

        Create.Table("OutputStreamMeasurement")
            .WithColumn("AdapterID").AsInt32().NotNullable().ForeignKey("OutputStream", "ID").OnDelete(System.Data.Rule.Cascade)
            .WithColumn("ID").AsInt32().PrimaryKey().Identity().NotNullable()
            .WithColumn("HistorianID").AsInt32().Nullable().ForeignKey("Historian", "ID")
            .WithColumn("PointID").AsInt32().NotNullable().ForeignKey("Measurement", "PointID").OnDelete(System.Data.Rule.Cascade)
            .WithColumn("SignalReference").AsString(200).NotNullable()
            .WithCreatedBy();

        Create.Table("OutputStreamDeviceAnalog")
            .WithColumn("OutputStreamDeviceID").AsInt32().NotNullable().ForeignKey("OutputStreamDevice", "ID").OnDelete(System.Data.Rule.Cascade)
            .WithColumn("ID").AsInt32().PrimaryKey().Identity().NotNullable()
            .WithColumn("Label").AsString(200).NotNullable()
            .WithColumn("Type").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("ScalingValue").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("LoadOrder").AsInt32().NotNullable().WithDefaultValue(0)
            .WithCreatedBy();

        Create.Table("OutputStreamDeviceDigital")
            .WithColumn("OutputStreamDeviceID").AsInt32().NotNullable().ForeignKey("OutputStreamDevice", "ID").OnDelete(System.Data.Rule.Cascade)
            .WithColumn("ID").AsInt32().PrimaryKey().Identity().NotNullable()
            .WithColumn("Label").AsString().NotNullable()
            .WithColumn("MaskValue").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("LoadOrder").AsInt32().NotNullable().WithDefaultValue(0)
            .WithCreatedBy();

        Create.Table("OutputStreamDevicePhasor")
            .WithColumn("OutputStreamDeviceID").AsInt32().NotNullable().ForeignKey("OutputStreamDevice", "ID").OnDelete(System.Data.Rule.Cascade)
            .WithColumn("ID").AsInt32().PrimaryKey().Identity().NotNullable()
            .WithColumn("Label").AsString(200).NotNullable()
            .WithColumn("Type").AsString(1).NotNullable().WithDefaultValue("V")
            .WithColumn("Phase").AsString(1).NotNullable().WithDefaultValue("+")
            .WithColumn("ScalingValue").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("LoadOrder").AsInt32().NotNullable().WithDefaultValue(0)
            .WithCreatedBy();

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

        Create.UniqueConstraint("IX_Statistic_Source_SignalIndex").OnTable("Statistic").Columns("Source", "SignalIndex");

        Create.Table("Subscriber")
            .WithColumn("ID").AsString(36).NotNullable().WithDefaultValue("").PrimaryKey()
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
            .WithCreatedBy();

        Create.UniqueConstraint("IX_Subscriber_Acronym").OnTable("Subscriber").Columns("Acronym");

        Create.Table("SubscriberMeasurement")
            .WithColumn("SubscriberID").AsString(36).NotNullable().PrimaryKey()
            .WithColumn("SignalID").AsString(36).NotNullable().ForeignKey("Measurement", "SignalID").OnDelete(System.Data.Rule.Cascade).OnUpdate(System.Data.Rule.Cascade).PrimaryKey()
            .WithColumn("Allowed").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithCreatedBy();

        Create.Table("MeasurementGroup")
            .WithColumn("ID").AsInt32().PrimaryKey().Identity()
            .WithColumn("Name").AsString(200).NotNullable()
            .WithColumn("Description").AsString(int.MaxValue).Nullable()
            .WithColumn("FilterExpression").AsString(int.MaxValue).Nullable()
            .WithCreatedBy();

        Create.Table("SubscriberMeasurementGroup")
            .WithColumn("SubscriberID").AsString(36).NotNullable().ForeignKey("Subscriber", "ID").OnDelete(System.Data.Rule.Cascade).OnUpdate(System.Data.Rule.Cascade).PrimaryKey()
            .WithColumn("MeasurementGroupID").AsInt32().NotNullable().ForeignKey("MeasurementGroup", "ID").OnDelete(System.Data.Rule.Cascade).OnUpdate(System.Data.Rule.Cascade).PrimaryKey()
            .WithColumn("Allowed").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithCreatedBy();

        Create.Table("MeasurementGroupMeasurement")
            .WithColumn("MeasurementGroupID").AsInt32().NotNullable().ForeignKey("MeasurementGroup", "ID").OnDelete(System.Data.Rule.Cascade).OnUpdate(System.Data.Rule.Cascade).PrimaryKey()
            .WithColumn("SignalID").AsString(36).NotNullable().ForeignKey("Measurement", "SignalID").OnDelete(System.Data.Rule.Cascade).OnUpdate(System.Data.Rule.Cascade).PrimaryKey()
            .WithCreatedBy();


        //Security related
        Create.Table("AccessLog")
            .WithColumn("ID").AsInt32().PrimaryKey().Identity()
            .WithColumn("UserName").AsString(200).NotNullable()
            .WithColumn("AccessGranted").AsBoolean().NotNullable()
            .WithColumn("CreatedOn").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime);

        Create.Table("UserAccount")
            .WithColumn("ID").AsString(36).NotNullable().WithDefaultValue("").PrimaryKey()
            .WithColumn("Name").AsString(200).NotNullable().Unique()
            .WithColumn("Password").AsString(200).Nullable()
            .WithColumn("FirstName").AsString(200).Nullable()
            .WithColumn("LastName").AsString(200).Nullable()
            .WithColumn("Phone").AsString(200).Nullable()
            .WithColumn("Email").AsString(200).Nullable()
            .WithColumn("LockedOut").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("UseADAuthentication").AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("ChangePasswordOn").AsDateTime().Nullable()
            .WithCreatedBy();

        Create.Table("SecurityGroup")
            .WithColumn("ID").AsString(36).NotNullable().WithDefaultValue("").PrimaryKey()
            .WithColumn("Name").AsString(200).NotNullable().Unique()
            .WithColumn("Description").AsString(int.MaxValue).Nullable()
            .WithCreatedBy();

        Create.Table("ApplicationRole")
            .WithColumn("ID").AsString(36).NotNullable().WithDefaultValue("").PrimaryKey()
            .WithColumn("Name").AsString(200).NotNullable().Unique()
            .WithColumn("Description").AsString(int.MaxValue).Nullable()
            .WithCreatedBy();

        Create.Table("ApplicationRoleSecurityGroup")
            .WithColumn("ApplicationRoleID").AsString(36).NotNullable().ForeignKey("ApplicationRole", "ID").OnDelete(System.Data.Rule.Cascade).OnUpdate(System.Data.Rule.Cascade)
            .WithColumn("SecurityGroupID").AsString(36).NotNullable().ForeignKey("SecurityGroup", "ID").OnDelete(System.Data.Rule.Cascade).OnUpdate(System.Data.Rule.Cascade);

        Create.Table("ApplicationRoleUserAccount")
            .WithColumn("ApplicationRoleID").AsString(36).NotNullable().ForeignKey("ApplicationRole", "ID").OnDelete(System.Data.Rule.Cascade).OnUpdate(System.Data.Rule.Cascade)
            .WithColumn("UserAccountID").AsString(36).NotNullable().ForeignKey("UserAccount", "ID").OnDelete(System.Data.Rule.Cascade).OnUpdate(System.Data.Rule.Cascade);
           
        Create.Table("SecurityGroupUserAccount")
            .WithColumn("SecurityGroupID").AsString(36).NotNullable().ForeignKey("SecurityGroup", "ID").OnDelete(System.Data.Rule.Cascade).OnUpdate(System.Data.Rule.Cascade)
            .WithColumn("UserAccountID").AsString(36).NotNullable().ForeignKey("UserAccount", "ID").OnDelete(System.Data.Rule.Cascade).OnUpdate(System.Data.Rule.Cascade);

        // System related
      

        Create.Table("ErrorLog")
            .WithColumn("ID").AsInt32().PrimaryKey().Identity()
            .WithColumn("Source").AsString(200).NotNullable()
            .WithColumn("Type").AsString(200).Nullable()
            .WithColumn("Message").AsString(int.MaxValue).NotNullable()
            .WithColumn("Detail").AsString(int.MaxValue).Nullable()
            .WithColumn("CreatedOn").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentUTCDateTime);

        Create.Table("Runtime")
            .WithColumn("ID").AsInt32().PrimaryKey().Identity()
            .WithColumn("SourceID").AsInt32().NotNullable()
            .WithColumn("SourceTable").AsString(200).NotNullable();

        Create.Table("ConfigurationEntity")
            .WithColumn("ID").AsInt32().PrimaryKey().Identity()
            .WithColumn("SourceName").AsString(200).NotNullable()
            .WithColumn("RuntimeName").AsString(200).NotNullable()
            .WithColumn("Description").AsString(int.MaxValue).Nullable()
            .WithColumn("LoadOrder").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("Enabled").AsBoolean().NotNullable().WithDefaultValue(false);

        Create.Table("DataOperation")
            .WithColumn("ID").AsInt32().PrimaryKey().Identity()
            .WithColumn("Description").AsString().Nullable()
            .WithColumn("AssemblyName").AsString().NotNullable()
            .WithColumn("TypeName").AsString().NotNullable()
            .WithColumn("MethodName").AsString(200).NotNullable()
            .WithColumn("Arguments").AsString().Nullable()
            .WithColumn("LoadOrder").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("Enabled").AsBoolean().NotNullable().WithDefaultValue(false);

        Create.Table("Theme")
            .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
            .WithColumn("FileName").AsString(200).NotNullable()
            .WithColumn("Name").AsString(200).NotNullable()
            .WithColumn("LoadOrder").AsInt32().NotNullable().WithDefaultValue(0)
            .WithCreatedBy();

        //Adapter related
        Create.Table("CustomActionAdapter")
             .WithColumn("ID").AsInt32().PrimaryKey().Identity()
             .WithColumn("AdapterName").AsString(200).NotNullable()
             .WithColumn("AssemblyName").AsString(int.MaxValue).NotNullable()
             .WithColumn("TypeName").AsString(int.MaxValue).NotNullable()
             .WithColumn("ConnectionString").AsString(int.MaxValue).Nullable()
             .WithColumn("LoadOrder").AsInt32().NotNullable().WithDefaultValue(0)
             .WithColumn("Enabled").AsBoolean().NotNullable().WithDefaultValue(false)
             .WithCreatedBy();

        Create.Table("CustomInputAdapter")
            .WithColumn("ID").AsInt32().PrimaryKey().Identity()
            .WithColumn("AdapterName").AsString(200).NotNullable()
            .WithColumn("AssemblyName").AsString(int.MaxValue).NotNullable()
            .WithColumn("TypeName").AsString(int.MaxValue).NotNullable()
            .WithColumn("ConnectionString").AsString(int.MaxValue).Nullable()
            .WithColumn("LoadOrder").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("Enabled").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithCreatedBy();

        Create.Table("CustomFilterAdapter")
            .WithColumn("ID").AsInt32().PrimaryKey().Identity()
            .WithColumn("AdapterName").AsString(200).NotNullable()
            .WithColumn("AssemblyName").AsString(int.MaxValue).NotNullable()
            .WithColumn("TypeName").AsString(int.MaxValue).NotNullable()
            .WithColumn("ConnectionString").AsString(int.MaxValue).Nullable()
            .WithColumn("LoadOrder").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("Enabled").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithCreatedBy();

        Create.Table("CustomOutputAdapter")
            .WithColumn("ID").AsInt32().PrimaryKey().Identity()
            .WithColumn("AdapterName").AsString(200).NotNullable()
            .WithColumn("AssemblyName").AsString(int.MaxValue).NotNullable()
            .WithColumn("TypeName").AsString(int.MaxValue).NotNullable()
            .WithColumn("ConnectionString").AsString(int.MaxValue).Nullable()
            .WithColumn("LoadOrder").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("Enabled").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithCreatedBy();

        // Related to Grafana Device Status
        Create.Table("DeviceState")
           .WithColumn("ID").AsInt32().PrimaryKey().Identity()
           .WithColumn("State").AsString(50).Nullable()
           .WithColumn("Color").AsString(50).Nullable()
           .WithColumn("RecommendedAction").AsString(500).Nullable()
           .WithColumn("Priority").AsInt32().WithDefaultValue(0)
           .WithColumn("Rules").AsString(int.MaxValue).Nullable();

        Create.Table("DeviceStatus")
            .WithColumn("ID").AsInt32().PrimaryKey().Identity()
            .WithColumn("DeviceID").AsInt32().Nullable().ForeignKey("Device", "ID").OnDelete(Rule.Cascade).OnUpdate(Rule.Cascade)
            .WithColumn("StateID").AsInt32().Nullable().ForeignKey("DeviceState", "ID").OnDelete(Rule.Cascade).OnUpdate(Rule.Cascade)
            .WithColumn("TimeStamp").AsDateTime().Nullable()
            .WithColumn("DisplayData").AsString(10).Nullable();

        Create.Table("DataAvailability")
            .WithColumn("ID").AsInt32().PrimaryKey().Identity()
            .WithColumn("GoodAvailableData").AsDouble().NotNullable()
            .WithColumn("BadAvailableData").AsDouble().NotNullable()
            .WithColumn("TotalAvailableData").AsDouble().NotNullable();

        // EventRelated
        Create.Table("Alarm")
            .WithColumn("ID").AsInt32().PrimaryKey().Identity()
            .WithColumn("TagName").AsString(200).NotNullable()
            .WithColumn("SignalID").AsString(36).NotNullable().ForeignKey("Measurement", "SignalID")
            .WithColumn("InputMeasurementKeys").AsString(int.MaxValue).NotNullable()
            .WithColumn("Description").AsString(int.MaxValue).Nullable()
            .WithColumn("Severity").AsInt32().NotNullable()
            .WithColumn("Operation").AsInt32().NotNullable()
            .WithColumn("Combination").AsInt32().NotNullable()
            .WithColumn("SetPoint").AsDouble().Nullable()
            .WithColumn("Tolerance").AsDouble().Nullable()
            .WithColumn("Delay").AsDouble().Nullable()
            .WithColumn("Hysteresis").AsDouble().Nullable()
            .WithColumn("Timeout").AsDouble().Nullable()
            .WithColumn("Enabled").AsBoolean().NotNullable().WithDefaultValue(false);

        Create.Table("EventDetails")
            .WithColumn("ID").AsInt32().PrimaryKey().Identity()
            .WithColumn("EventGuid").AsString(36).NotNullable()
            .WithColumn("StartTime").AsDateTime().NotNullable()
            .WithColumn("EndTime").AsDateTime().Nullable()
            .WithColumn("MeasurementID").AsString(36).NotNullable().ForeignKey("Measurement", "SignalID")
            .WithColumn("Details").AsString(int.MaxValue).NotNullable()
            .WithColumn("Type").AsString(20).NotNullable();

        Create.Table("FailoverLog")
            .WithColumn("ID").AsInt32().PrimaryKey().Identity()
            .WithColumn("SystemName").AsString(50).NotNullable()
            .WithColumn("Message").AsString(int.MaxValue).NotNullable()
            .WithColumn("Timestamp").AsDateTime().Nullable()
            .WithColumn("Priority").AsInt32().NotNullable();

        this.AddRunTimeSync("Historian");
        this.AddRunTimeSync("OutputStream");
        this.AddRunTimeSync("Device");
        this.AddRunTimeSync("CustomOutputAdapter");
        this.AddRunTimeSync("CustomInputAdapter");
        this.AddRunTimeSync("CustomFilterAdapter");
        this.AddRunTimeSync("CustomActionAdapter");

        // Views
        this.AddView("RuntimeOutputStreamMeasurement", @"
                Runtime.ID AS AdapterID,
                Historian.Acronym AS Historian,
                OutputStreamMeasurement.PointID,
                OutputStreamMeasurement.SignalReference
            FROM 
                OutputStreamMeasurement
                LEFT OUTER JOIN Historian 
                    ON OutputStreamMeasurement.HistorianID = Historian.ID
                LEFT OUTER JOIN Runtime 
                    ON OutputStreamMeasurement.AdapterID = Runtime.SourceID 
                    AND Runtime.SourceTable = 'OutputStream'
            ORDER BY 
                OutputStreamMeasurement.HistorianID, 
                OutputStreamMeasurement.PointID;
        ");

        this.AddView("RuntimeHistorian", @"
                Runtime.ID,
                Historian.Acronym AS AdapterName,
                COALESCE(NULLIF(TRIM(Historian.AssemblyName), ''), 'HistorianAdapters.dll') AS AssemblyName, 
                COALESCE(
                    NULLIF(TRIM(Historian.TypeName), ''), 
                    CASE WHEN IsLocal = 1 THEN 'HistorianAdapters.LocalOutputAdapter' ELSE 'HistorianAdapters.RemoteOutputAdapter' END
                ) AS TypeName, 
                COALESCE(Historian.ConnectionString || ';', '') ||
                COALESCE('instanceName=' || Historian.Acronym || ';', '') ||
                COALESCE('sourceids=' || Historian.Acronym || ';', '') ||
                COALESCE('measurementReportingInterval=' || Historian.MeasurementReportingInterval, '') AS ConnectionString
            FROM 
                Historian
                LEFT OUTER JOIN Runtime 
                    ON Historian.ID = Runtime.SourceID 
                    AND Runtime.SourceTable = 'Historian'
            WHERE 
                (Historian.Enabled <> 0)
            ORDER BY 
                Historian.LoadOrder;
        ");

        this.AddView("RuntimeDevice", @"
                Runtime.ID,
                Device.Acronym AS AdapterName,
                COALESCE(Device.ConnectionString || ';', '') ||
                COALESCE('isConcentrator=' || Device.IsConcentrator || ';', '') ||
                COALESCE('accessID=' || Device.AccessID || ';', '') ||
                COALESCE('timeZone=' || Device.TimeZone || ';', '') ||
                COALESCE('timeAdjustmentTicks=' || Device.TimeAdjustmentTicks || ';', '') ||
                COALESCE('measurementReportingInterval=' || Device.MeasurementReportingInterval || ';', '') ||
                COALESCE('connectOnDemand=' || Device.ConnectOnDemand, '') AS ConnectionString
            FROM 
                Device
                LEFT OUTER JOIN Runtime 
                    ON Device.ID = Runtime.SourceID 
                    AND Runtime.SourceTable = 'Device'
            WHERE 
                (Device.Enabled <> 0 AND Device.ParentID IS NULL)
            ORDER BY 
                Device.LoadOrder;
        ");

        this.AddView("RuntimeCustomOutputAdapter", @"
                Runtime.ID,
                CustomOutputAdapter.AdapterName,
                TRIM(CustomOutputAdapter.AssemblyName) AS AssemblyName,
                TRIM(CustomOutputAdapter.TypeName) AS TypeName,
                CustomOutputAdapter.ConnectionString
            FROM 
                CustomOutputAdapter
                LEFT OUTER JOIN Runtime 
                    ON CustomOutputAdapter.ID = Runtime.SourceID 
                    AND Runtime.SourceTable = 'CustomOutputAdapter'
            WHERE 
                (CustomOutputAdapter.Enabled <> 0)
            ORDER BY 
                CustomOutputAdapter.LoadOrder;
        ");

        this.AddView("RuntimeInputStreamDevice", @"
                Runtime_P.ID AS ParentID,
                Runtime.ID,
                Device.Acronym,
                Device.Name,
                Device.AccessID
            FROM 
                Device
                LEFT OUTER JOIN Runtime 
                    ON Device.ID = Runtime.SourceID 
                    AND Runtime.SourceTable = 'Device'
                LEFT OUTER JOIN Runtime AS Runtime_P 
                    ON Device.ParentID = Runtime_P.SourceID 
                    AND Runtime_P.SourceTable = 'Device'
            WHERE 
                (Device.IsConcentrator = 0) 
                AND (Device.Enabled <> 0) 
                AND (Device.ParentID IS NOT NULL)
            ORDER BY 
                Device.LoadOrder;
        ");

        this.AddView("RuntimeCustomInputAdapter", @"
                Runtime.ID,
                CustomInputAdapter.AdapterName,
                TRIM(CustomInputAdapter.AssemblyName) AS AssemblyName,
                TRIM(CustomInputAdapter.TypeName) AS TypeName,
                CustomInputAdapter.ConnectionString
            FROM 
                CustomInputAdapter
                LEFT OUTER JOIN Runtime 
                    ON CustomInputAdapter.ID = Runtime.SourceID 
                    AND Runtime.SourceTable = 'CustomInputAdapter'
            WHERE 
                (CustomInputAdapter.Enabled <> 0)
            ORDER BY 
                CustomInputAdapter.LoadOrder;
        ");

        this.AddView("RuntimeCustomFilterAdapter", @"
                Runtime.ID,
                CustomFilterAdapter.AdapterName,
                TRIM(CustomFilterAdapter.AssemblyName) AS AssemblyName,
                TRIM(CustomFilterAdapter.TypeName) AS TypeName,
                CustomFilterAdapter.ConnectionString
            FROM 
                CustomFilterAdapter
                LEFT OUTER JOIN Runtime 
                    ON CustomFilterAdapter.ID = Runtime.SourceID 
                    AND Runtime.SourceTable = 'CustomFilterAdapter'
            WHERE 
                (CustomFilterAdapter.Enabled <> 0)
            ORDER BY 
                CustomFilterAdapter.LoadOrder;
        ");

        this.AddView("RuntimeOutputStreamDevice", @"
                Runtime.ID AS ParentID,
                OutputStreamDevice.ID,
                OutputStreamDevice.IDCode,
                OutputStreamDevice.Acronym,
                OutputStreamDevice.BpaAcronym,
                OutputStreamDevice.Name,
                NULLIF(OutputStreamDevice.PhasorDataFormat, '') AS PhasorDataFormat,
                NULLIF(OutputStreamDevice.FrequencyDataFormat, '') AS FrequencyDataFormat,
                NULLIF(OutputStreamDevice.AnalogDataFormat, '') AS AnalogDataFormat,
                NULLIF(OutputStreamDevice.CoordinateFormat, '') AS CoordinateFormat,
                OutputStreamDevice.LoadOrder
            FROM 
                OutputStreamDevice
                LEFT OUTER JOIN Runtime 
                    ON OutputStreamDevice.AdapterID = Runtime.SourceID 
                    AND Runtime.SourceTable = 'OutputStream'
            WHERE 
                (OutputStreamDevice.Enabled <> 0)
            ORDER BY 
                OutputStreamDevice.LoadOrder;
        ");

        this.AddView("RuntimeOutputStream", @"
                Runtime.ID,
                OutputStream.Acronym AS AdapterName,
                'PhasorProtocolAdapters.dll' AS AssemblyName,
                CASE 
                    Type 
                    WHEN 1 THEN 'PhasorProtocolAdapters.BpaPdcStream.Concentrator'
                    WHEN 2 THEN 'PhasorProtocolAdapters.Iec61850_90_5.Concentrator'
                    ELSE 'PhasorProtocolAdapters.IeeeC37_118.Concentrator'
                END AS TypeName,
                COALESCE(OutputStream.ConnectionString || ';', '') ||
                COALESCE('dataChannel={' || OutputStream.DataChannel || '};', '') ||
                COALESCE('commandChannel={' || OutputStream.CommandChannel || '};', '') ||
                COALESCE('idCode=' || OutputStream.IDCode || ';', '') ||
                COALESCE('autoPublishConfigFrame=' || OutputStream.AutoPublishConfigFrame || ';', '') ||
                COALESCE('autoStartDataChannel=' || OutputStream.AutoStartDataChannel || ';', '') ||
                COALESCE('nominalFrequency=' || OutputStream.NominalFrequency || ';', '') ||
                COALESCE('lagTime=' || OutputStream.LagTime || ';', '') ||
                COALESCE('leadTime=' || OutputStream.LeadTime || ';', '') ||
                COALESCE('framesPerSecond=' || OutputStream.FramesPerSecond || ';', '') ||
                COALESCE('useLocalClockAsRealTime=' || OutputStream.UseLocalClockAsRealTime || ';', '') ||
                COALESCE('allowSortsByArrival=' || OutputStream.AllowSortsByArrival || ';', '') ||
                COALESCE('ignoreBadTimestamps=' || OutputStream.IgnoreBadTimeStamps || ';', '') ||
                COALESCE('timeResolution=' || OutputStream.TimeResolution || ';', '') ||
                COALESCE('allowPreemptivePublishing=' || OutputStream.AllowPreemptivePublishing || ';', '') ||
                COALESCE('downsamplingMethod=' || OutputStream.DownsamplingMethod || ';', '') ||
                COALESCE('dataFormat=' || OutputStream.DataFormat || ';', '') ||
                COALESCE('coordinateFormat=' || OutputStream.CoordinateFormat || ';', '') ||
                COALESCE('currentScalingValue=' || OutputStream.CurrentScalingValue || ';', '') ||
                COALESCE('voltageScalingValue=' || OutputStream.VoltageScalingValue || ';', '') ||
                COALESCE('analogScalingValue=' || OutputStream.AnalogScalingValue || ';', '') ||
                COALESCE('performTimestampReasonabilityCheck=' || OutputStream.PerformTimeReasonabilityCheck || ';', '') ||
                COALESCE('digitalMaskValue=' || OutputStream.DigitalMaskValue, '') AS ConnectionString
            FROM 
                OutputStream
                LEFT OUTER JOIN Runtime 
                    ON OutputStream.ID = Runtime.SourceID 
                    AND Runtime.SourceTable = 'OutputStream'
            WHERE 
                (OutputStream.Enabled <> 0)
            ORDER BY 
                OutputStream.LoadOrder;
        ");

        this.AddView("RuntimeCustomActionAdapter", @"
                Runtime.ID,
                CustomActionAdapter.AdapterName,
                TRIM(CustomActionAdapter.AssemblyName) AS AssemblyName,
                TRIM(CustomActionAdapter.TypeName) AS TypeName,
                CustomActionAdapter.ConnectionString
            FROM 
                CustomActionAdapter
                LEFT OUTER JOIN Runtime 
                    ON CustomActionAdapter.ID = Runtime.SourceID 
                    AND Runtime.SourceTable = 'CustomActionAdapter'
            WHERE 
                (CustomActionAdapter.Enabled <> 0)
            ORDER BY 
                CustomActionAdapter.LoadOrder;
        ");

        this.AddView("ActiveMeasurement", @"
                COALESCE(Device.Acronym, '__') || ':' || Measurement.PointID AS ID,
                Measurement.SignalID,
                Measurement.PointTag,
                Measurement.AlternateTag,
                Measurement.SignalReference,
                Measurement.Internal,
                Measurement.Subscribed,
                Device.Acronym AS Device,
                CASE 
                  WHEN Device.IsConcentrator = 0 AND Device.ParentID IS NOT NULL 
                       THEN RuntimeP.ID 
                       ELSE Runtime.ID 
                END AS DeviceID,
                COALESCE(Device.FramesPerSecond, 30) AS FramesPerSecond,
                Measurement.SignalType,
                Measurement.EngineeringUnits,
                Phasor.ID AS PhasorID,
                Phasor.Label AS PhasorLabel,
                Phasor.Type AS PhasorType,
                Phasor.Phase,
                Phasor.BaseKV,
                Measurement.Adder,
                Measurement.Multiplier,
                Device.CompanyAcronym AS Company, 
                Device.Longitude,
                Device.Latitude,
                Measurement.Description,
                Measurement.UpdatedOn
            FROM 
            (
                SELECT 
                    M.*,
                    ST.Acronym AS SignalType,
                    ST.EngineeringUnits
                FROM 
                    Measurement AS M
                    LEFT OUTER JOIN SignalType AS ST 
                        ON M.SignalTypeID = ST.ID
            ) AS Measurement
            LEFT OUTER JOIN
            (
                SELECT 
                    D.*,
                    C.Acronym AS CompanyAcronym
                FROM 
                    Device AS D
                    LEFT OUTER JOIN Company AS C 
                        ON D.CompanyID = C.ID
            ) AS Device
                ON Device.ID = Measurement.DeviceID
            LEFT OUTER JOIN Phasor 
                ON Measurement.DeviceID = Phasor.DeviceID 
                AND Measurement.PhasorSourceIndex = Phasor.SourceIndex
            LEFT OUTER JOIN Historian 
                ON Measurement.HistorianID = Historian.ID
            LEFT OUTER JOIN Runtime 
                ON Device.ID = Runtime.SourceID 
                AND Runtime.SourceTable = 'Device'
            LEFT OUTER JOIN Runtime AS RuntimeP 
                ON RuntimeP.SourceID = Device.ParentID 
                AND RuntimeP.SourceTable = 'Device'
            WHERE 
                (Device.Enabled <> 0 OR Device.Enabled IS NULL)
                AND (Measurement.Enabled <> 0);
        ");

        this.AddView("RuntimeStatistic", @"
                Statistic.ID,
                Statistic.Source,
                Statistic.SignalIndex,
                Statistic.Name,
                Statistic.Description,
                Statistic.AssemblyName,
                Statistic.TypeName,
                Statistic.MethodName,
                Statistic.Arguments,
                Statistic.IsConnectedState,
                Statistic.DataType,
                Statistic.DisplayFormat,
                Statistic.Enabled
            FROM 
                Statistic;
        ");

        this.AddView("IaonOutputAdapter", @"
                RH.ID,
                RH.AdapterName,
                RH.AssemblyName,
                RH.TypeName,
                RH.ConnectionString
            FROM RuntimeHistorian AS RH
            UNION
            SELECT
                RCOA.ID,
                RCOA.AdapterName,
                RCOA.AssemblyName,
                RCOA.TypeName,
                RCOA.ConnectionString
            FROM RuntimeCustomOutputAdapter AS RCOA;
        ");

        this.AddView("IaonInputAdapter", @"
                RD.ID,
                RD.AdapterName,
                RD.AssemblyName,
                RD.TypeName,
                RD.ConnectionString
            FROM RuntimeDevice AS RD
            UNION
            SELECT
                RCIA.ID,
                RCIA.AdapterName,
                RCIA.AssemblyName,
                RCIA.TypeName,
                RCIA.ConnectionString
            FROM RuntimeCustomInputAdapter AS RCIA;
        ");

        this.AddView("IaonActionAdapter", @"
                RCA.ID,
                RCA.AdapterName,
                RCA.AssemblyName,
                RCA.TypeName,
                RCA.ConnectionString
            FROM RuntimeCustomActionAdapter AS RCA;
        ");

        this.AddView("IaonFilterAdapter", @"
                RCFA.ID,
                RCFA.AdapterName,
                RCFA.AssemblyName,
                RCFA.TypeName,
                RCFA.ConnectionString
            FROM RuntimeCustomFilterAdapter AS RCFA;
        ");

        this.AddView("CurrentAlarmState", @"
                Alarm.ID,
                Alarm.Acronym,
                Alarm.Type,
                Alarm.State,
                Alarm.ActiveSince,
                Alarm.ActiveUntil
            FROM Alarm;
        ");

        this.AddView("IaonTreeView", @"
                Device.Acronym AS DeviceAcronym,
                Device.Name AS DeviceName,
                Device.ID AS DeviceID,
                Device.Type AS DeviceType,
                ParentDevice.Acronym AS ParentDeviceAcronym,
                ParentDevice.Name AS ParentDeviceName,
                ParentDevice.ID AS ParentDeviceID
            FROM Device
            LEFT OUTER JOIN Device AS ParentDevice 
                ON Device.ParentID = ParentDevice.ID;
        ");

        this.AddView("FailoverNodeView", @"
                FailoverLog.SystemName,
				FailoverLog.Priority,
				Max(FailoverLog.Timestamp) AS LastLog
            FROM 
                FailoverLog
            GROUP BY 
                FailoverLog.SystemName, FailoverLog.Priority;
        ");

        IfDatabase(ProcessorId.SQLite).Execute.Sql(@"
            CREATE VIEW NEW_GUID AS
                SELECT lower(
                    hex(randomblob(4)) || '-' ||
                    hex(randomblob(2)) || '-' ||
                    '4' || substr(hex(randomblob(2)), 2) || '-' ||
                    substr('AB89', 1 + (abs(random()) % 4), 1) || substr(hex(randomblob(2)), 2) || '-' ||
                    hex(randomblob(6))
                );
        ");


        
    }
}