using System;
using System.Buffers;

namespace Discord.Gateway
{
    internal struct RentedArray<T> : IDisposable
    {
        public T[] Array;

        private readonly ArrayPool<T> _pool;

        private bool _disposed;

        public RentedArray(T[] array, ArrayPool<T> pool)
        {
            Array = array;
            _pool = pool;
        }

        public readonly Span<T> AsSpan()
            => Array.AsSpan();

        public readonly Memory<T> AsMemory()
            => Array.AsMemory();

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                _pool.Return(Array);
            }
        }
    }
}

