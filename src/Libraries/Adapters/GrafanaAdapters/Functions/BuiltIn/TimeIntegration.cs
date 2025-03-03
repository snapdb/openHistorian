﻿using GrafanaAdapters.DataSourceValueTypes;
using GrafanaAdapters.DataSourceValueTypes.BuiltIn;
using Gemstone.Units;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable AccessToModifiedClosure

namespace GrafanaAdapters.Functions.BuiltIn;

/// <summary>
/// Returns a single value that represents the time-based integration, i.e., the sum of <c>V(n) * (T(n) - T(n-1))</c> where time difference is
/// calculated in the specified time units of the values in the source series. The <c>units</c>parameter, optional, specifies the type of time
/// units and must be one of the following: Seconds, Nanoseconds, Microseconds, Milliseconds, Minutes, Hours, Days, Weeks, Ke (i.e., traditional
/// Chinese unit of decimal time), Ticks (i.e., 100-nanosecond intervals), PlanckTime or AtomicUnitsOfTime - defaults to Hours.
/// </summary>
/// <remarks>
/// Signature: <c>TimeIntegration([units = Hours], expression)</c><br/>
/// Returns: Single value.<br/>
/// Example: <c>TimeIntegration(FILTER ActiveMeasurements WHERE SignalType='CALC' AND PointTag LIKE '%-MW:%')</c><br/>
/// Variants: TimeIntegration, TimeInt<br/>
/// Execution: Immediate enumeration.<br/>
/// Group Operations: Set
/// </remarks>
public abstract class TimeIntegration<T> : GrafanaFunctionBase<T> where T : struct, IDataSourceValueType<T>
{
    /// <inheritdoc />
    public override string Name => nameof(TimeIntegration<T>);

    /// <inheritdoc />
    public override string Description => "Returns a single value that represents the time-based integration.";

    /// <inheritdoc />
    public override string[] Aliases => ["TimeInt"];

    /// <inheritdoc />
    public override ReturnType ReturnType => ReturnType.Scalar;

    /// <inheritdoc />
    // Slice operation has no meaning for this time-focused function and Set operation will have an aberration between series,
    // so we override the exposed behaviors, i.e., use of Slice will produce an error and use of Set will be hidden:
    public override GroupOperations AllowedGroupOperations => GroupOperations.None | GroupOperations.Set;

    /// <inheritdoc />
    public override GroupOperations PublishedGroupOperations => GroupOperations.None;

    /// <inheritdoc />
    public override ParameterDefinitions ParameterDefinitions => new List<IParameter>
    {
        new ParameterDefinition<TargetTimeUnit>
        {
            Name = "units",
            Default = new TargetTimeUnit { Unit = TimeUnit.Hours },
            Parse = TargetTimeUnit.Parse,
            Description =
                "Specifies the type of time units and must be one of the following: Seconds, Nanoseconds, Microseconds, Milliseconds, " +
                "Minutes, Hours, Days, Weeks, Ke (i.e., traditional Chinese unit of decimal time), Ticks (i.e., 100-nanosecond intervals), PlanckTime or " +
                "AtomicUnitsOfTime.",
            Required = false
        }
    };

    /// <inheritdoc />
    public override async IAsyncEnumerable<T> ComputeAsync(Parameters parameters, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        TargetTimeUnit units = parameters.Value<TargetTimeUnit>(0);
        T lastResult = default;

        // Transpose computed value
        T transposeCompute(T dataValue)
        {
            if (lastResult.Time == 0.0D)
                return dataValue;

            return dataValue with
            {
                Value = lastResult.Value + dataValue.Value * TargetTimeUnit.ToTimeUnits((dataValue.Time - lastResult.Time) * SI.Milli, units)
            };
        }

        // Immediately enumerate to compute values - only enumerate once
        await foreach (T dataValue in GetDataSourceValues(parameters).Select(transposeCompute).WithCancellation(cancellationToken).ConfigureAwait(false))
            lastResult = dataValue;

        // Return computed value
        if (lastResult.Time > 0.0D)
            yield return lastResult;
    }

    /// <inheritdoc />
    public class ComputeMeasurementValue : TimeIntegration<MeasurementValue>
    {
    }

    /// <inheritdoc />
    public class ComputePhasorValue : TimeIntegration<PhasorValue>
    {
        // Operating on magnitude only
    }
}