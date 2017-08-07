// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace System.Text
{
    internal static class EncodingHelper
    {
        #region Constants

        private const uint FirstNotSupportedCodePoint = 0x110000; // 17 * 2^16
        private const uint BasicMultilingualPlaneEndMarker = 0x10000;

        // TODO: Make this immutable and let them be strong typed
        // http://unicode.org/cldr/utility/list-unicodeset.jsp?a=\p{whitespace}&g=&i=
        private static readonly uint[] SortedWhitespaceCodePoints = new uint[25]
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

        public const char HighSurrogateStart = '\ud800';
        public const char HighSurrogateEnd = '\udbff';
        public const char LowSurrogateStart = '\udc00';
        public const char LowSurrogateEnd = '\udfff';

        // To get this to compile with dotnet cli, we need to temporarily un-binary the magic values
        public const byte b0000_0111U = 0x07; //7
        public const byte b0000_1111U = 0x0F; //15
        public const byte b0001_1111U = 0x1F; //31
        public const byte b0011_1111U = 0x3F; //63
        public const byte b0111_1111U = 0x7F; //127
        public const byte b1000_0000U = 0x80; //128
        public const byte b1100_0000U = 0xC0; //192
        public const byte b1110_0000U = 0xE0; //224
        public const byte b1111_0000U = 0xF0; //240
        public const byte b1111_1000U = 0xF8; //248

        #endregion Constants

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWhitespace(uint codePoint)
        {
            return Array.BinarySearch<uint>(SortedWhitespaceCodePoints, codePoint) >= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSupportedCodePoint(uint codePoint)
        {
            if (codePoint >= FirstNotSupportedCodePoint)
                return false;
            if (codePoint >= HighSurrogateStart && codePoint <= LowSurrogateEnd)
                return false;
            if (codePoint >= 0xFDD0 && codePoint <= 0xFDEF)
                return false;
            if (codePoint == 0xFFFE || codePoint == 0xFFFF)
                return false;

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBmp(uint codePoint)
        {
            return codePoint < BasicMultilingualPlaneEndMarker;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static int PtrDiff(char* a, char* b)
        {
            return (int)(((uint)((byte*)a - (byte*)b)) >> 1);
        }

        // byte* flavor just for parity
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static int PtrDiff(byte* a, byte* b)
        {
            return (int)(a - b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InRange(int ch, int start, int end)
        {
            return (uint)(ch - start) <= (uint)(end - start);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetUtf8DecodedBytes(byte b)
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

        internal static int GetUtf8EncodedBytes(uint codePoint)
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
        public static bool IsSurrogate(uint codePoint)
        {
            return codePoint >= HighSurrogateStart && codePoint <= LowSurrogateEnd;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLowSurrogate(uint codePoint)
        {
            return codePoint >= LowSurrogateStart && codePoint <= LowSurrogateEnd;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsHighSurrogate(uint codePoint)
        {
            return codePoint >= HighSurrogateStart && codePoint <= HighSurrogateEnd;
        }
    }
}
