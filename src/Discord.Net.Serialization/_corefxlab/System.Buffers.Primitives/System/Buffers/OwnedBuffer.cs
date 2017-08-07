// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.Runtime.CompilerServices;

namespace System.Buffers
{
    public abstract class OwnedBuffer<T> : IDisposable, IRetainable
    {
        protected OwnedBuffer() { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator OwnedBuffer<T>(T[] array)
        {
            return new Internal.OwnedArray<T>(array);
        }

        public abstract int Length { get; }

        public abstract Span<T> AsSpan(int index, int length);

        public virtual Span<T> AsSpan()
        {
            if (IsDisposed) BufferPrimitivesThrowHelper.ThrowObjectDisposedException(nameof(OwnedBuffer<T>));
            return AsSpan(0, Length);
        }

        public Buffer<T> Buffer
        {
            get {
                if (IsDisposed) BufferPrimitivesThrowHelper.ThrowObjectDisposedException(nameof(OwnedBuffer<T>));
                return new Buffer<T>(this, 0, Length);
            }
        }

        public ReadOnlyBuffer<T> ReadOnlyBuffer
        {
            get {
                if (IsDisposed) BufferPrimitivesThrowHelper.ThrowObjectDisposedException(nameof(OwnedBuffer<T>));
                return new ReadOnlyBuffer<T>(this, 0, Length);
            }
        }

        public abstract BufferHandle Pin(int index = 0);

        protected internal abstract bool TryGetArray(out ArraySegment<T> arraySegment);

        #region Lifetime Management
        public abstract bool IsDisposed { get; }

        public void Dispose()
        {
            if (IsRetained) throw new InvalidOperationException("outstanding references detected.");
            Dispose(true);
        }

        protected abstract void Dispose(bool disposing);

        public abstract bool IsRetained { get; }

        public abstract void Retain();

        public abstract void Release();
        #endregion

        protected internal static unsafe void* Add(void* pointer, int offset)
        {
            return (byte*)pointer + ((ulong)Unsafe.SizeOf<T>() * (ulong)offset);
        }

        internal static readonly T[] EmptyArray = new T[0];
    }
}
