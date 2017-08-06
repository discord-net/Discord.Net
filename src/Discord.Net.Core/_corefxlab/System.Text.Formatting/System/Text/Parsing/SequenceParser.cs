// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Buffers;
using System.Collections.Sequences;

namespace System.Text.Parsing
{
    public static class TextSequenceExtensions
    {
        const int StackBufferSize = 128;

        public static bool TryParseUInt64<T>(this T bufferSequence, out ulong value, out int consumed) where T : ISequence<ReadOnlyBuffer<byte>>
        {
            value = default;
            consumed = default;
            Position position = Position.First;

            // Fetch the first segment
            ReadOnlyBuffer<byte> first;
            if (!bufferSequence.TryGet(ref position, out first)) {
                return false;
            }

            // Attempt to parse the first segment. If it works (and it should in most cases), then return success.
            bool parsed = PrimitiveParser.InvariantUtf8.TryParseUInt64(first.Span, out value, out consumed);
            if (parsed && consumed < first.Length) {
                return true;
            }

            // Apparently the we need data from the second segment to succesfully parse, and so fetch the second segment.
            ReadOnlyBuffer<byte> second;
            if (!bufferSequence.TryGet(ref position, out second)) {
                // if there is no second segment and the first parsed succesfully, return the result of the parsing.
                if (parsed) return true;
                return false;
            }

            // Combine the first, the second, and potentially more segments into a stack allocated buffer
            ReadOnlySpan<byte> combinedSpan;
            unsafe
            {
                if (first.Length < StackBufferSize) {
                    var data = stackalloc byte[StackBufferSize];
                    var destination = new Span<byte>(data, StackBufferSize);

                    first.CopyTo(destination);
                    var free = destination.Slice(first.Length);

                    if (second.Length > free.Length) second = second.Slice(0, free.Length);
                    second.CopyTo(free);
                    free = free.Slice(second.Length);

                    ReadOnlyBuffer<byte> next;
                    while (free.Length > 0) {
                        if (bufferSequence.TryGet(ref position, out next)) {
                            if (next.Length > free.Length) next = next.Slice(0, free.Length);
                            next.CopyTo(free);
                            free = free.Slice(next.Length);
                        }
                        else {
                            break;
                        }
                    }

                    combinedSpan = destination.Slice(0, StackBufferSize - free.Length);

                    // if the stack allocated buffer parsed succesfully (and for uint it should always do), then return success. 
                    if (PrimitiveParser.InvariantUtf8.TryParseUInt64(combinedSpan, out value, out consumed)) {
                        if(consumed < combinedSpan.Length || combinedSpan.Length < StackBufferSize) {
                            return true;
                        }
                    }
                }
            }

            // for invariant culture, we should never reach this point, as invariant uint text is never longer than 127 bytes. 
            // I left this code here, as we will need it for custom cultures and possibly when we shrink the stack allocated buffer.
            combinedSpan = bufferSequence.ToSpan();
            if (!PrimitiveParser.InvariantUtf8.TryParseUInt64(first.Span, out value, out consumed)) {
                return false;
            }
            return true;
        }

        public static bool TryParseUInt32<T>(this T bufferSequence, out uint value, out int consumed) where T : ISequence<ReadOnlyBuffer<byte>>
        {
            value = default;
            consumed = default;
            Position position = Position.First;

            // Fetch the first segment
            ReadOnlyBuffer<byte> first;
            if (!bufferSequence.TryGet(ref position, out first)) {
                return false;
            }

            // Attempt to parse the first segment. If it works (and it should in most cases), then return success.
            bool parsed = PrimitiveParser.InvariantUtf8.TryParseUInt32(first.Span, out value, out consumed);
            if (parsed && consumed < first.Length) {
                return true;
            }

            // Apparently the we need data from the second segment to succesfully parse, and so fetch the second segment.
            ReadOnlyBuffer<byte> second;
            if (!bufferSequence.TryGet(ref position, out second)) {
                // if there is no second segment and the first parsed succesfully, return the result of the parsing.
                if (parsed) return true;
                return false;
            }

            // Combine the first, the second, and potentially more segments into a stack allocated buffer
            ReadOnlySpan<byte> combinedSpan;
            unsafe
            {
                if (first.Length < StackBufferSize) {
                    var data = stackalloc byte[StackBufferSize];
                    var destination = new Span<byte>(data, StackBufferSize);

                    first.CopyTo(destination);
                    var free = destination.Slice(first.Length);

                    if (second.Length > free.Length) second = second.Slice(0, free.Length);
                    second.CopyTo(free);
                    free = free.Slice(second.Length);

                    ReadOnlyBuffer<byte> next;
                    while (free.Length > 0) {
                        if (bufferSequence.TryGet(ref position, out next)) {
                            if (next.Length > free.Length) next = next.Slice(0, free.Length);
                            next.CopyTo(free);
                            free = free.Slice(next.Length);
                        }
                        else {
                            break;
                        }
                    }

                    combinedSpan = destination.Slice(0, StackBufferSize - free.Length);

                    // if the stack allocated buffer parsed succesfully (and for uint it should always do), then return success. 
                    if (PrimitiveParser.InvariantUtf8.TryParseUInt32(combinedSpan, out value, out consumed)) {
                        if(consumed < combinedSpan.Length || combinedSpan.Length < StackBufferSize) {
                            return true;
                        }
                    }
                }
            }

            // for invariant culture, we should never reach this point, as invariant uint text is never longer than 127 bytes. 
            // I left this code here, as we will need it for custom cultures and possibly when we shrink the stack allocated buffer.
            combinedSpan = bufferSequence.ToSpan();
            if (!PrimitiveParser.InvariantUtf8.TryParseUInt32(combinedSpan, out value, out consumed)) {
                return false;
            }
            return true;
        }
    }
}