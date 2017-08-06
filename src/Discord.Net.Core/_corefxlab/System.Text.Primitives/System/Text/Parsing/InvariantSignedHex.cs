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
                #region SByte
                public unsafe static bool TryParseSByte(byte* text, int length, out sbyte value)
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

                    if (length <= SByteOverflowLengthHex)
                    {
                        // Length is less than or equal to SByteOverflowLengthHex; overflow is not possible
                        for (int index = 1; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = (sbyte)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than SByteOverflowLengthHex; overflow is only possible after SByteOverflowLengthHex
                        // digits. There may be no overflow after SByteOverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < SByteOverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = (sbyte)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = SByteOverflowLengthHex; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = (sbyte)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than -(sbyte.MinValue / 0x08), there will be overflow
                            if (parsedValue >= -(sbyte.MinValue / 0x08))
                            {
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }

                    value = (sbyte)(parsedValue);
                    return true;
                }

                public unsafe static bool TryParseSByte(byte* text, int length, out sbyte value, out int bytesConsumed)
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

                    if (length <= SByteOverflowLengthHex)
                    {
                        // Length is less than or equal to SByteOverflowLengthHex; overflow is not possible
                        for (int index = 1; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = (sbyte)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than SByteOverflowLengthHex; overflow is only possible after SByteOverflowLengthHex
                        // digits. There may be no overflow after SByteOverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < SByteOverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = (sbyte)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = SByteOverflowLengthHex; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = (sbyte)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than -(sbyte.MinValue / 0x08), there will be overflow
                            if (parsedValue >= -(sbyte.MinValue / 0x08))
                            {
                                bytesConsumed = 0;
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }

                    bytesConsumed = length;
                    value = (sbyte)(parsedValue);
                    return true;
                }

                public static bool TryParseSByte(ReadOnlySpan<byte> text, out sbyte value)
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

                    if (text.Length <= SByteOverflowLengthHex)
                    {
                        // Length is less than or equal to SByteOverflowLengthHex; overflow is not possible
                        for (int index = 1; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = (sbyte)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than SByteOverflowLengthHex; overflow is only possible after SByteOverflowLengthHex
                        // digits. There may be no overflow after SByteOverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < SByteOverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = (sbyte)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = SByteOverflowLengthHex; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = (sbyte)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than -(sbyte.MinValue / 0x08), there will be overflow
                            if (parsedValue >= -(sbyte.MinValue / 0x08))
                            {
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }

                    value = (sbyte)(parsedValue);
                    return true;
                }

                public static bool TryParseSByte(ReadOnlySpan<byte> text, out sbyte value, out int bytesConsumed)
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

                    if (text.Length <= SByteOverflowLengthHex)
                    {
                        // Length is less than or equal to SByteOverflowLengthHex; overflow is not possible
                        for (int index = 1; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = (sbyte)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than SByteOverflowLengthHex; overflow is only possible after SByteOverflowLengthHex
                        // digits. There may be no overflow after SByteOverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < SByteOverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = (sbyte)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = SByteOverflowLengthHex; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = (sbyte)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than -(sbyte.MinValue / 0x08), there will be overflow
                            if (parsedValue >= -(sbyte.MinValue / 0x08))
                            {
                                bytesConsumed = 0;
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }

                    bytesConsumed = text.Length;
                    value = (sbyte)(parsedValue);
                    return true;
                }

                #endregion

                #region Int16
                public unsafe static bool TryParseInt16(byte* text, int length, out short value)
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

                    if (length <= Int16OverflowLengthHex)
                    {
                        // Length is less than or equal to Int16OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = (short)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than Int16OverflowLengthHex; overflow is only possible after Int16OverflowLengthHex
                        // digits. There may be no overflow after Int16OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < Int16OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = (short)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = Int16OverflowLengthHex; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = (short)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than -(short.MinValue / 0x08), there will be overflow
                            if (parsedValue >= -(short.MinValue / 0x08))
                            {
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }

                    value = (short)(parsedValue);
                    return true;
                }

                public unsafe static bool TryParseInt16(byte* text, int length, out short value, out int bytesConsumed)
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

                    if (length <= Int16OverflowLengthHex)
                    {
                        // Length is less than or equal to Int16OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = (short)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than Int16OverflowLengthHex; overflow is only possible after Int16OverflowLengthHex
                        // digits. There may be no overflow after Int16OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < Int16OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = (short)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = Int16OverflowLengthHex; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = (short)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than -(short.MinValue / 0x08), there will be overflow
                            if (parsedValue >= -(short.MinValue / 0x08))
                            {
                                bytesConsumed = 0;
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }

                    bytesConsumed = length;
                    value = (short)(parsedValue);
                    return true;
                }

                public static bool TryParseInt16(ReadOnlySpan<byte> text, out short value)
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

                    if (text.Length <= Int16OverflowLengthHex)
                    {
                        // Length is less than or equal to Int16OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = (short)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than Int16OverflowLengthHex; overflow is only possible after Int16OverflowLengthHex
                        // digits. There may be no overflow after Int16OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < Int16OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = (short)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = Int16OverflowLengthHex; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = (short)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than -(short.MinValue / 0x08), there will be overflow
                            if (parsedValue >= -(short.MinValue / 0x08))
                            {
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }

                    value = (short)(parsedValue);
                    return true;
                }

                public static bool TryParseInt16(ReadOnlySpan<byte> text, out short value, out int bytesConsumed)
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

                    if (text.Length <= Int16OverflowLengthHex)
                    {
                        // Length is less than or equal to Int16OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = (short)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than Int16OverflowLengthHex; overflow is only possible after Int16OverflowLengthHex
                        // digits. There may be no overflow after Int16OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < Int16OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = (short)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = Int16OverflowLengthHex; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = (short)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than -(short.MinValue / 0x08), there will be overflow
                            if (parsedValue >= -(short.MinValue / 0x08))
                            {
                                bytesConsumed = 0;
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }

                    bytesConsumed = text.Length;
                    value = (short)(parsedValue);
                    return true;
                }

                #endregion

                #region Int32
                public unsafe static bool TryParseInt32(byte* text, int length, out int value)
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

                    if (length <= Int32OverflowLengthHex)
                    {
                        // Length is less than or equal to Int32OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = (int)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than Int32OverflowLengthHex; overflow is only possible after Int32OverflowLengthHex
                        // digits. There may be no overflow after Int32OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < Int32OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = (int)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = Int32OverflowLengthHex; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = (int)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than -(int.MinValue / 0x08), there will be overflow
                            if (parsedValue >= -(int.MinValue / 0x08))
                            {
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }

                    value = (int)(parsedValue);
                    return true;
                }

                public unsafe static bool TryParseInt32(byte* text, int length, out int value, out int bytesConsumed)
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

                    if (length <= Int32OverflowLengthHex)
                    {
                        // Length is less than or equal to Int32OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = (int)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than Int32OverflowLengthHex; overflow is only possible after Int32OverflowLengthHex
                        // digits. There may be no overflow after Int32OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < Int32OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = (int)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = Int32OverflowLengthHex; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = (int)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than -(int.MinValue / 0x08), there will be overflow
                            if (parsedValue >= -(int.MinValue / 0x08))
                            {
                                bytesConsumed = 0;
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }

                    bytesConsumed = length;
                    value = (int)(parsedValue);
                    return true;
                }

                public static bool TryParseInt32(ReadOnlySpan<byte> text, out int value)
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

                    if (text.Length <= Int32OverflowLengthHex)
                    {
                        // Length is less than or equal to Int32OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = (int)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than Int32OverflowLengthHex; overflow is only possible after Int32OverflowLengthHex
                        // digits. There may be no overflow after Int32OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < Int32OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = (int)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = Int32OverflowLengthHex; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = (int)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than -(int.MinValue / 0x08), there will be overflow
                            if (parsedValue >= -(int.MinValue / 0x08))
                            {
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }

                    value = (int)(parsedValue);
                    return true;
                }

                public static bool TryParseInt32(ReadOnlySpan<byte> text, out int value, out int bytesConsumed)
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

                    if (text.Length <= Int32OverflowLengthHex)
                    {
                        // Length is less than or equal to Int32OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = (int)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than Int32OverflowLengthHex; overflow is only possible after Int32OverflowLengthHex
                        // digits. There may be no overflow after Int32OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < Int32OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = (int)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = Int32OverflowLengthHex; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = (int)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than -(int.MinValue / 0x08), there will be overflow
                            if (parsedValue >= -(int.MinValue / 0x08))
                            {
                                bytesConsumed = 0;
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }

                    bytesConsumed = text.Length;
                    value = (int)(parsedValue);
                    return true;
                }

                #endregion

                #region Int64
                public unsafe static bool TryParseInt64(byte* text, int length, out long value)
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

                    if (length <= Int64OverflowLengthHex)
                    {
                        // Length is less than or equal to Int64OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = (long)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than Int64OverflowLengthHex; overflow is only possible after Int64OverflowLengthHex
                        // digits. There may be no overflow after Int64OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < Int64OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = (long)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = Int64OverflowLengthHex; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = (long)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than -(long.MinValue / 0x08), there will be overflow
                            if (parsedValue >= -(long.MinValue / 0x08))
                            {
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }

                    value = (long)(parsedValue);
                    return true;
                }

                public unsafe static bool TryParseInt64(byte* text, int length, out long value, out int bytesConsumed)
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

                    if (length <= Int64OverflowLengthHex)
                    {
                        // Length is less than or equal to Int64OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = (long)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than Int64OverflowLengthHex; overflow is only possible after Int64OverflowLengthHex
                        // digits. There may be no overflow after Int64OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < Int64OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = (long)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = Int64OverflowLengthHex; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = (long)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than -(long.MinValue / 0x08), there will be overflow
                            if (parsedValue >= -(long.MinValue / 0x08))
                            {
                                bytesConsumed = 0;
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }

                    bytesConsumed = length;
                    value = (long)(parsedValue);
                    return true;
                }

                public static bool TryParseInt64(ReadOnlySpan<byte> text, out long value)
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

                    if (text.Length <= Int64OverflowLengthHex)
                    {
                        // Length is less than or equal to Int64OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = (long)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than Int64OverflowLengthHex; overflow is only possible after Int64OverflowLengthHex
                        // digits. There may be no overflow after Int64OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < Int64OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = (long)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = Int64OverflowLengthHex; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                value = (long)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than -(long.MinValue / 0x08), there will be overflow
                            if (parsedValue >= -(long.MinValue / 0x08))
                            {
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }

                    value = (long)(parsedValue);
                    return true;
                }

                public static bool TryParseInt64(ReadOnlySpan<byte> text, out long value, out int bytesConsumed)
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

                    if (text.Length <= Int64OverflowLengthHex)
                    {
                        // Length is less than or equal to Int64OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = (long)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than Int64OverflowLengthHex; overflow is only possible after Int64OverflowLengthHex
                        // digits. There may be no overflow after Int64OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < Int64OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = (long)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = Int64OverflowLengthHex; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[nextCharacter];
                            if (nextDigit == 0xFF)
                            {
                                bytesConsumed = index;
                                value = (long)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than -(long.MinValue / 0x08), there will be overflow
                            if (parsedValue >= -(long.MinValue / 0x08))
                            {
                                bytesConsumed = 0;
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }

                    bytesConsumed = text.Length;
                    value = (long)(parsedValue);
                    return true;
                }

                #endregion

            }
        }
        public static partial class InvariantUtf16
        {
            public static partial class Hex
            {
                #region SByte
                public unsafe static bool TryParseSByte(char* text, int length, out sbyte value)
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

                    if (length <= SByteOverflowLengthHex)
                    {
                        // Length is less than or equal to SByteOverflowLengthHex; overflow is not possible
                        for (int index = 1; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = (sbyte)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than SByteOverflowLengthHex; overflow is only possible after SByteOverflowLengthHex
                        // digits. There may be no overflow after SByteOverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < SByteOverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = (sbyte)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = SByteOverflowLengthHex; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = (sbyte)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than -(sbyte.MinValue / 0x08), there will be overflow
                            if (parsedValue >= -(sbyte.MinValue / 0x08))
                            {
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }

                    value = (sbyte)(parsedValue);
                    return true;
                }

                public unsafe static bool TryParseSByte(char* text, int length, out sbyte value, out int charsConsumed)
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

                    if (length <= SByteOverflowLengthHex)
                    {
                        // Length is less than or equal to SByteOverflowLengthHex; overflow is not possible
                        for (int index = 1; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = (sbyte)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than SByteOverflowLengthHex; overflow is only possible after SByteOverflowLengthHex
                        // digits. There may be no overflow after SByteOverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < SByteOverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = (sbyte)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = SByteOverflowLengthHex; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = (sbyte)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than -(sbyte.MinValue / 0x08), there will be overflow
                            if (parsedValue >= -(sbyte.MinValue / 0x08))
                            {
                                charsConsumed = 0;
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }

                    charsConsumed = length;
                    value = (sbyte)(parsedValue);
                    return true;
                }

                public static bool TryParseSByte(ReadOnlySpan<char> text, out sbyte value)
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

                    if (text.Length <= SByteOverflowLengthHex)
                    {
                        // Length is less than or equal to SByteOverflowLengthHex; overflow is not possible
                        for (int index = 1; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = (sbyte)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than SByteOverflowLengthHex; overflow is only possible after SByteOverflowLengthHex
                        // digits. There may be no overflow after SByteOverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < SByteOverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = (sbyte)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = SByteOverflowLengthHex; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = (sbyte)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than -(sbyte.MinValue / 0x08), there will be overflow
                            if (parsedValue >= -(sbyte.MinValue / 0x08))
                            {
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }

                    value = (sbyte)(parsedValue);
                    return true;
                }

                public static bool TryParseSByte(ReadOnlySpan<char> text, out sbyte value, out int charsConsumed)
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

                    if (text.Length <= SByteOverflowLengthHex)
                    {
                        // Length is less than or equal to SByteOverflowLengthHex; overflow is not possible
                        for (int index = 1; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = (sbyte)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than SByteOverflowLengthHex; overflow is only possible after SByteOverflowLengthHex
                        // digits. There may be no overflow after SByteOverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < SByteOverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = (sbyte)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = SByteOverflowLengthHex; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = (sbyte)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than -(sbyte.MinValue / 0x08), there will be overflow
                            if (parsedValue >= -(sbyte.MinValue / 0x08))
                            {
                                charsConsumed = 0;
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }

                    charsConsumed = text.Length;
                    value = (sbyte)(parsedValue);
                    return true;
                }

                #endregion

                #region Int16
                public unsafe static bool TryParseInt16(char* text, int length, out short value)
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

                    if (length <= Int16OverflowLengthHex)
                    {
                        // Length is less than or equal to Int16OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = (short)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than Int16OverflowLengthHex; overflow is only possible after Int16OverflowLengthHex
                        // digits. There may be no overflow after Int16OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < Int16OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = (short)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = Int16OverflowLengthHex; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = (short)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than -(short.MinValue / 0x08), there will be overflow
                            if (parsedValue >= -(short.MinValue / 0x08))
                            {
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }

                    value = (short)(parsedValue);
                    return true;
                }

                public unsafe static bool TryParseInt16(char* text, int length, out short value, out int charsConsumed)
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

                    if (length <= Int16OverflowLengthHex)
                    {
                        // Length is less than or equal to Int16OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = (short)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than Int16OverflowLengthHex; overflow is only possible after Int16OverflowLengthHex
                        // digits. There may be no overflow after Int16OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < Int16OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = (short)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = Int16OverflowLengthHex; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = (short)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than -(short.MinValue / 0x08), there will be overflow
                            if (parsedValue >= -(short.MinValue / 0x08))
                            {
                                charsConsumed = 0;
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }

                    charsConsumed = length;
                    value = (short)(parsedValue);
                    return true;
                }

                public static bool TryParseInt16(ReadOnlySpan<char> text, out short value)
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

                    if (text.Length <= Int16OverflowLengthHex)
                    {
                        // Length is less than or equal to Int16OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = (short)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than Int16OverflowLengthHex; overflow is only possible after Int16OverflowLengthHex
                        // digits. There may be no overflow after Int16OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < Int16OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = (short)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = Int16OverflowLengthHex; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = (short)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than -(short.MinValue / 0x08), there will be overflow
                            if (parsedValue >= -(short.MinValue / 0x08))
                            {
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }

                    value = (short)(parsedValue);
                    return true;
                }

                public static bool TryParseInt16(ReadOnlySpan<char> text, out short value, out int charsConsumed)
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

                    if (text.Length <= Int16OverflowLengthHex)
                    {
                        // Length is less than or equal to Int16OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = (short)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than Int16OverflowLengthHex; overflow is only possible after Int16OverflowLengthHex
                        // digits. There may be no overflow after Int16OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < Int16OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = (short)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = Int16OverflowLengthHex; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = (short)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than -(short.MinValue / 0x08), there will be overflow
                            if (parsedValue >= -(short.MinValue / 0x08))
                            {
                                charsConsumed = 0;
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }

                    charsConsumed = text.Length;
                    value = (short)(parsedValue);
                    return true;
                }

                #endregion

                #region Int32
                public unsafe static bool TryParseInt32(char* text, int length, out int value)
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

                    if (length <= Int32OverflowLengthHex)
                    {
                        // Length is less than or equal to Int32OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = (int)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than Int32OverflowLengthHex; overflow is only possible after Int32OverflowLengthHex
                        // digits. There may be no overflow after Int32OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < Int32OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = (int)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = Int32OverflowLengthHex; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = (int)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than -(int.MinValue / 0x08), there will be overflow
                            if (parsedValue >= -(int.MinValue / 0x08))
                            {
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }

                    value = (int)(parsedValue);
                    return true;
                }

                public unsafe static bool TryParseInt32(char* text, int length, out int value, out int charsConsumed)
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

                    if (length <= Int32OverflowLengthHex)
                    {
                        // Length is less than or equal to Int32OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = (int)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than Int32OverflowLengthHex; overflow is only possible after Int32OverflowLengthHex
                        // digits. There may be no overflow after Int32OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < Int32OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = (int)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = Int32OverflowLengthHex; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = (int)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than -(int.MinValue / 0x08), there will be overflow
                            if (parsedValue >= -(int.MinValue / 0x08))
                            {
                                charsConsumed = 0;
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }

                    charsConsumed = length;
                    value = (int)(parsedValue);
                    return true;
                }

                public static bool TryParseInt32(ReadOnlySpan<char> text, out int value)
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

                    if (text.Length <= Int32OverflowLengthHex)
                    {
                        // Length is less than or equal to Int32OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = (int)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than Int32OverflowLengthHex; overflow is only possible after Int32OverflowLengthHex
                        // digits. There may be no overflow after Int32OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < Int32OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = (int)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = Int32OverflowLengthHex; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = (int)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than -(int.MinValue / 0x08), there will be overflow
                            if (parsedValue >= -(int.MinValue / 0x08))
                            {
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }

                    value = (int)(parsedValue);
                    return true;
                }

                public static bool TryParseInt32(ReadOnlySpan<char> text, out int value, out int charsConsumed)
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

                    if (text.Length <= Int32OverflowLengthHex)
                    {
                        // Length is less than or equal to Int32OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = (int)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than Int32OverflowLengthHex; overflow is only possible after Int32OverflowLengthHex
                        // digits. There may be no overflow after Int32OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < Int32OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = (int)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = Int32OverflowLengthHex; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = (int)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than -(int.MinValue / 0x08), there will be overflow
                            if (parsedValue >= -(int.MinValue / 0x08))
                            {
                                charsConsumed = 0;
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }

                    charsConsumed = text.Length;
                    value = (int)(parsedValue);
                    return true;
                }

                #endregion

                #region Int64
                public unsafe static bool TryParseInt64(char* text, int length, out long value)
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

                    if (length <= Int64OverflowLengthHex)
                    {
                        // Length is less than or equal to Int64OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = (long)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than Int64OverflowLengthHex; overflow is only possible after Int64OverflowLengthHex
                        // digits. There may be no overflow after Int64OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < Int64OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = (long)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = Int64OverflowLengthHex; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = (long)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than -(long.MinValue / 0x08), there will be overflow
                            if (parsedValue >= -(long.MinValue / 0x08))
                            {
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }

                    value = (long)(parsedValue);
                    return true;
                }

                public unsafe static bool TryParseInt64(char* text, int length, out long value, out int charsConsumed)
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

                    if (length <= Int64OverflowLengthHex)
                    {
                        // Length is less than or equal to Int64OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = (long)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than Int64OverflowLengthHex; overflow is only possible after Int64OverflowLengthHex
                        // digits. There may be no overflow after Int64OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < Int64OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = (long)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = Int64OverflowLengthHex; index < length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = (long)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than -(long.MinValue / 0x08), there will be overflow
                            if (parsedValue >= -(long.MinValue / 0x08))
                            {
                                charsConsumed = 0;
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }

                    charsConsumed = length;
                    value = (long)(parsedValue);
                    return true;
                }

                public static bool TryParseInt64(ReadOnlySpan<char> text, out long value)
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

                    if (text.Length <= Int64OverflowLengthHex)
                    {
                        // Length is less than or equal to Int64OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = (long)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than Int64OverflowLengthHex; overflow is only possible after Int64OverflowLengthHex
                        // digits. There may be no overflow after Int64OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < Int64OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = (long)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = Int64OverflowLengthHex; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                value = (long)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than -(long.MinValue / 0x08), there will be overflow
                            if (parsedValue >= -(long.MinValue / 0x08))
                            {
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }

                    value = (long)(parsedValue);
                    return true;
                }

                public static bool TryParseInt64(ReadOnlySpan<char> text, out long value, out int charsConsumed)
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

                    if (text.Length <= Int64OverflowLengthHex)
                    {
                        // Length is less than or equal to Int64OverflowLengthHex; overflow is not possible
                        for (int index = 1; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = (long)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }
                    else
                    {
                        // Length is greater than Int64OverflowLengthHex; overflow is only possible after Int64OverflowLengthHex
                        // digits. There may be no overflow after Int64OverflowLengthHex if there are leading zeroes.
                        for (int index = 1; index < Int64OverflowLengthHex; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = (long)(parsedValue);
                                return true;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                        for (int index = Int64OverflowLengthHex; index < text.Length; index++)
                        {
                            nextCharacter = text[index];
                            nextDigit = hexLookup[(byte)nextCharacter];
                            if (nextDigit == 0xFF || (nextCharacter >> 8) != 0)
                            {
                                charsConsumed = index;
                                value = (long)(parsedValue);
                                return true;
                            }
                            // If we try to append a digit to anything larger than -(long.MinValue / 0x08), there will be overflow
                            if (parsedValue >= -(long.MinValue / 0x08))
                            {
                                charsConsumed = 0;
                                value = default;
                                return false;
                            }
                            parsedValue = (parsedValue << 4) + nextDigit;
                        }
                    }

                    charsConsumed = text.Length;
                    value = (long)(parsedValue);
                    return true;
                }

                #endregion

            }
        }
    }
}
