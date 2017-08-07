// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace System.Text
{
    internal static class InvariantUtf16GuidFormatter
    {
        #region Constants

        private const int GuidChars = 32;

        private const char OpenBrace = '{';
        private const char CloseBrace = '}';

        private const char OpenParen = '(';
        private const char CloseParen = ')';

        private const char Dash = '-';

        #endregion Constants

        public static unsafe bool TryFormat(this Guid value, Span<byte> buffer, out int bytesWritten, ParsedFormat format)
        {
            bool dash = format.Symbol != 'N';
            bool bookEnds = (format.Symbol == 'B') || (format.Symbol == 'P');

            bytesWritten = (GuidChars + (dash ? 4 : 0) + (bookEnds ? 2 : 0)) * sizeof(char);
            if (buffer.Length < bytesWritten)
            {
                bytesWritten = 0;
                return false;
            }

            Span<char> dst = buffer.NonPortableCast<byte, char>();
            ref char utf16Bytes = ref dst.DangerousGetPinnableReference();
            byte* bytes = (byte*)&value;
            int idx = 0;

            if (bookEnds && format.Symbol == 'B')
                Unsafe.Add(ref utf16Bytes, idx++) = OpenBrace;
            else if (bookEnds && format.Symbol == (byte)'P')
                Unsafe.Add(ref utf16Bytes, idx++) = OpenParen;

            FormattingHelpers.WriteHexByte(bytes[3], ref utf16Bytes, idx);
            FormattingHelpers.WriteHexByte(bytes[2], ref utf16Bytes, idx + 2);
            FormattingHelpers.WriteHexByte(bytes[1], ref utf16Bytes, idx + 4);
            FormattingHelpers.WriteHexByte(bytes[0], ref utf16Bytes, idx + 6);
            idx += 8;

            if (dash)
                Unsafe.Add(ref utf16Bytes, idx++) = Dash;

            FormattingHelpers.WriteHexByte(bytes[5], ref utf16Bytes, idx);
            FormattingHelpers.WriteHexByte(bytes[4], ref utf16Bytes, idx + 2);
            idx += 4;

            if (dash)
                Unsafe.Add(ref utf16Bytes, idx++) = Dash;

            FormattingHelpers.WriteHexByte(bytes[7], ref utf16Bytes, idx);
            FormattingHelpers.WriteHexByte(bytes[6], ref utf16Bytes, idx + 2);
            idx += 4;

            if (dash)
                Unsafe.Add(ref utf16Bytes, idx++) = Dash;

            FormattingHelpers.WriteHexByte(bytes[8], ref utf16Bytes, idx);
            FormattingHelpers.WriteHexByte(bytes[9], ref utf16Bytes, idx + 2);
            idx += 4;

            if (dash)
                Unsafe.Add(ref utf16Bytes, idx++) = Dash;

            FormattingHelpers.WriteHexByte(bytes[10], ref utf16Bytes, idx);
            FormattingHelpers.WriteHexByte(bytes[11], ref utf16Bytes, idx + 2);
            FormattingHelpers.WriteHexByte(bytes[12], ref utf16Bytes, idx + 4);
            FormattingHelpers.WriteHexByte(bytes[13], ref utf16Bytes, idx + 6);
            FormattingHelpers.WriteHexByte(bytes[14], ref utf16Bytes, idx + 8);
            FormattingHelpers.WriteHexByte(bytes[15], ref utf16Bytes, idx + 10);
            idx += 12;

            if (bookEnds && format.Symbol == 'B')
                Unsafe.Add(ref utf16Bytes, idx++) = CloseBrace;
            else if (bookEnds && format.Symbol == 'P')
                Unsafe.Add(ref utf16Bytes, idx++) = CloseParen;

            return true;
        }
    }
}
