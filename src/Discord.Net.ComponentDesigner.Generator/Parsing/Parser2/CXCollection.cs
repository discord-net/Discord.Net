using Microsoft.CodeAnalysis.Text;
using System.Collections;
using System.Collections.Generic;

namespace Discord.ComponentDesignerGenerator.Parser;

public sealed class CXCollection<T> : CXNode, IReadOnlyList<T>
    where T : CXNode
{
    public T this[int index] => _items[index];

    public int Count => _items.Count;

    private readonly List<T> _items;

    public CXCollection(params IEnumerable<T> items)
    {
        Slot(_items = [..items]);
    }

    public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) _items).GetEnumerator();
}
