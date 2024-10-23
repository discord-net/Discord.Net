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

    private readonly ImmutableArray<TElement> _elements;

    public Grouping(ImmutableArray<TElement> elements, Func<TElement, TKey> keySelector)
    {
        _elements = elements;

        _groups = elements
            .GroupBy(keySelector)
            .ToDictionary(
                x => x.Key,
                x => new ImmutableEquatableArray<TElement>(x)
            );
    }

    public bool TryGetGroup(TKey key, out ImmutableEquatableArray<TElement> elements)
        => _groups.TryGetValue(key, out elements);

    public ImmutableEquatableArray<TElement> GetGroupOrEmpty(TKey key)
        => TryGetGroup(key, out var elements) 
            ? elements 
            : ImmutableEquatableArray<TElement>.Empty;

    public bool Equals(Grouping<TKey, TElement> other)
        => _elements.SequenceEqual(other._elements);

    public override int GetHashCode()
        => HashCode.OfEach(_elements);
}

public static partial class ProviderExtensions
{
    public static IncrementalValueProvider<Grouping<TKey, TElement>> GroupBy<TKey, TElement>(
        this IncrementalValuesProvider<TElement> source,
        Func<TElement, TKey> keySelector
    )
        where TElement : IEquatable<TElement>
        where TKey : IEquatable<TKey>
    {
        return source.Collect().Select((items, _) =>
            new Grouping<TKey, TElement>(items, keySelector)
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