using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord
{
    internal class Page<T> : IReadOnlyCollection<T>
    {
        private readonly IReadOnlyCollection<T> _items;

        public Page(PageInfo info, IEnumerable<T> source)
        {
            Index = info.Page;
            _items = source.ToImmutableArray();
        }

        public int Index { get; }

        int IReadOnlyCollection<T>.Count => _items.Count;
        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => _items.GetEnumerator();
    }
}
