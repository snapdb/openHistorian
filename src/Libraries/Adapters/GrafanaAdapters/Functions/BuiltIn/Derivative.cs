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
/// Returns a series of values that represent the rate of change, per time units, for the difference between consecutive values in the source series.
/// The <c>units</c> parameter, optional, specifies the type of time units and must be one of the following: Seconds, Nanoseconds, Microseconds,
/// Milliseconds, Minutes, Hours, Days, Weeks, Ke (i.e., traditional Chinese unit of decimal time), Ticks (i.e., 100-nanosecond intervals),
/// PlanckTime or AtomicUnitsOfTime - defaults to Seconds.
/// </summary>
/// <remarks>
/// Signature: <c>Derivative([units = Seconds], expression)</c><br/>
/// Returns: Series of values.<br/>
/// Example: <c>Derivative(FILTER ActiveMeasurements WHERE SignalType='FREQ')</c><br/>
/// Variants: Derivative, Der<br/>
/// Execution: Deferred enumeration.
/// </remarks>
public abstract class Derivative<T> : GrafanaFunctionBase<T> where T : struct, IDataSourceValueType<T>
{
    /// <inheritdoc />
    public override string Name => nameof(Derivative<T>);

    /// <inheritdoc />
    public override string Description => "Returns a series of values that represent the rate of change, per time units, for the difference between consecutive values in the source series.";

    /// <inheritdoc />
    public override string[] Aliases => ["Der"];

    /// <inheritdoc />
    public override ReturnType ReturnType => ReturnType.Series;

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
            Default = new TargetTimeUnit { Unit = TimeUnit.Seconds },
            Parse = TargetTimeUnit.Parse,
            Description = "Specifies the type of time units and must be one of the following: Seconds, Nanoseconds, Microseconds, Milliseconds, " +
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
                Value = (dataValue.Value - lastResult.Value) / TargetTimeUnit.ToTimeUnits((dataValue.Time - lastResult.Time) * SI.Milli, units)
            };
        }

        // Return deferred enumeration of computed values
        await foreach (T dataValue in GetDataSourceValues(parameters).Select(transposeCompute).WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            if (lastResult.Time > 0.0D)
                yield return dataValue;

            lastResult = dataValue;
        }
    }

    /// <inheritdoc />
    public class ComputeMeasurementValue : Derivative<MeasurementValue>
    {
    }

    /// <inheritdoc />
    public class ComputePhasorValue : Derivative<PhasorValue>
    {
        // Operating on magnitude only
    }
}