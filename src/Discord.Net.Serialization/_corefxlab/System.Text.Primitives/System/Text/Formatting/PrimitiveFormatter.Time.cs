// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Text
{
    public static partial class PrimitiveFormatter
    {
        internal static readonly TimeSpan NullOffset = TimeSpan.MinValue;

        public static bool TryFormat(this DateTimeOffset value, Span<byte> buffer, out int bytesWritten, ParsedFormat format = default, SymbolTable symbolTable = null)
        {
            TimeSpan offset = NullOffset;
            char symbol = format.Symbol;
            if (format.IsDefault)
            {
                symbol = 'G';
                offset = value.Offset;
            }

            symbolTable = symbolTable ?? SymbolTable.InvariantUtf8;

            switch (symbol)
            {
                case 'R':
                    return TryFormatDateTimeRfc1123(value.UtcDateTime, buffer, out bytesWritten, symbolTable);

                case 'l':
                    return TryFormatDateTimeRfc1123Lowercase(value.UtcDateTime, buffer, out bytesWritten, symbolTable);

                case 'O':
                    return TryFormatDateTimeFormatO(value.DateTime, value.Offset, buffer, out bytesWritten, symbolTable);

                case 'G':
                    return TryFormatDateTimeFormatG(value.DateTime, offset, buffer, out bytesWritten, symbolTable);

                default:
                    ThrowNotImplemented();
                    bytesWritten = 0;
                    return false;
            }
        }

        public static bool TryFormat(this DateTime value, Span<byte> buffer, out int bytesWritten, ParsedFormat format = default, SymbolTable symbolTable = null)
        {
            char symbol = format.IsDefault ? 'G' : format.Symbol;

            symbolTable = symbolTable ?? SymbolTable.InvariantUtf8;

            switch (symbol)
            {
                case 'R':
                    return TryFormatDateTimeRfc1123(value, buffer, out bytesWritten, symbolTable);

                case 'l':
                    return TryFormatDateTimeRfc1123Lowercase(value, buffer, out bytesWritten, symbolTable);

                case 'O':
                    return TryFormatDateTimeFormatO(value, NullOffset, buffer, out bytesWritten, symbolTable);

                case 'G':
                    return TryFormatDateTimeFormatG(value, NullOffset, buffer, out bytesWritten, symbolTable);

                default:
                    ThrowNotImplemented();
                    bytesWritten = 0;
                    return false;
            }
        }

        public static bool TryFormat(this TimeSpan value, Span<byte> buffer, out int bytesWritten, ParsedFormat format = default, SymbolTable symbolTable = null)
        {
            char symbol = format.IsDefault ? 'c' : format.Symbol;

            Precondition.Require(symbol == 'G' || symbol == 'g' || symbol == 'c' || symbol == 't' || symbol == 'T');

            symbolTable = symbolTable ?? SymbolTable.InvariantUtf8;

            return TryFormatTimeSpan(value, symbol, buffer, out bytesWritten, symbolTable);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool TryFormatDateTimeFormatG(DateTime value, TimeSpan offset, Span<byte> buffer, out int bytesWritten, SymbolTable symbolTable)
        {
            // for now it only works for invariant culture
            if (symbolTable == SymbolTable.InvariantUtf8)
                return InvariantUtf8TimeFormatter.TryFormatG(value, offset, buffer, out bytesWritten);
            else if (symbolTable == SymbolTable.InvariantUtf16)
                return InvariantUtf16TimeFormatter.TryFormatG(value, offset, buffer, out bytesWritten);

            ThrowNotImplemented();
            bytesWritten = 0;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool TryFormatDateTimeFormatO(DateTime value, TimeSpan offset, Span<byte> buffer, out int bytesWritten, SymbolTable symbolTable)
        {
            // for now it only works for invariant culture
            if (symbolTable == SymbolTable.InvariantUtf8)
                return InvariantUtf8TimeFormatter.TryFormatO(value, offset, buffer, out bytesWritten);
            else if (symbolTable == SymbolTable.InvariantUtf16)
                return InvariantUtf16TimeFormatter.TryFormatO(value, offset, buffer, out bytesWritten);

            ThrowNotImplemented();
            bytesWritten = 0;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool TryFormatDateTimeRfc1123(DateTime value, Span<byte> buffer, out int bytesWritten, SymbolTable symbolTable)
        {
            // for now it only works for invariant culture
            if (symbolTable == SymbolTable.InvariantUtf8)
                return InvariantUtf8TimeFormatter.TryFormatRfc1123(value, buffer, out bytesWritten);
            else if (symbolTable == SymbolTable.InvariantUtf16)
                return InvariantUtf16TimeFormatter.TryFormatRfc1123(value, buffer, out bytesWritten);

            ThrowNotImplemented();
            bytesWritten = 0;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool TryFormatDateTimeRfc1123Lowercase(DateTime value, Span<byte> buffer, out int bytesWritten, SymbolTable symbolTable)
        {
            // for now it only works for invariant culture
            if (symbolTable == SymbolTable.InvariantUtf8)
                return InvariantUtf8TimeFormatter.TryFormatRfc1123Lowercase(value, buffer, out bytesWritten);
            else if (symbolTable == SymbolTable.InvariantUtf16)
                return InvariantUtf16TimeFormatter.TryFormatRfc1123Lowercase(value, buffer, out bytesWritten);

            ThrowNotImplemented();
            bytesWritten = 0;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool TryFormatTimeSpan(TimeSpan value, char format, Span<byte> buffer, out int bytesWritten, SymbolTable symbolTable)
        {
            // for now it only works for invariant culture
            if (symbolTable == SymbolTable.InvariantUtf8)
                return InvariantUtf8TimeFormatter.TryFormat(value, format, buffer, out bytesWritten);
            else if (symbolTable == SymbolTable.InvariantUtf16)
                return InvariantUtf16TimeFormatter.TryFormat(value, format, buffer, out bytesWritten);

            ThrowNotImplemented();
            bytesWritten = 0;
            return false;
        }

        // Methods won't be inlined if they contain a throw, so we factor out the throw to a separate method.
        static void ThrowNotImplemented()
        {
            throw new NotImplementedException();
        }
    }
}
