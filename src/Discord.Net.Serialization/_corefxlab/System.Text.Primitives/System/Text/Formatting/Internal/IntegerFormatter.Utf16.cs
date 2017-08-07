// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace System.Text
{
    internal static class InvariantUtf16IntegerFormatter
    {
        private const char Minus = '-';
        private const char Period = '.';
        private const char Seperator = ',';

        // Invariant formatting uses groups of 3 for each number group seperated by commas.
        //   ex. 1,234,567,890
        private const int GroupSize = 3;

        public static bool TryFormatDecimalInt64(long value, byte precision, Span<byte> buffer, out int bytesWritten)
        {
            int digitCount = FormattingHelpers.CountDigits(value);
            int charsNeeded = digitCount + (int)((value >> 63) & 1);
            Span<char> span = buffer.NonPortableCast<byte, char>();

            if (span.Length < charsNeeded)
            {
                bytesWritten = 0;
                return false;
            }

            ref char utf16Bytes = ref span.DangerousGetPinnableReference();
            int idx = 0;

            if (value < 0)
            {
                Unsafe.Add(ref utf16Bytes, idx++) = Minus;

                // Abs(long.MinValue) == long.MaxValue + 1, so we need to re-route to unsigned to handle value
                if (value == long.MinValue)
                {
                    if (!TryFormatDecimalUInt64((ulong)long.MaxValue + 1, precision, buffer.Slice(2), out bytesWritten))
                        return false;

                    bytesWritten += sizeof(char); // Add the minus sign
                    return true;
                }

                value = -value;
            }

            if (precision != ParsedFormat.NoPrecision)
            {
                int leadingZeros = (int)precision - digitCount;
                while (leadingZeros-- > 0)
                    Unsafe.Add(ref utf16Bytes, idx++) = '0';
            }

            idx += FormattingHelpers.WriteDigits(value, digitCount, ref utf16Bytes, idx);

            bytesWritten = idx * sizeof(char);
            return true;
        }

        public static bool TryFormatDecimalUInt64(ulong value, byte precision, Span<byte> buffer, out int bytesWritten)
        {
            if (value <= long.MaxValue)
                return TryFormatDecimalInt64((long)value, precision, buffer, out bytesWritten);

            // Remove a single digit from the number. This will get it below long.MaxValue
            // Then we call the faster long version and follow-up with writing the last
            // digit. This ends up being faster by a factor of 2 than to just do the entire
            // operation using the unsigned versions.
            value = FormattingHelpers.DivMod(value, 10, out ulong lastDigit);

            if (precision != ParsedFormat.NoPrecision && precision > 0)
                precision -= 1;

            if (!TryFormatDecimalInt64((long)value, precision, buffer, out bytesWritten))
                return false;

            Span<char> span = buffer.Slice(bytesWritten).NonPortableCast<byte, char>();

            if (span.Length < sizeof(char))
            {
                bytesWritten = 0;
                return false;
            }

            ref char utf16Bytes = ref span.DangerousGetPinnableReference();
            FormattingHelpers.WriteDigits(lastDigit, 1, ref utf16Bytes, 0);
            bytesWritten += sizeof(char);
            return true;
        }

        public static bool TryFormatNumericInt64(long value, byte precision, Span<byte> buffer, out int bytesWritten)
        {
            int digitCount = FormattingHelpers.CountDigits(value);
            int groupSeperators = (int)FormattingHelpers.DivMod(digitCount, GroupSize, out long firstGroup);
            if (firstGroup == 0)
            {
                firstGroup = 3;
                groupSeperators--;
            }

            int trailingZeros = (precision == ParsedFormat.NoPrecision) ? 2 : precision;
            int charsNeeded = (int)((value >> 63) & 1) + digitCount + groupSeperators;
            int idx = charsNeeded;

            if (trailingZeros > 0)
                charsNeeded += trailingZeros + 1; // +1 for period.

            Span<char> span = buffer.NonPortableCast<byte, char>();

            if (span.Length < charsNeeded)
            {
                bytesWritten = 0;
                return false;
            }

            ref char utf16Bytes = ref span.DangerousGetPinnableReference();
            long v = value;

            if (v < 0)
            {
                Unsafe.Add(ref utf16Bytes, 0) = Minus;

                // Abs(long.MinValue) == long.MaxValue + 1, so we need to re-route to unsigned to handle value
                if (v == long.MinValue)
                {
                    if (!TryFormatNumericUInt64((ulong)long.MaxValue + 1, precision, buffer.Slice(2), out bytesWritten))
                        return false;

                    bytesWritten += sizeof(char); // Add the minus sign
                    return true;
                }

                v = -v;
            }

            // Write out the trailing zeros
            if (trailingZeros > 0)
            {
                Unsafe.Add(ref utf16Bytes, idx) = Period;
                FormattingHelpers.WriteDigits(0, trailingZeros, ref utf16Bytes, idx + 1);
            }

            // Starting from the back, write each group of digits except the first group
            while (digitCount > 3)
            {
                idx -= 3;
                v = FormattingHelpers.DivMod(v, 1000, out long groupValue);
                FormattingHelpers.WriteDigits(groupValue, 3, ref utf16Bytes, idx);
                Unsafe.Add(ref utf16Bytes, --idx) = Seperator;
                digitCount -= 3;
            }

            // Write the first group of digits.
            FormattingHelpers.WriteDigits(v, (int)firstGroup, ref utf16Bytes, idx - (int)firstGroup);

            bytesWritten = charsNeeded * sizeof(char);
            return true;
        }

        public static bool TryFormatNumericUInt64(ulong value, byte precision, Span<byte> buffer, out int bytesWritten)
        {
            if (value <= long.MaxValue)
                return TryFormatNumericInt64((long)value, precision, buffer, out bytesWritten);

            // The ulong path is much slower than the long path here, so we are doing the last group
            // inside this method plus the zero padding but routing to the long version for the rest.
            value = FormattingHelpers.DivMod(value, 1000, out ulong lastGroup);

            if (!TryFormatNumericInt64((long)value, 0, buffer, out bytesWritten))
                return false;

            if (precision == ParsedFormat.NoPrecision)
                precision = 2;

            // Since this method routes entirely to the long version if the number is smaller than
            // long.MaxValue, we are guaranteed to need to write 3 more digits here before the set
            // of trailing zeros.

            int extraChars = 4; // 3 digits + group seperator
            if (precision > 0)
                extraChars += precision + 1; // +1 for period.

            Span<char> span = buffer.Slice(bytesWritten).NonPortableCast<byte, char>();

            if (span.Length < extraChars)
            {
                bytesWritten = 0;
                return false;
            }

            ref char utf16Bytes = ref span.DangerousGetPinnableReference();
            var idx = 0;

            // Write the last group
            Unsafe.Add(ref utf16Bytes, idx++) = Seperator;
            idx += FormattingHelpers.WriteDigits(lastGroup, 3, ref utf16Bytes, idx);

            // Write out the trailing zeros
            if (precision > 0)
            {
                Unsafe.Add(ref utf16Bytes, idx++) = Period;
                idx += FormattingHelpers.WriteDigits(0, precision, ref utf16Bytes, idx);
            }

            bytesWritten += extraChars * sizeof(char);
            return true;
        }

        public static bool TryFormatHexUInt64(ulong value, byte precision, bool useLower, Span<byte> buffer, out int bytesWritten)
        {
            const string HexTableLower = "0123456789abcdef";
            const string HexTableUpper = "0123456789ABCDEF";

            var digits = 1;
            var v = value;
            if (v > 0xFFFFFFFF)
            {
                digits += 8;
                v >>= 0x20;
            }
            if (v > 0xFFFF)
            {
                digits += 4;
                v >>= 0x10;
            }
            if (v > 0xFF)
            {
                digits += 2;
                v >>= 0x8;
            }
            if (v > 0xF) digits++;

            int paddingCount = (precision == ParsedFormat.NoPrecision) ? 0 : precision - digits;
            if (paddingCount < 0) paddingCount = 0;

            int charsNeeded = digits + paddingCount;
            Span<char> span = buffer.NonPortableCast<byte, char>();

            if (span.Length < charsNeeded)
            {
                bytesWritten = 0;
                return false;
            }

            string hexTable = useLower ? HexTableLower : HexTableUpper;
            ref char utf16Bytes = ref span.DangerousGetPinnableReference();
            int idx = charsNeeded;

            for (v = value; digits-- > 0; v >>= 4)
                Unsafe.Add(ref utf16Bytes, --idx) = hexTable[(int)(v & 0xF)];

            while (paddingCount-- > 0)
                Unsafe.Add(ref utf16Bytes, --idx) = '0';

            bytesWritten = charsNeeded * sizeof(char);
            return true;
        }
    }
}
