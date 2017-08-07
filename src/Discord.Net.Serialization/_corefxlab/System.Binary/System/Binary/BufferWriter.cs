// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime;
using System.Runtime.CompilerServices;

namespace System.Binary
{
    /// <summary>
    /// Writes endian-specific primitives into spans.
    /// </summary>
    /// <remarks>
    /// Use these helpers when you need to write specific endinaness.
    /// </remarks>
    public static class BufferWriter
    {
        /// <summary>
        /// Writes a structure of type T to a span of bytes.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteBigEndian<[Primitive]T>(this Span<byte> span, T value) where T : struct
            => span.Write(BitConverter.IsLittleEndian ? UnsafeUtilities.Reverse(value) : value);

        /// <summary>
        /// Writes a structure of type T to a span of bytes.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteLittleEndian<[Primitive]T>(this Span<byte> span, T value) where T : struct
            => span.Write(BitConverter.IsLittleEndian ? value : UnsafeUtilities.Reverse(value));



        /// <summary>
        /// Writes a structure of type T into a slice of bytes.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write<[Primitive]T>(this Span<byte> slice, T value)
            where T : struct
        {
            if ((uint)Unsafe.SizeOf<T>() > (uint)slice.Length)
            {
                throw new ArgumentOutOfRangeException();
            }
            Unsafe.WriteUnaligned<T>(ref slice.DangerousGetPinnableReference(), value);
        }

        /// <summary>
        /// Writes a structure of type T into a slice of bytes.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryWrite<[Primitive]T>(this Span<byte> slice, T value)
            where T : struct
        {
            if (Unsafe.SizeOf<T>() > (uint)slice.Length)
            {
                return false;
            }
            Unsafe.WriteUnaligned<T>(ref slice.DangerousGetPinnableReference(), value);
            return true;
        }
    }
}