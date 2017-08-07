// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
    [DebuggerTypeProxy(typeof(BufferDebuggerView<>))]
    public struct Buffer<T>
    {
        // The highest order bit of _index is used to discern whether _arrayOrOwnedBuffer is an array or an owned buffer
        // if (_index >> 31) == 1, object _arrayOrOwnedBuffer is an OwnedBuffer<T>
        // else, object _arrayOrOwnedBuffer is a T[]
        readonly object _arrayOrOwnedBuffer;
        readonly int _index;
        readonly int _length;

        private const int bitMask = 0x7FFFFFFF;

        internal Buffer(OwnedBuffer<T> owner, int index, int length)
        {
            _arrayOrOwnedBuffer = owner;
            _index = index | (1 << 31); // Before using _index, check if _index < 0, then 'and' it with bitMask
            _length = length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Buffer(T[] array)
        {
            if (array == null)
                BufferPrimitivesThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
            if (default(T) == null && array.GetType() != typeof(T[]))
                BufferPrimitivesThrowHelper.ThrowArrayTypeMismatchException(typeof(T));

            _arrayOrOwnedBuffer = array;
            _index = 0;
            _length = array.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Buffer(T[] array, int start)
        {
            if (array == null)
                BufferPrimitivesThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
            if (default(T) == null && array.GetType() != typeof(T[]))
                BufferPrimitivesThrowHelper.ThrowArrayTypeMismatchException(typeof(T));

            int arrayLength = array.Length;
            if ((uint)start > (uint)arrayLength)
                BufferPrimitivesThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);

            _arrayOrOwnedBuffer = array;
            _index = start;
            _length = arrayLength - start;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Buffer(T[] array, int start, int length)
        {
            if (array == null)
                BufferPrimitivesThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
            if (default(T) == null && array.GetType() != typeof(T[]))
                BufferPrimitivesThrowHelper.ThrowArrayTypeMismatchException(typeof(T));
            if ((uint)start > (uint)array.Length || (uint)length > (uint)(array.Length - start))
                BufferPrimitivesThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);

            _arrayOrOwnedBuffer = array;
            _index = start;
            _length = length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ReadOnlyBuffer<T>(Buffer<T> buffer)
        {
            // There is no need to 'and' _index by the bit mask here 
            // since the constructor will set the highest order bit again anyway
            if (buffer._index < 0)
                return new ReadOnlyBuffer<T>(Unsafe.As<OwnedBuffer<T>>(buffer._arrayOrOwnedBuffer), buffer._index, buffer._length);
            return new ReadOnlyBuffer<T>(Unsafe.As<T[]>(buffer._arrayOrOwnedBuffer), buffer._index, buffer._length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Buffer<T>(T[] array)
        {
            return new Buffer<T>(array, 0, array.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Buffer<T>(ArraySegment<T> arraySegment)
        {
            return new Buffer<T>(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
        }

        public static Buffer<T> Empty { get; } = OwnedBuffer<T>.EmptyArray;

        public int Length => _length;

        public bool IsEmpty => Length == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Buffer<T> Slice(int start)
        {
            if ((uint)start > (uint)_length)
                BufferPrimitivesThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);

            // There is no need to and _index by the bit mask here 
            // since the constructor will set the highest order bit again anyway
            if (_index < 0)
                return new Buffer<T>(Unsafe.As<OwnedBuffer<T>>(_arrayOrOwnedBuffer), _index + start, _length - start);
            return new Buffer<T>(Unsafe.As<T[]>(_arrayOrOwnedBuffer), _index + start, _length - start);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Buffer<T> Slice(int start, int length)
        {
            if ((uint)start > (uint)_length || (uint)length > (uint)(_length - start))
                BufferPrimitivesThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);

            // There is no need to 'and' _index by the bit mask here 
            // since the constructor will set the highest order bit again anyway
            if (_index < 0)
                return new Buffer<T>(Unsafe.As<OwnedBuffer<T>>(_arrayOrOwnedBuffer), _index + start, length);
            return new Buffer<T>(Unsafe.As<T[]>(_arrayOrOwnedBuffer), _index + start, length);
        }

        public Span<T> Span
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (_index < 0)
                    return Unsafe.As<OwnedBuffer<T>>(_arrayOrOwnedBuffer).AsSpan(_index & bitMask, _length);
                return new Span<T>(Unsafe.As<T[]>(_arrayOrOwnedBuffer), _index, _length);
            }
        }

        public BufferHandle Retain(bool pin = false)
        {
            BufferHandle bufferHandle;
            if (pin)
            {
                if (_index < 0)
                {
                    bufferHandle = Unsafe.As<OwnedBuffer<T>>(_arrayOrOwnedBuffer).Pin(_index & bitMask);
                }
                else
                {
                    var handle = GCHandle.Alloc(Unsafe.As<T[]>(_arrayOrOwnedBuffer), GCHandleType.Pinned);
                    unsafe
                    {
                        var pointer = OwnedBuffer<T>.Add((void*)handle.AddrOfPinnedObject(), _index);
                        bufferHandle = new BufferHandle(null, pointer, handle);
                    }
                }
            }
            else
            {
                if (_index < 0)
                {
                    Unsafe.As<OwnedBuffer<T>>(_arrayOrOwnedBuffer).Retain();
                    bufferHandle = new BufferHandle(Unsafe.As<OwnedBuffer<T>>(_arrayOrOwnedBuffer));
                }
                else
                {
                    bufferHandle = new BufferHandle(null);
                }
            }
            return bufferHandle;
        }

        public bool TryGetArray(out ArraySegment<T> arraySegment)
        {
            if (_index < 0)
            {
                if (Unsafe.As<OwnedBuffer<T>>(_arrayOrOwnedBuffer).TryGetArray(out var segment))
                {
                    arraySegment = new ArraySegment<T>(segment.Array, segment.Offset + (_index & bitMask), _length);
                    return true;
                }
            }
            else
            {
                arraySegment = new ArraySegment<T>(Unsafe.As<T[]>(_arrayOrOwnedBuffer), _index, _length);
                return true;
            }

            arraySegment = default;
            return false;
        }

        public T[] ToArray() => Span.ToArray();

        public void CopyTo(Span<T> span) => Span.CopyTo(span);

        public void CopyTo(Buffer<T> buffer) => Span.CopyTo(buffer.Span);

        public bool TryCopyTo(Span<T> span) => Span.TryCopyTo(span);

        public bool TryCopyTo(Buffer<T> buffer) => Span.TryCopyTo(buffer.Span);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            if (obj is ReadOnlyBuffer<T> readOnlyBuffer)
            {
                return readOnlyBuffer.Equals(this);
            }
            else if (obj is Buffer<T> buffer)
            {
                return Equals(buffer);
            }
            else
            {
                return false;
            }
        }

        public bool Equals(Buffer<T> other)
        {
            return
                _arrayOrOwnedBuffer == other._arrayOrOwnedBuffer &&
                (_index & bitMask) == (other._index & bitMask) &&
                _length == other._length;
        }

        [EditorBrowsable( EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            return HashingHelper.CombineHashCodes(_arrayOrOwnedBuffer.GetHashCode(), (_index & bitMask).GetHashCode(), _length.GetHashCode());
        }
    }
}
