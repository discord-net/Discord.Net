using System.Collections.Immutable;
using Discord.Net.Hanz.Utils.Bakery;
using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz;

public static class IncrementalProviderExtensions
{
    public static IncrementalValuesProvider<V> ForEach<T, U, V>(
        this IncrementalValuesProvider<T> source,
        IEnumerable<U> enumerable,
        Func<IncrementalValuesProvider<T>, U, IncrementalValuesProvider<V>> func
    )
    {
        var arr = enumerable.ToArray();

        switch (arr.Length)
        {
            case 0:
                throw new ArgumentOutOfRangeException(nameof(enumerable), "Must have atleast one element.");
            default:
                return arr
                    .Skip(1)
                    .Aggregate(
                        func(source, arr[0]),
                        (current, next) =>
                            func(source, next)
                                .Collect()
                                .Combine(current.Collect())
                                .SelectMany(IEnumerable<V> (entry, token) => [..entry.Left, ..entry.Right])
                    );
        }
    }

    public static IncrementalValuesProvider<T> WhereNonNull<T>(
        this IncrementalValuesProvider<T?> source
    ) where T : class
    {
        return source.Where(x => x is not null)!;
    }

    public static IncrementalValuesProvider<T> WhereNonNull<T>(
        this IncrementalValuesProvider<T?> source
    ) where T : struct
    {
        return source.Where(x => x.HasValue).Select((x, _) => x!.Value);
    }

    public static IncrementalValueProvider<T?> FirstOrDefault<T>(
        this IncrementalValuesProvider<T> source,
        Func<T, bool> predicate)
        where T : class
    {
        return source.Collect().Select((x, _) => x.FirstOrDefault(predicate));
    }
}