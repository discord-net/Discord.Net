// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace System.Text
{
    internal static class InvariantUtf8IntegerFormatter
    {
        private const byte Minus = (byte)'-';
        private const byte Period = (byte)'.';
        private const byte Seperator = (byte)',';

        // Invariant formatting uses groups of 3 for each number group seperated by commas.
        //   ex. 1,234,567,890
        private const int GroupSize = 3;

        public static bool TryFormatDecimalInt64(long value, byte precision, Span<byte> buffer, out int bytesWritten)
        {
            int digitCount = FormattingHelpers.CountDigits(value);
            int bytesNeeded = digitCount + (int)((value >> 63) & 1);

            if (buffer.Length < bytesNeeded)
            {
                bytesWritten = 0;
                return false;
            }

            ref byte utf8Bytes = ref buffer.DangerousGetPinnableReference();
            int idx = 0;

            if (value < 0)
            {
                Unsafe.Add(ref utf8Bytes, idx++) = Minus;

                // Abs(long.MinValue) == long.MaxValue + 1, so we need to re-route to unsigned to handle value
                if (value == long.MinValue)
                {
                    if (!TryFormatDecimalUInt64((ulong)long.MaxValue + 1, precision, buffer.Slice(1), out bytesWritten))
                        return false;

                    bytesWritten += 1; // Add the minus sign
                    return true;
                }

                value = -value;
            }

            if (precision != ParsedFormat.NoPrecision)
            {
                int leadingZeros = (int)precision - digitCount;
                while (leadingZeros-- > 0)
                    Unsafe.Add(ref utf8Bytes, idx++) = (byte)'0';
            }

            idx += FormattingHelpers.WriteDigits(value, digitCount, ref utf8Bytes, idx);

            bytesWritten = idx;
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

            if (buffer.Length - 1 < bytesWritten)
            {
                bytesWritten = 0;
                return false;
            }

            ref byte utf8Bytes = ref buffer.DangerousGetPinnableReference();
            bytesWritten += FormattingHelpers.WriteDigits(lastDigit, 1, ref utf8Bytes, bytesWritten);
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
            int idx = (int)((value >> 63) & 1) + digitCount + groupSeperators;

            bytesWritten = idx;
            if (trailingZeros > 0)
                bytesWritten += trailingZeros + 1; // +1 for period.

            if (buffer.Length < bytesWritten)
            {
                bytesWritten = 0;
                return false;
            }

            ref byte utf8Bytes = ref buffer.DangerousGetPinnableReference();
            long v = value;

            if (v < 0)
            {
                Unsafe.Add(ref utf8Bytes, 0) = Minus;

                // Abs(long.MinValue) == long.MaxValue + 1, so we need to re-route to unsigned to handle value
                if (v == long.MinValue)
                {
                    if (!TryFormatNumericUInt64((ulong)long.MaxValue + 1, precision, buffer.Slice(1), out bytesWritten))
                        return false;

                    bytesWritten += 1; // Add the minus sign
                    return true;
                }

                v = -v;
            }

            // Write out the trailing zeros
            if (trailingZeros > 0)
            {
                Unsafe.Add(ref utf8Bytes, idx) = Period;
                FormattingHelpers.WriteDigits(0, trailingZeros, ref utf8Bytes, idx + 1);
            }

            // Starting from the back, write each group of digits except the first group
            while (digitCount > 3)
            {
                digitCount -= 3;
                idx -= 3;
                v = FormattingHelpers.DivMod(v, 1000, out long groupValue);
                FormattingHelpers.WriteDigits(groupValue, 3, ref utf8Bytes, idx);
                Unsafe.Add(ref utf8Bytes, --idx) = Seperator;
            }

            // Write the first group of digits.
            FormattingHelpers.WriteDigits(v, (int)firstGroup, ref utf8Bytes, idx - (int)firstGroup);

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

            int idx = bytesWritten;

            // Since this method routes entirely to the long version if the number is smaller than
            // long.MaxValue, we are guaranteed to need to write 3 more digits here before the set
            // of trailing zeros.

            bytesWritten += 4; // 3 digits + group seperator
            if (precision > 0)
                bytesWritten += precision + 1; // +1 for period.

            if (buffer.Length < bytesWritten)
            {
                bytesWritten = 0;
                return false;
            }

            ref byte utf8Bytes = ref buffer.DangerousGetPinnableReference();

            // Write the last group
            Unsafe.Add(ref utf8Bytes, idx++) = Seperator;
            idx += FormattingHelpers.WriteDigits(lastGroup, 3, ref utf8Bytes, idx);

            // Write out the trailing zeros
            if (precision > 0)
            {
                Unsafe.Add(ref utf8Bytes, idx) = Period;
                FormattingHelpers.WriteDigits(0, precision, ref utf8Bytes, idx + 1);
            }

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

            bytesWritten = digits + paddingCount;
            if (buffer.Length < bytesWritten)
            {
                bytesWritten = 0;
                return false;
            }

            string hexTable = useLower ? HexTableLower : HexTableUpper;
            ref byte utf8Bytes = ref buffer.DangerousGetPinnableReference();
            int idx = bytesWritten;

            for (v = value; digits-- > 0; v >>= 4)
                Unsafe.Add(ref utf8Bytes, --idx) = (byte)hexTable[(int)(v & 0xF)];

            while (paddingCount-- > 0)
                Unsafe.Add(ref utf8Bytes, --idx) = (byte)'0';

            return true;
        }
    }
}
