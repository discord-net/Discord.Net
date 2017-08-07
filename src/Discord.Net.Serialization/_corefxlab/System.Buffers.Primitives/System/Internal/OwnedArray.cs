// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Buffers.Internal
{
    internal class OwnedArray<T> : OwnedBuffer<T>
    {
        T[] _array;
        int _referenceCount;

        public OwnedArray(int length)
        {
            _array = new T[length];
        }

        public OwnedArray(T[] array)
        {
            if (array == null) BufferPrimitivesThrowHelper.ThrowArgumentNullException(nameof(array)); 
            _array = array;
        }

        public static implicit operator OwnedArray<T>(T[] array) => new OwnedArray<T>(array);

        public override int Length => _array.Length;

        public override Span<T> AsSpan(int index, int length)
        {
            if (IsDisposed) BufferPrimitivesThrowHelper.ThrowObjectDisposedException(nameof(OwnedArray<T>));
            return new Span<T>(_array, index, length);
        }

        public override Span<T> AsSpan()
        {
            if (IsDisposed) BufferPrimitivesThrowHelper.ThrowObjectDisposedException(nameof(OwnedArray<T>));
            return new Span<T>(_array, 0, _array.Length);
        }

        public override BufferHandle Pin(int index = 0)
        {
            unsafe
            {
                Retain();
                var handle = GCHandle.Alloc(_array, GCHandleType.Pinned);
                var pointer = Add((void*)handle.AddrOfPinnedObject(), index);
                return new BufferHandle(this, pointer, handle);
            }
        }

        protected internal override bool TryGetArray(out ArraySegment<T> arraySegment)
        {
            if (IsDisposed) BufferPrimitivesThrowHelper.ThrowObjectDisposedException(nameof(OwnedArray<T>));
            arraySegment = new ArraySegment<T>(_array);
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            _array = null;
        }

        public override void Retain()
        {
            if (IsDisposed) BufferPrimitivesThrowHelper.ThrowObjectDisposedException(nameof(OwnedArray<T>));
            Interlocked.Increment(ref _referenceCount);
        }

        public override void Release()
        {
            if (!IsRetained) BufferPrimitivesThrowHelper.ThrowInvalidOperationException();
            if (Interlocked.Decrement(ref _referenceCount) == 0)
            {
                OnNoReferences();
            }
        }

        protected virtual void OnNoReferences()
        {
        }

        public override bool IsRetained => _referenceCount > 0;

        public override bool IsDisposed => _array == null;
    }
}
