﻿//******************************************************************************************************
//  AlarmDevice.cs - Gbtc
//
//  Copyright © 2018, Grid Protection Alliance.  All Rights Reserved.
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
//  07/18/2018 - J. Ritchie Carroll
//       Generated original version of source code.
//  27/11/2019 - C. Lackner
//      Moved Adapter to GSF
//
//******************************************************************************************************

using Gemstone.ComponentModel;
using Gemstone.Data.Model;
using System;
using System.ComponentModel.DataAnnotations;
using Gemstone.Expressions.Model;

namespace GrafanaAdapters.Model.Database;

/// <summary>
/// Represents a connected device in an alarm state.
/// </summary>
public class AlarmDevice
{
    /// <summary>
    /// Gets or sets unique ID.
    /// </summary>
    [PrimaryKey(true)]
    public int ID { get; set; }

    /// <summary>
    /// Gets or sets device ID of the Alarmed Device.
    /// </summary>
    public int DeviceID { get; set; }

    /// <summary>
    /// Gets or sets ID of the <see cref="AlarmState"/>.
    /// </summary>
    public int StateID { get; set; }

    /// <summary>
    /// Gets or sets time of the last update.
    /// </summary>
    [DefaultValueExpression("DateTime.UtcNow")]
    [UpdateValueExpression("DateTime.UtcNow")]
    public DateTime TimeStamp { get; set; }

    /// <summary>
    /// Gets or sets string to display on the Grafana Alarm Dashboard.
    /// </summary>
    [StringLength(10)]
    public string DisplayData { get; set; }
}