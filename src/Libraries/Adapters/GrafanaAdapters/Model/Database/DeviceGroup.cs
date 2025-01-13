﻿//******************************************************************************************************
//  DeviceGroup.cs - Gbtc
//
//  Copyright © 2020, Grid Protection Alliance.  All Rights Reserved.
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
//  03/12/2020 - C. Lackner
//       Generated original version of source code.
//
//******************************************************************************************************

using Gemstone.Data.Model;
using System.Collections.Generic;

namespace GrafanaAdapters.Model.Database;

/// <summary>
/// Represents a group of devices modeled as a separate virtual device with connection string.
/// </summary>
public class DeviceGroup
{
    /// <summary>
    /// Gets or sets unique ID.
    /// </summary>
    [PrimaryKey(true)]
    public int ID { get; set; }

    /// <summary>
    /// Gets or sets name of the device.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets list of attached device IDs.
    /// </summary>
    public List<int> Devices { get; set; }

}