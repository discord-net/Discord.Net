using Microsoft.CodeAnalysis.Text;
using System.Collections;
using System.Collections.Generic;

namespace Discord.CX.Parser;

public interface ICXCollection : ICXNode
{
    int Count { get; }
    ICXNode this[int index] { get; }
}

public sealed class CXCollection<T> :
    CXNode,
    ICXCollection,
    IReadOnlyList<T>
    where T : class, ICXNode
{
    public T this[int index] => _items[index];

    public int Count => _items.Count;

    private readonly List<T> _items;

    public CXCollection(params IEnumerable<T> items)
    {
        Slot((IEnumerable<ICXNode>)(_items = [..items]));
    }


    public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) _items).GetEnumerator();
    ICXNode ICXCollection.this[int index] => this[index];
    int ICXCollection.Count => Count;
}
