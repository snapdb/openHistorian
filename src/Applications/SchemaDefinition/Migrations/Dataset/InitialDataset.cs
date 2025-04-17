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
            .Row(new { Description = "Time Series Startup Operations", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.TimeSeriesStartupOperations", MethodName = "PerformTimeSeriesStartupOperations", Arguments = "", LoadOrder = 0, Enabled = 1 })
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

        Insert.IntoTable("Statistic")
            .Row(new { Source = "System", SignalIndex = 1, Name = "CPU Usage", Description = "Percentage of CPU currently used by this process.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.PerformanceStatistics", MethodName = "GetSystemStatistic_CPUUsage", Arguments = "", Enabled = 1, DataType = "System.Double", DisplayFormat = "{0:N3}", IsConnectedState = 0, LoadOrder = 1 })
            .Row(new { Source = "System", SignalIndex = 2, Name = "Average CPU Usage", Description = "Average percentage of CPU used by this process.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.PerformanceStatistics", MethodName = "GetSystemStatistic_AverageCPUUsage", Arguments = "", Enabled = 1, DataType = "System.Double", DisplayFormat = "{0:N3}", IsConnectedState = 0, LoadOrder = 2 })
            .Row(new { Source = "System", SignalIndex = 3, Name = "Memory Usage", Description = "Amount of memory currently used by this process in megabytes.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.PerformanceStatistics", MethodName = "GetSystemStatistic_MemoryUsage", Arguments = "", Enabled = 1, DataType = "System.Double", DisplayFormat = "{0:N3}", IsConnectedState = 0, LoadOrder = 3 })
            .Row(new { Source = "System", SignalIndex = 4, Name = "Average Memory Usage", Description = "Average amount of memory used by this process in megabytes.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.PerformanceStatistics", MethodName = "GetSystemStatistic_AverageMemoryUsage", Arguments = "", Enabled = 1, DataType = "System.Double", DisplayFormat = "{0:N3}", IsConnectedState = 0, LoadOrder = 4 })
            .Row(new { Source = "System", SignalIndex = 5, Name = "Thread Count", Description = "Number of threads currently used by this process.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.PerformanceStatistics", MethodName = "GetSystemStatistic_ThreadCount", Arguments = "", Enabled = 1, DataType = "System.Double", DisplayFormat = "{0:N3}", IsConnectedState = 0, LoadOrder = 5 })
            .Row(new { Source = "System", SignalIndex = 6, Name = "Average Thread Count", Description = "Average number of threads used by this process.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.PerformanceStatistics", MethodName = "GetSystemStatistic_AverageThreadCount", Arguments = "", Enabled = 1, DataType = "System.Double", DisplayFormat = "{0:N3}", IsConnectedState = 0, LoadOrder = 6 })
            .Row(new { Source = "System", SignalIndex = 7, Name = "Threading Contention Rate", Description = "Current thread lock contention rate in attempts per second.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.PerformanceStatistics", MethodName = "GetSystemStatistic_ThreadingContentionRate", Arguments = "", Enabled = 1, DataType = "System.Double", DisplayFormat = "{0:N3}", IsConnectedState = 0, LoadOrder = 7 })
            .Row(new { Source = "System", SignalIndex = 8, Name = "Average Threading Contention Rate", Description = "Average thread lock contention rate in attempts per second.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.PerformanceStatistics", MethodName = "GetSystemStatistic_AverageThreadingContentionRate", Arguments = "", Enabled = 1, DataType = "System.Double", DisplayFormat = "{0:N3}", IsConnectedState = 0, LoadOrder = 8 })
            .Row(new { Source = "System", SignalIndex = 9, Name = "IO Usage", Description = "Amount of IO currently used by this process in kilobytes per second.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.PerformanceStatistics", MethodName = "GetSystemStatistic_IOUsage", Arguments = "", Enabled = 1, DataType = "System.Double", DisplayFormat = "{0:N3}", IsConnectedState = 0, LoadOrder = 9 })
            .Row(new { Source = "System", SignalIndex = 10, Name = "Average IO Usage", Description = "Average amount of IO used by this process in kilobytes per second.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.PerformanceStatistics", MethodName = "GetSystemStatistic_AverageIOUsage", Arguments = "", Enabled = 1, DataType = "System.Double", DisplayFormat = "{0:N3}", IsConnectedState = 0, LoadOrder = 10 })
            .Row(new { Source = "System", SignalIndex = 11, Name = "IP Data Send Rate", Description = "Number of IP datagrams (or bytes on Mono) currently sent by this process per second.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.PerformanceStatistics", MethodName = "GetSystemStatistic_IPDataSendRate", Arguments = "", Enabled = 1, DataType = "System.Double", DisplayFormat = "{0:N3}", IsConnectedState = 0, LoadOrder = 11 })
            .Row(new { Source = "System", SignalIndex = 12, Name = "Average IP Data Send Rate", Description = "Average number of IP datagrams (or bytes on Mono) sent by this process per second.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.PerformanceStatistics", MethodName = "GetSystemStatistic_AverageIPDataSendRate", Arguments = "", Enabled = 1, DataType = "System.Double", DisplayFormat = "{0:N3}", IsConnectedState = 0, LoadOrder = 12 })
            .Row(new { Source = "System", SignalIndex = 13, Name = "IP Data Receive Rate", Description = "Number of IP datagrams (or bytes on Mono) currently received by this process per second.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.PerformanceStatistics", MethodName = "GetSystemStatistic_IPDataReceiveRate", Arguments = "", Enabled = 1, DataType = "System.Double", DisplayFormat = "{0:N3}", IsConnectedState = 0, LoadOrder = 13 })
            .Row(new { Source = "System", SignalIndex = 14, Name = "Average IP Data Receive Rate", Description = "Average number of IP datagrams (or bytes on Mono) received by this process per second.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.PerformanceStatistics", MethodName = "GetSystemStatistic_AverageIPDataReceiveRate", Arguments = "", Enabled = 1, DataType = "System.Double", DisplayFormat = "{0:N3}", IsConnectedState = 0, LoadOrder = 14 })
            .Row(new { Source = "System", SignalIndex = 15, Name = "Up Time", Description = "Total number of seconds system has been running.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.PerformanceStatistics", MethodName = "GetSystemStatistic_UpTime", Arguments = "", Enabled = 1, DataType = "System.Double", DisplayFormat = "{0:N3} s", IsConnectedState = 0, LoadOrder = 15 })

            .Row(new { Source = "Device", SignalIndex = 1, Name = "Data Quality Errors", Description = "Number of data quality errors reported by device during last reporting interval.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.DeviceStatistics", MethodName = "GetDeviceStatistic_DataQualityErrors", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 1 })
            .Row(new { Source = "Device", SignalIndex = 2, Name = "Time Quality Errors", Description = "Number of time quality errors reported by device during last reporting interval.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.DeviceStatistics", MethodName = "GetDeviceStatistic_TimeQualityErrors", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 2 })
            .Row(new { Source = "Device", SignalIndex = 3, Name = "Device Errors", Description = "Number of device errors reported by device during last reporting interval.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.DeviceStatistics", MethodName = "GetDeviceStatistic_DeviceErrors", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 3 })
            .Row(new { Source = "Device", SignalIndex = 4, Name = "Measurements Received", Description = "Number of measurements received from device during last reporting interval.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.DeviceStatistics", MethodName = "GetDeviceStatistic_MeasurementsReceived", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 4 })
            .Row(new { Source = "Device", SignalIndex = 5, Name = "Measurements Expected", Description = "Expected number of measurements received from device during last reporting interval.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.DeviceStatistics", MethodName = "GetDeviceStatistic_MeasurementsExpected", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 5 })
            .Row(new { Source = "Device", SignalIndex = 6, Name = "Measurements With Error", Description = "Number of measurements received while device was reporting errors during last reporting interval.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.DeviceStatistics", MethodName = "GetDeviceStatistic_MeasurementsWithError", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 6 })
            .Row(new { Source = "Device", SignalIndex = 7, Name = "Measurements Defined", Description = "Number of defined measurements (per frame) from device during last reporting interval.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.DeviceStatistics", MethodName = "GetDeviceStatistic_MeasurementsDefined", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 7 })

            .Row(new { Source = "InputStream", SignalIndex = 1, Name = "Total Frames", Description = "Total number of frames received from input stream during last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetInputStreamStatistic_TotalFrames", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 2 })
            .Row(new { Source = "InputStream", SignalIndex = 2, Name = "Last Report Time", Description = "Timestamp of last received data frame from input stream.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetInputStreamStatistic_LastReportTime", Arguments = "", Enabled = 1, DataType = "System.DateTime", DisplayFormat = "{0:mm':'ss'.'fff}", IsConnectedState = 0, LoadOrder = 1 })
            .Row(new { Source = "InputStream", SignalIndex = 3, Name = "Missing Frames", Description = "Number of frames that were not received from input stream during last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetInputStreamStatistic_MissingFrames", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 3 })
            .Row(new { Source = "InputStream", SignalIndex = 4, Name = "CRC Errors", Description = "Number of CRC errors reported from input stream during last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetInputStreamStatistic_CRCErrors", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 16 })
            .Row(new { Source = "InputStream", SignalIndex = 5, Name = "Out of Order Frames", Description = "Number of out-of-order frames received from input stream during last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetInputStreamStatistic_OutOfOrderFrames", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 17 })
            .Row(new { Source = "InputStream", SignalIndex = 6, Name = "Minimum Latency", Description = "Minimum latency from input stream, in milliseconds, during last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetInputStreamStatistic_MinimumLatency", Arguments = "", Enabled = 1, DataType = "System.Double", DisplayFormat = "{0:N3} ms", IsConnectedState = 0, LoadOrder = 10 })
            .Row(new { Source = "InputStream", SignalIndex = 7, Name = "Maximum Latency", Description = "Maximum latency from input stream, in milliseconds, during last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetInputStreamStatistic_MaximumLatency", Arguments = "", Enabled = 1, DataType = "System.Double", DisplayFormat = "{0:N3} ms", IsConnectedState = 0, LoadOrder = 11 })
            .Row(new { Source = "InputStream", SignalIndex = 8, Name = "Input Stream Connected", Description = "Boolean value representing if input stream was continually connected during last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetInputStreamStatistic_Connected", Arguments = "", Enabled = 1, DataType = "System.Boolean", DisplayFormat = "{0}", IsConnectedState = 1, LoadOrder = 18 })
            .Row(new { Source = "InputStream", SignalIndex = 9, Name = "Received Configuration", Description = "Boolean value representing if input stream has received (or has cached) a configuration frame during last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetInputStreamStatistic_ReceivedConfiguration", Arguments = "", Enabled = 1, DataType = "System.Boolean", DisplayFormat = "{0}", IsConnectedState = 0, LoadOrder = 8 })
            .Row(new { Source = "InputStream", SignalIndex = 10, Name = "Configuration Changes", Description = "Number of configuration changes reported by input stream during last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetInputStreamStatistic_ConfigurationChanges", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 9 })
            .Row(new { Source = "InputStream", SignalIndex = 11, Name = "Configuration Frame Rate", Description = "Number of configuration frames received per second from input stream during last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetInputStreamStatistic_ConfigurationFrameRate", Arguments = "", Enabled = 1, DataType = "System.Double", DisplayFormat = "{0:N3}", IsConnectedState = 0, LoadOrder = 12 })
            .Row(new { Source = "InputStream", SignalIndex = 12, Name = "Data Frame Rate", Description = "Number of data frames received per second from input stream during last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetInputStreamStatistic_DataFrameRate", Arguments = "", Enabled = 1, DataType = "System.Double", DisplayFormat = "{0:N3}", IsConnectedState = 0, LoadOrder = 13 })
            .Row(new { Source = "InputStream", SignalIndex = 13, Name = "Total Bytes Received", Description = "Total number of bytes received from input stream during last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetInputStreamStatistic_TotalBytesReceived", Arguments = "", Enabled = 1, DataType = "System.Int64", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 14 })
            .Row(new { Source = "InputStream", SignalIndex = 14, Name = "Total Bytes Expected", Description = "Total number of bytes expected to be received from input stream during last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetInputStreamStatistic_TotalBytesExpected", Arguments = "", Enabled = 1, DataType = "System.Int64", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 15 })
            .Row(new { Source = "InputStream", SignalIndex = 15, Name = "Total Data Errors", Description = "Total number of data errors reported by input stream during last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetInputStreamStatistic_TotalDataErrors", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 16 })
            .Row(new { Source = "InputStream", SignalIndex = 16, Name = "Total Time Errors", Description = "Total number of time errors reported by input stream during last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetInputStreamStatistic_TotalTimeErrors", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 17 })
            .Row(new { Source = "InputStream", SignalIndex = 17, Name = "Total Device Errors", Description = "Total number of device errors reported by input stream during last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetInputStreamStatistic_TotalDeviceErrors", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 18 })
            .Row(new { Source = "InputStream", SignalIndex = 18, Name = "Total Measurements Received", Description = "Total number of measurements received from input stream during last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetInputStreamStatistic_TotalMeasurementsReceived", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 19 })
            .Row(new { Source = "InputStream", SignalIndex = 19, Name = "Total Bytes Received", Description = "Number of bytes received from the input source during last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetInputStreamStatistic_TotalBytesReceived", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 19 })
            .Row(new { Source = "InputStream", SignalIndex = 20, Name = "Lifetime Measurements", Description = "Number of processed measurements reported by the input stream during the lifetime of the input stream.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetInputStreamStatistic_LifetimeMeasurements", Arguments = "", Enabled = 1, DataType = "System.Int64", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 20 })
            .Row(new { Source = "InputStream", SignalIndex = 21, Name = "Lifetime Bytes Received", Description = "Number of bytes received from the input source during the lifetime of the input stream.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetInputStreamStatistic_LifetimeBytesReceived", Arguments = "", Enabled = 1, DataType = "System.Int64", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 21 })
            .Row(new { Source = "InputStream", SignalIndex = 22, Name = "Minimum Measurements Per Second", Description = "The minimum number of measurements received per second during the last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetInputStreamStatistic_MinimumMeasurementsPerSecond", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 22 })
            .Row(new { Source = "InputStream", SignalIndex = 23, Name = "Maximum Measurements Per Second", Description = "The maximum number of measurements received per second during the last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetInputStreamStatistic_MaximumMeasurementsPerSecond", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 23 })
            .Row(new { Source = "InputStream", SignalIndex = 24, Name = "Average Measurements Per Second", Description = "The average number of measurements received per second during the last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetInputStreamStatistic_AverageMeasurementsPerSecond", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 24 })
            .Row(new { Source = "InputStream", SignalIndex = 25, Name = "Lifetime Minimum Latency", Description = "Minimum latency from input stream, in milliseconds, during the lifetime of the input stream.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetInputStreamStatistic_LifetimeMinimumLatency", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0} ms", IsConnectedState = 0, LoadOrder = 25 })
            .Row(new { Source = "InputStream", SignalIndex = 26, Name = "Lifetime Maximum Latency", Description = "Maximum latency from input stream, in milliseconds, during the lifetime of the input stream.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetInputStreamStatistic_LifetimeMaximumLatency", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0} ms", IsConnectedState = 0, LoadOrder = 26 })
            .Row(new { Source = "InputStream", SignalIndex = 27, Name = "Lifetime Average Latency", Description = "Average latency, in milliseconds, for data received from input stream during the lifetime of the input stream.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetInputStreamStatistic_LifetimeAverageLatency", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0} ms", IsConnectedState = 0, LoadOrder = 27 })
            .Row(new { Source = "InputStream", SignalIndex = 28, Name = "Up Time", Description = "Total number of seconds input stream has been running.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetInputStreamStatistic_UpTime", Arguments = "", Enabled = 1, DataType = "System.Double", DisplayFormat = "{0:N3} s", IsConnectedState = 0, LoadOrder = 28 })

            .Row(new { Source = "OutputStream", SignalIndex = 1, Name = "Discarded Measurements", Description = "Number of discarded measurements reported by output stream during last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetOutputStreamStatistic_DiscardedMeasurements", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 4 })
            .Row(new { Source = "OutputStream", SignalIndex = 2, Name = "Received Measurements", Description = "Number of received measurements reported by the output stream during last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetOutputStreamStatistic_ReceivedMeasurements", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 2 })
            .Row(new { Source = "OutputStream", SignalIndex = 3, Name = "Expected Measurements", Description = "Number of expected measurements reported by the output stream during last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetOutputStreamStatistic_ExpectedMeasurements", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 1 })
            .Row(new { Source = "OutputStream", SignalIndex = 4, Name = "Processed Measurements", Description = "Number of processed measurements reported by the output stream during last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetOutputStreamStatistic_ProcessedMeasurements", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 3 })
            .Row(new { Source = "OutputStream", SignalIndex = 5, Name = "Measurements Sorted by Arrival", Description = "Number of measurements sorted by arrival reported by the output stream during last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetOutputStreamStatistic_MeasurementsSortedByArrival", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 7 })
            .Row(new { Source = "OutputStream", SignalIndex = 6, Name = "Published Measurements", Description = "Number of published measurements reported by output stream during last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetOutputStreamStatistic_PublishedMeasurements", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 5 })
            .Row(new { Source = "OutputStream", SignalIndex = 7, Name = "Downsampled Measurements", Description = "Number of downsampled measurements reported by the output stream during last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetOutputStreamStatistic_DownsampledMeasurements", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 6 })
            .Row(new { Source = "OutputStream", SignalIndex = 8, Name = "Missed Sorts by Timeout", Description = "Number of missed sorts by timeout reported by the output stream during last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetOutputStreamStatistic_MissedSortsByTimeout", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 8 })
            .Row(new { Source = "OutputStream", SignalIndex = 9, Name = "Frames Ahead of Schedule", Description = "Number of frames ahead of schedule reported by the output stream during last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetOutputStreamStatistic_FramesAheadOfSchedule", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 9 })
            .Row(new { Source = "OutputStream", SignalIndex = 10, Name = "Published Frames", Description = "Number of published frames reported by the output stream during last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetOutputStreamStatistic_PublishedFrames", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 10 })
            .Row(new { Source = "OutputStream", SignalIndex = 11, Name = "Output Stream Connected", Description = "Boolean value representing if the output stream was continually connected during last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetOutputStreamStatistic_Connected", Arguments = "", Enabled = 1, DataType = "System.Boolean", DisplayFormat = "{0}", IsConnectedState = 1, LoadOrder = 11 })
            .Row(new { Source = "OutputStream", SignalIndex = 12, Name = "Minimum Latency", Description = "Minimum latency from output stream, in milliseconds, during last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetOutputStreamStatistic_MinimumLatency", Arguments = "", Enabled = 1, DataType = "System.Double", DisplayFormat = "{0:N3} ms", IsConnectedState = 0, LoadOrder = 12 })
            .Row(new { Source = "OutputStream", SignalIndex = 13, Name = "Maximum Latency", Description = "Maximum latency from output stream, in milliseconds, during last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetOutputStreamStatistic_MaximumLatency", Arguments = "", Enabled = 1, DataType = "System.Double", DisplayFormat = "{0:N3} ms", IsConnectedState = 0, LoadOrder = 13 })
            .Row(new { Source = "OutputStream", SignalIndex = 14, Name = "Average Latency", Description = "Average latency, in milliseconds, for data published from output stream during last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetOutputStreamStatistic_AverageLatency", Arguments = "", Enabled = 1, DataType = "System.Double", DisplayFormat = "{0:N3} ms", IsConnectedState = 0, LoadOrder = 14 })
            .Row(new { Source = "OutputStream", SignalIndex = 15, Name = "Connected Clients", Description = "Number of clients connected to the command channel of the output stream during last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetOutputStreamStatistic_ConnectedClientCount", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 15 })
            .Row(new { Source = "OutputStream", SignalIndex = 16, Name = "Total Bytes Sent", Description = "Number of bytes sent from output stream during last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetOutputStreamStatistic_TotalBytesSent", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 16 })
            .Row(new { Source = "OutputStream", SignalIndex = 17, Name = "Lifetime Measurements", Description = "Number of processed measurements reported by the output stream during the lifetime of the output stream.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetOutputStreamStatistic_LifetimeMeasurements", Arguments = "", Enabled = 1, DataType = "System.Int64", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 17 })
            .Row(new { Source = "OutputStream", SignalIndex = 18, Name = "Lifetime Bytes Sent", Description = "Number of bytes sent from the output source during the lifetime of the output stream.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetOutputStreamStatistic_LifetimeBytesSent", Arguments = "", Enabled = 1, DataType = "System.Int64", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 18 })
            .Row(new { Source = "OutputStream", SignalIndex = 19, Name = "Minimum Measurements Per Second", Description = "The minimum number of measurements sent per second during the last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetOutputStreamStatistic_MinimumMeasurementsPerSecond", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 19 })
            .Row(new { Source = "OutputStream", SignalIndex = 20, Name = "Maximum Measurements Per Second", Description = "The maximum number of measurements sent per second during the last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetOutputStreamStatistic_MaximumMeasurementsPerSecond", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 20 })
            .Row(new { Source = "OutputStream", SignalIndex = 21, Name = "Average Measurements Per Second", Description = "The average number of measurements sent per second during the last reporting interval.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetOutputStreamStatistic_AverageMeasurementsPerSecond", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 21 })
            .Row(new { Source = "OutputStream", SignalIndex = 22, Name = "Lifetime Minimum Latency", Description = "Minimum latency from output stream, in milliseconds, during the lifetime of the output stream.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetOutputStreamStatistic_LifetimeMinimumLatency", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0} ms", IsConnectedState = 0, LoadOrder = 22 })
            .Row(new { Source = "OutputStream", SignalIndex = 23, Name = "Lifetime Maximum Latency", Description = "Maximum latency from output stream, in milliseconds, during the lifetime of the output stream.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetOutputStreamStatistic_LifetimeMaximumLatency", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0} ms", IsConnectedState = 0, LoadOrder = 23 })
            .Row(new { Source = "OutputStream", SignalIndex = 24, Name = "Lifetime Average Latency", Description = "Average latency from output stream, in milliseconds, during the lifetime of the output stream.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetOutputStreamStatistic_LifetimeAverageLatency", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0} ms", IsConnectedState = 0, LoadOrder = 24 })
            .Row(new { Source = "OutputStream", SignalIndex = 25, Name = "Lifetime Discarded Measurements", Description = "Number of discarded measurements reported by output stream during the lifetime of the output stream.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetOutputStreamStatistic_LifetimeDiscardedMeasurements", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 25 })
            .Row(new { Source = "OutputStream", SignalIndex = 26, Name = "Lifetime Downsampled Measurements", Description = "Number of downsampled measurements reported by the output stream during the lifetime of the output stream.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetOutputStreamStatistic_LifetimeDownsampledMeasurements", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 26 })
            .Row(new { Source = "OutputStream", SignalIndex = 27, Name = "Up Time", Description = "Total number of seconds output stream has been running.", AssemblyName = "PhasorProtocolAdapters.dll", TypeName = "PhasorProtocolAdapters.CommonPhasorServices", MethodName = "GetOutputStreamStatistic_UpTime", Arguments = "", Enabled = 1, DataType = "System.Double", DisplayFormat = "{0:N3} s", IsConnectedState = 0, LoadOrder = 27 })

            .Row(new { Source = "Subscriber", SignalIndex = 1, Name = "Subscriber Connected", Description = "Boolean value representing if the subscriber was continually connected during last reporting interval.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.GatewayStatistics", MethodName = "GetSubscriberStatistic_Connected", Arguments = "", Enabled = 1, DataType = "System.Boolean", DisplayFormat = "{0}", IsConnectedState = 1, LoadOrder = 1 })
            .Row(new { Source = "Subscriber", SignalIndex = 2, Name = "Subscriber Authenticated", Description = "Boolean value representing if the subscriber was authenticated to the publisher during last reporting interval.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.GatewayStatistics", MethodName = "GetSubscriberStatistic_Authenticated", Arguments = "", Enabled = 1, DataType = "System.Boolean", DisplayFormat = "{0}", IsConnectedState = 0, LoadOrder = 5 })
            .Row(new { Source = "Subscriber", SignalIndex = 3, Name = "Processed Measurements", Description = "Number of processed measurements reported by the subscriber during last reporting interval.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.GatewayStatistics", MethodName = "GetSubscriberStatistic_ProcessedMeasurements", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 3 })
            .Row(new { Source = "Subscriber", SignalIndex = 4, Name = "Total Bytes Received", Description = "Number of bytes received from subscriber during last reporting interval.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.GatewayStatistics", MethodName = "GetSubscriberStatistic_TotalBytesReceived", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 4 })
            .Row(new { Source = "Subscriber", SignalIndex = 5, Name = "Authorized Signal Count", Description = "Number of signals authorized to the subscriber by the publisher.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.GatewayStatistics", MethodName = "GetSubscriberStatistic_AuthorizedCount", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 5 })
            .Row(new { Source = "Subscriber", SignalIndex = 6, Name = "Unauthorized Signal Count", Description = "Number of signals denied to the subscriber by the publisher.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.GatewayStatistics", MethodName = "GetSubscriberStatistic_UnauthorizedCount", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 6 })
            .Row(new { Source = "Subscriber", SignalIndex = 7, Name = "Lifetime Measurements", Description = "Number of processed measurements reported by the subscriber during the lifetime of the subscriber.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.GatewayStatistics", MethodName = "GetSubscriberStatistic_LifetimeMeasurements", Arguments = "", Enabled = 1, DataType = "System.Int64", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 7 })
            .Row(new { Source = "Subscriber", SignalIndex = 8, Name = "Lifetime Bytes Received", Description = "Number of bytes received from subscriber during the lifetime of the subscriber.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.GatewayStatistics", MethodName = "GetSubscriberStatistic_LifetimeBytesReceived", Arguments = "", Enabled = 1, DataType = "System.Int64", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 8 })
            .Row(new { Source = "Subscriber", SignalIndex = 9, Name = "Minimum Measurements Per Second", Description = "The minimum number of measurements received per second during the last reporting interval.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.GatewayStatistics", MethodName = "GetSubscriberStatistic_MinimumMeasurementsPerSecond", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 9 })
            .Row(new { Source = "Subscriber", SignalIndex = 10, Name = "Maximum Measurements Per Second", Description = "The maximum number of measurements received per second during the last reporting interval.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.GatewayStatistics", MethodName = "GetSubscriberStatistic_MaximumMeasurementsPerSecond", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 10 })
            .Row(new { Source = "Subscriber", SignalIndex = 11, Name = "Average Measurements Per Second", Description = "The average number of measurements received per second during the last reporting interval.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.GatewayStatistics", MethodName = "GetSubscriberStatistic_AverageMeasurementsPerSecond", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 11 })
            .Row(new { Source = "Subscriber", SignalIndex = 12, Name = "Lifetime Minimum Latency", Description = "Minimum latency from subscriber, in milliseconds, during the lifetime of the subscriber.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.GatewayStatistics", MethodName = "GetSubscriberStatistic_LifetimeMinimumLatency", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0} ms", IsConnectedState = 0, LoadOrder = 12 })
            .Row(new { Source = "Subscriber", SignalIndex = 13, Name = "Lifetime Maximum Latency", Description = "Maximum latency from subscriber, in milliseconds, during the lifetime of the subscriber.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.GatewayStatistics", MethodName = "GetSubscriberStatistic_LifetimeMaximumLatency", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0} ms", IsConnectedState = 0, LoadOrder = 13 })
            .Row(new { Source = "Subscriber", SignalIndex = 14, Name = "Lifetime Average Latency", Description = "Average latency, in milliseconds, for data received from subscriber during the lifetime of the subscriber.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.GatewayStatistics", MethodName = "GetSubscriberStatistic_LifetimeAverageLatency", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0} ms", IsConnectedState = 0, LoadOrder = 14 })
            .Row(new { Source = "Subscriber", SignalIndex = 15, Name = "Up Time", Description = "Total number of seconds subscriber has been running.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.GatewayStatistics", MethodName = "GetSubscriberStatistic_UpTime", Arguments = "", Enabled = 1, DataType = "System.Double", DisplayFormat = "{0:N3} s", IsConnectedState = 0, LoadOrder = 15 })

            .Row(new { Source = "Publisher", SignalIndex = 1, Name = "Publisher Connected", Description = "Boolean value representing if the publisher was continually connected during last reporting interval.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.GatewayStatistics", MethodName = "GetPublisherStatistic_Connected", Arguments = "", Enabled = 1, DataType = "System.Boolean", DisplayFormat = "{0}", IsConnectedState = 1, LoadOrder = 1 })
            .Row(new { Source = "Publisher", SignalIndex = 2, Name = "Connected Clients", Description = "Number of clients connected to the command channel of the publisher during last reporting interval.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.GatewayStatistics", MethodName = "GetPublisherStatistic_ConnectedClientCount", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 2 })
            .Row(new { Source = "Publisher", SignalIndex = 3, Name = "Processed Measurements", Description = "Number of processed measurements reported by the publisher during last reporting interval.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.GatewayStatistics", MethodName = "GetPublisherStatistic_ProcessedMeasurements", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 3 })
            .Row(new { Source = "Publisher", SignalIndex = 4, Name = "Total Bytes Sent", Description = "Number of bytes sent by the publisher during the last reporting interval.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.GatewayStatistics", MethodName = "GetPublisherStatistic_TotalBytesSent", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 4 })
            .Row(new { Source = "Publisher", SignalIndex = 5, Name = "Lifetime Measurements", Description = "Number of processed measurements reported by the publisher during the lifetime of the publisher.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.GatewayStatistics", MethodName = "GetPublisherStatistic_LifetimeMeasurements", Arguments = "", Enabled = 1, DataType = "System.Int64", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 5 })
            .Row(new { Source = "Publisher", SignalIndex = 6, Name = "Lifetime Bytes Sent", Description = "Number of bytes sent by the publisher during the lifetime of the publisher.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.GatewayStatistics", MethodName = "GetPublisherStatistic_LifetimeBytesSent", Arguments = "", Enabled = 1, DataType = "System.Int64", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 6 })
            .Row(new { Source = "Publisher", SignalIndex = 7, Name = "Minimum Measurements Per Second", Description = "The minimum number of measurements sent per second during the last reporting interval.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.GatewayStatistics", MethodName = "GetPublisherStatistic_MinimumMeasurementsPerSecond", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 7 })
            .Row(new { Source = "Publisher", SignalIndex = 8, Name = "Maximum Measurements Per Second", Description = "The maximum number of measurements sent per second during the last reporting interval.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.GatewayStatistics", MethodName = "GetPublisherStatistic_MaximumMeasurementsPerSecond", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 8 })
            .Row(new { Source = "Publisher", SignalIndex = 9, Name = "Average Measurements Per Second", Description = "The average number of measurements sent per second during the last reporting interval.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.GatewayStatistics", MethodName = "GetPublisherStatistic_AverageMeasurementsPerSecond", Arguments = "", Enabled = 1, DataType = "System.Int32", DisplayFormat = "{0:N0}", IsConnectedState = 0, LoadOrder = 9 })
            .Row(new { Source = "Publisher", SignalIndex = 10, Name = "Lifetime Minimum Latency", Description = "Minimum latency from output stream, in milliseconds, during the lifetime of the publisher.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.GatewayStatistics", MethodName = "GetPublisherStatistic_LifetimeMinimumLatency", Arguments = "", Enabled = 1, DataType = "System.Int64", DisplayFormat = "{0:N0} ms", IsConnectedState = 0, LoadOrder = 10 })
            .Row(new { Source = "Publisher", SignalIndex = 11, Name = "Lifetime Maximum Latency", Description = "Maximum latency from output stream, in milliseconds, during the lifetime of the publisher.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.GatewayStatistics", MethodName = "GetPublisherStatistic_LifetimeMaximumLatency", Arguments = "", Enabled = 1, DataType = "System.Int64", DisplayFormat = "{0:N0} ms", IsConnectedState = 0, LoadOrder = 11 })
            .Row(new { Source = "Publisher", SignalIndex = 12, Name = "Lifetime Average Latency", Description = "Average latency, in milliseconds, for data sent from output stream during the lifetime of the publisher.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.GatewayStatistics", MethodName = "GetPublisherStatistic_LifetimeAverageLatency", Arguments = "", Enabled = 1, DataType = "System.Int64", DisplayFormat = "{0:N0} ms", IsConnectedState = 0, LoadOrder = 12 })
            .Row(new { Source = "Publisher", SignalIndex = 13, Name = "Up Time", Description = "Total number of seconds publisher has been running.", AssemblyName = "Gemstone.Timeseries.dll", TypeName = "Gemstone.Timeseries.Statistics.GatewayStatistics", MethodName = "GetPublisherStatistic_UpTime", Arguments = "", Enabled = 1, DataType = "System.Double", DisplayFormat = "{0:N3} s", IsConnectedState = 0, LoadOrder = 13 });

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