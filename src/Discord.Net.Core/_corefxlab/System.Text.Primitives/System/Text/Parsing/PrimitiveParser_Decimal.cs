// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


namespace System.Text
{
    public static partial class PrimitiveParser
    {
        public static bool TryParseDecimal(ReadOnlySpan<byte> text, out decimal value, out int bytesConsumed, SymbolTable symbolTable = null)
        {
            symbolTable = symbolTable ?? SymbolTable.InvariantUtf8;

            bytesConsumed = 0;
            value = default;

            if (symbolTable == SymbolTable.InvariantUtf8)
            {
                return InvariantUtf8.TryParseDecimal(text, out value, out bytesConsumed);
            }
            else if (symbolTable == SymbolTable.InvariantUtf16)
            {
                ReadOnlySpan<char> textChars = text.NonPortableCast<byte, char>();
                int charactersConsumed;
                bool result = InvariantUtf16.TryParseDecimal(textChars, out value, out charactersConsumed);
                bytesConsumed = charactersConsumed * sizeof(char);
                return result;
            }

            return false;
        }
    }
}
