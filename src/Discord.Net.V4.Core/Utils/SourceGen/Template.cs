using System.Runtime.CompilerServices;

namespace Discord;

internal static class Template
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Template<T> T<T>() => default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Template<T> Of<T>() => default;
}

internal readonly struct Template<T>;
