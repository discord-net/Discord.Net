using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Discord
{
    internal static class CollectionExtensions
    {
        //public static IReadOnlyCollection<TValue> ToReadOnlyCollection<TValue>(this IReadOnlyCollection<TValue> source)
        //    => new CollectionWrapper<TValue>(source, () => source.Count);
        public static IReadOnlyCollection<TValue> ToReadOnlyCollection<TValue>(this ICollection<TValue> source)
            => new CollectionWrapper<TValue>(source, () => source.Count);
        //public static IReadOnlyCollection<TValue> ToReadOnlyCollection<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> source)
        //    => new CollectionWrapper<TValue>(source.Select(x => x.Value), () => source.Count);
        public static IReadOnlyCollection<TValue> ToReadOnlyCollection<TKey, TValue>(this IDictionary<TKey, TValue> source)
            => new CollectionWrapper<TValue>(source.Select(x => x.Value), () => source.Count);
        public static IReadOnlyCollection<TValue> ToReadOnlyCollection<TValue, TSource>(this IEnumerable<TValue> query, IReadOnlyCollection<TSource> source)
            => new CollectionWrapper<TValue>(query, () => source.Count);
        public static IReadOnlyCollection<TValue> ToReadOnlyCollection<TValue>(this IEnumerable<TValue> query, Func<int> countFunc)
            => new CollectionWrapper<TValue>(query, countFunc);
    }

    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    internal struct CollectionWrapper<TValue> : IReadOnlyCollection<TValue>
    {
        private readonly IEnumerable<TValue> _query;
        private readonly Func<int> _countFunc;

        //It's okay that this count is affected by race conditions - we're wrapping a concurrent collection and that's to be expected
        public int Count => _countFunc();

        public CollectionWrapper(IEnumerable<TValue> query, Func<int> countFunc)
        {
            _query = query;
            _countFunc = countFunc;
        }

        private string DebuggerDisplay => $"Count = {Count}";

        public IEnumerator<TValue> GetEnumerator() => _query.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _query.GetEnumerator();
    }
}
