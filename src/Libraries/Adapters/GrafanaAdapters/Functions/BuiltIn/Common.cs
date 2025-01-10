using System;

namespace GrafanaAdapters.Functions.BuiltIn;

internal static class HelperExtensions
{
    public static T NotZero<T>(this T source, T nonZeroReturnValue) where T : struct, IEquatable<T>
    {
        return source.Equals(default) ? nonZeroReturnValue : source;
    }
}
