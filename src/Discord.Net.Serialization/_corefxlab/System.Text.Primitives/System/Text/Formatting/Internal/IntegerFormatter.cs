// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace System.Text
{
    internal static class IntegerFormatter
    {
        internal static bool TryFormatInt64(long value, ulong mask, Span<byte> buffer, out int bytesWritten, ParsedFormat format, SymbolTable symbolTable)
        {
            if (value >= 0)
            {
                return TryFormatUInt64(unchecked((ulong)value), buffer, out bytesWritten, format, symbolTable);
            }
            else if (format.Symbol == 'x' || format.Symbol == 'X')
            {
                return TryFormatUInt64(unchecked((ulong)value) & mask, buffer, out bytesWritten, format, symbolTable);
            }
            else
            {
                int minusSignBytes = 0;
                if (!symbolTable.TryEncode(SymbolTable.Symbol.MinusSign, buffer, out minusSignBytes))
                {
                    bytesWritten = 0;
                    return false;
                }

                int digitBytes = 0;
                if (!TryFormatUInt64(unchecked((ulong)-value), buffer.Slice(minusSignBytes), out digitBytes, format, symbolTable))
                {
                    bytesWritten = 0;
                    return false;
                }
                bytesWritten = digitBytes + minusSignBytes;
                return true;
            }
        }

        internal static bool TryFormatUInt64(ulong value, Span<byte> buffer, out int bytesWritten, ParsedFormat format, SymbolTable symbolTable)
        {
            switch (format.Symbol)
            {
                case 'x':
                case 'X':
                    if (symbolTable == SymbolTable.InvariantUtf8)
                        return TryFormatHexadecimalInvariantCultureUtf8(value, buffer, out bytesWritten, format);
                    else if (symbolTable == SymbolTable.InvariantUtf16)
                        return TryFormatHexadecimalInvariantCultureUtf16(value, buffer, out bytesWritten, format);
                    else
                        throw new NotSupportedException();

                case 'd':
                case 'D':
                case 'g':
                case 'G':
                    if (symbolTable == SymbolTable.InvariantUtf8)
                        return TryFormatDecimalInvariantCultureUtf8(value, buffer, out bytesWritten, format);
                    else if (symbolTable == SymbolTable.InvariantUtf16)
                        return TryFormatDecimalInvariantCultureUtf16(value, buffer, out bytesWritten, format);
                    else
                        return TryFormatDecimal(value, buffer, out bytesWritten, format, symbolTable);

                case 'n':
                case 'N':
                    return TryFormatDecimal(value, buffer, out bytesWritten, format, symbolTable);

                default:
                    throw new FormatException();
            }
        }

        private static bool TryFormatDecimalInvariantCultureUtf16(ulong value, Span<byte> buffer, out int bytesWritten, ParsedFormat format)
        {
            char symbol = char.ToUpperInvariant(format.Symbol);
            Precondition.Require(symbol == 'D' || symbol == 'G');

            // Count digits
            var valueToCountDigits = value;
            var digitsCount = 1;
            while (valueToCountDigits >= 10UL)
            {
                valueToCountDigits = valueToCountDigits / 10UL;
                digitsCount++;
            }

            var index = 0;
            var bytesCount = digitsCount * 2;

            // If format is D and precision is greater than digits count, append leading zeros
            if ((symbol == 'D') && format.HasPrecision)
            {
                var leadingZerosCount = format.Precision - digitsCount;
                if (leadingZerosCount > 0)
                {
                    bytesCount += leadingZerosCount * 2;
                }

                if (bytesCount > buffer.Length)
                {
                    bytesWritten = 0;
                    return false;
                }

                while (leadingZerosCount-- > 0)
                {
                    buffer[index++] = (byte)'0';
                    buffer[index++] = 0;
                }
            }
            else if (bytesCount > buffer.Length)
            {
                bytesWritten = 0;
                return false;
            }

            index = bytesCount;
            while (digitsCount-- > 0)
            {
                ulong digit = value % 10UL;
                value /= 10UL;
                buffer[--index] = 0;
                buffer[--index] = (byte)(digit + (ulong)'0');
            }

            bytesWritten = bytesCount;
            return true;
        }

        private static bool TryFormatDecimalInvariantCultureUtf8(ulong value, Span<byte> buffer, out int bytesWritten, ParsedFormat format)
        {
            char symbol = char.ToUpperInvariant(format.Symbol);
            Precondition.Require(symbol == 'D' || symbol == 'G');

            // Count digits
            var valueToCountDigits = value;
            var digitsCount = 1;
            while (valueToCountDigits >= 10UL)
            {
                valueToCountDigits = valueToCountDigits / 10UL;
                digitsCount++;
            }

            var index = 0;
            var bytesCount = digitsCount;

            // If format is D and precision is greater than digits count, append leading zeros
            if ((symbol == 'D') && format.HasPrecision)
            {
                var leadingZerosCount = format.Precision - digitsCount;
                if (leadingZerosCount > 0)
                {
                    bytesCount += leadingZerosCount;
                }

                if (bytesCount > buffer.Length)
                {
                    bytesWritten = 0;
                    return false;
                }

                while (leadingZerosCount-- > 0)
                {
                    buffer[index++] = (byte)'0';
                }
            }
            else if (bytesCount > buffer.Length)
            {
                bytesWritten = 0;
                return false;
            }

            index = bytesCount;
            while (digitsCount-- > 0)
            {
                ulong digit = value % 10UL;
                value /= 10UL;
                buffer[--index] = (byte)(digit + (ulong)'0');
            }

            bytesWritten = bytesCount;
            return true;
        }

        private static bool TryFormatHexadecimalInvariantCultureUtf16(ulong value, Span<byte> buffer, out int bytesWritten, ParsedFormat format)
        {
            Precondition.Require(format.Symbol == 'X' || format.Symbol == 'x');

            byte firstDigitOffset = (byte)'0';
            byte firstHexCharOffset = format.Symbol == 'x' ? (byte)'a' : (byte)'A';
            firstHexCharOffset -= 10;

            // Count amount of hex digits
            var hexDigitsCount = 1;
            ulong valueToCount = value;
            if (valueToCount > 0xFFFFFFFF)
            {
                hexDigitsCount += 8;
                valueToCount >>= 0x20;
            }
            if (valueToCount > 0xFFFF)
            {
                hexDigitsCount += 4;
                valueToCount >>= 0x10;
            }
            if (valueToCount > 0xFF)
            {
                hexDigitsCount += 2;
                valueToCount >>= 0x8;
            }
            if (valueToCount > 0xF)
            {
                hexDigitsCount++;
            }

            var bytesCount = hexDigitsCount * 2;

            // Count leading zeros
            var leadingZerosCount = format.HasPrecision ? format.Precision - hexDigitsCount : 0;
            bytesCount += leadingZerosCount > 0 ? leadingZerosCount * 2 : 0;

            if (bytesCount > buffer.Length)
            {
                bytesWritten = 0;
                return false;
            }

            var index = bytesCount;
            while (hexDigitsCount-- > 0)
            {
                byte digit = (byte)(value & 0xF);
                value >>= 0x4;
                digit += digit < 10 ? firstDigitOffset : firstHexCharOffset;

                buffer[--index] = 0;
                buffer[--index] = digit;
            }

            // Write leading zeros if any
            while (leadingZerosCount-- > 0)
            {
                buffer[--index] = 0;
                buffer[--index] = firstDigitOffset;
            }

            bytesWritten = bytesCount;
            return true;
        }

        private static bool TryFormatHexadecimalInvariantCultureUtf8(ulong value, Span<byte> buffer, out int bytesWritten, ParsedFormat format)
        {
            Precondition.Require(format.Symbol == 'X' || format.Symbol == 'x');

            byte firstDigitOffset = (byte)'0';
            byte firstHexCharOffset = format.Symbol == 'X' ? (byte)'A' : (byte)'a';
            firstHexCharOffset -= 10;

            // Count amount of hex digits
            var hexDigitsCount = 1;
            ulong valueToCount = value;
            if (valueToCount > 0xFFFFFFFF)
            {
                hexDigitsCount += 8;
                valueToCount >>= 0x20;
            }
            if (valueToCount > 0xFFFF)
            {
                hexDigitsCount += 4;
                valueToCount >>= 0x10;
            }
            if (valueToCount > 0xFF)
            {
                hexDigitsCount += 2;
                valueToCount >>= 0x8;
            }
            if (valueToCount > 0xF)
            {
                hexDigitsCount++;
            }

            var bytesCount = hexDigitsCount;

            // Count leading zeros
            var leadingZerosCount = format.HasPrecision ? format.Precision - hexDigitsCount : 0;
            bytesCount += leadingZerosCount > 0 ? leadingZerosCount : 0;

            if (bytesCount > buffer.Length)
            {
                bytesWritten = 0;
                return false;
            }

            var index = bytesCount;
            while (hexDigitsCount-- > 0)
            {
                byte digit = (byte)(value & 0xF);
                value >>= 0x4;
                digit += digit < 10 ? firstDigitOffset : firstHexCharOffset;
                buffer[--index] = digit;
            }

            // Write leading zeros if any
            while (leadingZerosCount-- > 0)
            {
                buffer[--index] = firstDigitOffset;
            }

            bytesWritten = bytesCount;
            return true;
        }

        // TODO: this whole routine is too slow. It does div and mod twice, which are both costly (especially that some JITs cannot optimize it).
        // It does it twice to avoid reversing the formatted buffer, which can be tricky given it should handle arbitrary cultures.
        // One optimization I thought we could do is to do div/mod once and store digits in a temp buffer (but that would allocate). Modification to the idea would be to store the digits in a local struct
        // Another idea possibly worth tying would be to special case cultures that have constant digit size, and go back to the format + reverse buffer approach.
        private static bool TryFormatDecimal(ulong value, Span<byte> buffer, out int bytesWritten, ParsedFormat format, SymbolTable symbolTable)
        {
            char symbol = char.ToUpperInvariant(format.Symbol);
            Precondition.Require(symbol == 'D' || format.Symbol == 'G' || format.Symbol == 'N');

            // Reverse value on decimal basis, count digits and trailing zeros before the decimal separator
            ulong reversedValueExceptFirst = 0;
            var digitsCount = 1;
            var trailingZerosCount = 0;

            // We reverse the digits in numeric form because reversing encoded digits is hard and/or costly.
            // If value contains 20 digits, its reversed value will not fit into ulong size.
            // So reverse it till last digit (reversedValueExceptFirst will have all the digits except the first one).
            while (value >= 10)
            {
                var digit = value % 10UL;
                value = value / 10UL;

                if (reversedValueExceptFirst == 0 && digit == 0)
                {
                    trailingZerosCount++;
                }
                else
                {
                    reversedValueExceptFirst = reversedValueExceptFirst * 10UL + digit;
                    digitsCount++;
                }
            }

            bytesWritten = 0;
            int digitBytes;
            // If format is D and precision is greater than digitsCount + trailingZerosCount, append leading zeros
            if (symbol == 'D' && format.HasPrecision)
            {
                var leadingZerosCount = format.Precision - digitsCount - trailingZerosCount;
                while (leadingZerosCount-- > 0)
                {
                    if (!symbolTable.TryEncode(SymbolTable.Symbol.D0, buffer.Slice(bytesWritten), out digitBytes))
                    {
                        bytesWritten = 0;
                        return false;
                    }
                    bytesWritten += digitBytes;
                }
            }

            // Append first digit
            if (!symbolTable.TryEncode((SymbolTable.Symbol)value, buffer.Slice(bytesWritten), out digitBytes))
            {
                bytesWritten = 0;
                return false;
            }
            bytesWritten += digitBytes;
            digitsCount--;

            if (symbol == 'N')
            {
                const int GroupSize = 3;

                // Count amount of digits before first group separator. It will be reset to groupSize every time digitsLeftInGroup == zero
                var digitsLeftInGroup = (digitsCount + trailingZerosCount) % GroupSize;
                if (digitsLeftInGroup == 0)
                {
                    if (digitsCount + trailingZerosCount > 0)
                    {
                        // There is a new group immediately after the first digit
                        if (!symbolTable.TryEncode(SymbolTable.Symbol.GroupSeparator, buffer.Slice(bytesWritten), out digitBytes))
                        {
                            bytesWritten = 0;
                            return false;
                        }
                        bytesWritten += digitBytes;
                    }
                    digitsLeftInGroup = GroupSize;
                }

                // Append digits
                while (reversedValueExceptFirst > 0)
                {
                    if (digitsLeftInGroup == 0)
                    {
                        if (!symbolTable.TryEncode(SymbolTable.Symbol.GroupSeparator, buffer.Slice(bytesWritten), out digitBytes))
                        {
                            bytesWritten = 0;
                            return false;
                        }
                        bytesWritten += digitBytes;
                        digitsLeftInGroup = GroupSize;
                    }

                    var nextDigit = reversedValueExceptFirst % 10UL;
                    reversedValueExceptFirst = reversedValueExceptFirst / 10UL;

                    if (!symbolTable.TryEncode((SymbolTable.Symbol)nextDigit, buffer.Slice(bytesWritten), out digitBytes))
                    {
                        bytesWritten = 0;
                        return false;
                    }
                    bytesWritten += digitBytes;
                    digitsLeftInGroup--;
                }

                // Append trailing zeros if any
                while (trailingZerosCount-- > 0)
                {
                    if (digitsLeftInGroup == 0)
                    {
                        if (!symbolTable.TryEncode(SymbolTable.Symbol.GroupSeparator, buffer.Slice(bytesWritten), out digitBytes))
                        {
                            bytesWritten = 0;
                            return false;
                        }
                        bytesWritten += digitBytes;
                        digitsLeftInGroup = GroupSize;
                    }

                    if (!symbolTable.TryEncode(SymbolTable.Symbol.D0, buffer.Slice(bytesWritten), out digitBytes))
                    {
                        bytesWritten = 0;
                        return false;
                    }
                    bytesWritten += digitBytes;
                    digitsLeftInGroup--;
                }
            }
            else
            {
                while (reversedValueExceptFirst > 0)
                {
                    var bufferSlice = buffer.Slice(bytesWritten);
                    var nextDigit = reversedValueExceptFirst % 10UL;
                    reversedValueExceptFirst = reversedValueExceptFirst / 10UL;
                    if (!symbolTable.TryEncode((SymbolTable.Symbol)nextDigit, bufferSlice, out digitBytes))
                    {
                        bytesWritten = 0;
                        return false;
                    }
                    bytesWritten += digitBytes;
                }

                // Append trailing zeros if any
                while (trailingZerosCount-- > 0)
                {
                    if (!symbolTable.TryEncode(SymbolTable.Symbol.D0, buffer.Slice(bytesWritten), out digitBytes))
                    {
                        bytesWritten = 0;
                        return false;
                    }
                    bytesWritten += digitBytes;
                }
            }

            // If format is N and precision is not defined or is greater than zero, append trailing zeros after decimal point
            if (symbol == 'N')
            {
                int trailingZerosAfterDecimalCount = format.HasPrecision ? format.Precision : 2;

                if (trailingZerosAfterDecimalCount > 0)
                {
                    if (!symbolTable.TryEncode(SymbolTable.Symbol.DecimalSeparator, buffer.Slice(bytesWritten), out digitBytes))
                    {
                        bytesWritten = 0;
                        return false;
                    }
                    bytesWritten += digitBytes;

                    while (trailingZerosAfterDecimalCount-- > 0)
                    {
                        if (!symbolTable.TryEncode(SymbolTable.Symbol.D0, buffer.Slice(bytesWritten), out digitBytes))
                        {
                            bytesWritten = 0;
                            return false;
                        }
                        bytesWritten += digitBytes;
                    }
                }
            }

            return true;
        }
    }
}
