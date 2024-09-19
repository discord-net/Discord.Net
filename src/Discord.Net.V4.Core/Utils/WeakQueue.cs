using System.Diagnostics.CodeAnalysis;
using System.Runtime;

namespace Discord;

internal sealed class WeakQueue<T> where T : class
{
    private sealed class Entry
    {
        public Entry? Next { get; set; }
        public Entry? Previous { get; set; }

        private DependentHandle _handle;
        private readonly WeakQueue<T> _owner;

        public Entry(T value, WeakQueue<T> owner)
        {
            _owner = owner;
            _handle = new(value, this);
        }

        ~Entry()
        {
            Remove();
        }

        public void Remove()
        {
            lock (_owner._syncRoot)
            {
                if (this == _owner._head)
                    _owner._head = Next;
                else if (this == _owner._tail)
                    _owner._tail = Previous;
            
                if (Next is not null)
                    Next.Previous = Previous;

                if (Previous is not null)
                    Previous.Next = Next;

                _owner._count--;
                Dealloc();
            }
        }

        public void Dealloc()
        {
            GC.SuppressFinalize(this);
            _handle.Dispose();
            Next = null;
            Previous = null;
        }

        public bool TryGetValue([MaybeNullWhen(false)] out T value)
        {
            if (_handle.IsAllocated)
            {
                value = (T)_handle.Target!;
                return true;
            }

            GC.KeepAlive(_handle.Target);
            
            value = null;
            return false;
        }
    }

    [MemberNotNullWhen(false, nameof(_head), nameof(_tail))]
    public bool IsEmpty => _head is null || _tail is null;

    private int _count;
    
    private Entry? _head;
    private Entry? _tail;

    private readonly object _syncRoot = new();
    
    public void Enqueue(T value)
    {
        lock (_syncRoot)
        {
            var node = new Entry(value, this);

            if (IsEmpty)
            {
                _head = node;
                _tail = node;

                return;
            }

            _tail.Next = node;
        }
    }

    public T Dequeue()
    {
        lock (_syncRoot)
        {
            while (!IsEmpty)
            {
                if (_head.TryGetValue(out var value))
                    return value;
                
                _head.Remove();
            }
            
            throw new InvalidOperationException("The queue is empty");
        }
    }

    public bool TryDequeue([MaybeNullWhen(false)] out T value)
    {
        lock (_syncRoot)
        {
            while (!IsEmpty)
            {
                if (_head.TryGetValue(out value))
                    return true;
                
                _head.Remove();
            }
        }

        value = null;
        return false;
    }

    public void Clear()
    {
        lock (_syncRoot)
        {
            var node = _head;
            
            while (node is not null)
            {
                var next = node.Next;
                node.Dealloc();
                node = next;
            }
        }
    }
}