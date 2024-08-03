using System.Diagnostics.CodeAnalysis;

namespace Discord;

public static class NullableExtensions
{
    [return: NotNullIfNotNull(nameof(nullable))]
    public static T? Map<T, U>(this U? nullable, T value)
        where U : struct
        where T : struct
        => nullable.HasValue ? value : null;

    public static T? Map<T, U>(this U? nullable, Func<U, T> map)
        where U : struct
    {
        return nullable.HasValue ? map(nullable.Value) : default;
    }

    public static T? Map<T, U, P1>(this U? nullable, Func<U, P1, T> map, P1 arg1)
        where U : struct
    {
        return nullable.HasValue ? map(nullable.Value, arg1) : default;
    }

    public static T? Map<T, U, P1, P2>(this U? nullable, Func<U, P1, P2, T> map, P1 arg1, P2 arg2)
        where U : struct
    {
        return nullable.HasValue ? map(nullable.Value, arg1, arg2) : default;
    }

    public static T? Map<T, U, P1, P2, P3>(this U? nullable, Func<U, P1, P2, P3, T> map, P1 arg1, P2 arg2, P3 args3)
        where U : struct
    {
        return nullable.HasValue ? map(nullable.Value, arg1, arg2, args3) : default;
    }
}
