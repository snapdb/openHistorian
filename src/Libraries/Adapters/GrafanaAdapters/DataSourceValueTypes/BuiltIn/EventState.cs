//******************************************************************************************************
//  EventState.cs - Gbtc
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
//  03/25/2025 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using Gemstone.Timeseries;

namespace GrafanaAdapters.DataSourceValueTypes.BuiltIn;

/// <summary>
/// Represents a default target for an event state.
/// </summary>
public enum EventStateTarget
{
    /// <summary>
    /// Target the raised component of the event state.
    /// </summary>
    Raised,
    /// <summary>
    /// Target the duration component of the event state.
    /// </summary>
    Duration
}

/// <summary>
/// Represents an individual event state from a data source.
/// </summary>
public partial struct EventState
{
    /// <summary>
    /// Defines the primary metadata table name for a <see cref="EventState"/>.
    /// </summary>
    public const string MetadataTableName = "EventStates";

    /// <summary>
    /// Query target, i.e., a point-tag representing the event.
    /// </summary>
    public string Target;

    /// <summary>
    /// Event details.
    /// </summary>
    public string Details;

    /// <summary>
    /// Queried active event alarm state, 0 or 1.
    /// </summary>
    public double Raised;

    /// <summary>
    /// Event duration, in milliseconds, since raised state if cleared; otherwise, <c>double.NaN</c>.
    /// </summary>
    /// <remarks>
    /// If 'Raised' is 0, then 'Duration' is the time since the event was last raised.
    /// If 'Raised' is 1, then 'Duration' will be <c>double.NaN</c>, i.e., ongoing.
    /// </remarks>
    public double Duration;

    /// <summary>
    /// Timestamp, in Unix epoch milliseconds, of queried value.
    /// </summary>
    public double Time;

    /// <summary>
    /// Flags for queried value.
    /// </summary>
    public MeasurementStateFlags Flags;

    /// <summary>
    /// Gets or sets the primary target for the event state.
    /// </summary>
    /// <remarks>
    /// This property is used to determine which value field of the event state to use when using the data
    /// source as an <see cref="IDataSourceValueType{T}"/>. This is useful in default function computations that
    /// do not need to operate on both the 'Raised' and 'Duration' values of the event state struct.
    /// </remarks>
    public EventStateTarget PrimaryValueTarget;
}