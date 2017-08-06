// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


namespace System.Text
{
    public static partial class PrimitiveParser
    {
        public static bool TryParseBoolean(ReadOnlySpan<byte> text, out bool value, out int bytesConsumed, SymbolTable symbolTable = null)
        {
            symbolTable = symbolTable ?? SymbolTable.InvariantUtf8;

            bytesConsumed = 0;
            value = default;

            if (symbolTable == SymbolTable.InvariantUtf8)
            {
                return InvariantUtf8.TryParseBoolean(text, out value, out bytesConsumed);
            }
            if (symbolTable == SymbolTable.InvariantUtf16)
            {
                ReadOnlySpan<char> textChars = text.NonPortableCast<byte, char>();
                int charactersConsumed;
                bool result = InvariantUtf16.TryParseBoolean(textChars, out value, out charactersConsumed);
                bytesConsumed = charactersConsumed * sizeof(char);
                return result;
            }

            return false;
        }
    }
}
