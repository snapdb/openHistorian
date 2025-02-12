//******************************************************************************************************
//  ImportedMeasurement.cs - Gbtc
//
//  Copyright © 2017, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  10/25/2024 - Preston Crawford
//       Generated original version of source code.
//
//******************************************************************************************************

// ReSharper disable CheckNamespace
#pragma warning disable 1591
using System.ComponentModel.DataAnnotations;
using Gemstone.Data.Model;
using System.Data;

namespace openHistorian.Model;

public class MeasurementDetail : Measurement
{
    public int? CompanyID { get; set; }

    public string? CompanyName { get; set; }

    //public Guid? NodeID { get; set; }

    public string? HistorianAcronym { get; set; }

    public string HistorianConnectionString { get; set; } = "";

    [StringLength(200)]
    public string Source { get; set; } = "";

    public string? DeviceName { get; set; }

    public string? DeviceAcronym { get; set; }

    public bool? DeviceEnabled { get; set; }

    public string? ContactList { get; set; }

    public int? VendorDeviceID { get; set; }

    public string? VendorDeviceName { get; set; }

    public string? VendorDeviceDescription { get; set; }

    [StringLength(4)]
    public string SignalAcronym { get; set; }

    public int? FramesPerSecond { get; set; }

    public int? ProtocolID { get; set; }

    public string? ProtocolName { get; set; }

    public string? PhasorLabel { get; set; }

    public float? BaseKV { get; set; }

    [StringLength(200)]
    public string ProtocolAcronym { get; set; }

    [FieldDataType(DbType.String)]
    public char? PhasorType { get; set; }

    [FieldDataType(DbType.String)]
    public char? Phase { get; set; }

    [StringLength(200)]
    public string? CompanyAcronym { get; set; }

    public double? Longitude { get; set; }

    public double? Latitude { get; set; }

    public string EngineeringUnits { get; set; } = "";

    public string SignalName { get; set; }

    public string SignalTypeSuffix { get; set; }
}
