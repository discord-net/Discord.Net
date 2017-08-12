using System;
using System.Diagnostics;
using System.Threading;

namespace Discord.Serialization
{
    //TODO: Replace pools in audio with this
    public sealed class Pool<T>
        where T : class
    {
        private const int DefaultMaxElementsPerBucket = 50;

        private readonly T[] _buffer;
        private readonly Func<T> _createFunc;

        private SpinLock _lock;
        private int _index;

        public Pool(Func<T> createFunc)
            : this(createFunc, DefaultMaxElementsPerBucket) { }
        public Pool(Func<T> createFunc, int maxElementsPerBucket)
        {
            _createFunc = createFunc;
            _lock = new SpinLock(Debugger.IsAttached);
            _buffer = new T[maxElementsPerBucket];
        }

        public T Rent()
        {
            T result = null;
            bool lockTaken = false;
            try
            {
                _lock.Enter(ref lockTaken);

                if (_index < _buffer.Length)
                {
                    result = _buffer[_index];
                    _buffer[_index++] = null;
                }
            }
            finally
            {
                if (lockTaken)
                    _lock.Exit(false);
            }
            
            if (result == null)
                result = _createFunc();

            return result;
        }

        public void Return(T obj)
        {
            bool lockTaken = false;
            try
            {
                _lock.Enter(ref lockTaken);

                if (_index != 0)
                    _buffer[--_index] = obj;
            }
            finally
            {
                if (lockTaken)
                    _lock.Exit(false);
            }
        }
    }
}
