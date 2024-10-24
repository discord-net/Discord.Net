using Discord.Net.Hanz.Utils.Bakery;
using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz;

public readonly record struct Keyed<TKey, TValue>(
    TKey Key,
    TValue Value
);

public static partial class ProviderExtensions
{
    public static IncrementalValuesProvider<Keyed<TKey, TValue>> ToKeyed<TKey, TValue>(
        this IncrementalValuesProvider<TValue> source,
        Func<TValue, CancellationToken, TKey> keySelector
    )
        where TValue : IEquatable<TValue>
        where TKey : IEquatable<TKey>
        => source.Select((element, token) => new Keyed<TKey, TValue>(keySelector(element, token), element));

    public static IncrementalValueProvider<Grouping<TKey, TValue>> ToGroup<TKey, TValue>(
        this IncrementalValuesProvider<Keyed<TKey, TValue>> keyed
    )
        where TValue : IEquatable<TValue>
        where TKey : IEquatable<TKey>
        => keyed
            .Collect()
            .Select((group, token) =>
            {
                var dict = new Dictionary<TKey, List<TValue>>();
                foreach (var entry in group)
                {
                    if (!dict.TryGetValue(entry.Key, out var values))
                        dict[entry.Key] = values = new();

                    values.Add(entry.Value);
                    
                    token.ThrowIfCancellationRequested();
                }

                return new Grouping<TKey, TValue>(
                    dict.ToDictionary(
                        x => x.Key,
                        x => x.Value.ToImmutableEquatableArray()
                    ),
                    group.GetHashCode()
                );
            });

    public static IncrementalValuesProvider<TResult> Combine<TKey, TValue, TSource, TResult>(
        this IncrementalValuesProvider<Keyed<TKey, TValue>> keyed,
        IncrementalValuesProvider<TSource> source,
        Func<TSource, TKey> keySelector,
        Func<TSource, TValue, TResult> resultSelector
    ) 
        where TValue : IEquatable<TValue>
        where TKey : IEquatable<TKey>
        => source.Pair(keyed.ToGroup(), keySelector, resultSelector);
}