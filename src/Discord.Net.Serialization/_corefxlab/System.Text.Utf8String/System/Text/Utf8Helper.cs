// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace System.Text
{
    static class Utf8Helper
    {
        #region Constants

        // TODO: Make this immutable and let them be strong typed
        // http://unicode.org/cldr/utility/list-unicodeset.jsp?a=\p{whitespace}&g=&i=
        private static readonly uint[] SortedWhitespaceCodePoints = new uint[]
        {
            0x0009, 0x000A, 0x000B, 0x000C, 0x000D,
            0x0020,
            0x0085,
            0x00A0,
            0x1680,
            0x2000, 0x2001, 0x2002, 0x2003, 0x2004, 0x2005, 0x2006,
            0x2007,
            0x2008, 0x2009, 0x200A,
            0x2028, 0x2029,
            0x202F,
            0x205F,
            0x3000
        };

        // To get this to compile with dotnet cli, we need to temporarily un-binary the magic values
        private const byte b0000_0111U = 0x07; //7
        private const byte b0000_1111U = 0x0F; //15
        private const byte b0001_1111U = 0x1F; //31
        private const byte b0011_1111U = 0x3F; //63
        private const byte b0111_1111U = 0x7F; //127
        private const byte b1000_0000U = 0x80; //128
        private const byte b1100_0000U = 0xC0; //192
        private const byte b1110_0000U = 0xE0; //224
        private const byte b1111_0000U = 0xF0; //240
        private const byte b1111_1000U = 0xF8; //248

        private const byte NonFirstByteInCodePointValue = 0x80;
        private const byte NonFirstByteInCodePointMask = 0xC0;

        public const int MaxCodeUnitsPerCodePoint = 4;

        #endregion Constants

        public static bool TryDecodeCodePoint(ReadOnlySpan<byte> utf8, int index, out uint codePoint, out int bytesConsumed)
        {
            if (index >= utf8.Length)
            {
                codePoint = default;
                bytesConsumed = 0;
                return false;
            }

            var first = utf8[index];

            bytesConsumed = GetEncodedBytes(first);
            if (bytesConsumed == 0 || utf8.Length - index < bytesConsumed)
            {
                bytesConsumed = 0;
                codePoint = default;
                return false;
            }

            switch (bytesConsumed)
            {
                case 1:
                    codePoint = first;
                    break;

                case 2:
                    codePoint = (uint)(first & b0001_1111U);
                    break;

                case 3:
                    codePoint = (uint)(first & b0000_1111U);
                    break;

                case 4:
                    codePoint = (uint)(first & b0000_0111U);
                    break;

                default:
                    codePoint = default;
                    bytesConsumed = 0;
                    return false;
            }

            for (var i = 1; i < bytesConsumed; i++)
            {
                uint current = utf8[index + i];
                if ((current & b1100_0000U) != b1000_0000U)
                {
                    bytesConsumed = 0;
                    codePoint = default;
                    return false;
                }

                codePoint = (codePoint << 6) | (b0011_1111U & current);
            }

            return true;
        }

        private static int GetEncodedBytes(byte b)
        {
            if ((b & b1000_0000U) == 0)
                return 1;

            if ((b & b1110_0000U) == b1100_0000U)
                return 2;

            if ((b & b1111_0000U) == b1110_0000U)
                return 3;

            if ((b & b1111_1000U) == b1111_0000U)
                return 4;

            return 0;
        }

        public static int GetNumberOfEncodedBytes(uint codePoint)
        {
            if (codePoint <= 0x7F)
                return 1;

            if (codePoint <= 0x7FF)
                return 2;

            if (codePoint <= 0xFFFF)
                return 3;

            if (codePoint <= 0x10FFFF)
                return 4;

            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsFirstCodeUnitInEncodedCodePoint(byte codeUnit)
        {
            return (codeUnit & NonFirstByteInCodePointMask) != NonFirstByteInCodePointValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryFindEncodedCodePointBytesCountGoingBackwards(ReadOnlySpan<byte> buffer, out int encodedBytes)
        {
            encodedBytes = 1;
            ReadOnlySpan<byte> it = buffer;
            // TODO: Should we have something like: Span<byte>.(Slice from the back)
            for (; encodedBytes <= MaxCodeUnitsPerCodePoint; encodedBytes++, it = it.Slice(0, it.Length - 1))
            {
                if (it.Length == 0)
                {
                    encodedBytes = default;
                    return false;
                }

                // TODO: Should we have Span<byte>.Last?
                if (IsFirstCodeUnitInEncodedCodePoint(it[it.Length - 1]))
                {
                    // output: encodedBytes
                    return true;
                }
            }

            // Invalid unicode character or stream prematurely ended (which is still invalid character in that stream)
            encodedBytes = default;
            return false;
        }

        // TODO: Name TBD
        // TODO: optimize?
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryDecodeCodePointBackwards(ReadOnlySpan<byte> buffer, out uint codePoint, out int encodedBytes)
        {
            if (TryFindEncodedCodePointBytesCountGoingBackwards(buffer, out encodedBytes))
            {
                int realEncodedBytes;
                // TODO: Inline decoding, as the invalid surrogate check can be done faster
                bool ret = TryDecodeCodePoint(buffer, buffer.Length - encodedBytes, out codePoint, out realEncodedBytes);
                if (ret && encodedBytes != realEncodedBytes)
                {
                    // invalid surrogate character
                    // we know the character length by iterating on surrogate characters from the end
                    // but the first byte of the character has also encoded length
                    // seems like the lengths don't match
                    codePoint = default;
                    return false;
                }

                return true;
            }

            codePoint = default;
            encodedBytes = default;
            return false;
        }

        // TODO: Should we rewrite this to not use char.ConvertToUtf32 or is it fast enough?
        public static bool TryDecodeCodePointFromString(string s, int index, out uint codePoint, out int encodedChars)
        {
            if (index < 0 || index >= s.Length)
            {
                codePoint = default;
                encodedChars = 0;
                return false;
            }

            if (index == s.Length - 1 && char.IsSurrogate(s[index]))
            {
                codePoint = default;
                encodedChars = 0;
                return false;
            }

            encodedChars = char.IsHighSurrogate(s[index]) ? 2 : 1;
            codePoint = unchecked((uint)char.ConvertToUtf32(s, index));

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWhitespace(uint codePoint)
        {
            return Array.BinarySearch<uint>(SortedWhitespaceCodePoints, codePoint) >= 0;
        }
    }
}
