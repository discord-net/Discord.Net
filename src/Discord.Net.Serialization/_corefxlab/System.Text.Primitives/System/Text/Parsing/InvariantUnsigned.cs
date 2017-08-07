// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Text
{
    public static partial class PrimitiveParser
    {
        public static partial class InvariantUtf8
        {
            #region Byte
            public unsafe static bool TryParseByte(byte* text, int length, out byte value)
            {
                if (length < 1)
                {
                    value = default;
                    return false;
                }

                // Parse the first digit separately. If invalid here, we need to return false.
                uint firstDigit = text[0] - 48u; // '0'
                if (firstDigit > 9)
                {
                    value = default;
                    return false;
                }
                uint parsedValue = firstDigit;

                if (length < ByteOverflowLength)
                {
                    // Length is less than ByteOverflowLength; overflow is not possible
                    for (int index = 1; index < length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = (byte)(parsedValue);
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }
                else
                {
                    // Length is greater than ByteOverflowLength; overflow is only possible after ByteOverflowLength
                    // digits. There may be no overflow after ByteOverflowLength if there are leading zeroes.
                    for (int index = 1; index < ByteOverflowLength - 1; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = (byte)(parsedValue);
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                    for (int index = ByteOverflowLength - 1; index < length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = (byte)(parsedValue);
                            return true;
                        }
                        // If parsedValue > (byte.MaxValue / 10), any more appended digits will cause overflow.
                        // if parsedValue == (byte.MaxValue / 10), any nextDigit greater than 5 implies overflow.
                        if (parsedValue > byte.MaxValue / 10 || (parsedValue == byte.MaxValue / 10 && nextDigit > 5))
                        {
                            value = default;
                            return false;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }

                value = (byte)(parsedValue);
                return true;
            }

            public unsafe static bool TryParseByte(byte* text, int length, out byte value, out int bytesConsumed)
            {
                if (length < 1)
                {
                    bytesConsumed = 0;
                    value = default;
                    return false;
                }

                // Parse the first digit separately. If invalid here, we need to return false.
                uint firstDigit = text[0] - 48u; // '0'
                if (firstDigit > 9)
                {
                    bytesConsumed = 0;
                    value = default;
                    return false;
                }
                uint parsedValue = firstDigit;

                if (length < ByteOverflowLength)
                {
                    // Length is less than ByteOverflowLength; overflow is not possible
                    for (int index = 1; index < length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            bytesConsumed = index;
                            value = (byte)(parsedValue);
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }
                else
                {
                    // Length is greater than ByteOverflowLength; overflow is only possible after ByteOverflowLength
                    // digits. There may be no overflow after ByteOverflowLength if there are leading zeroes.
                    for (int index = 1; index < ByteOverflowLength - 1; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            bytesConsumed = index;
                            value = (byte)(parsedValue);
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                    for (int index = ByteOverflowLength - 1; index < length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            bytesConsumed = index;
                            value = (byte)(parsedValue);
                            return true;
                        }
                        // If parsedValue > (byte.MaxValue / 10), any more appended digits will cause overflow.
                        // if parsedValue == (byte.MaxValue / 10), any nextDigit greater than 5 implies overflow.
                        if (parsedValue > byte.MaxValue / 10 || (parsedValue == byte.MaxValue / 10 && nextDigit > 5))
                        {
                            bytesConsumed = 0;
                            value = default;
                            return false;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }

                bytesConsumed = length;
                value = (byte)(parsedValue);
                return true;
            }

            public static bool TryParseByte(ReadOnlySpan<byte> text, out byte value)
            {
                if (text.Length < 1)
                {
                    value = default;
                    return false;
                }

                // Parse the first digit separately. If invalid here, we need to return false.
                uint firstDigit = text[0] - 48u; // '0'
                if (firstDigit > 9)
                {
                    value = default;
                    return false;
                }
                uint parsedValue = firstDigit;

                if (text.Length < ByteOverflowLength)
                {
                    // Length is less than ByteOverflowLength; overflow is not possible
                    for (int index = 1; index < text.Length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = (byte)(parsedValue);
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }
                else
                {
                    // Length is greater than ByteOverflowLength; overflow is only possible after ByteOverflowLength
                    // digits. There may be no overflow after ByteOverflowLength if there are leading zeroes.
                    for (int index = 1; index < ByteOverflowLength - 1; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = (byte)(parsedValue);
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                    for (int index = ByteOverflowLength - 1; index < text.Length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = (byte)(parsedValue);
                            return true;
                        }
                        // If parsedValue > (byte.MaxValue / 10), any more appended digits will cause overflow.
                        // if parsedValue == (byte.MaxValue / 10), any nextDigit greater than 5 implies overflow.
                        if (parsedValue > byte.MaxValue / 10 || (parsedValue == byte.MaxValue / 10 && nextDigit > 5))
                        {
                            value = default;
                            return false;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }

                value = (byte)(parsedValue);
                return true;
            }

            public static bool TryParseByte(ReadOnlySpan<byte> text, out byte value, out int bytesConsumed)
            {
                if (text.Length < 1)
                {
                    bytesConsumed = 0;
                    value = default;
                    return false;
                }

                // Parse the first digit separately. If invalid here, we need to return false.
                uint firstDigit = text[0] - 48u; // '0'
                if (firstDigit > 9)
                {
                    bytesConsumed = 0;
                    value = default;
                    return false;
                }
                uint parsedValue = firstDigit;

                if (text.Length < ByteOverflowLength)
                {
                    // Length is less than ByteOverflowLength; overflow is not possible
                    for (int index = 1; index < text.Length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            bytesConsumed = index;
                            value = (byte)(parsedValue);
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }
                else
                {
                    // Length is greater than ByteOverflowLength; overflow is only possible after ByteOverflowLength
                    // digits. There may be no overflow after ByteOverflowLength if there are leading zeroes.
                    for (int index = 1; index < ByteOverflowLength - 1; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            bytesConsumed = index;
                            value = (byte)(parsedValue);
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                    for (int index = ByteOverflowLength - 1; index < text.Length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            bytesConsumed = index;
                            value = (byte)(parsedValue);
                            return true;
                        }
                        // If parsedValue > (byte.MaxValue / 10), any more appended digits will cause overflow.
                        // if parsedValue == (byte.MaxValue / 10), any nextDigit greater than 5 implies overflow.
                        if (parsedValue > byte.MaxValue / 10 || (parsedValue == byte.MaxValue / 10 && nextDigit > 5))
                        {
                            bytesConsumed = 0;
                            value = default;
                            return false;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }

                bytesConsumed = text.Length;
                value = (byte)(parsedValue);
                return true;
            }

            #endregion

            #region UInt16
            public unsafe static bool TryParseUInt16(byte* text, int length, out ushort value)
            {
                if (length < 1)
                {
                    value = default;
                    return false;
                }

                // Parse the first digit separately. If invalid here, we need to return false.
                uint firstDigit = text[0] - 48u; // '0'
                if (firstDigit > 9)
                {
                    value = default;
                    return false;
                }
                uint parsedValue = firstDigit;

                if (length < UInt16OverflowLength)
                {
                    // Length is less than UInt16OverflowLength; overflow is not possible
                    for (int index = 1; index < length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = (ushort)(parsedValue);
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }
                else
                {
                    // Length is greater than UInt16OverflowLength; overflow is only possible after UInt16OverflowLength
                    // digits. There may be no overflow after UInt16OverflowLength if there are leading zeroes.
                    for (int index = 1; index < UInt16OverflowLength - 1; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = (ushort)(parsedValue);
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                    for (int index = UInt16OverflowLength - 1; index < length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = (ushort)(parsedValue);
                            return true;
                        }
                        // If parsedValue > (ushort.MaxValue / 10), any more appended digits will cause overflow.
                        // if parsedValue == (ushort.MaxValue / 10), any nextDigit greater than 5 implies overflow.
                        if (parsedValue > ushort.MaxValue / 10 || (parsedValue == ushort.MaxValue / 10 && nextDigit > 5))
                        {
                            value = default;
                            return false;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }

                value = (ushort)(parsedValue);
                return true;
            }

            public unsafe static bool TryParseUInt16(byte* text, int length, out ushort value, out int bytesConsumed)
            {
                if (length < 1)
                {
                    bytesConsumed = 0;
                    value = default;
                    return false;
                }

                // Parse the first digit separately. If invalid here, we need to return false.
                uint firstDigit = text[0] - 48u; // '0'
                if (firstDigit > 9)
                {
                    bytesConsumed = 0;
                    value = default;
                    return false;
                }
                uint parsedValue = firstDigit;

                if (length < UInt16OverflowLength)
                {
                    // Length is less than UInt16OverflowLength; overflow is not possible
                    for (int index = 1; index < length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            bytesConsumed = index;
                            value = (ushort)(parsedValue);
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }
                else
                {
                    // Length is greater than UInt16OverflowLength; overflow is only possible after UInt16OverflowLength
                    // digits. There may be no overflow after UInt16OverflowLength if there are leading zeroes.
                    for (int index = 1; index < UInt16OverflowLength - 1; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            bytesConsumed = index;
                            value = (ushort)(parsedValue);
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                    for (int index = UInt16OverflowLength - 1; index < length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            bytesConsumed = index;
                            value = (ushort)(parsedValue);
                            return true;
                        }
                        // If parsedValue > (ushort.MaxValue / 10), any more appended digits will cause overflow.
                        // if parsedValue == (ushort.MaxValue / 10), any nextDigit greater than 5 implies overflow.
                        if (parsedValue > ushort.MaxValue / 10 || (parsedValue == ushort.MaxValue / 10 && nextDigit > 5))
                        {
                            bytesConsumed = 0;
                            value = default;
                            return false;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }

                bytesConsumed = length;
                value = (ushort)(parsedValue);
                return true;
            }

            public static bool TryParseUInt16(ReadOnlySpan<byte> text, out ushort value)
            {
                if (text.Length < 1)
                {
                    value = default;
                    return false;
                }

                // Parse the first digit separately. If invalid here, we need to return false.
                uint firstDigit = text[0] - 48u; // '0'
                if (firstDigit > 9)
                {
                    value = default;
                    return false;
                }
                uint parsedValue = firstDigit;

                if (text.Length < UInt16OverflowLength)
                {
                    // Length is less than UInt16OverflowLength; overflow is not possible
                    for (int index = 1; index < text.Length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = (ushort)(parsedValue);
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }
                else
                {
                    // Length is greater than UInt16OverflowLength; overflow is only possible after UInt16OverflowLength
                    // digits. There may be no overflow after UInt16OverflowLength if there are leading zeroes.
                    for (int index = 1; index < UInt16OverflowLength - 1; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = (ushort)(parsedValue);
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                    for (int index = UInt16OverflowLength - 1; index < text.Length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = (ushort)(parsedValue);
                            return true;
                        }
                        // If parsedValue > (ushort.MaxValue / 10), any more appended digits will cause overflow.
                        // if parsedValue == (ushort.MaxValue / 10), any nextDigit greater than 5 implies overflow.
                        if (parsedValue > ushort.MaxValue / 10 || (parsedValue == ushort.MaxValue / 10 && nextDigit > 5))
                        {
                            value = default;
                            return false;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }

                value = (ushort)(parsedValue);
                return true;
            }

            public static bool TryParseUInt16(ReadOnlySpan<byte> text, out ushort value, out int bytesConsumed)
            {
                if (text.Length < 1)
                {
                    bytesConsumed = 0;
                    value = default;
                    return false;
                }

                // Parse the first digit separately. If invalid here, we need to return false.
                uint firstDigit = text[0] - 48u; // '0'
                if (firstDigit > 9)
                {
                    bytesConsumed = 0;
                    value = default;
                    return false;
                }
                uint parsedValue = firstDigit;

                if (text.Length < UInt16OverflowLength)
                {
                    // Length is less than UInt16OverflowLength; overflow is not possible
                    for (int index = 1; index < text.Length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            bytesConsumed = index;
                            value = (ushort)(parsedValue);
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }
                else
                {
                    // Length is greater than UInt16OverflowLength; overflow is only possible after UInt16OverflowLength
                    // digits. There may be no overflow after UInt16OverflowLength if there are leading zeroes.
                    for (int index = 1; index < UInt16OverflowLength - 1; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            bytesConsumed = index;
                            value = (ushort)(parsedValue);
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                    for (int index = UInt16OverflowLength - 1; index < text.Length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            bytesConsumed = index;
                            value = (ushort)(parsedValue);
                            return true;
                        }
                        // If parsedValue > (ushort.MaxValue / 10), any more appended digits will cause overflow.
                        // if parsedValue == (ushort.MaxValue / 10), any nextDigit greater than 5 implies overflow.
                        if (parsedValue > ushort.MaxValue / 10 || (parsedValue == ushort.MaxValue / 10 && nextDigit > 5))
                        {
                            bytesConsumed = 0;
                            value = default;
                            return false;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }

                bytesConsumed = text.Length;
                value = (ushort)(parsedValue);
                return true;
            }

            #endregion

            #region UInt32
            public unsafe static bool TryParseUInt32(byte* text, int length, out uint value)
            {
                if (length < 1)
                {
                    value = default;
                    return false;
                }

                // Parse the first digit separately. If invalid here, we need to return false.
                uint firstDigit = text[0] - 48u; // '0'
                if (firstDigit > 9)
                {
                    value = default;
                    return false;
                }
                uint parsedValue = firstDigit;

                if (length < UInt32OverflowLength)
                {
                    // Length is less than UInt32OverflowLength; overflow is not possible
                    for (int index = 1; index < length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = parsedValue;
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }
                else
                {
                    // Length is greater than UInt32OverflowLength; overflow is only possible after UInt32OverflowLength
                    // digits. There may be no overflow after UInt32OverflowLength if there are leading zeroes.
                    for (int index = 1; index < UInt32OverflowLength - 1; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = parsedValue;
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                    for (int index = UInt32OverflowLength - 1; index < length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = parsedValue;
                            return true;
                        }
                        // If parsedValue > (uint.MaxValue / 10), any more appended digits will cause overflow.
                        // if parsedValue == (uint.MaxValue / 10), any nextDigit greater than 5 implies overflow.
                        if (parsedValue > uint.MaxValue / 10 || (parsedValue == uint.MaxValue / 10 && nextDigit > 5))
                        {
                            value = default;
                            return false;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }

                value = parsedValue;
                return true;
            }

            public unsafe static bool TryParseUInt32(byte* text, int length, out uint value, out int bytesConsumed)
            {
                if (length < 1)
                {
                    bytesConsumed = 0;
                    value = default;
                    return false;
                }

                // Parse the first digit separately. If invalid here, we need to return false.
                uint firstDigit = text[0] - 48u; // '0'
                if (firstDigit > 9)
                {
                    bytesConsumed = 0;
                    value = default;
                    return false;
                }
                uint parsedValue = firstDigit;

                if (length < UInt32OverflowLength)
                {
                    // Length is less than UInt32OverflowLength; overflow is not possible
                    for (int index = 1; index < length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            bytesConsumed = index;
                            value = parsedValue;
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }
                else
                {
                    // Length is greater than UInt32OverflowLength; overflow is only possible after UInt32OverflowLength
                    // digits. There may be no overflow after UInt32OverflowLength if there are leading zeroes.
                    for (int index = 1; index < UInt32OverflowLength - 1; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            bytesConsumed = index;
                            value = parsedValue;
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                    for (int index = UInt32OverflowLength - 1; index < length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            bytesConsumed = index;
                            value = parsedValue;
                            return true;
                        }
                        // If parsedValue > (uint.MaxValue / 10), any more appended digits will cause overflow.
                        // if parsedValue == (uint.MaxValue / 10), any nextDigit greater than 5 implies overflow.
                        if (parsedValue > uint.MaxValue / 10 || (parsedValue == uint.MaxValue / 10 && nextDigit > 5))
                        {
                            bytesConsumed = 0;
                            value = default;
                            return false;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }

                bytesConsumed = length;
                value = parsedValue;
                return true;
            }

            public static bool TryParseUInt32(ReadOnlySpan<byte> text, out uint value)
            {
                if (text.Length < 1)
                {
                    value = default;
                    return false;
                }

                // Parse the first digit separately. If invalid here, we need to return false.
                uint firstDigit = text[0] - 48u; // '0'
                if (firstDigit > 9)
                {
                    value = default;
                    return false;
                }
                uint parsedValue = firstDigit;

                if (text.Length < UInt32OverflowLength)
                {
                    // Length is less than UInt32OverflowLength; overflow is not possible
                    for (int index = 1; index < text.Length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = parsedValue;
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }
                else
                {
                    // Length is greater than UInt32OverflowLength; overflow is only possible after UInt32OverflowLength
                    // digits. There may be no overflow after UInt32OverflowLength if there are leading zeroes.
                    for (int index = 1; index < UInt32OverflowLength - 1; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = parsedValue;
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                    for (int index = UInt32OverflowLength - 1; index < text.Length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = parsedValue;
                            return true;
                        }
                        // If parsedValue > (uint.MaxValue / 10), any more appended digits will cause overflow.
                        // if parsedValue == (uint.MaxValue / 10), any nextDigit greater than 5 implies overflow.
                        if (parsedValue > uint.MaxValue / 10 || (parsedValue == uint.MaxValue / 10 && nextDigit > 5))
                        {
                            value = default;
                            return false;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }

                value = parsedValue;
                return true;
            }

            public static bool TryParseUInt32(ReadOnlySpan<byte> text, out uint value, out int bytesConsumed)
            {
                if (text.Length < 1)
                {
                    bytesConsumed = 0;
                    value = default;
                    return false;
                }

                // Parse the first digit separately. If invalid here, we need to return false.
                uint firstDigit = text[0] - 48u; // '0'
                if (firstDigit > 9)
                {
                    bytesConsumed = 0;
                    value = default;
                    return false;
                }
                uint parsedValue = firstDigit;

                if (text.Length < UInt32OverflowLength)
                {
                    // Length is less than UInt32OverflowLength; overflow is not possible
                    for (int index = 1; index < text.Length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            bytesConsumed = index;
                            value = parsedValue;
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }
                else
                {
                    // Length is greater than UInt32OverflowLength; overflow is only possible after UInt32OverflowLength
                    // digits. There may be no overflow after UInt32OverflowLength if there are leading zeroes.
                    for (int index = 1; index < UInt32OverflowLength - 1; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            bytesConsumed = index;
                            value = parsedValue;
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                    for (int index = UInt32OverflowLength - 1; index < text.Length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            bytesConsumed = index;
                            value = parsedValue;
                            return true;
                        }
                        // If parsedValue > (uint.MaxValue / 10), any more appended digits will cause overflow.
                        // if parsedValue == (uint.MaxValue / 10), any nextDigit greater than 5 implies overflow.
                        if (parsedValue > uint.MaxValue / 10 || (parsedValue == uint.MaxValue / 10 && nextDigit > 5))
                        {
                            bytesConsumed = 0;
                            value = default;
                            return false;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }

                bytesConsumed = text.Length;
                value = parsedValue;
                return true;
            }

            #endregion

            #region UInt64
            public unsafe static bool TryParseUInt64(byte* text, int length, out ulong value)
            {
                if (length < 1)
                {
                    value = default;
                    return false;
                }

                // Parse the first digit separately. If invalid here, we need to return false.
                ulong firstDigit = text[0] - 48u; // '0'
                if (firstDigit > 9)
                {
                    value = default;
                    return false;
                }
                ulong parsedValue = firstDigit;

                if (length < UInt64OverflowLength)
                {
                    // Length is less than UInt64OverflowLength; overflow is not possible
                    for (int index = 1; index < length; index++)
                    {
                        ulong nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = parsedValue;
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }
                else
                {
                    // Length is greater than UInt64OverflowLength; overflow is only possible after UInt64OverflowLength
                    // digits. There may be no overflow after UInt64OverflowLength if there are leading zeroes.
                    for (int index = 1; index < UInt64OverflowLength - 1; index++)
                    {
                        ulong nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = parsedValue;
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                    for (int index = UInt64OverflowLength - 1; index < length; index++)
                    {
                        ulong nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = parsedValue;
                            return true;
                        }
                        // If parsedValue > (ulong.MaxValue / 10), any more appended digits will cause overflow.
                        // if parsedValue == (ulong.MaxValue / 10), any nextDigit greater than 5 implies overflow.
                        if (parsedValue > ulong.MaxValue / 10 || (parsedValue == ulong.MaxValue / 10 && nextDigit > 5))
                        {
                            value = default;
                            return false;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }

                value = parsedValue;
                return true;
            }

            public unsafe static bool TryParseUInt64(byte* text, int length, out ulong value, out int bytesConsumed)
            {
                if (length < 1)
                {
                    bytesConsumed = 0;
                    value = default;
                    return false;
                }

                // Parse the first digit separately. If invalid here, we need to return false.
                ulong firstDigit = text[0] - 48u; // '0'
                if (firstDigit > 9)
                {
                    bytesConsumed = 0;
                    value = default;
                    return false;
                }
                ulong parsedValue = firstDigit;

                if (length < UInt64OverflowLength)
                {
                    // Length is less than UInt64OverflowLength; overflow is not possible
                    for (int index = 1; index < length; index++)
                    {
                        ulong nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            bytesConsumed = index;
                            value = parsedValue;
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }
                else
                {
                    // Length is greater than UInt64OverflowLength; overflow is only possible after UInt64OverflowLength
                    // digits. There may be no overflow after UInt64OverflowLength if there are leading zeroes.
                    for (int index = 1; index < UInt64OverflowLength - 1; index++)
                    {
                        ulong nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            bytesConsumed = index;
                            value = parsedValue;
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                    for (int index = UInt64OverflowLength - 1; index < length; index++)
                    {
                        ulong nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            bytesConsumed = index;
                            value = parsedValue;
                            return true;
                        }
                        // If parsedValue > (ulong.MaxValue / 10), any more appended digits will cause overflow.
                        // if parsedValue == (ulong.MaxValue / 10), any nextDigit greater than 5 implies overflow.
                        if (parsedValue > ulong.MaxValue / 10 || (parsedValue == ulong.MaxValue / 10 && nextDigit > 5))
                        {
                            bytesConsumed = 0;
                            value = default;
                            return false;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }

                bytesConsumed = length;
                value = parsedValue;
                return true;
            }

            public static bool TryParseUInt64(ReadOnlySpan<byte> text, out ulong value)
            {
                if (text.Length < 1)
                {
                    value = default;
                    return false;
                }

                // Parse the first digit separately. If invalid here, we need to return false.
                ulong firstDigit = text[0] - 48u; // '0'
                if (firstDigit > 9)
                {
                    value = default;
                    return false;
                }
                ulong parsedValue = firstDigit;

                if (text.Length < UInt64OverflowLength)
                {
                    // Length is less than UInt64OverflowLength; overflow is not possible
                    for (int index = 1; index < text.Length; index++)
                    {
                        ulong nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = parsedValue;
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }
                else
                {
                    // Length is greater than UInt64OverflowLength; overflow is only possible after UInt64OverflowLength
                    // digits. There may be no overflow after UInt64OverflowLength if there are leading zeroes.
                    for (int index = 1; index < UInt64OverflowLength - 1; index++)
                    {
                        ulong nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = parsedValue;
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                    for (int index = UInt64OverflowLength - 1; index < text.Length; index++)
                    {
                        ulong nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = parsedValue;
                            return true;
                        }
                        // If parsedValue > (ulong.MaxValue / 10), any more appended digits will cause overflow.
                        // if parsedValue == (ulong.MaxValue / 10), any nextDigit greater than 5 implies overflow.
                        if (parsedValue > ulong.MaxValue / 10 || (parsedValue == ulong.MaxValue / 10 && nextDigit > 5))
                        {
                            value = default;
                            return false;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }

                value = parsedValue;
                return true;
            }

            public static bool TryParseUInt64(ReadOnlySpan<byte> text, out ulong value, out int bytesConsumed)
            {
                if (text.Length < 1)
                {
                    bytesConsumed = 0;
                    value = default;
                    return false;
                }

                // Parse the first digit separately. If invalid here, we need to return false.
                ulong firstDigit = text[0] - 48u; // '0'
                if (firstDigit > 9)
                {
                    bytesConsumed = 0;
                    value = default;
                    return false;
                }
                ulong parsedValue = firstDigit;

                if (text.Length < UInt64OverflowLength)
                {
                    // Length is less than UInt64OverflowLength; overflow is not possible
                    for (int index = 1; index < text.Length; index++)
                    {
                        ulong nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            bytesConsumed = index;
                            value = parsedValue;
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }
                else
                {
                    // Length is greater than UInt64OverflowLength; overflow is only possible after UInt64OverflowLength
                    // digits. There may be no overflow after UInt64OverflowLength if there are leading zeroes.
                    for (int index = 1; index < UInt64OverflowLength - 1; index++)
                    {
                        ulong nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            bytesConsumed = index;
                            value = parsedValue;
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                    for (int index = UInt64OverflowLength - 1; index < text.Length; index++)
                    {
                        ulong nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            bytesConsumed = index;
                            value = parsedValue;
                            return true;
                        }
                        // If parsedValue > (ulong.MaxValue / 10), any more appended digits will cause overflow.
                        // if parsedValue == (ulong.MaxValue / 10), any nextDigit greater than 5 implies overflow.
                        if (parsedValue > ulong.MaxValue / 10 || (parsedValue == ulong.MaxValue / 10 && nextDigit > 5))
                        {
                            bytesConsumed = 0;
                            value = default;
                            return false;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }

                bytesConsumed = text.Length;
                value = parsedValue;
                return true;
            }

            #endregion

        }
        public static partial class InvariantUtf16
        {
            #region Byte
            public unsafe static bool TryParseByte(char* text, int length, out byte value)
            {
                if (length < 1)
                {
                    value = default;
                    return false;
                }

                // Parse the first digit separately. If invalid here, we need to return false.
                uint firstDigit = text[0] - 48u; // '0'
                if (firstDigit > 9)
                {
                    value = default;
                    return false;
                }
                uint parsedValue = firstDigit;

                if (length < ByteOverflowLength)
                {
                    // Length is less than ByteOverflowLength; overflow is not possible
                    for (int index = 1; index < length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = (byte)(parsedValue);
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }
                else
                {
                    // Length is greater than ByteOverflowLength; overflow is only possible after ByteOverflowLength
                    // digits. There may be no overflow after ByteOverflowLength if there are leading zeroes.
                    for (int index = 1; index < ByteOverflowLength - 1; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = (byte)(parsedValue);
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                    for (int index = ByteOverflowLength - 1; index < length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = (byte)(parsedValue);
                            return true;
                        }
                        // If parsedValue > (byte.MaxValue / 10), any more appended digits will cause overflow.
                        // if parsedValue == (byte.MaxValue / 10), any nextDigit greater than 5 implies overflow.
                        if (parsedValue > byte.MaxValue / 10 || (parsedValue == byte.MaxValue / 10 && nextDigit > 5))
                        {
                            value = default;
                            return false;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }

                value = (byte)(parsedValue);
                return true;
            }

            public unsafe static bool TryParseByte(char* text, int length, out byte value, out int charsConsumed)
            {
                if (length < 1)
                {
                    charsConsumed = 0;
                    value = default;
                    return false;
                }

                // Parse the first digit separately. If invalid here, we need to return false.
                uint firstDigit = text[0] - 48u; // '0'
                if (firstDigit > 9)
                {
                    charsConsumed = 0;
                    value = default;
                    return false;
                }
                uint parsedValue = firstDigit;

                if (length < ByteOverflowLength)
                {
                    // Length is less than ByteOverflowLength; overflow is not possible
                    for (int index = 1; index < length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            charsConsumed = index;
                            value = (byte)(parsedValue);
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }
                else
                {
                    // Length is greater than ByteOverflowLength; overflow is only possible after ByteOverflowLength
                    // digits. There may be no overflow after ByteOverflowLength if there are leading zeroes.
                    for (int index = 1; index < ByteOverflowLength - 1; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            charsConsumed = index;
                            value = (byte)(parsedValue);
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                    for (int index = ByteOverflowLength - 1; index < length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            charsConsumed = index;
                            value = (byte)(parsedValue);
                            return true;
                        }
                        // If parsedValue > (byte.MaxValue / 10), any more appended digits will cause overflow.
                        // if parsedValue == (byte.MaxValue / 10), any nextDigit greater than 5 implies overflow.
                        if (parsedValue > byte.MaxValue / 10 || (parsedValue == byte.MaxValue / 10 && nextDigit > 5))
                        {
                            charsConsumed = 0;
                            value = default;
                            return false;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }

                charsConsumed = length;
                value = (byte)(parsedValue);
                return true;
            }

            public static bool TryParseByte(ReadOnlySpan<char> text, out byte value)
            {
                if (text.Length < 1)
                {
                    value = default;
                    return false;
                }

                // Parse the first digit separately. If invalid here, we need to return false.
                uint firstDigit = text[0] - 48u; // '0'
                if (firstDigit > 9)
                {
                    value = default;
                    return false;
                }
                uint parsedValue = firstDigit;

                if (text.Length < ByteOverflowLength)
                {
                    // Length is less than ByteOverflowLength; overflow is not possible
                    for (int index = 1; index < text.Length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = (byte)(parsedValue);
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }
                else
                {
                    // Length is greater than ByteOverflowLength; overflow is only possible after ByteOverflowLength
                    // digits. There may be no overflow after ByteOverflowLength if there are leading zeroes.
                    for (int index = 1; index < ByteOverflowLength - 1; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = (byte)(parsedValue);
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                    for (int index = ByteOverflowLength - 1; index < text.Length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = (byte)(parsedValue);
                            return true;
                        }
                        // If parsedValue > (byte.MaxValue / 10), any more appended digits will cause overflow.
                        // if parsedValue == (byte.MaxValue / 10), any nextDigit greater than 5 implies overflow.
                        if (parsedValue > byte.MaxValue / 10 || (parsedValue == byte.MaxValue / 10 && nextDigit > 5))
                        {
                            value = default;
                            return false;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }

                value = (byte)(parsedValue);
                return true;
            }

            public static bool TryParseByte(ReadOnlySpan<char> text, out byte value, out int charsConsumed)
            {
                if (text.Length < 1)
                {
                    charsConsumed = 0;
                    value = default;
                    return false;
                }

                // Parse the first digit separately. If invalid here, we need to return false.
                uint firstDigit = text[0] - 48u; // '0'
                if (firstDigit > 9)
                {
                    charsConsumed = 0;
                    value = default;
                    return false;
                }
                uint parsedValue = firstDigit;

                if (text.Length < ByteOverflowLength)
                {
                    // Length is less than ByteOverflowLength; overflow is not possible
                    for (int index = 1; index < text.Length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            charsConsumed = index;
                            value = (byte)(parsedValue);
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }
                else
                {
                    // Length is greater than ByteOverflowLength; overflow is only possible after ByteOverflowLength
                    // digits. There may be no overflow after ByteOverflowLength if there are leading zeroes.
                    for (int index = 1; index < ByteOverflowLength - 1; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            charsConsumed = index;
                            value = (byte)(parsedValue);
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                    for (int index = ByteOverflowLength - 1; index < text.Length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            charsConsumed = index;
                            value = (byte)(parsedValue);
                            return true;
                        }
                        // If parsedValue > (byte.MaxValue / 10), any more appended digits will cause overflow.
                        // if parsedValue == (byte.MaxValue / 10), any nextDigit greater than 5 implies overflow.
                        if (parsedValue > byte.MaxValue / 10 || (parsedValue == byte.MaxValue / 10 && nextDigit > 5))
                        {
                            charsConsumed = 0;
                            value = default;
                            return false;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }

                charsConsumed = text.Length;
                value = (byte)(parsedValue);
                return true;
            }

            #endregion

            #region UInt16
            public unsafe static bool TryParseUInt16(char* text, int length, out ushort value)
            {
                if (length < 1)
                {
                    value = default;
                    return false;
                }

                // Parse the first digit separately. If invalid here, we need to return false.
                uint firstDigit = text[0] - 48u; // '0'
                if (firstDigit > 9)
                {
                    value = default;
                    return false;
                }
                uint parsedValue = firstDigit;

                if (length < UInt16OverflowLength)
                {
                    // Length is less than UInt16OverflowLength; overflow is not possible
                    for (int index = 1; index < length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = (ushort)(parsedValue);
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }
                else
                {
                    // Length is greater than UInt16OverflowLength; overflow is only possible after UInt16OverflowLength
                    // digits. There may be no overflow after UInt16OverflowLength if there are leading zeroes.
                    for (int index = 1; index < UInt16OverflowLength - 1; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = (ushort)(parsedValue);
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                    for (int index = UInt16OverflowLength - 1; index < length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = (ushort)(parsedValue);
                            return true;
                        }
                        // If parsedValue > (ushort.MaxValue / 10), any more appended digits will cause overflow.
                        // if parsedValue == (ushort.MaxValue / 10), any nextDigit greater than 5 implies overflow.
                        if (parsedValue > ushort.MaxValue / 10 || (parsedValue == ushort.MaxValue / 10 && nextDigit > 5))
                        {
                            value = default;
                            return false;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }

                value = (ushort)(parsedValue);
                return true;
            }

            public unsafe static bool TryParseUInt16(char* text, int length, out ushort value, out int charsConsumed)
            {
                if (length < 1)
                {
                    charsConsumed = 0;
                    value = default;
                    return false;
                }

                // Parse the first digit separately. If invalid here, we need to return false.
                uint firstDigit = text[0] - 48u; // '0'
                if (firstDigit > 9)
                {
                    charsConsumed = 0;
                    value = default;
                    return false;
                }
                uint parsedValue = firstDigit;

                if (length < UInt16OverflowLength)
                {
                    // Length is less than UInt16OverflowLength; overflow is not possible
                    for (int index = 1; index < length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            charsConsumed = index;
                            value = (ushort)(parsedValue);
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }
                else
                {
                    // Length is greater than UInt16OverflowLength; overflow is only possible after UInt16OverflowLength
                    // digits. There may be no overflow after UInt16OverflowLength if there are leading zeroes.
                    for (int index = 1; index < UInt16OverflowLength - 1; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            charsConsumed = index;
                            value = (ushort)(parsedValue);
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                    for (int index = UInt16OverflowLength - 1; index < length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            charsConsumed = index;
                            value = (ushort)(parsedValue);
                            return true;
                        }
                        // If parsedValue > (ushort.MaxValue / 10), any more appended digits will cause overflow.
                        // if parsedValue == (ushort.MaxValue / 10), any nextDigit greater than 5 implies overflow.
                        if (parsedValue > ushort.MaxValue / 10 || (parsedValue == ushort.MaxValue / 10 && nextDigit > 5))
                        {
                            charsConsumed = 0;
                            value = default;
                            return false;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }

                charsConsumed = length;
                value = (ushort)(parsedValue);
                return true;
            }

            public static bool TryParseUInt16(ReadOnlySpan<char> text, out ushort value)
            {
                if (text.Length < 1)
                {
                    value = default;
                    return false;
                }

                // Parse the first digit separately. If invalid here, we need to return false.
                uint firstDigit = text[0] - 48u; // '0'
                if (firstDigit > 9)
                {
                    value = default;
                    return false;
                }
                uint parsedValue = firstDigit;

                if (text.Length < UInt16OverflowLength)
                {
                    // Length is less than UInt16OverflowLength; overflow is not possible
                    for (int index = 1; index < text.Length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = (ushort)(parsedValue);
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }
                else
                {
                    // Length is greater than UInt16OverflowLength; overflow is only possible after UInt16OverflowLength
                    // digits. There may be no overflow after UInt16OverflowLength if there are leading zeroes.
                    for (int index = 1; index < UInt16OverflowLength - 1; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = (ushort)(parsedValue);
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                    for (int index = UInt16OverflowLength - 1; index < text.Length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = (ushort)(parsedValue);
                            return true;
                        }
                        // If parsedValue > (ushort.MaxValue / 10), any more appended digits will cause overflow.
                        // if parsedValue == (ushort.MaxValue / 10), any nextDigit greater than 5 implies overflow.
                        if (parsedValue > ushort.MaxValue / 10 || (parsedValue == ushort.MaxValue / 10 && nextDigit > 5))
                        {
                            value = default;
                            return false;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }

                value = (ushort)(parsedValue);
                return true;
            }

            public static bool TryParseUInt16(ReadOnlySpan<char> text, out ushort value, out int charsConsumed)
            {
                if (text.Length < 1)
                {
                    charsConsumed = 0;
                    value = default;
                    return false;
                }

                // Parse the first digit separately. If invalid here, we need to return false.
                uint firstDigit = text[0] - 48u; // '0'
                if (firstDigit > 9)
                {
                    charsConsumed = 0;
                    value = default;
                    return false;
                }
                uint parsedValue = firstDigit;

                if (text.Length < UInt16OverflowLength)
                {
                    // Length is less than UInt16OverflowLength; overflow is not possible
                    for (int index = 1; index < text.Length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            charsConsumed = index;
                            value = (ushort)(parsedValue);
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }
                else
                {
                    // Length is greater than UInt16OverflowLength; overflow is only possible after UInt16OverflowLength
                    // digits. There may be no overflow after UInt16OverflowLength if there are leading zeroes.
                    for (int index = 1; index < UInt16OverflowLength - 1; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            charsConsumed = index;
                            value = (ushort)(parsedValue);
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                    for (int index = UInt16OverflowLength - 1; index < text.Length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            charsConsumed = index;
                            value = (ushort)(parsedValue);
                            return true;
                        }
                        // If parsedValue > (ushort.MaxValue / 10), any more appended digits will cause overflow.
                        // if parsedValue == (ushort.MaxValue / 10), any nextDigit greater than 5 implies overflow.
                        if (parsedValue > ushort.MaxValue / 10 || (parsedValue == ushort.MaxValue / 10 && nextDigit > 5))
                        {
                            charsConsumed = 0;
                            value = default;
                            return false;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }

                charsConsumed = text.Length;
                value = (ushort)(parsedValue);
                return true;
            }

            #endregion

            #region UInt32
            public unsafe static bool TryParseUInt32(char* text, int length, out uint value)
            {
                if (length < 1)
                {
                    value = default;
                    return false;
                }

                // Parse the first digit separately. If invalid here, we need to return false.
                uint firstDigit = text[0] - 48u; // '0'
                if (firstDigit > 9)
                {
                    value = default;
                    return false;
                }
                uint parsedValue = firstDigit;

                if (length < UInt32OverflowLength)
                {
                    // Length is less than UInt32OverflowLength; overflow is not possible
                    for (int index = 1; index < length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = parsedValue;
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }
                else
                {
                    // Length is greater than UInt32OverflowLength; overflow is only possible after UInt32OverflowLength
                    // digits. There may be no overflow after UInt32OverflowLength if there are leading zeroes.
                    for (int index = 1; index < UInt32OverflowLength - 1; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = parsedValue;
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                    for (int index = UInt32OverflowLength - 1; index < length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = parsedValue;
                            return true;
                        }
                        // If parsedValue > (uint.MaxValue / 10), any more appended digits will cause overflow.
                        // if parsedValue == (uint.MaxValue / 10), any nextDigit greater than 5 implies overflow.
                        if (parsedValue > uint.MaxValue / 10 || (parsedValue == uint.MaxValue / 10 && nextDigit > 5))
                        {
                            value = default;
                            return false;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }

                value = parsedValue;
                return true;
            }

            public unsafe static bool TryParseUInt32(char* text, int length, out uint value, out int charsConsumed)
            {
                if (length < 1)
                {
                    charsConsumed = 0;
                    value = default;
                    return false;
                }

                // Parse the first digit separately. If invalid here, we need to return false.
                uint firstDigit = text[0] - 48u; // '0'
                if (firstDigit > 9)
                {
                    charsConsumed = 0;
                    value = default;
                    return false;
                }
                uint parsedValue = firstDigit;

                if (length < UInt32OverflowLength)
                {
                    // Length is less than UInt32OverflowLength; overflow is not possible
                    for (int index = 1; index < length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            charsConsumed = index;
                            value = parsedValue;
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }
                else
                {
                    // Length is greater than UInt32OverflowLength; overflow is only possible after UInt32OverflowLength
                    // digits. There may be no overflow after UInt32OverflowLength if there are leading zeroes.
                    for (int index = 1; index < UInt32OverflowLength - 1; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            charsConsumed = index;
                            value = parsedValue;
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                    for (int index = UInt32OverflowLength - 1; index < length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            charsConsumed = index;
                            value = parsedValue;
                            return true;
                        }
                        // If parsedValue > (uint.MaxValue / 10), any more appended digits will cause overflow.
                        // if parsedValue == (uint.MaxValue / 10), any nextDigit greater than 5 implies overflow.
                        if (parsedValue > uint.MaxValue / 10 || (parsedValue == uint.MaxValue / 10 && nextDigit > 5))
                        {
                            charsConsumed = 0;
                            value = default;
                            return false;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }

                charsConsumed = length;
                value = parsedValue;
                return true;
            }

            public static bool TryParseUInt32(ReadOnlySpan<char> text, out uint value)
            {
                if (text.Length < 1)
                {
                    value = default;
                    return false;
                }

                // Parse the first digit separately. If invalid here, we need to return false.
                uint firstDigit = text[0] - 48u; // '0'
                if (firstDigit > 9)
                {
                    value = default;
                    return false;
                }
                uint parsedValue = firstDigit;

                if (text.Length < UInt32OverflowLength)
                {
                    // Length is less than UInt32OverflowLength; overflow is not possible
                    for (int index = 1; index < text.Length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = parsedValue;
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }
                else
                {
                    // Length is greater than UInt32OverflowLength; overflow is only possible after UInt32OverflowLength
                    // digits. There may be no overflow after UInt32OverflowLength if there are leading zeroes.
                    for (int index = 1; index < UInt32OverflowLength - 1; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = parsedValue;
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                    for (int index = UInt32OverflowLength - 1; index < text.Length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = parsedValue;
                            return true;
                        }
                        // If parsedValue > (uint.MaxValue / 10), any more appended digits will cause overflow.
                        // if parsedValue == (uint.MaxValue / 10), any nextDigit greater than 5 implies overflow.
                        if (parsedValue > uint.MaxValue / 10 || (parsedValue == uint.MaxValue / 10 && nextDigit > 5))
                        {
                            value = default;
                            return false;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }

                value = parsedValue;
                return true;
            }

            public static bool TryParseUInt32(ReadOnlySpan<char> text, out uint value, out int charsConsumed)
            {
                if (text.Length < 1)
                {
                    charsConsumed = 0;
                    value = default;
                    return false;
                }

                // Parse the first digit separately. If invalid here, we need to return false.
                uint firstDigit = text[0] - 48u; // '0'
                if (firstDigit > 9)
                {
                    charsConsumed = 0;
                    value = default;
                    return false;
                }
                uint parsedValue = firstDigit;

                if (text.Length < UInt32OverflowLength)
                {
                    // Length is less than UInt32OverflowLength; overflow is not possible
                    for (int index = 1; index < text.Length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            charsConsumed = index;
                            value = parsedValue;
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }
                else
                {
                    // Length is greater than UInt32OverflowLength; overflow is only possible after UInt32OverflowLength
                    // digits. There may be no overflow after UInt32OverflowLength if there are leading zeroes.
                    for (int index = 1; index < UInt32OverflowLength - 1; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            charsConsumed = index;
                            value = parsedValue;
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                    for (int index = UInt32OverflowLength - 1; index < text.Length; index++)
                    {
                        uint nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            charsConsumed = index;
                            value = parsedValue;
                            return true;
                        }
                        // If parsedValue > (uint.MaxValue / 10), any more appended digits will cause overflow.
                        // if parsedValue == (uint.MaxValue / 10), any nextDigit greater than 5 implies overflow.
                        if (parsedValue > uint.MaxValue / 10 || (parsedValue == uint.MaxValue / 10 && nextDigit > 5))
                        {
                            charsConsumed = 0;
                            value = default;
                            return false;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }

                charsConsumed = text.Length;
                value = parsedValue;
                return true;
            }

            #endregion

            #region UInt64
            public unsafe static bool TryParseUInt64(char* text, int length, out ulong value)
            {
                if (length < 1)
                {
                    value = default;
                    return false;
                }

                // Parse the first digit separately. If invalid here, we need to return false.
                ulong firstDigit = text[0] - 48u; // '0'
                if (firstDigit > 9)
                {
                    value = default;
                    return false;
                }
                ulong parsedValue = firstDigit;

                if (length < UInt64OverflowLength)
                {
                    // Length is less than UInt64OverflowLength; overflow is not possible
                    for (int index = 1; index < length; index++)
                    {
                        ulong nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = parsedValue;
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }
                else
                {
                    // Length is greater than UInt64OverflowLength; overflow is only possible after UInt64OverflowLength
                    // digits. There may be no overflow after UInt64OverflowLength if there are leading zeroes.
                    for (int index = 1; index < UInt64OverflowLength - 1; index++)
                    {
                        ulong nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = parsedValue;
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                    for (int index = UInt64OverflowLength - 1; index < length; index++)
                    {
                        ulong nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = parsedValue;
                            return true;
                        }
                        // If parsedValue > (ulong.MaxValue / 10), any more appended digits will cause overflow.
                        // if parsedValue == (ulong.MaxValue / 10), any nextDigit greater than 5 implies overflow.
                        if (parsedValue > ulong.MaxValue / 10 || (parsedValue == ulong.MaxValue / 10 && nextDigit > 5))
                        {
                            value = default;
                            return false;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }

                value = parsedValue;
                return true;
            }

            public unsafe static bool TryParseUInt64(char* text, int length, out ulong value, out int charsConsumed)
            {
                if (length < 1)
                {
                    charsConsumed = 0;
                    value = default;
                    return false;
                }

                // Parse the first digit separately. If invalid here, we need to return false.
                ulong firstDigit = text[0] - 48u; // '0'
                if (firstDigit > 9)
                {
                    charsConsumed = 0;
                    value = default;
                    return false;
                }
                ulong parsedValue = firstDigit;

                if (length < UInt64OverflowLength)
                {
                    // Length is less than UInt64OverflowLength; overflow is not possible
                    for (int index = 1; index < length; index++)
                    {
                        ulong nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            charsConsumed = index;
                            value = parsedValue;
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }
                else
                {
                    // Length is greater than UInt64OverflowLength; overflow is only possible after UInt64OverflowLength
                    // digits. There may be no overflow after UInt64OverflowLength if there are leading zeroes.
                    for (int index = 1; index < UInt64OverflowLength - 1; index++)
                    {
                        ulong nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            charsConsumed = index;
                            value = parsedValue;
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                    for (int index = UInt64OverflowLength - 1; index < length; index++)
                    {
                        ulong nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            charsConsumed = index;
                            value = parsedValue;
                            return true;
                        }
                        // If parsedValue > (ulong.MaxValue / 10), any more appended digits will cause overflow.
                        // if parsedValue == (ulong.MaxValue / 10), any nextDigit greater than 5 implies overflow.
                        if (parsedValue > ulong.MaxValue / 10 || (parsedValue == ulong.MaxValue / 10 && nextDigit > 5))
                        {
                            charsConsumed = 0;
                            value = default;
                            return false;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }

                charsConsumed = length;
                value = parsedValue;
                return true;
            }

            public static bool TryParseUInt64(ReadOnlySpan<char> text, out ulong value)
            {
                if (text.Length < 1)
                {
                    value = default;
                    return false;
                }

                // Parse the first digit separately. If invalid here, we need to return false.
                ulong firstDigit = text[0] - 48u; // '0'
                if (firstDigit > 9)
                {
                    value = default;
                    return false;
                }
                ulong parsedValue = firstDigit;

                if (text.Length < UInt64OverflowLength)
                {
                    // Length is less than UInt64OverflowLength; overflow is not possible
                    for (int index = 1; index < text.Length; index++)
                    {
                        ulong nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = parsedValue;
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }
                else
                {
                    // Length is greater than UInt64OverflowLength; overflow is only possible after UInt64OverflowLength
                    // digits. There may be no overflow after UInt64OverflowLength if there are leading zeroes.
                    for (int index = 1; index < UInt64OverflowLength - 1; index++)
                    {
                        ulong nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = parsedValue;
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                    for (int index = UInt64OverflowLength - 1; index < text.Length; index++)
                    {
                        ulong nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            value = parsedValue;
                            return true;
                        }
                        // If parsedValue > (ulong.MaxValue / 10), any more appended digits will cause overflow.
                        // if parsedValue == (ulong.MaxValue / 10), any nextDigit greater than 5 implies overflow.
                        if (parsedValue > ulong.MaxValue / 10 || (parsedValue == ulong.MaxValue / 10 && nextDigit > 5))
                        {
                            value = default;
                            return false;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }

                value = parsedValue;
                return true;
            }

            public static bool TryParseUInt64(ReadOnlySpan<char> text, out ulong value, out int charsConsumed)
            {
                if (text.Length < 1)
                {
                    charsConsumed = 0;
                    value = default;
                    return false;
                }

                // Parse the first digit separately. If invalid here, we need to return false.
                ulong firstDigit = text[0] - 48u; // '0'
                if (firstDigit > 9)
                {
                    charsConsumed = 0;
                    value = default;
                    return false;
                }
                ulong parsedValue = firstDigit;

                if (text.Length < UInt64OverflowLength)
                {
                    // Length is less than UInt64OverflowLength; overflow is not possible
                    for (int index = 1; index < text.Length; index++)
                    {
                        ulong nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            charsConsumed = index;
                            value = parsedValue;
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }
                else
                {
                    // Length is greater than UInt64OverflowLength; overflow is only possible after UInt64OverflowLength
                    // digits. There may be no overflow after UInt64OverflowLength if there are leading zeroes.
                    for (int index = 1; index < UInt64OverflowLength - 1; index++)
                    {
                        ulong nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            charsConsumed = index;
                            value = parsedValue;
                            return true;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                    for (int index = UInt64OverflowLength - 1; index < text.Length; index++)
                    {
                        ulong nextDigit = text[index] - 48u; // '0'
                        if (nextDigit > 9)
                        {
                            charsConsumed = index;
                            value = parsedValue;
                            return true;
                        }
                        // If parsedValue > (ulong.MaxValue / 10), any more appended digits will cause overflow.
                        // if parsedValue == (ulong.MaxValue / 10), any nextDigit greater than 5 implies overflow.
                        if (parsedValue > ulong.MaxValue / 10 || (parsedValue == ulong.MaxValue / 10 && nextDigit > 5))
                        {
                            charsConsumed = 0;
                            value = default;
                            return false;
                        }
                        parsedValue = parsedValue * 10 + nextDigit;
                    }
                }

                charsConsumed = text.Length;
                value = parsedValue;
                return true;
            }

            #endregion

        }
    }
}
