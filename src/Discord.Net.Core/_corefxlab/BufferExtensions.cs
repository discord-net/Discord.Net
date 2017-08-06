// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Sequences;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Buffers
{
    public static class ExperimentalBufferExtensions
    {
        public static ReadOnlySpan<byte> ToSpan<T>(this T bufferSequence) where T : ISequence<ReadOnlyBuffer<byte>>
        {
            Position position = Position.First;
            ReadOnlyBuffer<byte> buffer;
            ResizableArray<byte> array = new ResizableArray<byte>(1024); 
            while (bufferSequence.TryGet(ref position, out buffer))
            {
                array.AddAll(buffer.Span);
            }
            array.Resize(array.Count);
            return array.Items.Slice(0, array.Count);
        }

        /// <summary>
        /// Creates a new slice over the portion of the target array segment.
        /// </summary>
        /// <param name="arraySegment">The target array segment.</param>
        /// </exception>
        public static Span<T> Slice<T>(this ArraySegment<T> arraySegment)
        {
            return new Span<T>(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
        }

        /// <summary>
        /// Creates a new slice over the portion of the target array.
        /// </summary>
        /// <param name="array">The target array.</param>
        /// <exception cref="System.ArgumentException">
        /// Thrown if the 'array' parameter is null.
        /// </exception>
        public static Span<T> Slice<T>(this T[] array)
        {
            return new Span<T>(array);
        }

        /// <summary>
        /// Creates a new slice over the portion of the target array beginning
        /// at 'start' index.
        /// </summary>
        /// <param name="array">The target array.</param>
        /// <param name="start">The index at which to begin the slice.</param>
        /// <exception cref="System.ArgumentException">
        /// Thrown if the 'array' parameter is null.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when the specified start index is not in range (&lt;0 or &gt;&eq;length).
        /// </exception>
        public static Span<T> Slice<T>(this T[] array, int start)
        {
            return new Span<T>(array, start);
        }

        /// <summary>
        /// Creates a new slice over the portion of the target array beginning
        /// at 'start' index and with 'length' items.
        /// </summary>
        /// <param name="array">The target array.</param>
        /// <param name="start">The index at which to begin the slice.</param>
        /// <param name="length">The number of items in the new slice.</param>
        /// <exception cref="System.ArgumentException">
        /// Thrown if the 'array' parameter is null.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when the specified start or end index is not in range (&lt;0 or &gt;&eq;length).
        /// </exception>
        public static Span<T> Slice<T>(this T[] array, int start, int length)
        {
            return new Span<T>(array, start, length);
        }

        /// <summary>
        /// Creates a new readonly span over the portion of the target string.
        /// </summary>
        /// <param name="text">The target string.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe ReadOnlySpan<char> Slice(this string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            int textLength = text.Length;

            if (textLength == 0) return ReadOnlySpan<char>.Empty;

            fixed (char* charPointer = text)
            {
                return ReadOnlySpan<char>.DangerousCreate(text, ref *charPointer, textLength);
            }
        }

        /// <summary>
        /// Creates a new readonly span over the portion of the target string, beginning at 'start'.
        /// </summary>
        /// <param name="text">The target string.</param>
        /// <param name="start">The index at which to begin this slice.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when the specified <paramref name="start"/> index is not in range (&lt;0 or &gt;Length).
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe ReadOnlySpan<char> Slice(this string text, int start)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            int textLength = text.Length;

            if ((uint) start > (uint) textLength)
                throw new ArgumentOutOfRangeException(nameof(start));

            if (textLength - start == 0) return ReadOnlySpan<char>.Empty;

            fixed (char* charPointer = text)
            {
                return ReadOnlySpan<char>.DangerousCreate(text, ref *(charPointer + start), textLength - start);
            }
        }

        /// <summary>
        /// Creates a new readonly span over the portion of the target string, beginning at <paramref name="start"/>, of given <paramref name="length"/>.
        /// </summary>
        /// <param name="text">The target string.</param>
        /// <param name="start">The index at which to begin this slice.</param>
        /// <param name="length">The number of items in the span.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when the specified <paramref name="start"/> or end index is not in range (&lt;0 or &gt;=Length).
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe ReadOnlySpan<char> Slice(this string text, int start, int length)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            int textLength = text.Length;

            if ((uint)start > (uint)textLength || (uint)length > (uint)(textLength - start))
                throw new ArgumentOutOfRangeException(nameof(start));

            if (length == 0) return ReadOnlySpan<char>.Empty;

            fixed (char* charPointer = text)
            {
                return ReadOnlySpan<char>.DangerousCreate(text, ref *(charPointer + start), length);
            }
        }
    }
}
