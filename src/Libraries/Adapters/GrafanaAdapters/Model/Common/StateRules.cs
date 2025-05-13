//******************************************************************************************************
//  StateRules.cs - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
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
//  05/13/2025 - C. Lackner
//       Generated original version of source code.
//
//******************************************************************************************************
// ReSharper disable InconsistentNaming

using Gemstone.Configuration;
using Gemstone.Timeseries.Adapters;
using Gemstone.Timeseries;
using Gemstone;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GrafanaAdapters.Model.Common;

/// <summary>
/// Defines a Rue to be checked for determinig state of a Device
/// </summary>
public class StateRule(StateRuleDefinition definition)
{

    public AlarmCombination Combination { get; } = definition.Combination;
    public double SetPoint { get; } = definition.SetPoint;
    public AlarmOperation Operation { get; } = definition.Operation;
    public string Query { get; } = definition.Query;
    public double Delay { get; } = definition.Delay;
    public Dictionary<int, MeasurementKey[]> MeasurementKeys { get; set; }
    public Dictionary<int, Ticks> LastUpdateTime { get; set; }

    public StateRule(string definition) : this(new StateRuleDefinition(definition))
    {
        MeasurementKeys = new Dictionary<int, MeasurementKey[]>();
        LastUpdateTime = new Dictionary<int, Ticks>();
    }

    /// <summary>
    /// Checks whether this rule is satsified.
    /// </summary>
    /// <returns></returns>
    public bool Test(IReadOnlyDictionary<MeasurementKey, IMeasurement> measurements, int deviceID, Ticks Time)
    {
        if (!MeasurementKeys.TryGetValue(deviceID, out MeasurementKey[] keys))
            return false;

        bool test = false;

        Func<double, bool> func = GetSuccededTest();

        if (Combination == AlarmCombination.AND)
            test = keys.All((key) => measurements.TryGetValue(key, out IMeasurement meas) && func.Invoke(meas.AdjustedValue));
        if (Combination == AlarmCombination.OR)
            test = keys.Any((key) => measurements.TryGetValue(key, out IMeasurement meas) && func.Invoke(meas.AdjustedValue));

        if (!test && LastUpdateTime.ContainsKey(deviceID))
            LastUpdateTime[deviceID] = Time;
        else if (!test)
            LastUpdateTime.Add(deviceID, Time);

        if (Delay >= 0 && LastUpdateTime.ContainsKey(deviceID))
            return test && (Time - LastUpdateTime[deviceID]).ToSeconds() > Delay;

        return test;
    }

    // Returns the function used to determine when the rule is satisfied.
    private Func<double, bool> GetSuccededTest() =>
        Operation switch
        {
            AlarmOperation.Equal => RaiseIfEqual,
            AlarmOperation.NotEqual => RaiseIfNotEqual,
            AlarmOperation.GreaterOrEqual => RaiseIfGreaterOrEqual,
            AlarmOperation.LessOrEqual => RaiseIfLessOrEqual,
            AlarmOperation.GreaterThan => RaiseIfGreaterThan,
            AlarmOperation.LessThan => RaiseIfLessThan,
            AlarmOperation.Or => RaiseIfOr,
            AlarmOperation.And => RaiseIfAnd,
            _ => throw new ArgumentOutOfRangeException()
        };

    // Indicates whether the given measurement is
    // equal to the set point within the tolerance.
    private bool RaiseIfEqual(double measurement) => measurement <= SetPoint + s_tolerance &&
                       measurement >= SetPoint - s_tolerance;

    // Indicates whether the given measurement is outside
    // the range defined by the set point and tolerance.
    private bool RaiseIfNotEqual(double measurement) => measurement < SetPoint - s_tolerance ||
                          measurement > SetPoint + s_tolerance;

    // Indicates whether the given measurement
    // is greater than or equal to the set point.
    private bool RaiseIfGreaterOrEqual(double measurement) => measurement >= SetPoint;

    // Indicates whether the given measurement
    // is less than or equal to the set point.
    private bool RaiseIfLessOrEqual(double measurement) => measurement <= SetPoint;

    // Indicates whether the given measurement
    // is greater than the set point.
    private bool RaiseIfGreaterThan(double measurement) => measurement > SetPoint;

    // Indicates whether the given measurement
    // is less than the set point.
    private bool RaiseIfLessThan(double measurement) => measurement < SetPoint;

    // Indicates whether the given measurement is a
    // Binary AND to the set point.
    private bool RaiseIfAnd(double measurement) => ((ulong)measurement & (ulong)SetPoint) == 0;

    // Indicates whether the given measurement is a
    // Binary OR to the set point.
    private bool RaiseIfOr(double measurement) => ((ulong)measurement | (ulong)SetPoint) == 0;


    static double s_tolerance = double.Epsilon;
}

public class StateRuleDefinition
{
    [ConnectionStringParameter]
    public AlarmCombination Combination { get; set; }

    [ConnectionStringParameter]
    public double SetPoint { get; set; }

    [ConnectionStringParameter]
    public AlarmOperation Operation { get; set; }

    [ConnectionStringParameter]
    public string Query { get; set; }

    [ConnectionStringParameter]
    public double Delay { get; set; }

    public StateRuleDefinition(string definition)
    {
        ConnectionStringParser<ConnectionStringParameterAttribute> parser = new();
        parser.ParseConnectionString(definition, this);
    }

}