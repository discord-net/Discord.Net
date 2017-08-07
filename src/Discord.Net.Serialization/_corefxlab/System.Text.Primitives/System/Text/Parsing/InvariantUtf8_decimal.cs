// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


namespace System.Text
{
    public static partial class PrimitiveParser
    {
        public static partial class InvariantUtf8
        {
            public unsafe static bool TryParseDecimal(byte* text, int length, out decimal value)
            {
                int consumed;
                var span = new ReadOnlySpan<byte>(text, length);
                return TryParseDecimal(span, out value, out consumed);
            }
            public unsafe static bool TryParseDecimal(byte* text, int length, out decimal value, out int bytesConsumed)
            {
                var span = new ReadOnlySpan<byte>(text, length);
                return TryParseDecimal(span, out value, out bytesConsumed);
            }
            public static bool TryParseDecimal(ReadOnlySpan<byte> text, out decimal value)
            {
                int consumed;
                return TryParseDecimal(text, out value, out consumed);
            }
            public static bool TryParseDecimal(ReadOnlySpan<byte> text, out decimal value, out int bytesConsumed)
            {
                // Precondition replacement
                if (text.Length < 1)
                {
                    value = 0;
                    bytesConsumed = 0;
                    return false;
                }

                value = 0.0M;
                bytesConsumed = 0;
                string decimalString = "";
                bool decimalPlace = false, signed = false;

                int indexOfFirstDigit = 0;
                if (text[0] == '-' || text[0] == '+')
                {
                    signed = true;
                    decimalString += (char)text[0];
                    indexOfFirstDigit = 1;
                    bytesConsumed++;
                }

                for (int byteIndex = indexOfFirstDigit; byteIndex < text.Length; byteIndex++)
                {
                    byte nextByte = text[byteIndex];
                    byte nextByteVal = (byte)(nextByte - '0');

                    if (nextByteVal > 9)
                    {
                        if (!decimalPlace && nextByte == '.')
                        {
                            bytesConsumed++;
                            decimalPlace = true;
                            decimalString += (char)nextByte;
                        }
                        else if ((decimalPlace && signed && bytesConsumed == 2) || ((signed || decimalPlace) && bytesConsumed == 1))
                        {
                            value = 0;
                            bytesConsumed = 0;
                            return false;
                        }
                        else
                        {
                            if (decimal.TryParse(decimalString, out value))
                            {
                                return true;
                            }
                            else
                            {
                                bytesConsumed = 0;
                                return false;
                            }
                        }
                    }
                    else
                    {
                        bytesConsumed++;
                        decimalString += (char)nextByte;
                    }
                }

                if ((decimalPlace && signed && bytesConsumed == 2) || ((signed || decimalPlace) && bytesConsumed == 1))
                {
                    value = 0;
                    bytesConsumed = 0;
                    return false;
                }
                else
                {
                    if (decimal.TryParse(decimalString, out value))
                    {
                        return true;
                    }
                    else
                    {
                        bytesConsumed = 0;
                        return false;
                    }
                }
            }
        }
    }
}