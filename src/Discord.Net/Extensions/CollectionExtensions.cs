using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Discord.Extensions
{
    internal static class CollectionExtensions
    {
        public static IReadOnlyCollection<TValue> ToReadOnlyCollection<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> source)
            => new ConcurrentDictionaryWrapper<TValue, KeyValuePair<TKey, TValue>>(source, source.Select(x => x.Value));
        public static IReadOnlyCollection<TValue> ToReadOnlyCollection<TValue, TSource>(this IEnumerable<TValue> query, IReadOnlyCollection<TSource> source)
            => new ConcurrentDictionaryWrapper<TValue, TSource>(source, query);
    }
    
    internal struct ConcurrentDictionaryWrapper<TValue, TSource> : IReadOnlyCollection<TValue>
    {
        private readonly IReadOnlyCollection<TSource> _source;
        private readonly IEnumerable<TValue> _query;

        //It's okay that this count is affected by race conditions - we're wrapping a concurrent collection and that's to be expected
        public int Count => _source.Count;
        
        public ConcurrentDictionaryWrapper(IReadOnlyCollection<TSource> source, IEnumerable<TValue> query)
        {
            _source = source;
            _query = query;
        }

        public IEnumerator<TValue> GetEnumerator() => _query.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _query.GetEnumerator();
    }
}
