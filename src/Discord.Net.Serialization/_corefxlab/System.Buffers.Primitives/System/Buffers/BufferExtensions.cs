// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Buffers
{
    public static class BufferExtensions
    {
        public static bool SequenceEqual<T>(this Buffer<T> first, Buffer<T> second) where T : struct, IEquatable<T>
        {
            return first.Span.SequenceEqual(second.Span);
        }

        public static bool SequenceEqual<T>(this ReadOnlyBuffer<T> first, ReadOnlyBuffer<T> second) where T : struct, IEquatable<T>
        {
            return first.Span.SequenceEqual(second.Span);
        }
    }
}
