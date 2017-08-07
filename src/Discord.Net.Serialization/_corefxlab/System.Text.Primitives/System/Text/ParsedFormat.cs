// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Text
{
    public struct ParsedFormat
    {
        public const byte NoPrecision = byte.MaxValue;
        public const byte MaxPrecision = 99;

        private byte _format;
        private byte _precision;

        public char Symbol => (char)_format;
        public byte Precision => _precision;
        public bool HasPrecision => _precision != NoPrecision;
        public bool IsDefault => _format == 0 && _precision == 0;

        public ParsedFormat(char symbol, byte precision = NoPrecision)
        {
            if (precision != NoPrecision && precision > MaxPrecision)
                throw new ArgumentOutOfRangeException("precision");
            if (symbol != (byte)symbol)
                throw new ArgumentOutOfRangeException("symbol");

            _format = (byte)symbol;
            _precision = precision;
        }

        public static implicit operator ParsedFormat(char symbol) => new ParsedFormat(symbol);

        public static ParsedFormat Parse(ReadOnlySpan<char> format)
        {
            if (format.IsEmpty)
                return default;

            char specifier = format[0];
            byte precision = NoPrecision;

            if (format.Length > 1)
            {
                var span = format.Slice(1);

                if (!PrimitiveParser.InvariantUtf16.TryParseByte(span, out precision))
                    throw new FormatException("format");

                if (precision > MaxPrecision)
                    throw new FormatException("precision");
            }

            return new ParsedFormat(specifier, precision);
        }

        public static ParsedFormat Parse(string format)
        {
            if (string.IsNullOrEmpty(format))
                return default;

            return Parse(format.AsSpan());
        }
    }
}
