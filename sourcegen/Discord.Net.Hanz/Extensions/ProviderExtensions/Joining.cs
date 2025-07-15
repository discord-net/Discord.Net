using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz;

public abstract record MaybePair<T, U>
{
    public sealed record Left(T Value) : MaybePair<T, U>;
    public sealed record Right(U Value) : MaybePair<T, U>;
    public sealed record Both(T LeftValue, U RightValue) : MaybePair<T, U>;

    public Optional<U> MaybeRight
        => this is Right(var v) ? v.Some() : default;
    
    public Optional<T> MaybeLeft
        => this is Left(var v) ? v.Some() : default;
}

public static class Joining
{
    private static IEnumerable<TResult> JoinCollections<TLeft, TRight, TKey, TResult>(
        ImmutableArray<TLeft> left,
        ImmutableArray<TRight> right,
        Func<TLeft, TKey> leftKeySelector,
        Func<TRight, TKey> rightKeySelector,
        Func<TKey, MaybePair<TLeft, TRight>, TResult> selector,
        CancellationToken token = default)
    {
        var leftKeys = left.Select(leftKeySelector).ToArray();
        var rightKeys = right.Select(rightKeySelector).ToArray();
        var leftArr = left.ToArray();
        var rightArr = right.ToArray();
        
        Array.Sort(leftKeys, leftArr);
        Array.Sort(rightKeys, rightArr);
        
        var attenuation = leftKeys.Length + rightKeys.Length;

        for (var i = 0; i != attenuation; ++i)
        {
            var isRight = i >= leftKeys.Length;
            
            var key = isRight ? rightKeys[i] : leftKeys[i];
            
            var otherIndex = Array.IndexOf(
                isRight ? rightKeys : leftKeys,
                key
            );

            if (otherIndex == -1)
                yield return selector(
                    key,
                    isRight
                        ? new MaybePair<TLeft, TRight>.Right(rightArr[i])
                        : new MaybePair<TLeft, TRight>.Left(leftArr[i])
                );
            else if(!isRight) // both clauses are handled in the first half
                yield return selector(
                    key,
                    new MaybePair<TLeft, TRight>.Both(
                        isRight ? leftArr[otherIndex] : leftArr[i],
                        isRight ? rightArr[i] : rightArr[otherIndex]
                    )
                );
            
            token.ThrowIfCancellationRequested();
        }
    }
    
    public static IncrementalValuesProvider<TResult> Join<TValue, TOther, TResult>(
        this IncrementalValuesProvider<TValue> source,
        IncrementalValuesProvider<TOther> other,
        Func<TValue, TOther> keySelector,
        Func<TOther, Optional<TValue>, TResult> resultSelector
    ) => Join(
        source.Collect(), 
        other.Collect(), 
        keySelector,
        x => x,
        (key, maybe) => resultSelector(key, maybe.MaybeLeft)
    );
    
    public static IncrementalValuesProvider<TResult> Join<TValue, TOther, TResult>(
        this IncrementalValuesProvider<TValue> source,
        IncrementalValuesProvider<TOther> other,
        Func<TOther, TValue> keySelector,
        Func<TValue, Optional<TOther>, TResult> resultSelector
    ) => Join(
        source.Collect(), 
        other.Collect(), 
        x => x,
        keySelector,
        (key, maybe) => resultSelector(key, maybe.MaybeRight)
        );

    public static IncrementalValuesProvider<TResult> Join<TValue, TOther, TKey, TResult>(
        this IncrementalValuesProvider<TValue> source,
        IncrementalValuesProvider<TOther> other,
        Func<TValue, TKey> keySelector,
        Func<TOther, TKey> otherKeySelector,
        Func<TKey, MaybePair<TValue, TOther>, TResult> resultSelector
    ) => Join(source.Collect(), other.Collect(), keySelector, otherKeySelector, resultSelector);
    
    public static IncrementalValuesProvider<TResult> Join<TValue, TOther, TKey, TResult>(
        this IncrementalValueProvider<ImmutableArray<TValue>> source,
        IncrementalValueProvider<ImmutableArray<TOther>> other,
        Func<TValue, TKey> keySelector,
        Func<TOther, TKey> otherKeySelector,
        Func<TKey, MaybePair<TValue, TOther>, TResult> resultSelector
    )
    {
        return source
            .Combine(other)
            .SelectMany((pair, token) =>
                JoinCollections(
                    pair.Left,
                    pair.Right,
                    keySelector,
                    otherKeySelector,
                    resultSelector,
                    token
                )
            );
    } 
}