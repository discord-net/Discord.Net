// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Text.Encoders
{
    public static class Utf16
    {
        #region UTF-8 Conversions

        /// <summary>
        /// Calculates the byte count needed to encode the UTF-16 bytes from the specified UTF-8 sequence.
        ///
        /// This method will consume as many of the input bytes as possible.
        /// </summary>
        /// <param name="source">A span containing a sequence of UTF-8 bytes.</param>
        /// <param name="bytesNeeded">On exit, contains the number of bytes required for encoding from the <paramref name="source"/>.</param>
        /// <returns>A <see cref="TransformationStatus"/> value representing the expected state of the conversion.</returns>
        public static TransformationStatus FromUtf8Length(ReadOnlySpan<byte> source, out int bytesNeeded)
            => Utf8.ToUtf16Length(source, out bytesNeeded);

        /// <summary>
        /// Converts a span containing a sequence of UTF-8 bytes into UTF-16 bytes.
        ///
        /// This method will consume as many of the input bytes as possible.
        ///
        /// On successful exit, the entire input was consumed and encoded successfully. In this case, <paramref name="bytesConsumed"/> will be
        /// equal to the length of the <paramref name="source"/> and <paramref name="bytesWritten"/> will equal the total number of bytes written to
        /// the <paramref name="destination"/>.
        /// </summary>
        /// <param name="source">A span containing a sequence of UTF-8 bytes.</param>
        /// <param name="destination">A span to write the UTF-16 bytes into.</param>
        /// <param name="bytesConsumed">On exit, contains the number of bytes that were consumed from the <paramref name="source"/>.</param>
        /// <param name="bytesWritten">On exit, contains the number of bytes written to <paramref name="destination"/></param>
        /// <returns>A <see cref="TransformationStatus"/> value representing the state of the conversion.</returns>
        public static TransformationStatus FromUtf8(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesConsumed, out int bytesWritten)
            => Utf8.ToUtf16(source, destination, out bytesConsumed, out bytesWritten);

        /// <summary>
        /// Calculates the byte count needed to encode the UTF-8 bytes from the specified UTF-16 sequence.
        ///
        /// This method will consume as many of the input bytes as possible.
        /// </summary>
        /// <param name="source">A span containing a sequence of UTF-16 bytes.</param>
        /// <param name="bytesNeeded">On exit, contains the number of bytes required for encoding from the <paramref name="source"/>.</param>
        /// <returns>A <see cref="TransformationStatus"/> value representing the expected state of the conversion.</returns>
        public static TransformationStatus ToUtf8Length(ReadOnlySpan<byte> source, out int bytesNeeded)
        {
            bytesNeeded = 0;

            // try? because Convert.ConvertToUtf32 can throw
            // if the high/low surrogates aren't valid; no point
            // running all the tests twice per code-point
            try
            {
                ref char utf16 = ref Unsafe.As<byte, char>(ref source.DangerousGetPinnableReference());
                int utf16Length = source.Length >> 1; // byte => char count

                for (int i = 0; i < utf16Length; i++)
                {
                    var ch = Unsafe.Add(ref utf16, i);

                    if ((ushort)ch <= 0x7f) // Fast path for ASCII
                        bytesNeeded++;
                    else if (!char.IsSurrogate(ch))
                        bytesNeeded += EncodingHelper.GetUtf8EncodedBytes((uint)ch);
                    else
                    {
                        if (++i >= utf16Length)
                            return TransformationStatus.NeedMoreSourceData;

                        uint codePoint = (uint)char.ConvertToUtf32(ch, Unsafe.Add(ref utf16, i));
                        bytesNeeded += EncodingHelper.GetUtf8EncodedBytes(codePoint);
                    }
                }

                if ((utf16Length << 1) != source.Length)
                    return TransformationStatus.NeedMoreSourceData;

                return TransformationStatus.Done;
            }
            catch (ArgumentOutOfRangeException)
            {
                return TransformationStatus.InvalidData;
            }
        }

        /// <summary>
        /// Converts a span containing a sequence of UTF-16 bytes into UTF-8 bytes.
        ///
        /// This method will consume as many of the input bytes as possible.
        ///
        /// On successful exit, the entire input was consumed and encoded successfully. In this case, <paramref name="bytesConsumed"/> will be
        /// equal to the length of the <paramref name="source"/> and <paramref name="bytesWritten"/> will equal the total number of bytes written to
        /// the <paramref name="destination"/>.
        /// </summary>
        /// <param name="source">A span containing a sequence of UTF-16 bytes.</param>
        /// <param name="destination">A span to write the UTF-8 bytes into.</param>
        /// <param name="bytesConsumed">On exit, contains the number of bytes that were consumed from the <paramref name="source"/>.</param>
        /// <param name="bytesWritten">On exit, contains the number of bytes written to <paramref name="destination"/></param>
        /// <returns>A <see cref="TransformationStatus"/> value representing the state of the conversion.</returns>
        public unsafe static TransformationStatus ToUtf8(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesConsumed, out int bytesWritten)
        {
            //
            //
            // KEEP THIS IMPLEMENTATION IN SYNC WITH https://github.com/dotnet/corert/blob/master/src/System.Private.CoreLib/src/System/Text/UTF8Encoding.cs
            //
            //
            fixed (byte* chars = &source.DangerousGetPinnableReference())
            fixed (byte* bytes = &destination.DangerousGetPinnableReference())
            {
                char* pSrc = (char*)chars;
                byte* pTarget = bytes;

                char* pEnd = (char*)(chars + source.Length);
                byte* pAllocatedBufferEnd = pTarget + destination.Length;

                // assume that JIT will enregister pSrc, pTarget and ch

                // Entering the fast encoding loop incurs some overhead that does not get amortized for small
                // number of characters, and the slow encoding loop typically ends up running for the last few
                // characters anyway since the fast encoding loop needs 5 characters on input at least.
                // Thus don't use the fast decoding loop at all if we don't have enough characters. The threashold
                // was choosen based on performance testing.
                // Note that if we don't have enough bytes, pStop will prevent us from entering the fast loop.
                while (pEnd - pSrc > 13)
                {
                    // we need at least 1 byte per character, but Convert might allow us to convert
                    // only part of the input, so try as much as we can.  Reduce charCount if necessary
                    int available = Math.Min(EncodingHelper.PtrDiff(pEnd, pSrc), EncodingHelper.PtrDiff(pAllocatedBufferEnd, pTarget));

                    // FASTLOOP:
                    // - optimistic range checks
                    // - fallbacks to the slow loop for all special cases, exception throwing, etc.

                    // To compute the upper bound, assume that all characters are ASCII characters at this point,
                    //  the boundary will be decreased for every non-ASCII character we encounter
                    // Also, we need 5 chars reserve for the unrolled ansi decoding loop and for decoding of surrogates
                    // If there aren't enough bytes for the output, then pStop will be <= pSrc and will bypass the loop.
                    char* pStop = pSrc + available - 5;
                    if (pSrc >= pStop)
                        break;

                    do
                    {
                        int ch = *pSrc;
                        pSrc++;

                        if (ch > 0x7F)
                        {
                            goto LongCode;
                        }
                        *pTarget = (byte)ch;
                        pTarget++;

                        // get pSrc aligned
                        if ((unchecked((int)pSrc) & 0x2) != 0)
                        {
                            ch = *pSrc;
                            pSrc++;
                            if (ch > 0x7F)
                            {
                                goto LongCode;
                            }
                            *pTarget = (byte)ch;
                            pTarget++;
                        }

                        // Run 4 characters at a time!
                        while (pSrc < pStop)
                        {
                            ch = *(int*)pSrc;
                            int chc = *(int*)(pSrc + 2);
                            if (((ch | chc) & unchecked((int)0xFF80FF80)) != 0)
                            {
                                goto LongCodeWithMask;
                            }

                            // Unfortunately, this is endianess sensitive
#if BIGENDIAN
                            *pTarget = (byte)(ch >> 16);
                            *(pTarget + 1) = (byte)ch;
                            pSrc += 4;
                            *(pTarget + 2) = (byte)(chc >> 16);
                            *(pTarget + 3) = (byte)chc;
                            pTarget += 4;
#else // BIGENDIAN
                            *pTarget = (byte)ch;
                            *(pTarget + 1) = (byte)(ch >> 16);
                            pSrc += 4;
                            *(pTarget + 2) = (byte)chc;
                            *(pTarget + 3) = (byte)(chc >> 16);
                            pTarget += 4;
#endif // BIGENDIAN
                        }
                        continue;

                    LongCodeWithMask:
#if BIGENDIAN
                        // be careful about the sign extension
                        ch = (int)(((uint)ch) >> 16);
#else // BIGENDIAN
                        ch = (char)ch;
#endif // BIGENDIAN
                        pSrc++;

                        if (ch > 0x7F)
                        {
                            goto LongCode;
                        }
                        *pTarget = (byte)ch;
                        pTarget++;
                        continue;

                    LongCode:
                        // use separate helper variables for slow and fast loop so that the jit optimizations
                        // won't get confused about the variable lifetimes
                        int chd;
                        if (ch <= 0x7FF)
                        {
                            // 2 byte encoding
                            chd = unchecked((sbyte)0xC0) | (ch >> 6);
                        }
                        else
                        {
                            // if (!IsLowSurrogate(ch) && !IsHighSurrogate(ch))
                            if (!EncodingHelper.InRange(ch, EncodingHelper.HighSurrogateStart, EncodingHelper.LowSurrogateEnd))
                            {
                                // 3 byte encoding
                                chd = unchecked((sbyte)0xE0) | (ch >> 12);
                            }
                            else
                            {
                                // 4 byte encoding - high surrogate + low surrogate
                                // if (!IsHighSurrogate(ch))
                                if (ch > EncodingHelper.HighSurrogateEnd)
                                {
                                    // low without high -> bad
                                    goto InvalidData;
                                }

                                chd = *pSrc;

                                // if (!IsLowSurrogate(chd)) {
                                if (!EncodingHelper.InRange(chd, EncodingHelper.LowSurrogateStart, EncodingHelper.LowSurrogateEnd))
                                {
                                    // high not followed by low -> bad
                                    goto InvalidData;
                                }

                                pSrc++;

                                ch = chd + (ch << 10) +
                                    (0x10000
                                    - EncodingHelper.LowSurrogateStart
                                    - (EncodingHelper.HighSurrogateStart << 10));

                                *pTarget = (byte)(unchecked((sbyte)0xF0) | (ch >> 18));
                                // pStop - this byte is compensated by the second surrogate character
                                // 2 input chars require 4 output bytes.  2 have been anticipated already
                                // and 2 more will be accounted for by the 2 pStop-- calls below.
                                pTarget++;

                                chd = unchecked((sbyte)0x80) | (ch >> 12) & 0x3F;
                            }
                            *pTarget = (byte)chd;
                            pStop--;                    // 3 byte sequence for 1 char, so need pStop-- and the one below too.
                            pTarget++;

                            chd = unchecked((sbyte)0x80) | (ch >> 6) & 0x3F;
                        }
                        *pTarget = (byte)chd;
                        pStop--;                        // 2 byte sequence for 1 char so need pStop--.

                        *(pTarget + 1) = (byte)(unchecked((sbyte)0x80) | ch & 0x3F);
                        // pStop - this byte is already included

                        pTarget += 2;
                    }
                    while (pSrc < pStop);

                    Debug.Assert(pTarget <= pAllocatedBufferEnd, "[UTF8Encoding.GetBytes]pTarget <= pAllocatedBufferEnd");
                }

                while (pSrc < pEnd)
                {
                    // SLOWLOOP: does all range checks, handles all special cases, but it is slow

                    // read next char. The JIT optimization seems to be getting confused when
                    // compiling "ch = *pSrc++;", so rather use "ch = *pSrc; pSrc++;" instead
                    int ch = *pSrc;
                    pSrc++;

                    if (ch <= 0x7F)
                    {
                        if (pAllocatedBufferEnd - pTarget <= 0)
                            goto DestinationFull;

                        *pTarget = (byte)ch;
                        pTarget++;
                        continue;
                    }

                    int chd;
                    if (ch <= 0x7FF)
                    {
                        if (pAllocatedBufferEnd - pTarget <= 1)
                            goto DestinationFull;

                        // 2 byte encoding
                        chd = unchecked((sbyte)0xC0) | (ch >> 6);
                    }
                    else
                    {
                        // if (!IsLowSurrogate(ch) && !IsHighSurrogate(ch))
                        if (!EncodingHelper.InRange(ch, EncodingHelper.HighSurrogateStart, EncodingHelper.LowSurrogateEnd))
                        {
                            if (pAllocatedBufferEnd - pTarget <= 2)
                                goto DestinationFull;

                            // 3 byte encoding
                            chd = unchecked((sbyte)0xE0) | (ch >> 12);
                        }
                        else
                        {
                            if (pAllocatedBufferEnd - pTarget <= 3)
                                goto DestinationFull;

                            // 4 byte encoding - high surrogate + low surrogate
                            // if (!IsHighSurrogate(ch))
                            if (ch > EncodingHelper.HighSurrogateEnd)
                            {
                                // low without high -> bad
                                goto InvalidData;
                            }

                            if (pSrc >= pEnd)
                                goto NeedMoreData;

                            chd = *pSrc;

                            // if (!IsLowSurrogate(chd)) {
                            if (!EncodingHelper.InRange(chd, EncodingHelper.LowSurrogateStart, EncodingHelper.LowSurrogateEnd))
                            {
                                // high not followed by low -> bad
                                goto InvalidData;
                            }

                            pSrc++;

                            ch = chd + (ch << 10) +
                                (0x10000
                                - EncodingHelper.LowSurrogateStart
                                - (EncodingHelper.HighSurrogateStart << 10));

                            *pTarget = (byte)(unchecked((sbyte)0xF0) | (ch >> 18));
                            pTarget++;

                            chd = unchecked((sbyte)0x80) | (ch >> 12) & 0x3F;
                        }
                        *pTarget = (byte)chd;
                        pTarget++;

                        chd = unchecked((sbyte)0x80) | (ch >> 6) & 0x3F;
                    }

                    *pTarget = (byte)chd;
                    *(pTarget + 1) = (byte)(unchecked((sbyte)0x80) | ch & 0x3F);

                    pTarget += 2;
                }

                bytesConsumed = (int)((byte*)pSrc - chars);
                bytesWritten = (int)(pTarget - bytes);
                return TransformationStatus.Done;

            InvalidData:
                bytesConsumed = (int)((byte*)(pSrc - 1) - chars);
                bytesWritten = (int)(pTarget - bytes);
                return TransformationStatus.InvalidData;

            DestinationFull:
                bytesConsumed = (int)((byte*)(pSrc - 1) - chars);
                bytesWritten = (int)(pTarget - bytes);
                return TransformationStatus.DestinationTooSmall;

            NeedMoreData:
                bytesConsumed = (int)((byte*)(pSrc - 1) - chars);
                bytesWritten = (int)(pTarget - bytes);
                return TransformationStatus.NeedMoreSourceData;
            }
        }

        #endregion UTF-8 Conversions

        #region UTF-32 Conversions

        /// <summary>
        /// Calculates the byte count needed to encode the UTF-16 bytes from the specified UTF-32 sequence.
        ///
        /// This method will consume as many of the input bytes as possible.
        /// </summary>
        /// <param name="source">A span containing a sequence of UTF-32 bytes.</param>
        /// <param name="bytesNeeded">On exit, contains the number of bytes required for encoding from the <paramref name="source"/>.</param>
        /// <returns>A <see cref="TransformationStatus"/> value representing the expected state of the conversion.</returns>
        public static TransformationStatus FromUtf32Length(ReadOnlySpan<byte> source, out int bytesNeeded)
            => Utf32.ToUtf16Length(source, out bytesNeeded);

        /// <summary>
        /// Converts a span containing a sequence of UTF-32 bytes into UTF-16 bytes.
        ///
        /// This method will consume as many of the input bytes as possible.
        ///
        /// On successful exit, the entire input was consumed and encoded successfully. In this case, <paramref name="bytesConsumed"/> will be
        /// equal to the length of the <paramref name="source"/> and <paramref name="bytesWritten"/> will equal the total number of bytes written to
        /// the <paramref name="destination"/>.
        /// </summary>
        /// <param name="source">A span containing a sequence of UTF-32 bytes.</param>
        /// <param name="destination">A span to write the UTF-16 bytes into.</param>
        /// <param name="bytesConsumed">On exit, contains the number of bytes that were consumed from the <paramref name="source"/>.</param>
        /// <param name="bytesWritten">On exit, contains the number of bytes written to <paramref name="destination"/></param>
        /// <returns>A <see cref="TransformationStatus"/> value representing the state of the conversion.</returns>
        public static TransformationStatus FromUtf32(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesConsumed, out int bytesWritten)
            => Utf32.ToUtf16(source, destination, out bytesConsumed, out bytesWritten);

        /// <summary>
        /// Calculates the byte count needed to encode the UTF-32 bytes from the specified UTF-16 sequence.
        ///
        /// This method will consume as many of the input bytes as possible.
        /// </summary>
        /// <param name="source">A span containing a sequence of UTF-16 bytes.</param>
        /// <param name="bytesNeeded">On exit, contains the number of bytes required for encoding from the <paramref name="source"/>.</param>
        /// <returns>A <see cref="TransformationStatus"/> value representing the expected state of the conversion.</returns>
        public static TransformationStatus ToUtf32Length(ReadOnlySpan<byte> source, out int bytesNeeded)
        {
            bytesNeeded = 0;

            ref byte src = ref source.DangerousGetPinnableReference();
            int srcLength = source.Length;
            int srcIndex = 0;

            while (srcLength - srcIndex >= sizeof(char))
            {
                uint codePoint = Unsafe.As<byte, char>(ref Unsafe.Add(ref src, srcIndex));
                if (EncodingHelper.IsSurrogate(codePoint))
                {
                    if (!EncodingHelper.IsHighSurrogate(codePoint))
                        return TransformationStatus.InvalidData;

                    if (srcLength - srcIndex < sizeof(char) * 2)
                        return TransformationStatus.NeedMoreSourceData;

                    uint lowSurrogate = Unsafe.As<byte, char>(ref Unsafe.Add(ref src, srcIndex + 2));
                    if (!EncodingHelper.IsLowSurrogate(lowSurrogate))
                        return TransformationStatus.InvalidData;

                    srcIndex += 2;
                }

                srcIndex += 2;
                bytesNeeded += 4;
            }

            return srcIndex < srcLength ? TransformationStatus.NeedMoreSourceData : TransformationStatus.Done;
        }

        /// <summary>
        /// Converts a span containing a sequence of UTF-16 bytes into UTF-32 bytes.
        ///
        /// This method will consume as many of the input bytes as possible.
        ///
        /// On successful exit, the entire input was consumed and encoded successfully. In this case, <paramref name="bytesConsumed"/> will be
        /// equal to the length of the <paramref name="source"/> and <paramref name="bytesWritten"/> will equal the total number of bytes written to
        /// the <paramref name="destination"/>.
        /// </summary>
        /// <param name="source">A span containing a sequence of UTF-16 bytes.</param>
        /// <param name="destination">A span to write the UTF-32 bytes into.</param>
        /// <param name="bytesConsumed">On exit, contains the number of bytes that were consumed from the <paramref name="source"/>.</param>
        /// <param name="bytesWritten">On exit, contains the number of bytes written to <paramref name="destination"/></param>
        /// <returns>A <see cref="TransformationStatus"/> value representing the state of the conversion.</returns>
        public static TransformationStatus ToUtf32(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesConsumed, out int bytesWritten)
        {
            bytesConsumed = 0;
            bytesWritten = 0;

            ref byte src = ref source.DangerousGetPinnableReference();
            int srcLength = source.Length;

            ref byte dst = ref destination.DangerousGetPinnableReference();
            int dstLength = destination.Length;

            while (srcLength - bytesConsumed >= sizeof(char))
            {
                if (dstLength - bytesWritten < sizeof(uint))
                    return TransformationStatus.DestinationTooSmall;

                uint codePoint = Unsafe.As<byte, char>(ref Unsafe.Add(ref src, bytesConsumed));
                if (EncodingHelper.IsSurrogate(codePoint))
                {
                    if (!EncodingHelper.IsHighSurrogate(codePoint))
                        return TransformationStatus.InvalidData;

                    if (srcLength - bytesConsumed < sizeof(char) * 2)
                        return TransformationStatus.NeedMoreSourceData;

                    uint lowSurrogate = Unsafe.As<byte, char>(ref Unsafe.Add(ref src, bytesConsumed + 2));
                    if (!EncodingHelper.IsLowSurrogate(lowSurrogate))
                        return TransformationStatus.InvalidData;

                    codePoint -= EncodingHelper.HighSurrogateStart;
                    lowSurrogate -= EncodingHelper.LowSurrogateStart;
                    codePoint = ((codePoint << 10) | lowSurrogate) + 0x010000u;
                    bytesConsumed += 2;
                }

                Unsafe.As<byte, uint>(ref Unsafe.Add(ref dst, bytesWritten)) = codePoint;
                bytesConsumed += 2;
                bytesWritten += 4;
            }

            return bytesConsumed < srcLength ? TransformationStatus.NeedMoreSourceData : TransformationStatus.Done;
        }

        #endregion UTF-32 Conversions
    }
}
