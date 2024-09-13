using System.Collections.Frozen;
using System.Collections.ObjectModel;

namespace Discord;

#if NET6_0

public static class ReadOnlyExtensions
{
    public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        where TKey : notnull
        => new(dictionary);

    public static IReadOnlyCollection<T> AsReadOnly<T>(this IList<T> list)
        => new ReadOnlyCollection<T>(list);

    public static IReadOnlySet<T> AsReadOnly<T>(this ISet<T> set)
        => set.ToFrozenSet();
}

#endif
