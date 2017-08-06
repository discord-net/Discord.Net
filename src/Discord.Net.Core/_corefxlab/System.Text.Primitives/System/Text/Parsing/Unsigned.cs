// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Text
{
    public static partial class PrimitiveParser
    {

        public static bool TryParseByte(ReadOnlySpan<byte> text, out byte value, out int bytesConsumed, ParsedFormat format = default, SymbolTable symbolTable = null)
        {
            symbolTable = symbolTable ?? SymbolTable.InvariantUtf8;

            if (!format.IsDefault && format.HasPrecision)
            {
                throw new NotImplementedException("Format with precision not supported.");
            }

            if (symbolTable == SymbolTable.InvariantUtf8)
            {
                if (IsHexFormat(format))
                {
                    return InvariantUtf8.Hex.TryParseByte(text, out value, out bytesConsumed);
                }
                else
                {
                    return InvariantUtf8.TryParseByte(text, out value, out bytesConsumed);
                }
            }
            else if (symbolTable == SymbolTable.InvariantUtf16)
            {
                ReadOnlySpan<char> utf16Text = text.NonPortableCast<byte, char>();
                int charsConsumed;
                bool result;
                if (IsHexFormat(format))
                {
                    result = InvariantUtf16.Hex.TryParseByte(utf16Text, out value, out charsConsumed);
                }
                else
                {
                    result = InvariantUtf16.TryParseByte(utf16Text, out value, out charsConsumed);
                }
                bytesConsumed = charsConsumed * sizeof(char);
                return result;
            }

            if (IsHexFormat(format))
            {
                throw new NotImplementedException("The only supported encodings for hexadecimal parsing are InvariantUtf8 and InvariantUtf16.");
            }

            if (!(format.IsDefault || format.Symbol == 'G' || format.Symbol == 'g'))
            {
                throw new NotImplementedException(String.Format("Format '{0}' not supported.", format.Symbol));
            }

            SymbolTable.Symbol nextSymbol;
            int thisSymbolConsumed;
            if (!symbolTable.TryParse(text, out nextSymbol, out thisSymbolConsumed))
            {
                value = default;
                bytesConsumed = 0;
                return false;
            }

            if (nextSymbol > SymbolTable.Symbol.D9)
            {
                value = default;
                bytesConsumed = 0;
                return false;
            }

            uint parsedValue = (uint)nextSymbol;
            int index = thisSymbolConsumed;

            while (index < text.Length)
            {
                bool success = symbolTable.TryParse(text.Slice(index), out nextSymbol, out thisSymbolConsumed);
                if (!success || nextSymbol > SymbolTable.Symbol.D9)
                {
                    bytesConsumed = index;
                    value = (byte) parsedValue;
                    return true;
                }

                // If parsedValue > (byte.MaxValue / 10), any more appended digits will cause overflow.
                // if parsedValue == (byte.MaxValue / 10), any nextDigit greater than 5 implies overflow.
                if (parsedValue > byte.MaxValue / 10 || (parsedValue == byte.MaxValue / 10 && nextSymbol > SymbolTable.Symbol.D5))
                {
                    bytesConsumed = 0;
                    value = default;
                    return false;
                }

                index += thisSymbolConsumed;
                parsedValue = parsedValue * 10 + (uint)nextSymbol;
            }

            bytesConsumed = text.Length;
            value = (byte) parsedValue;
            return true;
        }

        public static bool TryParseUInt16(ReadOnlySpan<byte> text, out ushort value, out int bytesConsumed, ParsedFormat format = default, SymbolTable symbolTable = null)
        {
            symbolTable = symbolTable ?? SymbolTable.InvariantUtf8;

            if (!format.IsDefault && format.HasPrecision)
            {
                throw new NotImplementedException("Format with precision not supported.");
            }

            if (symbolTable == SymbolTable.InvariantUtf8)
            {
                if (IsHexFormat(format))
                {
                    return InvariantUtf8.Hex.TryParseUInt16(text, out value, out bytesConsumed);
                }
                else
                {
                    return InvariantUtf8.TryParseUInt16(text, out value, out bytesConsumed);
                }
            }
            else if (symbolTable == SymbolTable.InvariantUtf16)
            {
                ReadOnlySpan<char> utf16Text = text.NonPortableCast<byte, char>();
                int charsConsumed;
                bool result;
                if (IsHexFormat(format))
                {
                    result = InvariantUtf16.Hex.TryParseUInt16(utf16Text, out value, out charsConsumed);
                }
                else
                {
                    result = InvariantUtf16.TryParseUInt16(utf16Text, out value, out charsConsumed);
                }
                bytesConsumed = charsConsumed * sizeof(char);
                return result;
            }

            if (IsHexFormat(format))
            {
                throw new NotImplementedException("The only supported encodings for hexadecimal parsing are InvariantUtf8 and InvariantUtf16.");
            }

            if (!(format.IsDefault || format.Symbol == 'G' || format.Symbol == 'g'))
            {
                throw new NotImplementedException(String.Format("Format '{0}' not supported.", format.Symbol));
            }

            SymbolTable.Symbol nextSymbol;
            int thisSymbolConsumed;
            if (!symbolTable.TryParse(text, out nextSymbol, out thisSymbolConsumed))
            {
                value = default;
                bytesConsumed = 0;
                return false;
            }

            if (nextSymbol > SymbolTable.Symbol.D9)
            {
                value = default;
                bytesConsumed = 0;
                return false;
            }

            uint parsedValue = (uint)nextSymbol;
            int index = thisSymbolConsumed;

            while (index < text.Length)
            {
                bool success = symbolTable.TryParse(text.Slice(index), out nextSymbol, out thisSymbolConsumed);
                if (!success || nextSymbol > SymbolTable.Symbol.D9)
                {
                    bytesConsumed = index;
                    value = (ushort) parsedValue;
                    return true;
                }

                // If parsedValue > (ushort.MaxValue / 10), any more appended digits will cause overflow.
                // if parsedValue == (ushort.MaxValue / 10), any nextDigit greater than 5 implies overflow.
                if (parsedValue > ushort.MaxValue / 10 || (parsedValue == ushort.MaxValue / 10 && nextSymbol > SymbolTable.Symbol.D5))
                {
                    bytesConsumed = 0;
                    value = default;
                    return false;
                }

                index += thisSymbolConsumed;
                parsedValue = parsedValue * 10 + (uint)nextSymbol;
            }

            bytesConsumed = text.Length;
            value = (ushort) parsedValue;
            return true;
        }

        public static bool TryParseUInt32(ReadOnlySpan<byte> text, out uint value, out int bytesConsumed, ParsedFormat format = default, SymbolTable symbolTable = null)
        {
            symbolTable = symbolTable ?? SymbolTable.InvariantUtf8;

            if (!format.IsDefault && format.HasPrecision)
            {
                throw new NotImplementedException("Format with precision not supported.");
            }

            if (symbolTable == SymbolTable.InvariantUtf8)
            {
                if (IsHexFormat(format))
                {
                    return InvariantUtf8.Hex.TryParseUInt32(text, out value, out bytesConsumed);
                }
                else
                {
                    return InvariantUtf8.TryParseUInt32(text, out value, out bytesConsumed);
                }
            }
            else if (symbolTable == SymbolTable.InvariantUtf16)
            {
                ReadOnlySpan<char> utf16Text = text.NonPortableCast<byte, char>();
                int charsConsumed;
                bool result;
                if (IsHexFormat(format))
                {
                    result = InvariantUtf16.Hex.TryParseUInt32(utf16Text, out value, out charsConsumed);
                }
                else
                {
                    result = InvariantUtf16.TryParseUInt32(utf16Text, out value, out charsConsumed);
                }
                bytesConsumed = charsConsumed * sizeof(char);
                return result;
            }

            if (IsHexFormat(format))
            {
                throw new NotImplementedException("The only supported encodings for hexadecimal parsing are InvariantUtf8 and InvariantUtf16.");
            }

            if (!(format.IsDefault || format.Symbol == 'G' || format.Symbol == 'g'))
            {
                throw new NotImplementedException(String.Format("Format '{0}' not supported.", format.Symbol));
            }

            SymbolTable.Symbol nextSymbol;
            int thisSymbolConsumed;
            if (!symbolTable.TryParse(text, out nextSymbol, out thisSymbolConsumed))
            {
                value = default;
                bytesConsumed = 0;
                return false;
            }

            if (nextSymbol > SymbolTable.Symbol.D9)
            {
                value = default;
                bytesConsumed = 0;
                return false;
            }

            uint parsedValue = (uint)nextSymbol;
            int index = thisSymbolConsumed;

            while (index < text.Length)
            {
                bool success = symbolTable.TryParse(text.Slice(index), out nextSymbol, out thisSymbolConsumed);
                if (!success || nextSymbol > SymbolTable.Symbol.D9)
                {
                    bytesConsumed = index;
                    value = (uint) parsedValue;
                    return true;
                }

                // If parsedValue > (uint.MaxValue / 10), any more appended digits will cause overflow.
                // if parsedValue == (uint.MaxValue / 10), any nextDigit greater than 5 implies overflow.
                if (parsedValue > uint.MaxValue / 10 || (parsedValue == uint.MaxValue / 10 && nextSymbol > SymbolTable.Symbol.D5))
                {
                    bytesConsumed = 0;
                    value = default;
                    return false;
                }

                index += thisSymbolConsumed;
                parsedValue = parsedValue * 10 + (uint)nextSymbol;
            }

            bytesConsumed = text.Length;
            value = (uint) parsedValue;
            return true;
        }

        public static bool TryParseUInt64(ReadOnlySpan<byte> text, out ulong value, out int bytesConsumed, ParsedFormat format = default, SymbolTable symbolTable = null)
        {
            symbolTable = symbolTable ?? SymbolTable.InvariantUtf8;

            if (!format.IsDefault && format.HasPrecision)
            {
                throw new NotImplementedException("Format with precision not supported.");
            }

            if (symbolTable == SymbolTable.InvariantUtf8)
            {
                if (IsHexFormat(format))
                {
                    return InvariantUtf8.Hex.TryParseUInt64(text, out value, out bytesConsumed);
                }
                else
                {
                    return InvariantUtf8.TryParseUInt64(text, out value, out bytesConsumed);
                }
            }
            else if (symbolTable == SymbolTable.InvariantUtf16)
            {
                ReadOnlySpan<char> utf16Text = text.NonPortableCast<byte, char>();
                int charsConsumed;
                bool result;
                if (IsHexFormat(format))
                {
                    result = InvariantUtf16.Hex.TryParseUInt64(utf16Text, out value, out charsConsumed);
                }
                else
                {
                    result = InvariantUtf16.TryParseUInt64(utf16Text, out value, out charsConsumed);
                }
                bytesConsumed = charsConsumed * sizeof(char);
                return result;
            }

            if (IsHexFormat(format))
            {
                throw new NotImplementedException("The only supported encodings for hexadecimal parsing are InvariantUtf8 and InvariantUtf16.");
            }

            if (!(format.IsDefault || format.Symbol == 'G' || format.Symbol == 'g'))
            {
                throw new NotImplementedException(String.Format("Format '{0}' not supported.", format.Symbol));
            }

            SymbolTable.Symbol nextSymbol;
            int thisSymbolConsumed;
            if (!symbolTable.TryParse(text, out nextSymbol, out thisSymbolConsumed))
            {
                value = default;
                bytesConsumed = 0;
                return false;
            }

            if (nextSymbol > SymbolTable.Symbol.D9)
            {
                value = default;
                bytesConsumed = 0;
                return false;
            }

            ulong parsedValue = (uint)nextSymbol;
            int index = thisSymbolConsumed;

            while (index < text.Length)
            {
                bool success = symbolTable.TryParse(text.Slice(index), out nextSymbol, out thisSymbolConsumed);
                if (!success || nextSymbol > SymbolTable.Symbol.D9)
                {
                    bytesConsumed = index;
                    value = (ulong) parsedValue;
                    return true;
                }

                // If parsedValue > (ulong.MaxValue / 10), any more appended digits will cause overflow.
                // if parsedValue == (ulong.MaxValue / 10), any nextDigit greater than 5 implies overflow.
                if (parsedValue > ulong.MaxValue / 10 || (parsedValue == ulong.MaxValue / 10 && nextSymbol > SymbolTable.Symbol.D5))
                {
                    bytesConsumed = 0;
                    value = default;
                    return false;
                }

                index += thisSymbolConsumed;
                parsedValue = parsedValue * 10 + (uint)nextSymbol;
            }

            bytesConsumed = text.Length;
            value = (ulong) parsedValue;
            return true;
        }
    }
}
