using System.Diagnostics.CodeAnalysis;

namespace Discord.Rest;

internal struct LazyCollection<T>(IEnumerable<T> enumerable)
{
    public T[] Items => _evaluated ??= enumerable.ToArray();

    private T[]? _evaluated;

    public void UpdateTo(IEnumerable<T> newEnumerable)
    {
        _evaluated = null;
        enumerable = newEnumerable;
    }
}

internal static class LazyCollectionExtensions
{
    [return: NotNullIfNotNull(nameof(enumerable))]
    public static LazyCollection<T>? ToLazy<T>(this IEnumerable<T>? enumerable)
        => enumerable is not null ? new LazyCollection<T>(enumerable) : null;
}
