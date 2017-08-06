// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace System.Runtime
{
    internal static class Contract
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Requires(bool condition)
        {
            if (!condition)
            {
                BufferPrimitivesThrowHelper.ThrowArgumentException();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RequiresNotNull<T>(ExceptionArgument argument, T obj) where T : class 
        {
            if (obj == null)
            {
                BufferPrimitivesThrowHelper.ThrowArgumentNullException(argument);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void RequiresNotNull(ExceptionArgument argument, void* ptr)
        {
            if (ptr == null)
            {
                BufferPrimitivesThrowHelper.ThrowArgumentNullException(argument);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void RequiresSameReference(void* ptr0, void* ptr1)
        {
            if (ptr0 != ptr1)
            {
                BufferPrimitivesThrowHelper.ThrowArgumentException(ExceptionArgument.pointer);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RequiresNonNegative(int n)
        {
            if (n < 0)
            {
                BufferPrimitivesThrowHelper.ThrowArgumentOutOfRangeException();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RequiresInRange(int start, uint length)
        {
            if ((uint)start >= length)
            {
                BufferPrimitivesThrowHelper.ThrowArgumentOutOfRangeException();
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RequiresInRange(uint start, uint length)
        {
            if (start >= length)
            {
                BufferPrimitivesThrowHelper.ThrowArgumentOutOfRangeException();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RequiresInInclusiveRange(int start, uint length)
        {
            if ((uint)start > length)
            {
                BufferPrimitivesThrowHelper.ThrowArgumentOutOfRangeException();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe void RequiresOneNotNull<T>(T[] array, void* pointer)
        {
            if (array == null && pointer == null)
            {
                BufferPrimitivesThrowHelper.ThrowArgumentException();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RequiresInInclusiveRange( uint start, uint length)
        {
            if (start > length)
            {
                BufferPrimitivesThrowHelper.ThrowArgumentOutOfRangeException();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RequiresInInclusiveRange(int start, int length, uint existingLength)
        {
            if ((uint)start > existingLength
                || length < 0
                || (uint)(start + length) > existingLength)
            {
                BufferPrimitivesThrowHelper.ThrowArgumentOutOfRangeException();
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RequiresInInclusiveRange(uint start, uint length, uint existingLength)
        {
            if (start > existingLength
                || (start + length) > existingLength)
            {
                BufferPrimitivesThrowHelper.ThrowArgumentOutOfRangeException();
            }
        }
    }
}

