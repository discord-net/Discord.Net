using System.Collections.Immutable;
using Discord.Net.Hanz.Utils.Bakery;
using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz;

public readonly struct Grouping<TKey, TElement> :
    IEquatable<Grouping<TKey, TElement>>
    where TElement : IEquatable<TElement>
    where TKey : IEquatable<TKey>
{
    private readonly Dictionary<TKey, ImmutableEquatableArray<TElement>> _groups;

    private readonly int _hashCode;

    internal Grouping(Dictionary<TKey, ImmutableEquatableArray<TElement>> groups, int hash)
    {
        _hashCode = hash;
        _groups = groups;
    }

    public static Grouping<TKey, TElement> Create<TIntermediate>(
        ImmutableArray<TIntermediate> elements,
        Func<TIntermediate, TKey> keySelector,
        Func<TIntermediate, IEnumerable<TElement>> resultSelector
    ) => new(
        elements
            .GroupBy(keySelector)
            .ToDictionary(
                x => x.Key,
                x => new ImmutableEquatableArray<TElement>(x.SelectMany(resultSelector))
            ),
        HashCode.OfEach(elements)
    );

    public static Grouping<TKey, TElement> Create(ImmutableArray<TElement> elements, Func<TElement, TKey> keySelector)
        => new(
            elements
                .GroupBy(keySelector)
                .ToDictionary(
                    x => x.Key,
                    x => new ImmutableEquatableArray<TElement>(x)
                ),
            HashCode.OfEach(elements)
        );

    public static Grouping<TKey, TElement> Create(ImmutableEquatableArray<TElement> elements,
        Func<TElement, TKey> keySelector)
        => new(
            elements
                .GroupBy(keySelector)
                .ToDictionary(
                    x => x.Key,
                    x => new ImmutableEquatableArray<TElement>(x)
                ),
            HashCode.OfEach(elements)
        );

    public bool TryGetGroup(TKey key, out ImmutableEquatableArray<TElement> elements)
        => _groups.TryGetValue(key, out elements);

    public ImmutableEquatableArray<TElement> GetGroupOrEmpty(TKey key)
        => TryGetGroup(key, out var elements)
            ? elements
            : ImmutableEquatableArray<TElement>.Empty;

    public bool HasGroup(TKey key)
        => _groups.ContainsKey(key);

    public bool Equals(Grouping<TKey, TElement> other)
        => _hashCode == other._hashCode;

    public override int GetHashCode()
        => _hashCode;
}

public static partial class ProviderExtensions
{
    public static TElement? GetValueOrDefault<TKey, TElement>(
        this Grouping<TKey, TElement> group,
        TKey key,
        TElement? defaultValue = default
    )
        where TElement : struct, IEquatable<TElement>
        where TKey : IEquatable<TKey>
        => group.TryGetGroup(key, out var elements) && elements.Count > 0
            ? elements[0]
            : defaultValue;

    public static IncrementalValueProvider<Grouping<TKey, TElement>> GroupBy<TKey, TElement>(
        this IncrementalValueProvider<ImmutableEquatableArray<TElement>> source,
        Func<TElement, TKey> keySelector
    )
        where TElement : IEquatable<TElement>
        where TKey : IEquatable<TKey>
    {
        return source.Select((items, _) =>
            Grouping<TKey, TElement>.Create(items, keySelector)
        );
    }

    public static IncrementalValueProvider<Grouping<TKey, TElement>> GroupBy<TKey, TElement>(
        this IncrementalValueProvider<ImmutableArray<TElement>> source,
        Func<TElement, TKey> keySelector
    )
        where TElement : IEquatable<TElement>
        where TKey : IEquatable<TKey>
    {
        return source.Select((items, _) =>
            Grouping<TKey, TElement>.Create(items, keySelector)
        );
    }

    public static IncrementalValueProvider<Grouping<TKey, TResult>> GroupBy<TKey, TElement, TResult>(
        this IncrementalValueProvider<ImmutableArray<TElement>> source,
        Func<TElement, TKey> keySelector,
        Func<TElement, IEnumerable<TResult>> valueSelector
    )
        where TElement : IEquatable<TElement>
        where TResult : IEquatable<TResult>
        where TKey : IEquatable<TKey>
    {
        return source.Select((items, _) =>
            Grouping<TKey, TResult>.Create(items, keySelector, valueSelector)
        );
    }

    public static IncrementalValueProvider<Grouping<TKey, TElement>> GroupBy<TKey, TElement>(
        this IncrementalValuesProvider<TElement> source,
        Func<TElement, TKey> keySelector
    )
        where TElement : IEquatable<TElement>
        where TKey : IEquatable<TKey>
        => GroupBy(source.Collect(), keySelector);

    public static IncrementalValuesProvider<TResult> Pair<
        TSource,
        TKey,
        TElement,
        TResult
    >(
        this IncrementalValuesProvider<TSource> source,
        IncrementalValueProvider<Grouping<TKey, TElement>> group,
        Func<TSource, TKey> keySelector,
        Func<TSource, TElement, TResult> selector
    )
        where TElement : IEquatable<TElement>
        where TKey : IEquatable<TKey>
    {
        return source
            .Combine(group)
            .SelectMany((pair, token) =>
                pair.Right
                    .GetGroupOrEmpty(keySelector(pair.Left))
                    .Select(
                        element => selector(pair.Left, element)
                    )
            );
    }

    public static IncrementalValuesProvider<(TSource Left, ImmutableEquatableArray<TElement> Right)> Combine<
        TSource,
        TKey,
        TElement
    >(
        this IncrementalValuesProvider<TSource> source,
        IncrementalValueProvider<Grouping<TKey, TElement>> group,
        Func<TSource, TKey> keySelector
    )
        where TElement : IEquatable<TElement>
        where TKey : IEquatable<TKey>
        => Combine(source, group, keySelector, (a, b) => (a, b));

    public static IncrementalValuesProvider<TResult> Combine<
        TSource,
        TKey,
        TElement,
        TResult
    >(
        this IncrementalValuesProvider<TSource> source,
        IncrementalValueProvider<Grouping<TKey, TElement>> group,
        Func<TSource, TKey> keySelector,
        Func<TSource, ImmutableEquatableArray<TElement>, TResult> resultSelector
    )
        where TElement : IEquatable<TElement>
        where TKey : IEquatable<TKey>
        => Combine(source, group, keySelector, (source, elements, _) => resultSelector(source, elements));

    public static IncrementalValuesProvider<TResult> Combine<
        TSource,
        TKey,
        TElement,
        TResult
    >(
        this IncrementalValuesProvider<TSource> source,
        IncrementalValueProvider<Grouping<TKey, TElement>> group,
        Func<TSource, TKey> keySelector,
        Func<TSource, ImmutableEquatableArray<TElement>, Grouping<TKey, TElement>, TResult> resultSelector
    )
        where TElement : IEquatable<TElement>
        where TKey : IEquatable<TKey>
    {
        return source
            .Combine(group)
            .Select((tuple, _) =>
                resultSelector(
                    tuple.Left,
                    tuple.Right.TryGetGroup(keySelector(tuple.Left), out var group)
                        ? group
                        : ImmutableEquatableArray<TElement>.Empty,
                    tuple.Right
                )
            );
    }
}