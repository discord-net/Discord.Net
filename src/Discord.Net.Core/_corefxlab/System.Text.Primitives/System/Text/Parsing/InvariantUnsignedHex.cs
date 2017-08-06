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
            public static partial class Hex
            {
                #region Byte
                public unsafe static bool TryParseByte(byte* text, int length, out byte value)
                {
                    if (length < 1)
                    {
                        value = default;
                        return false;
                    }
                    byte nextCharacter;
                    byte nextDigit;

                    // Cache s_hexLookup in order to avoid static constructor checks
                    byte[] hexLookup = s_HexLookup;

                    // Parse the first digit separately. If invalid here, we need to return false.
                    nextCharacter = text[0];
                    nextDigit = hexLookup[nextCharacter];
                    if (nextDigit == 0xFF)
                    {
                        value = default;
                        return false;
                    }
                    uint parsedValue = nextDigit;

                    if (length <= ByteOverflowLengthHex)
                    {
                        // Length is less than or equal to ByteOverflowLengthHex; overflow is not possible
                        for (int index = 1; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = (byte)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than ByteOverflowLengthHex; overflow is only possible after ByteOverflowLengthHex
                        // digits. There may be no overflow after ByteOverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < ByteOverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = (byte)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = ByteOverflowLengthHex; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = (byte)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than byte.MaxValue / 0x10, there will be overflow
                            if (parsedValue > byte.MaxValue / 0x10)
                            {
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
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
                    byte nextCharacter;
                    byte nextDigit;

                    // Cache s_hexLookup in order to avoid static constructor checks
                    byte[] hexLookup = s_HexLookup;

                    // Parse the first digit separately. If invalid here, we need to return false.
                    nextCharacter = text[0];
                    nextDigit = hexLookup[nextCharacter];
                    if (nextDigit == 0xFF)
                    {
                        bytesConsumed = 0;
                        value = default;
                        return false;
                    }
                    uint parsedValue = nextDigit;

                    if (length <= ByteOverflowLengthHex)
                    {
                        // Length is less than or equal to ByteOverflowLengthHex; overflow is not possible
                        for (int index = 1; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = (byte)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than ByteOverflowLengthHex; overflow is only possible after ByteOverflowLengthHex
                        // digits. There may be no overflow after ByteOverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < ByteOverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = (byte)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = ByteOverflowLengthHex; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = (byte)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than byte.MaxValue / 0x10, there will be overflow
                            if (parsedValue > byte.MaxValue / 0x10)
                            {
                                bytesConsumed = 0;
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
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
                    byte nextCharacter;
                    byte nextDigit;

                    // Cache s_hexLookup in order to avoid static constructor checks
                    byte[] hexLookup = s_HexLookup;

                    // Parse the first digit separately. If invalid here, we need to return false.
                    nextCharacter = text[0];
                    nextDigit = hexLookup[nextCharacter];
                    if (nextDigit == 0xFF)
                    {
                        value = default;
                        return false;
                    }
                    uint parsedValue = nextDigit;

                    if (text.Length <= ByteOverflowLengthHex)
                    {
                        // Length is less than or equal to ByteOverflowLengthHex; overflow is not possible
                        for (int index = 1; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = (byte)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than ByteOverflowLengthHex; overflow is only possible after ByteOverflowLengthHex
                        // digits. There may be no overflow after ByteOverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < ByteOverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = (byte)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = ByteOverflowLengthHex; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = (byte)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than byte.MaxValue / 0x10, there will be overflow
                            if (parsedValue > byte.MaxValue / 0x10)
                            {
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
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
                    byte nextCharacter;
                    byte nextDigit;

                    // Cache s_hexLookup in order to avoid static constructor checks
                    byte[] hexLookup = s_HexLookup;

                    // Parse the first digit separately. If invalid here, we need to return false.
                    nextCharacter = text[0];
                    nextDigit = hexLookup[nextCharacter];
                    if (nextDigit == 0xFF)
                    {
                        bytesConsumed = 0;
                        value = default;
                        return false;
                    }
                    uint parsedValue = nextDigit;

                    if (text.Length <= ByteOverflowLengthHex)
                    {
                        // Length is less than or equal to ByteOverflowLengthHex; overflow is not possible
                        for (int index = 1; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = (byte)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than ByteOverflowLengthHex; overflow is only possible after ByteOverflowLengthHex
                        // digits. There may be no overflow after ByteOverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < ByteOverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = (byte)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = ByteOverflowLengthHex; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = (byte)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than byte.MaxValue / 0x10, there will be overflow
                            if (parsedValue > byte.MaxValue / 0x10)
                            {
                                bytesConsumed = 0;
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
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
                    byte nextCharacter;
                    byte nextDigit;

                    // Cache s_hexLookup in order to avoid static constructor checks
                    byte[] hexLookup = s_HexLookup;

                    // Parse the first digit separately. If invalid here, we need to return false.
                    nextCharacter = text[0];
                    nextDigit = hexLookup[nextCharacter];
                    if (nextDigit == 0xFF)
                    {
                        value = default;
                        return false;
                    }
                    uint parsedValue = nextDigit;

                    if (length <= UInt16OverflowLengthHex)
                    {
                        // Length is less than or equal to UInt16OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = (ushort)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than UInt16OverflowLengthHex; overflow is only possible after UInt16OverflowLengthHex
                        // digits. There may be no overflow after UInt16OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < UInt16OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = (ushort)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = UInt16OverflowLengthHex; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = (ushort)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than ushort.MaxValue / 0x10, there will be overflow
                            if (parsedValue > ushort.MaxValue / 0x10)
                            {
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
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
                    byte nextCharacter;
                    byte nextDigit;

                    // Cache s_hexLookup in order to avoid static constructor checks
                    byte[] hexLookup = s_HexLookup;

                    // Parse the first digit separately. If invalid here, we need to return false.
                    nextCharacter = text[0];
                    nextDigit = hexLookup[nextCharacter];
                    if (nextDigit == 0xFF)
                    {
                        bytesConsumed = 0;
                        value = default;
                        return false;
                    }
                    uint parsedValue = nextDigit;

                    if (length <= UInt16OverflowLengthHex)
                    {
                        // Length is less than or equal to UInt16OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = (ushort)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than UInt16OverflowLengthHex; overflow is only possible after UInt16OverflowLengthHex
                        // digits. There may be no overflow after UInt16OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < UInt16OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = (ushort)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = UInt16OverflowLengthHex; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = (ushort)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than ushort.MaxValue / 0x10, there will be overflow
                            if (parsedValue > ushort.MaxValue / 0x10)
                            {
                                bytesConsumed = 0;
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
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
                    byte nextCharacter;
                    byte nextDigit;

                    // Cache s_hexLookup in order to avoid static constructor checks
                    byte[] hexLookup = s_HexLookup;

                    // Parse the first digit separately. If invalid here, we need to return false.
                    nextCharacter = text[0];
                    nextDigit = hexLookup[nextCharacter];
                    if (nextDigit == 0xFF)
                    {
                        value = default;
                        return false;
                    }
                    uint parsedValue = nextDigit;

                    if (text.Length <= UInt16OverflowLengthHex)
                    {
                        // Length is less than or equal to UInt16OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = (ushort)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than UInt16OverflowLengthHex; overflow is only possible after UInt16OverflowLengthHex
                        // digits. There may be no overflow after UInt16OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < UInt16OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = (ushort)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = UInt16OverflowLengthHex; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = (ushort)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than ushort.MaxValue / 0x10, there will be overflow
                            if (parsedValue > ushort.MaxValue / 0x10)
                            {
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
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
                    byte nextCharacter;
                    byte nextDigit;

                    // Cache s_hexLookup in order to avoid static constructor checks
                    byte[] hexLookup = s_HexLookup;

                    // Parse the first digit separately. If invalid here, we need to return false.
                    nextCharacter = text[0];
                    nextDigit = hexLookup[nextCharacter];
                    if (nextDigit == 0xFF)
                    {
                        bytesConsumed = 0;
                        value = default;
                        return false;
                    }
                    uint parsedValue = nextDigit;

                    if (text.Length <= UInt16OverflowLengthHex)
                    {
                        // Length is less than or equal to UInt16OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = (ushort)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than UInt16OverflowLengthHex; overflow is only possible after UInt16OverflowLengthHex
                        // digits. There may be no overflow after UInt16OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < UInt16OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = (ushort)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = UInt16OverflowLengthHex; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = (ushort)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than ushort.MaxValue / 0x10, there will be overflow
                            if (parsedValue > ushort.MaxValue / 0x10)
                            {
                                bytesConsumed = 0;
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
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
                    byte nextCharacter;
                    byte nextDigit;

                    // Cache s_hexLookup in order to avoid static constructor checks
                    byte[] hexLookup = s_HexLookup;

                    // Parse the first digit separately. If invalid here, we need to return false.
                    nextCharacter = text[0];
                    nextDigit = hexLookup[nextCharacter];
                    if (nextDigit == 0xFF)
                    {
                        value = default;
                        return false;
                    }
                    uint parsedValue = nextDigit;

                    if (length <= UInt32OverflowLengthHex)
                    {
                        // Length is less than or equal to UInt32OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = parsedValue;
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than UInt32OverflowLengthHex; overflow is only possible after UInt32OverflowLengthHex
                        // digits. There may be no overflow after UInt32OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < UInt32OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = parsedValue;
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = UInt32OverflowLengthHex; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = parsedValue;
                                return true;
                            }
                            // If we try to append a digit to anything larger than uint.MaxValue / 0x10, there will be overflow
                            if (parsedValue > uint.MaxValue / 0x10)
                            {
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
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
                    byte nextCharacter;
                    byte nextDigit;

                    // Cache s_hexLookup in order to avoid static constructor checks
                    byte[] hexLookup = s_HexLookup;

                    // Parse the first digit separately. If invalid here, we need to return false.
                    nextCharacter = text[0];
                    nextDigit = hexLookup[nextCharacter];
                    if (nextDigit == 0xFF)
                    {
                        bytesConsumed = 0;
                        value = default;
                        return false;
                    }
                    uint parsedValue = nextDigit;

                    if (length <= UInt32OverflowLengthHex)
                    {
                        // Length is less than or equal to UInt32OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = parsedValue;
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than UInt32OverflowLengthHex; overflow is only possible after UInt32OverflowLengthHex
                        // digits. There may be no overflow after UInt32OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < UInt32OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = parsedValue;
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = UInt32OverflowLengthHex; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = parsedValue;
                                return true;
                            }
                            // If we try to append a digit to anything larger than uint.MaxValue / 0x10, there will be overflow
                            if (parsedValue > uint.MaxValue / 0x10)
                            {
                                bytesConsumed = 0;
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
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
                    byte nextCharacter;
                    byte nextDigit;

                    // Cache s_hexLookup in order to avoid static constructor checks
                    byte[] hexLookup = s_HexLookup;

                    // Parse the first digit separately. If invalid here, we need to return false.
                    nextCharacter = text[0];
                    nextDigit = hexLookup[nextCharacter];
                    if (nextDigit == 0xFF)
                    {
                        value = default;
                        return false;
                    }
                    uint parsedValue = nextDigit;

                    if (text.Length <= UInt32OverflowLengthHex)
                    {
                        // Length is less than or equal to UInt32OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = parsedValue;
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than UInt32OverflowLengthHex; overflow is only possible after UInt32OverflowLengthHex
                        // digits. There may be no overflow after UInt32OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < UInt32OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = parsedValue;
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = UInt32OverflowLengthHex; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = parsedValue;
                                return true;
                            }
                            // If we try to append a digit to anything larger than uint.MaxValue / 0x10, there will be overflow
                            if (parsedValue > uint.MaxValue / 0x10)
                            {
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
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
                    byte nextCharacter;
                    byte nextDigit;

                    // Cache s_hexLookup in order to avoid static constructor checks
                    byte[] hexLookup = s_HexLookup;

                    // Parse the first digit separately. If invalid here, we need to return false.
                    nextCharacter = text[0];
                    nextDigit = hexLookup[nextCharacter];
                    if (nextDigit == 0xFF)
                    {
                        bytesConsumed = 0;
                        value = default;
                        return false;
                    }
                    uint parsedValue = nextDigit;

                    if (text.Length <= UInt32OverflowLengthHex)
                    {
                        // Length is less than or equal to UInt32OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = parsedValue;
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than UInt32OverflowLengthHex; overflow is only possible after UInt32OverflowLengthHex
                        // digits. There may be no overflow after UInt32OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < UInt32OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = parsedValue;
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = UInt32OverflowLengthHex; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = parsedValue;
                                return true;
                            }
                            // If we try to append a digit to anything larger than uint.MaxValue / 0x10, there will be overflow
                            if (parsedValue > uint.MaxValue / 0x10)
                            {
                                bytesConsumed = 0;
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
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
                    byte nextCharacter;
                    byte nextDigit;

                    // Cache s_hexLookup in order to avoid static constructor checks
                    byte[] hexLookup = s_HexLookup;

                    // Parse the first digit separately. If invalid here, we need to return false.
                    nextCharacter = text[0];
                    nextDigit = hexLookup[nextCharacter];
                    if (nextDigit == 0xFF)
                    {
                        value = default;
                        return false;
                    }
                    ulong parsedValue = nextDigit;

                    if (length <= UInt64OverflowLengthHex)
                    {
                        // Length is less than or equal to UInt64OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = parsedValue;
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than UInt64OverflowLengthHex; overflow is only possible after UInt64OverflowLengthHex
                        // digits. There may be no overflow after UInt64OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < UInt64OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = parsedValue;
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = UInt64OverflowLengthHex; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = parsedValue;
                                return true;
                            }
                            // If we try to append a digit to anything larger than ulong.MaxValue / 0x10, there will be overflow
                            if (parsedValue > ulong.MaxValue / 0x10)
                            {
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
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
                    byte nextCharacter;
                    byte nextDigit;

                    // Cache s_hexLookup in order to avoid static constructor checks
                    byte[] hexLookup = s_HexLookup;

                    // Parse the first digit separately. If invalid here, we need to return false.
                    nextCharacter = text[0];
                    nextDigit = hexLookup[nextCharacter];
                    if (nextDigit == 0xFF)
                    {
                        bytesConsumed = 0;
                        value = default;
                        return false;
                    }
                    ulong parsedValue = nextDigit;

                    if (length <= UInt64OverflowLengthHex)
                    {
                        // Length is less than or equal to UInt64OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = parsedValue;
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than UInt64OverflowLengthHex; overflow is only possible after UInt64OverflowLengthHex
                        // digits. There may be no overflow after UInt64OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < UInt64OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = parsedValue;
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = UInt64OverflowLengthHex; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = parsedValue;
                                return true;
                            }
                            // If we try to append a digit to anything larger than ulong.MaxValue / 0x10, there will be overflow
                            if (parsedValue > ulong.MaxValue / 0x10)
                            {
                                bytesConsumed = 0;
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
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
                    byte nextCharacter;
                    byte nextDigit;

                    // Cache s_hexLookup in order to avoid static constructor checks
                    byte[] hexLookup = s_HexLookup;

                    // Parse the first digit separately. If invalid here, we need to return false.
                    nextCharacter = text[0];
                    nextDigit = hexLookup[nextCharacter];
                    if (nextDigit == 0xFF)
                    {
                        value = default;
                        return false;
                    }
                    ulong parsedValue = nextDigit;

                    if (text.Length <= UInt64OverflowLengthHex)
                    {
                        // Length is less than or equal to UInt64OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = parsedValue;
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than UInt64OverflowLengthHex; overflow is only possible after UInt64OverflowLengthHex
                        // digits. There may be no overflow after UInt64OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < UInt64OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = parsedValue;
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = UInt64OverflowLengthHex; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = parsedValue;
                                return true;
                            }
                            // If we try to append a digit to anything larger than ulong.MaxValue / 0x10, there will be overflow
                            if (parsedValue > ulong.MaxValue / 0x10)
                            {
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
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
                    byte nextCharacter;
                    byte nextDigit;

                    // Cache s_hexLookup in order to avoid static constructor checks
                    byte[] hexLookup = s_HexLookup;

                    // Parse the first digit separately. If invalid here, we need to return false.
                    nextCharacter = text[0];
                    nextDigit = hexLookup[nextCharacter];
                    if (nextDigit == 0xFF)
                    {
                        bytesConsumed = 0;
                        value = default;
                        return false;
                    }
                    ulong parsedValue = nextDigit;

                    if (text.Length <= UInt64OverflowLengthHex)
                    {
                        // Length is less than or equal to UInt64OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = parsedValue;
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than UInt64OverflowLengthHex; overflow is only possible after UInt64OverflowLengthHex
                        // digits. There may be no overflow after UInt64OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < UInt64OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = parsedValue;
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = UInt64OverflowLengthHex; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = parsedValue;
                                return true;
                            }
                            // If we try to append a digit to anything larger than ulong.MaxValue / 0x10, there will be overflow
                            if (parsedValue > ulong.MaxValue / 0x10)
                            {
                                bytesConsumed = 0;
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }

                    bytesConsumed = text.Length;
                    value = parsedValue;
                    return true;
                }

                #endregion

            }
        }
        public static partial class InvariantUtf16
        {
            public static partial class Hex
            {
                #region Byte
                public unsafe static bool TryParseByte(char* text, int length, out byte value)
                {
                    if (length < 1)
                    {
                        value = default;
                        return false;
                    }
                    char nextCharacter;
                    byte nextDigit;

                    // Cache s_hexLookup in order to avoid static constructor checks
                    byte[] hexLookup = s_HexLookup;

                    // Parse the first digit separately. If invalid here, we need to return false.
                    nextCharacter = text[0];
                    nextDigit = hexLookup[(byte)nextCharacter];
                    if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                    {
                        value = default;
                        return false;
                    }
                    uint parsedValue = nextDigit;

                    if (length <= ByteOverflowLengthHex)
                    {
                        // Length is less than or equal to ByteOverflowLengthHex; overflow is not possible
                        for (int index = 1; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = (byte)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than ByteOverflowLengthHex; overflow is only possible after ByteOverflowLengthHex
                        // digits. There may be no overflow after ByteOverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < ByteOverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = (byte)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = ByteOverflowLengthHex; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = (byte)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than byte.MaxValue / 0x10, there will be overflow
                            if (parsedValue > byte.MaxValue / 0x10)
                            {
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
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
                    char nextCharacter;
                    byte nextDigit;

                    // Cache s_hexLookup in order to avoid static constructor checks
                    byte[] hexLookup = s_HexLookup;

                    // Parse the first digit separately. If invalid here, we need to return false.
                    nextCharacter = text[0];
                    nextDigit = hexLookup[(byte)nextCharacter];
                    if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                    {
                        charsConsumed = 0;
                        value = default;
                        return false;
                    }
                    uint parsedValue = nextDigit;

                    if (length <= ByteOverflowLengthHex)
                    {
                        // Length is less than or equal to ByteOverflowLengthHex; overflow is not possible
                        for (int index = 1; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = (byte)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than ByteOverflowLengthHex; overflow is only possible after ByteOverflowLengthHex
                        // digits. There may be no overflow after ByteOverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < ByteOverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = (byte)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = ByteOverflowLengthHex; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = (byte)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than byte.MaxValue / 0x10, there will be overflow
                            if (parsedValue > byte.MaxValue / 0x10)
                            {
                                charsConsumed = 0;
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
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
                    char nextCharacter;
                    byte nextDigit;

                    // Cache s_hexLookup in order to avoid static constructor checks
                    byte[] hexLookup = s_HexLookup;

                    // Parse the first digit separately. If invalid here, we need to return false.
                    nextCharacter = text[0];
                    nextDigit = hexLookup[(byte)nextCharacter];
                    if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                    {
                        value = default;
                        return false;
                    }
                    uint parsedValue = nextDigit;

                    if (text.Length <= ByteOverflowLengthHex)
                    {
                        // Length is less than or equal to ByteOverflowLengthHex; overflow is not possible
                        for (int index = 1; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = (byte)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than ByteOverflowLengthHex; overflow is only possible after ByteOverflowLengthHex
                        // digits. There may be no overflow after ByteOverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < ByteOverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = (byte)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = ByteOverflowLengthHex; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = (byte)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than byte.MaxValue / 0x10, there will be overflow
                            if (parsedValue > byte.MaxValue / 0x10)
                            {
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
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
                    char nextCharacter;
                    byte nextDigit;

                    // Cache s_hexLookup in order to avoid static constructor checks
                    byte[] hexLookup = s_HexLookup;

                    // Parse the first digit separately. If invalid here, we need to return false.
                    nextCharacter = text[0];
                    nextDigit = hexLookup[(byte)nextCharacter];
                    if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                    {
                        charsConsumed = 0;
                        value = default;
                        return false;
                    }
                    uint parsedValue = nextDigit;

                    if (text.Length <= ByteOverflowLengthHex)
                    {
                        // Length is less than or equal to ByteOverflowLengthHex; overflow is not possible
                        for (int index = 1; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = (byte)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than ByteOverflowLengthHex; overflow is only possible after ByteOverflowLengthHex
                        // digits. There may be no overflow after ByteOverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < ByteOverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = (byte)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = ByteOverflowLengthHex; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = (byte)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than byte.MaxValue / 0x10, there will be overflow
                            if (parsedValue > byte.MaxValue / 0x10)
                            {
                                charsConsumed = 0;
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
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
                    char nextCharacter;
                    byte nextDigit;

                    // Cache s_hexLookup in order to avoid static constructor checks
                    byte[] hexLookup = s_HexLookup;

                    // Parse the first digit separately. If invalid here, we need to return false.
                    nextCharacter = text[0];
                    nextDigit = hexLookup[(byte)nextCharacter];
                    if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                    {
                        value = default;
                        return false;
                    }
                    uint parsedValue = nextDigit;

                    if (length <= UInt16OverflowLengthHex)
                    {
                        // Length is less than or equal to UInt16OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = (ushort)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than UInt16OverflowLengthHex; overflow is only possible after UInt16OverflowLengthHex
                        // digits. There may be no overflow after UInt16OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < UInt16OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = (ushort)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = UInt16OverflowLengthHex; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = (ushort)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than ushort.MaxValue / 0x10, there will be overflow
                            if (parsedValue > ushort.MaxValue / 0x10)
                            {
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
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
                    char nextCharacter;
                    byte nextDigit;

                    // Cache s_hexLookup in order to avoid static constructor checks
                    byte[] hexLookup = s_HexLookup;

                    // Parse the first digit separately. If invalid here, we need to return false.
                    nextCharacter = text[0];
                    nextDigit = hexLookup[(byte)nextCharacter];
                    if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                    {
                        charsConsumed = 0;
                        value = default;
                        return false;
                    }
                    uint parsedValue = nextDigit;

                    if (length <= UInt16OverflowLengthHex)
                    {
                        // Length is less than or equal to UInt16OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = (ushort)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than UInt16OverflowLengthHex; overflow is only possible after UInt16OverflowLengthHex
                        // digits. There may be no overflow after UInt16OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < UInt16OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = (ushort)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = UInt16OverflowLengthHex; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = (ushort)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than ushort.MaxValue / 0x10, there will be overflow
                            if (parsedValue > ushort.MaxValue / 0x10)
                            {
                                charsConsumed = 0;
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
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
                    char nextCharacter;
                    byte nextDigit;

                    // Cache s_hexLookup in order to avoid static constructor checks
                    byte[] hexLookup = s_HexLookup;

                    // Parse the first digit separately. If invalid here, we need to return false.
                    nextCharacter = text[0];
                    nextDigit = hexLookup[(byte)nextCharacter];
                    if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                    {
                        value = default;
                        return false;
                    }
                    uint parsedValue = nextDigit;

                    if (text.Length <= UInt16OverflowLengthHex)
                    {
                        // Length is less than or equal to UInt16OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = (ushort)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than UInt16OverflowLengthHex; overflow is only possible after UInt16OverflowLengthHex
                        // digits. There may be no overflow after UInt16OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < UInt16OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = (ushort)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = UInt16OverflowLengthHex; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = (ushort)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than ushort.MaxValue / 0x10, there will be overflow
                            if (parsedValue > ushort.MaxValue / 0x10)
                            {
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
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
                    char nextCharacter;
                    byte nextDigit;

                    // Cache s_hexLookup in order to avoid static constructor checks
                    byte[] hexLookup = s_HexLookup;

                    // Parse the first digit separately. If invalid here, we need to return false.
                    nextCharacter = text[0];
                    nextDigit = hexLookup[(byte)nextCharacter];
                    if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                    {
                        charsConsumed = 0;
                        value = default;
                        return false;
                    }
                    uint parsedValue = nextDigit;

                    if (text.Length <= UInt16OverflowLengthHex)
                    {
                        // Length is less than or equal to UInt16OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = (ushort)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than UInt16OverflowLengthHex; overflow is only possible after UInt16OverflowLengthHex
                        // digits. There may be no overflow after UInt16OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < UInt16OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = (ushort)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = UInt16OverflowLengthHex; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = (ushort)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than ushort.MaxValue / 0x10, there will be overflow
                            if (parsedValue > ushort.MaxValue / 0x10)
                            {
                                charsConsumed = 0;
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
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
                    char nextCharacter;
                    byte nextDigit;

                    // Cache s_hexLookup in order to avoid static constructor checks
                    byte[] hexLookup = s_HexLookup;

                    // Parse the first digit separately. If invalid here, we need to return false.
                    nextCharacter = text[0];
                    nextDigit = hexLookup[(byte)nextCharacter];
                    if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                    {
                        value = default;
                        return false;
                    }
                    uint parsedValue = nextDigit;

                    if (length <= UInt32OverflowLengthHex)
                    {
                        // Length is less than or equal to UInt32OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = parsedValue;
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than UInt32OverflowLengthHex; overflow is only possible after UInt32OverflowLengthHex
                        // digits. There may be no overflow after UInt32OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < UInt32OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = parsedValue;
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = UInt32OverflowLengthHex; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = parsedValue;
                                return true;
                            }
                            // If we try to append a digit to anything larger than uint.MaxValue / 0x10, there will be overflow
                            if (parsedValue > uint.MaxValue / 0x10)
                            {
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
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
                    char nextCharacter;
                    byte nextDigit;

                    // Cache s_hexLookup in order to avoid static constructor checks
                    byte[] hexLookup = s_HexLookup;

                    // Parse the first digit separately. If invalid here, we need to return false.
                    nextCharacter = text[0];
                    nextDigit = hexLookup[(byte)nextCharacter];
                    if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                    {
                        charsConsumed = 0;
                        value = default;
                        return false;
                    }
                    uint parsedValue = nextDigit;

                    if (length <= UInt32OverflowLengthHex)
                    {
                        // Length is less than or equal to UInt32OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = parsedValue;
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than UInt32OverflowLengthHex; overflow is only possible after UInt32OverflowLengthHex
                        // digits. There may be no overflow after UInt32OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < UInt32OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = parsedValue;
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = UInt32OverflowLengthHex; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = parsedValue;
                                return true;
                            }
                            // If we try to append a digit to anything larger than uint.MaxValue / 0x10, there will be overflow
                            if (parsedValue > uint.MaxValue / 0x10)
                            {
                                charsConsumed = 0;
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
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
                    char nextCharacter;
                    byte nextDigit;

                    // Cache s_hexLookup in order to avoid static constructor checks
                    byte[] hexLookup = s_HexLookup;

                    // Parse the first digit separately. If invalid here, we need to return false.
                    nextCharacter = text[0];
                    nextDigit = hexLookup[(byte)nextCharacter];
                    if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                    {
                        value = default;
                        return false;
                    }
                    uint parsedValue = nextDigit;

                    if (text.Length <= UInt32OverflowLengthHex)
                    {
                        // Length is less than or equal to UInt32OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = parsedValue;
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than UInt32OverflowLengthHex; overflow is only possible after UInt32OverflowLengthHex
                        // digits. There may be no overflow after UInt32OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < UInt32OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = parsedValue;
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = UInt32OverflowLengthHex; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = parsedValue;
                                return true;
                            }
                            // If we try to append a digit to anything larger than uint.MaxValue / 0x10, there will be overflow
                            if (parsedValue > uint.MaxValue / 0x10)
                            {
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
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
                    char nextCharacter;
                    byte nextDigit;

                    // Cache s_hexLookup in order to avoid static constructor checks
                    byte[] hexLookup = s_HexLookup;

                    // Parse the first digit separately. If invalid here, we need to return false.
                    nextCharacter = text[0];
                    nextDigit = hexLookup[(byte)nextCharacter];
                    if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                    {
                        charsConsumed = 0;
                        value = default;
                        return false;
                    }
                    uint parsedValue = nextDigit;

                    if (text.Length <= UInt32OverflowLengthHex)
                    {
                        // Length is less than or equal to UInt32OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = parsedValue;
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than UInt32OverflowLengthHex; overflow is only possible after UInt32OverflowLengthHex
                        // digits. There may be no overflow after UInt32OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < UInt32OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = parsedValue;
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = UInt32OverflowLengthHex; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = parsedValue;
                                return true;
                            }
                            // If we try to append a digit to anything larger than uint.MaxValue / 0x10, there will be overflow
                            if (parsedValue > uint.MaxValue / 0x10)
                            {
                                charsConsumed = 0;
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
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
                    char nextCharacter;
                    byte nextDigit;

                    // Cache s_hexLookup in order to avoid static constructor checks
                    byte[] hexLookup = s_HexLookup;

                    // Parse the first digit separately. If invalid here, we need to return false.
                    nextCharacter = text[0];
                    nextDigit = hexLookup[(byte)nextCharacter];
                    if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                    {
                        value = default;
                        return false;
                    }
                    ulong parsedValue = nextDigit;

                    if (length <= UInt64OverflowLengthHex)
                    {
                        // Length is less than or equal to UInt64OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = parsedValue;
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than UInt64OverflowLengthHex; overflow is only possible after UInt64OverflowLengthHex
                        // digits. There may be no overflow after UInt64OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < UInt64OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = parsedValue;
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = UInt64OverflowLengthHex; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = parsedValue;
                                return true;
                            }
                            // If we try to append a digit to anything larger than ulong.MaxValue / 0x10, there will be overflow
                            if (parsedValue > ulong.MaxValue / 0x10)
                            {
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
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
                    char nextCharacter;
                    byte nextDigit;

                    // Cache s_hexLookup in order to avoid static constructor checks
                    byte[] hexLookup = s_HexLookup;

                    // Parse the first digit separately. If invalid here, we need to return false.
                    nextCharacter = text[0];
                    nextDigit = hexLookup[(byte)nextCharacter];
                    if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                    {
                        charsConsumed = 0;
                        value = default;
                        return false;
                    }
                    ulong parsedValue = nextDigit;

                    if (length <= UInt64OverflowLengthHex)
                    {
                        // Length is less than or equal to UInt64OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = parsedValue;
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than UInt64OverflowLengthHex; overflow is only possible after UInt64OverflowLengthHex
                        // digits. There may be no overflow after UInt64OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < UInt64OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = parsedValue;
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = UInt64OverflowLengthHex; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = parsedValue;
                                return true;
                            }
                            // If we try to append a digit to anything larger than ulong.MaxValue / 0x10, there will be overflow
                            if (parsedValue > ulong.MaxValue / 0x10)
                            {
                                charsConsumed = 0;
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
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
                    char nextCharacter;
                    byte nextDigit;

                    // Cache s_hexLookup in order to avoid static constructor checks
                    byte[] hexLookup = s_HexLookup;

                    // Parse the first digit separately. If invalid here, we need to return false.
                    nextCharacter = text[0];
                    nextDigit = hexLookup[(byte)nextCharacter];
                    if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                    {
                        value = default;
                        return false;
                    }
                    ulong parsedValue = nextDigit;

                    if (text.Length <= UInt64OverflowLengthHex)
                    {
                        // Length is less than or equal to UInt64OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = parsedValue;
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than UInt64OverflowLengthHex; overflow is only possible after UInt64OverflowLengthHex
                        // digits. There may be no overflow after UInt64OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < UInt64OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = parsedValue;
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = UInt64OverflowLengthHex; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = parsedValue;
                                return true;
                            }
                            // If we try to append a digit to anything larger than ulong.MaxValue / 0x10, there will be overflow
                            if (parsedValue > ulong.MaxValue / 0x10)
                            {
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
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
                    char nextCharacter;
                    byte nextDigit;

                    // Cache s_hexLookup in order to avoid static constructor checks
                    byte[] hexLookup = s_HexLookup;

                    // Parse the first digit separately. If invalid here, we need to return false.
                    nextCharacter = text[0];
                    nextDigit = hexLookup[(byte)nextCharacter];
                    if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                    {
                        charsConsumed = 0;
                        value = default;
                        return false;
                    }
                    ulong parsedValue = nextDigit;

                    if (text.Length <= UInt64OverflowLengthHex)
                    {
                        // Length is less than or equal to UInt64OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = parsedValue;
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than UInt64OverflowLengthHex; overflow is only possible after UInt64OverflowLengthHex
                        // digits. There may be no overflow after UInt64OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < UInt64OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = parsedValue;
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = UInt64OverflowLengthHex; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = parsedValue;
                                return true;
                            }
                            // If we try to append a digit to anything larger than ulong.MaxValue / 0x10, there will be overflow
                            if (parsedValue > ulong.MaxValue / 0x10)
                            {
                                charsConsumed = 0;
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
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
}
