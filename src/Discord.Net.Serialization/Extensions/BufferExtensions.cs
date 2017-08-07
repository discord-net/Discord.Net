using System;
using System.Text;
using System.Text.Utf8;

namespace Discord.Serialization
{
    public static class BufferExtensions
    {
        private static readonly ParsedFormat _numberFormat = new ParsedFormat('D');

        public static bool ParseBool(this ReadOnlySpan<byte> text)
        {
            if (PrimitiveParser.TryParseBoolean(text, out bool result, out int ignored, SymbolTable.InvariantUtf8))
                return result;
            throw new SerializationException("Failed to parse Boolean");
        }

        public static sbyte ParseInt8(this ReadOnlySpan<byte> text)
        {
            if (PrimitiveParser.TryParseSByte(text, out sbyte result, out int ignored, _numberFormat, SymbolTable.InvariantUtf8))
                return result;
            throw new SerializationException("Failed to parse Int8");
        }
        public static short ParseInt16(this ReadOnlySpan<byte> text)
        {
            if (PrimitiveParser.TryParseInt16(text, out short result, out int ignored, _numberFormat, SymbolTable.InvariantUtf8))
                return result;
            throw new SerializationException("Failed to parse Int16");
        }
        public static int ParseInt32(this ReadOnlySpan<byte> text)
        {
            if (PrimitiveParser.TryParseInt32(text, out int result, out int ignored, _numberFormat, SymbolTable.InvariantUtf8))
                return result;
            throw new SerializationException("Failed to parse Int32");
        }
        public static long ParseInt64(this ReadOnlySpan<byte> text)
        {
            if (PrimitiveParser.TryParseInt64(text, out long result, out int ignored, _numberFormat, SymbolTable.InvariantUtf8))
                return result;
            throw new SerializationException("Failed to parse Int64");
        }

        public static byte ParseUInt8(this ReadOnlySpan<byte> text)
        {
            if (PrimitiveParser.TryParseByte(text, out byte result, out int ignored, _numberFormat, SymbolTable.InvariantUtf8))
                return result;
            throw new SerializationException("Failed to parse UInt8");
        }
        public static ushort ParseUInt16(this ReadOnlySpan<byte> text)
        {
            if (PrimitiveParser.TryParseUInt16(text, out ushort result, out int ignored, _numberFormat, SymbolTable.InvariantUtf8))
                return result;
            throw new SerializationException("Failed to parse UInt16");
        }
        public static uint ParseUInt32(this ReadOnlySpan<byte> text)
        {
            if (PrimitiveParser.TryParseUInt32(text, out uint result, out int ignored, _numberFormat, SymbolTable.InvariantUtf8))
                return result;
            throw new SerializationException("Failed to parse UInt32");
        }
        public static ulong ParseUInt64(this ReadOnlySpan<byte> text)
        {
            if (PrimitiveParser.TryParseUInt64(text, out ulong result, out int ignored, _numberFormat, SymbolTable.InvariantUtf8))
                return result;
            throw new SerializationException("Failed to parse UInt64");
        }

        public static char ParseChar(this ReadOnlySpan<byte> text)
        {
            string str = ParseString(text);
            if (str.Length == 1)
                return str[0];
            throw new SerializationException("Failed to parse Char");
        }
        public static string ParseString(this ReadOnlySpan<byte> text) => new Utf8String(text).ToString();

        public static float ParseSingle(this ReadOnlySpan<byte> text)
        {
            if (PrimitiveParser.TryParseDecimal(text, out decimal result, out int ignored, SymbolTable.InvariantUtf8))
                return (float)result;
            throw new SerializationException("Failed to parse Single");
        }
        public static double ParseDouble(this ReadOnlySpan<byte> text)
        {
            if (PrimitiveParser.TryParseDecimal(text, out decimal result, out int ignored, SymbolTable.InvariantUtf8))
                return (double)result;
            throw new SerializationException("Failed to parse Double");
        }
        public static decimal ParseDecimal(this ReadOnlySpan<byte> text)
        {
            if (PrimitiveParser.TryParseDecimal(text, out decimal result, out int ignored, SymbolTable.InvariantUtf8))
                return result;
            throw new SerializationException("Failed to parse Decimal");
        }

        public static DateTime ParseDateTime(this ReadOnlySpan<byte> text)
        {
            string str = ParseString(text);
            if (DateTime.TryParse(str, out var result)) //TODO: Improve perf
                return result;
            throw new SerializationException("Failed to parse DateTime");
        }
        public static DateTimeOffset ParseDateTimeOffset(this ReadOnlySpan<byte> text)
        {
            string str = ParseString(text);
            if (DateTimeOffset.TryParse(str, out var result)) //TODO: Improve perf
                return result;
            throw new SerializationException("Failed to parse DateTimeOffset");
        }
    }
}
