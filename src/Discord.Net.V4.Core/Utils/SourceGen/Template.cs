namespace Discord;

internal static class Template
{
    public static Template<T> Of<T>() => default;
}

internal readonly struct Template<T>;
