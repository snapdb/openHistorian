//******************************************************************************************************
//  IntiialDataset.cs - Gbtc
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
/// The initial dataset for the openHistorian database.
/// </summary>
[SchemaMigration(author: "C. Lackner", branchNumber: 0, year: 2025, month: 02, day: 05)]
[Tags(TagBehavior.RequireAny, "Dataset")]
public class InitialDataset : Migration
{
    /// <inheritdoc/>
    public override void Down()
    {
        Delete.FromTable("ConfigurationEntity").Row(new { RunTimeName = "FilterAdapters" });
        Delete.FromTable("ConfigurationEntity").Row(new { RunTimeName = "InputAdapters" });
        Delete.FromTable("ConfigurationEntity").Row(new { RunTimeName = "ActionAdapters" });
        Delete.FromTable("ConfigurationEntity").Row(new { RunTimeName = "OutputAdapters" });
        Delete.FromTable("ConfigurationEntity").Row(new { RunTimeName = "ActiveMeasurements" });
        Delete.FromTable("ConfigurationEntity").Row(new { RunTimeName = "InputStreamDevices" });
        Delete.FromTable("ConfigurationEntity").Row(new { RunTimeName = "OutputStreamDevices" });
        Delete.FromTable("ConfigurationEntity").Row(new { RunTimeName = "OutputStreamMeasurements" });
        Delete.FromTable("ConfigurationEntity").Row(new { RunTimeName = "OutputStreamDevicePhasors" });
        Delete.FromTable("ConfigurationEntity").Row(new { RunTimeName = "OutputStreamDeviceAnalogs" });
        Delete.FromTable("ConfigurationEntity").Row(new { RunTimeName = "OutputStreamDeviceDigitals" });
        Delete.FromTable("ConfigurationEntity").Row(new { RunTimeName = "Statistics" });
        Delete.FromTable("ConfigurationEntity").Row(new { RunTimeName = "Subscribers" });
        Delete.FromTable("ConfigurationEntity").Row(new { RunTimeName = "SubscriberMeasurements" });
        Delete.FromTable("ConfigurationEntity").Row(new { RunTimeName = "SubscriberMeasurementGroups" });
        Delete.FromTable("ConfigurationEntity").Row(new { RunTimeName = "MeasurementGroups" });
        Delete.FromTable("ConfigurationEntity").Row(new { RunTimeName = "MeasurementGroupMeasurements" });
        Delete.FromTable("ConfigurationEntity").Row(new { RunTimeName = "Alarms" });

        Delete.FromTable("DataOperation").Row(new { MethodName = "PerformTimeSeriesStartupOperations" });
        Delete.FromTable("DataOperation").Row(new { MethodName = "PhasorDataSourceValidation" });
        Delete.FromTable("DataOperation").Row(new { MethodName = "OptimizeLocalHistorianSettings" });

        Delete.FromTable("SignalType").Row(new { Acronym = "IPHM" });
        Delete.FromTable("SignalType").Row(new { Acronym = "IPHA" });
        Delete.FromTable("SignalType").Row(new { Acronym = "VPHM" });
        Delete.FromTable("SignalType").Row(new { Acronym = "VPHA" });
        Delete.FromTable("SignalType").Row(new { Acronym = "FREQ" });
        Delete.FromTable("SignalType").Row(new { Acronym = "DFDT" });
        Delete.FromTable("SignalType").Row(new { Acronym = "ALOG" });
        Delete.FromTable("SignalType").Row(new { Acronym = "FLAG" });
        Delete.FromTable("SignalType").Row(new { Acronym = "DIGI" });
        Delete.FromTable("SignalType").Row(new { Acronym = "CALC" });
        Delete.FromTable("SignalType").Row(new { Acronym = "STAT" });
        Delete.FromTable("SignalType").Row(new { Acronym = "ALRM" });
        Delete.FromTable("SignalType").Row(new { Acronym = "QUAL" });

        Delete.FromTable("Statistic").Row(new { SignalIndex = 1, Source="System" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 2, Source = "System" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 3, Source = "System" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 4, Source = "System" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 5, Source = "System" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 6, Source = "System" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 7, Source = "System" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 8, Source = "System" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 9, Source = "System" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 10, Source = "System" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 11, Source = "System" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 12, Source = "System" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 13, Source = "System" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 14, Source = "System" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 15, Source = "System" });

        Delete.FromTable("Statistic").Row(new { SignalIndex = 1, Source = "Device" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 2, Source = "Device" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 3, Source = "Device" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 4, Source = "Device" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 5, Source = "Device" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 6, Source = "Device" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 7, Source = "Device" });

        Delete.FromTable("Statistic").Row(new { SignalIndex = 1, Source = "InputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 2, Source = "InputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 3, Source = "InputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 4, Source = "InputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 5, Source = "InputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 6, Source = "InputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 7, Source = "InputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 8, Source = "InputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 9, Source = "InputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 10, Source = "InputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 11, Source = "InputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 12, Source = "InputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 13, Source = "InputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 14, Source = "InputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 15, Source = "InputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 16, Source = "InputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 17, Source = "InputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 18, Source = "InputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 19, Source = "InputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 20, Source = "InputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 21, Source = "InputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 22, Source = "InputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 23, Source = "InputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 24, Source = "InputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 25, Source = "InputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 26, Source = "InputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 27, Source = "InputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 28, Source = "InputStream" });

        Delete.FromTable("Statistic").Row(new { SignalIndex = 1, Source = "OutputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 2, Source = "OutputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 3, Source = "OutputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 4, Source = "OutputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 5, Source = "OutputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 6, Source = "OutputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 7, Source = "OutputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 8, Source = "OutputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 9, Source = "OutputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 10, Source = "OutputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 11, Source = "OutputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 12, Source = "OutputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 13, Source = "OutputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 14, Source = "OutputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 15, Source = "OutputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 16, Source = "OutputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 17, Source = "OutputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 18, Source = "OutputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 19, Source = "OutputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 20, Source = "OutputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 21, Source = "OutputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 22, Source = "OutputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 23, Source = "OutputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 24, Source = "OutputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 25, Source = "OutputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 26, Source = "OutputStream" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 27, Source = "OutputStream" });

        Delete.FromTable("Statistic").Row(new { SignalIndex = 1, Source = "Subscriber" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 2, Source = "Subscriber" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 3, Source = "Subscriber" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 4, Source = "Subscriber" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 5, Source = "Subscriber" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 6, Source = "Subscriber" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 7, Source = "Subscriber" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 8, Source = "Subscriber" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 9, Source = "Subscriber" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 10, Source = "Subscriber" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 11, Source = "Subscriber" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 12, Source = "Subscriber" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 13, Source = "Subscriber" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 14, Source = "Subscriber" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 15, Source = "Subscriber" });

        Delete.FromTable("Statistic").Row(new { SignalIndex = 1, Source = "Publisher" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 2, Source = "Publisher" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 3, Source = "Publisher" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 4, Source = "Publisher" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 5, Source = "Publisher" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 6, Source = "Publisher" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 7, Source = "Publisher" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 8, Source = "Publisher" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 9, Source = "Publisher" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 10, Source = "Publisher" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 11, Source = "Publisher" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 12, Source = "Publisher" });
        Delete.FromTable("Statistic").Row(new { SignalIndex = 13, Source = "Publisher" });

        Delete.FromTable("Theme").Row(new { Name = "GPA Default" });

        Delete.FromTable("VendorDevice")
            .Row(new { VendorID = GetVendor("ARB"), Name = "Arbiter-1133A" })
            .Row(new { VendorID = GetVendor("ABB"), Name = "ABB-521" })
            .Row(new { VendorID = GetVendor("MTA"), Name = "Mehtatech 200" })
            .Row(new { VendorID = GetVendor("MAC"), Name = "Macrodyne 1690" })
            .Row(new { VendorID = GetVendor("SEL"), Name = "SEL-421" })
            .Row(new { VendorID = GetVendor("BPA"), Name = "BPA PDC" })
            .Row(new { VendorID = GetVendor("HWY"), Name = "Hathaway IDM" })
            .Row(new { VendorID = GetVendor("ATK"), Name = "Ametek" })
            .Row(new { VendorID = GetVendor("NPT"), Name = "NxtPhase" })
            .Row(new { VendorID = GetVendor("OTR"), Name = "Other" })
            .Row(new { VendorID = GetVendor("SEL"), Name = "SEL-5077" })
            .Row(new { VendorID = GetVendor("SEL"), Name = "SEL-451" })
            .Row(new { VendorID = GetVendor("SEL"), Name = "SEL-3306" })
            .Row(new { VendorID = GetVendor("GE"), Name = "GE N60" })
            .Row(new { VendorID = GetVendor("GPA"), Name = "openPDC" })
            .Row(new { VendorID = GetVendor("GPA"), Name = "openHistorian" })
            .Row(new { VendorID = GetVendor("GPA"), Name = "SIEGATE" })
            .Row(new { VendorID = GetVendor("ART"), Name = "AM-4" })
            .Row(new { VendorID = GetVendor("ART"), Name = "AM-5" })
            .Row(new { VendorID = GetVendor("UTK"), Name = "FNET" })
            .Row(new { VendorID = GetVendor("SEL"), Name = "SEL-3373" })
            .Row(new { VendorID = GetVendor("SIE"), Name = "SIMEAS R-PMU" })
            .Row(new { VendorID = GetVendor("EPG"), Name = "ePDC" });

        Delete.FromTable("Vendor")
            .Row(new { Acronym = "ABB"})
            .Row(new { Acronym = "ARB" })
            .Row(new { Acronym = "ART"})
            .Row(new { Acronym = "ATK"})
            .Row(new { Acronym = "BPA"})
            .Row(new { Acronym = "GE"})
            .Row(new { Acronym = "HWY" })
            .Row(new { Acronym = "MAC" })
            .Row(new { Acronym = "MTA"})
            .Row(new { Acronym = "NPT"})
            .Row(new { Acronym = "OTR"})
            .Row(new { Acronym = "SEL"})
            .Row(new { Acronym = "GPA"})
            .Row(new { Acronym = "UTK"})
            .Row(new { Acronym = "SIE"})
            .Row(new { Acronym = "EPG"});
    }

    /// <inheritdoc/>
    public override void Up()
    {
        Insert.IntoTable("ConfigurationEntity")
            .Row(new { SourceName = "IaonFilterAdapter", RuntimeName = "FilterAdapters", Description = "Defines IFilterAdapter definitions for a PDC node", LoadOrder = 1, Enabled = 1 })
            .Row(new { SourceName = "IaonInputAdapter", RuntimeName = "InputAdapters", Description = "Defines IInputAdapter definitions for a PDC node", LoadOrder = 2, Enabled = 1 })
            .Row(new { SourceName = "IaonActionAdapter", RuntimeName = "ActionAdapters", Description = "Defines IActionAdapter definitions for a PDC node", LoadOrder = 3, Enabled = 1 })
            .Row(new { SourceName = "IaonOutputAdapter", RuntimeName = "OutputAdapters", Description = "Defines IOutputAdapter definitions for a PDC node", LoadOrder = 4, Enabled = 1 })
            .Row(new { SourceName = "ActiveMeasurement", RuntimeName = "ActiveMeasurements", Description = "Defines active system measurements for a PDC node", LoadOrder = 5, Enabled = 1 })
            .Row(new { SourceName = "RuntimeInputStreamDevice", RuntimeName = "InputStreamDevices", Description = "Defines input stream devices associated with a concentrator", LoadOrder = 6, Enabled = 1 })
            .Row(new { SourceName = "RuntimeOutputStreamDevice", RuntimeName = "OutputStreamDevices", Description = "Defines output stream devices defined for a concentrator", LoadOrder = 7, Enabled = 1 })
            .Row(new { SourceName = "RuntimeOutputStreamMeasurement", RuntimeName = "OutputStreamMeasurements", Description = "Defines output stream measurements for an output stream", LoadOrder = 8, Enabled = 1 })
            .Row(new { SourceName = "OutputStreamDevicePhasor", RuntimeName = "OutputStreamDevicePhasors", Description = "Defines phasors for output stream devices", LoadOrder = 9, Enabled = 1 })
            .Row(new { SourceName = "OutputStreamDeviceAnalog", RuntimeName = "OutputStreamDeviceAnalogs", Description = "Defines analog values for output stream devices", LoadOrder = 10, Enabled = 1 })
            .Row(new { SourceName = "OutputStreamDeviceDigital", RuntimeName = "OutputStreamDeviceDigitals", Description = "Defines digital values for output stream devices", LoadOrder = 11, Enabled = 1 })
            .Row(new { SourceName = "RuntimeStatistic", RuntimeName = "Statistics", Description = "Defines statistics that are monitored for devices and output streams", LoadOrder = 12, Enabled = 1 })
            .Row(new { SourceName = "Subscriber", RuntimeName = "Subscribers", Description = "Defines subscribers that can request streaming points from a Gateway node", LoadOrder = 13, Enabled = 1 })
            .Row(new { SourceName = "SubscriberMeasurement", RuntimeName = "SubscriberMeasurements", Description = "Defines measurements associated with a Gateway subscriber", LoadOrder = 14, Enabled = 1 })
            .Row(new { SourceName = "SubscriberMeasurementGroup", RuntimeName = "SubscriberMeasurementGroups", Description = "Defines measurement groups associated with a Gateway subscriber", LoadOrder = 15, Enabled = 1 })
            .Row(new { SourceName = "MeasurementGroup", RuntimeName = "MeasurementGroups", Description = "Defines a group of measurements", LoadOrder = 16, Enabled = 1 })
            .Row(new { SourceName = "MeasurementGroupMeasurement", RuntimeName = "MeasurementGroupMeasurements", Description = "Defines the measurements in a measurement group", LoadOrder = 17, Enabled = 1 })
            .Row(new { SourceName = "Alarm", RuntimeName = "Alarms", Description = "Defines alarms that monitor the values of measurements", LoadOrder = 18, Enabled = 1 });

        Insert.IntoTable("DataOperation")
            .Row(new { Description = "Time Series Startup Operations", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.TimeseriesStartupOperations", MethodName = "PerformTimeseriesStartupOperations", Arguments = "", LoadOrder = 0, Enabled = 1 })
            .Row(new { Description = "Phasor Data Source Validation", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "PhasorDataSourceValidation", Arguments = "", LoadOrder = 1, Enabled = 1 })
            .Row(new { Description = "Optimize Local Historian Settings", AssemblyName = "openHistorian.Adapters.dll", TypeName = "openHistorian.Adapters.LocalOutputAdapter", MethodName = "OptimizeLocalHistorianSettings", Arguments = "", LoadOrder = 2, Enabled = 1 });

        Insert.IntoTable("Theme")
            .Row( new { Name = "GPA Default", FileName = "/Styles/bootstrap.min.css", LoadOrder = 1});

        Insert.IntoTable("SignalType")
            .Row(new { Name = "Current Magnitude", Acronym = "IPHM", Suffix = "PM", Abbreviation = "I", LongAcronym = "CurrentMagnitude", Source = "Phasor", EngineeringUnits = "Amps" })
            .Row(new { Name = "Current Phase Angle", Acronym = "IPHA", Suffix = "PA", Abbreviation = "IH", LongAcronym = "CurrentAngle", Source = "Phasor", EngineeringUnits = "Degrees" })
            .Row(new { Name = "Voltage Magnitude", Acronym = "VPHM", Suffix = "PM", Abbreviation = "V", LongAcronym = "VoltageMagnitude", Source = "Phasor", EngineeringUnits = "Volts" })
            .Row(new { Name = "Voltage Phase Angle", Acronym = "VPHA", Suffix = "PA", Abbreviation = "VH", LongAcronym = "VoltageAngle", Source = "Phasor", EngineeringUnits = "Degrees" })
            .Row(new { Name = "Frequency", Acronym = "FREQ", Suffix = "FQ", Abbreviation = "F", LongAcronym = "Frequency", Source = "PMU", EngineeringUnits = "Hz" })
            .Row(new { Name = "Frequency Delta (dF/dt)", Acronym = "DFDT", Suffix = "DF", Abbreviation = "DF", LongAcronym = "DfDt", Source = "PMU", EngineeringUnits = "" })
            .Row(new { Name = "Analog Value", Acronym = "ALOG", Suffix = "AV", Abbreviation = "AV", LongAcronym = "Analog", Source = "PMU", EngineeringUnits = "" })
            .Row(new { Name = "Status Flags", Acronym = "FLAG", Suffix = "SF", Abbreviation = "S", LongAcronym = "StatusFlags", Source = "PMU", EngineeringUnits = "" })
            .Row(new { Name = "Digital Value", Acronym = "DIGI", Suffix = "DV", Abbreviation = "DV", LongAcronym = "Digital", Source = "PMU", EngineeringUnits = "" })
            .Row(new { Name = "Calculated Value", Acronym = "CALC", Suffix = "CV", Abbreviation = "CV", LongAcronym = "Calculated", Source = "PMU", EngineeringUnits = "" })
            .Row(new { Name = "Statistic", Acronym = "STAT", Suffix = "ST", Abbreviation = "ST", LongAcronym = "Statistic", Source = "Any", EngineeringUnits = "" })
            .Row(new { Name = "Alarm", Acronym = "ALRM", Suffix = "AL", Abbreviation = "AL", LongAcronym = "Alarm", Source = "Any", EngineeringUnits = "" })
            .Row(new { Name = "Quality Flags", Acronym = "QUAL", Suffix = "QF", Abbreviation = "QF", LongAcronym = "QualityFlags", Source = "Frame", EngineeringUnits = "" });

        Insert.IntoTable("Vendor")
            .Row(new { Acronym = "ABB", Name = "ABB", PhoneNumber = "", ContactEmail = "", URL = "http://www.abb.com/" })
            .Row(new { Acronym = "ARB", Name = "Arbiter", PhoneNumber = "", ContactEmail = "", URL = "http://www.arbiter.com/" })
            .Row(new { Acronym = "ART", Name = "Artemes", PhoneNumber = "", ContactEmail = "", URL = "http://www.artemes.org/" })
            .Row(new { Acronym = "ATK", Name = "Ametek", PhoneNumber = "", ContactEmail = "", URL = "http://www.ametek.com/" })
            .Row(new { Acronym = "BPA", Name = "Bonneville Power Administration", PhoneNumber = "", ContactEmail = "", URL = "http://www.bpa.gov/" })
            .Row(new { Acronym = "GE", Name = "General Electric", PhoneNumber = "", ContactEmail = "", URL = "http://www.ge.com/" })
            .Row(new { Acronym = "HWY", Name = "Hathaway", PhoneNumber = "", ContactEmail = "", URL = "http://www.qualitrolcorp.com/" })
            .Row(new { Acronym = "MAC", Name = "Macrodyne", PhoneNumber = "", ContactEmail = "", URL = "http://www.macrodyneusa.com/" })
            .Row(new { Acronym = "MTA", Name = "Mehtatech", PhoneNumber = "", ContactEmail = "", URL = "http://www.mehtatech.com/" })
            .Row(new { Acronym = "NPT", Name = "NxtPhase", PhoneNumber = "", ContactEmail = "", URL = "http://www.nxtphase.com/" })
            .Row(new { Acronym = "OTR", Name = "Other / Unspecified", PhoneNumber = "", ContactEmail = "", URL = "" })
            .Row(new { Acronym = "SEL", Name = "Schweitzer", PhoneNumber = "", ContactEmail = "", URL = "http://www.selinc.com/" })
            .Row(new { Acronym = "GPA", Name = "Grid Protection Alliance", PhoneNumber = "", ContactEmail = "", URL = "http://www.gridprotectionalliance.org/" })
            .Row(new { Acronym = "UTK", Name = "University of Tennessee, Knoxville", PhoneNumber = "", ContactEmail = "", URL = "http://www.utk.edu/" })
            .Row(new { Acronym = "SIE", Name = "Siemens", PhoneNumber = "", ContactEmail = "", URL = "http://www.siemens.com/" })
            .Row(new { Acronym = "EPG", Name = "Electric Power Group", PhoneNumber = "", ContactEmail = "", URL = "http://www.electricpowergroup.com/" });

        Insert.IntoTable("VendorDevice")
            .Row(new { VendorID = GetVendor("ARB"), Name = "Arbiter-1133A", Description = "Arbiter 1133A Power Sentinel", URL = "http://www.arbiter.com/catalog/power/1133a/1133a.php" })
            .Row(new { VendorID = GetVendor("ABB"), Name = "ABB-521", Description = "ABB RES521", URL = "http://library.abb.com/GLOBAL/SCOT/SCOT296.nsf/VerityDisplay/79B16E5CF206C79CC125712D0074AC6F/$File/1MRK511113-HEN_D_en_Phasor_Measurement_Terminal_RES_521.pdf" })
            .Row(new { VendorID = GetVendor("MTA"), Name = "Mehtatech 200", Description = "Metha Tech Transcan 2000 IED", URL = "http://www.mehtatech.com/pdf/IEDbrochMay02b.pdf" })
            .Row(new { VendorID = GetVendor("MAC"), Name = "Macrodyne 1690", Description = "Macrodyne 1690", URL = "http://www.macrodyneusa.com/model_1690.htm" })
            .Row(new { VendorID = GetVendor("SEL"), Name = "SEL-421", Description = "SEL-421 Relay", URL = "http://www.selinc.com/sel-421.htm" })
            .Row(new { VendorID = GetVendor("BPA"), Name = "BPA PDC", Description = "Bonneville Power Administration", URL = "http://www.bpa.gov/" })
            .Row(new { VendorID = GetVendor("HWY"), Name = "Hathaway IDM", Description = "Qualitrol Hathaway IDM Fault Recorder", URL = "http://www.qualitrolcorp.com/docs/home/IDM_Brochure.pdf" })
            .Row(new { VendorID = GetVendor("ATK"), Name = "Ametek", Description = "Ametek TR-2000 Multi-Function Recorder", URL = "http://www.ametekpower.com/products/sku.cfm?SKU_Id=12328" })
            .Row(new { VendorID = GetVendor("NPT"), Name = "NxtPhase", Description = "NxtPhase Telsa 2000 Fault Recorder", URL = "http://www.nxtphase.com/sub-products-relays-tesla-model-2000P.htm" })
            .Row(new { VendorID = GetVendor("OTR"), Name = "Other", Description = "Other Device", URL = "" })
            .Row(new { VendorID = GetVendor("SEL"), Name = "SEL-5077", Description = "SEL-5077 SynchroWAVe Server Software", URL = "http://www.selinc.com/synchrowave.htm" })
            .Row(new { VendorID = GetVendor("SEL"), Name = "SEL-451", Description = "SEL-451 Relay", URL = "http://www.selinc.com/sel-451.htm" })
            .Row(new { VendorID = GetVendor("SEL"), Name = "SEL-3306", Description = "SEL-3306 Synchrophasor Processor", URL = "http://synchrophasor.selinc.com/" })
            .Row(new { VendorID = GetVendor("GE"), Name = "GE N60", Description = "GE N60 Synchrophasor Measurement System", URL = "http://www.geindustrial.com/cwc/products?pnlid=6&famid=31&catid=234&id=n60&lang=en_US" })
            .Row(new { VendorID = GetVendor("GPA"), Name = "openPDC", Description = "Open Source Phasor Data Concentrator", URL = "http://www.openpdc.com/" })
            .Row(new { VendorID = GetVendor("GPA"), Name = "openHistorian", Description = "Open Source Phasor Historian", URL = "http://www.openHistorian.com/" })
            .Row(new { VendorID = GetVendor("GPA"), Name = "SIEGATE", Description = "Open Source Substation Phasor Data Concentrator", URL = "http://www.SIEGATE.com/" })
            .Row(new { VendorID = GetVendor("ART"), Name = "AM-4", Description = "AM4 Series PMU and PQ monitor", URL = "https://www.artemes.org/en_GB/shop/am-4-am4-series-33205?category=1#attr=2249" })
            .Row(new { VendorID = GetVendor("ART"), Name = "AM-5", Description = "AM5 Series PMU and PQ monitor", URL = "https://www.artemes.org/en_GB/shop/am-5-serie-am5-33204?category=1#attr=2253,70,73,75" })
            .Row(new { VendorID = GetVendor("UTK"), Name = "FNET", Description = "UTK FNET Device", URL = "" })
            .Row(new { VendorID = GetVendor("SEL"), Name = "SEL-3373", Description = "SEL-3373 Synchrophasor Data Concentrator", URL = "http://www.selinc.com/SEL-3373/" })
            .Row(new { VendorID = GetVendor("SIE"), Name = "SIMEAS R-PMU", Description = "7KE6100 Digital Fault Recorder & PMU", URL = "http://www.energy.siemens.com/mx/en/automation/power-transmission-distribution/power-quality/simeas-r-pmu.htm" })
            .Row(new { VendorID = GetVendor("EPG"), Name = "ePDC", Description = "ePDC & eSPDC", URL = "http://www.electricpowergroup.com/solutions/epdc/index.html" });
    }

    private RawSql GetVendor(string acronym)
    {
        return RawSql.Insert("(SELECT ID FROM Vendor WHERE Acronym = '" + acronym + "')");

    }

}