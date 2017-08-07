// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


namespace System.Collections.Sequences
{
    // a List<T> like type designed to be embeded in other types
    public struct ResizableArray<T>
    {
        private T[] _array;
        private int _count;

        public ResizableArray(int capacity)
        {
            _array = new T[capacity];
            _count = 0;
        }

        public ResizableArray(T[] array, int count = 0)
        {
            _array = array;
            _count = count;
        }

        public T[] Items
        {
            get { return _array; }
            set { _array = value; }
        }
        public int Count
        {
            get { return _count; }
            set { _count = value; }
        }

        public int Capacity => _array.Length;

        public T this[int index]
        {
            get {
                if (index > _count - 1) throw new IndexOutOfRangeException();
                return _array[index];
            }
            set {
                if (index > _count - 1) throw new IndexOutOfRangeException();
                _array[index] = value;
            }
        }

        public void Add(T item)
        {
            if (_array.Length == _count) {
                Resize();
            }
            _array[_count++] = item;
        }

        public void AddAll(T[] items)
        {
            if (items.Length > _array.Length - _count) {
                Resize(items.Length + _count);
            }
            items.CopyTo(_array, _count);
            _count += items.Length;
        }

        public void AddAll(ReadOnlySpan<T> items)
        {
            if (items.Length > _array.Length - _count) {
                Resize(items.Length + _count);
            }
            items.CopyTo(new Span<T>(_array, _count));
            _count += items.Length;
        }

        public void Clear()
        {
            _count = 0;
        }

        public T[] Resize(int newSize = -1)
        {
            var oldArray = _array;
            if (newSize == -1) {
                if(_array == null || _array.Length == 0) {
                    newSize = 4;
                }
                else {
                    newSize = _array.Length << 1;
                }
            }           

            var newArray = new T[newSize];
            new Span<T>(_array, 0, _count).CopyTo(newArray);
            _array = newArray;
            return oldArray;
        }

        public T[] Resize(T[] newArray)
        {
            if (newArray.Length < _count) throw new ArgumentOutOfRangeException(nameof(newArray));
            var oldArray = _array;
            Array.Copy(_array, 0, newArray, 0, _count);
            _array = newArray;
            return oldArray;
        }

        public bool TryGet(ref Position position, out T item, bool advance = true)
        {
            if (position.IntegerPosition < _count) {
                item = _array[position.IntegerPosition];
                if (advance) { position.IntegerPosition++; }
                return true;
            }

            item = default;
            position = Position.AfterLast;
            return false;
        }

        public ArraySegment<T> Full => new ArraySegment<T>(_array, 0, _count);
        public ArraySegment<T> Free => new ArraySegment<T>(_array, _count, _array.Length - _count);
    }
}
