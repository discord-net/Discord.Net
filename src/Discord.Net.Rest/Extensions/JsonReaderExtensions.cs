using System;
using System.Text;
using System.Text.Json;
using System.Text.Utf8;

namespace Discord.Serialization
{
    internal static class JsonReaderExtensions
    {
        public static bool GetBool(this JsonReader reader) => GetBool(reader.Value);
        public static bool GetBool(this ReadOnlySpan<byte> text)
        {
            if (PrimitiveParser.TryParseBoolean(text, out bool result, out int ignored, SymbolTable.InvariantUtf8))
                return result;
            throw new SerializationException("Failed to parse Boolean");
        }

        public static sbyte GetInt8(this JsonReader reader) => GetInt8(reader.Value);
        public static sbyte GetInt8(this ReadOnlySpan<byte> text)
        {
            if (PrimitiveParser.TryParseSByte(text, out sbyte result, out int ignored, JsonConstants.NumberFormat, SymbolTable.InvariantUtf8))
                return result;
            throw new SerializationException("Failed to parse Int8");
        }
        public static short GetInt16(this JsonReader reader) => GetInt16(reader.Value);
        public static short GetInt16(this ReadOnlySpan<byte> text)
        {
            if (PrimitiveParser.TryParseInt16(text, out short result, out int ignored, JsonConstants.NumberFormat, SymbolTable.InvariantUtf8))
                return result;
            throw new SerializationException("Failed to parse Int16");
        }
        public static int GetInt32(this JsonReader reader) => GetInt32(reader.Value);
        public static int GetInt32(this ReadOnlySpan<byte> text)
        {
            if (PrimitiveParser.TryParseInt32(text, out int result, out int ignored, JsonConstants.NumberFormat, SymbolTable.InvariantUtf8))
                return result;
            throw new SerializationException("Failed to parse Int32");
        }
        public static long GetInt64(this JsonReader reader) => GetInt64(reader.Value);
        public static long GetInt64(this ReadOnlySpan<byte> text)
        {
            if (PrimitiveParser.TryParseInt64(text, out long result, out int ignored, JsonConstants.NumberFormat, SymbolTable.InvariantUtf8))
                return result;
            throw new SerializationException("Failed to parse Int64");
        }

        public static byte GetUInt8(this JsonReader reader) => GetUInt8(reader.Value);
        public static byte GetUInt8(this ReadOnlySpan<byte> text)
        {
            if (PrimitiveParser.TryParseByte(text, out byte result, out int ignored, JsonConstants.NumberFormat, SymbolTable.InvariantUtf8))
                return result;
            throw new SerializationException("Failed to parse UInt8");
        }
        public static ushort GetUInt16(this JsonReader reader) => GetUInt16(reader.Value);
        public static ushort GetUInt16(this ReadOnlySpan<byte> text)
        {
            if (PrimitiveParser.TryParseUInt16(text, out ushort result, out int ignored, JsonConstants.NumberFormat, SymbolTable.InvariantUtf8))
                return result;
            throw new SerializationException("Failed to parse UInt16");
        }
        public static uint GetUInt32(this JsonReader reader) => GetUInt32(reader.Value);
        public static uint GetUInt32(this ReadOnlySpan<byte> text)
        {
            if (PrimitiveParser.TryParseUInt32(text, out uint result, out int ignored, JsonConstants.NumberFormat, SymbolTable.InvariantUtf8))
                return result;
            throw new SerializationException("Failed to parse UInt32");
        }
        public static ulong GetUInt64(this JsonReader reader) => GetUInt64(reader.Value);
        public static ulong GetUInt64(this ReadOnlySpan<byte> text)
        {
            if (PrimitiveParser.TryParseUInt64(text, out ulong result, out int ignored, JsonConstants.NumberFormat, SymbolTable.InvariantUtf8))
                return result;
            throw new SerializationException("Failed to parse UInt64");
        }

        public static char GetChar(this JsonReader reader) => GetChar(reader.Value);
        public static char GetChar(this ReadOnlySpan<byte> text)
        {
            string str = GetString(text);
            if (str.Length == 1)
                return str[0];
            throw new SerializationException("Failed to parse Char");
        }
        public static string GetString(this JsonReader reader) => GetString(reader.Value);
        public static string GetString(this ReadOnlySpan<byte> text) => new Utf8String(text).ToString();

        public static float GetSingle(this JsonReader reader) => GetSingle(reader.Value);
        public static float GetSingle(this ReadOnlySpan<byte> text)
        {
            if (PrimitiveParser.TryParseDecimal(text, out decimal result, out int ignored, SymbolTable.InvariantUtf8))
                return (float)result;
            throw new SerializationException("Failed to parse Single");
        }
        public static double GetDouble(this JsonReader reader) => GetDouble(reader.Value);
        public static double GetDouble(this ReadOnlySpan<byte> text)
        {
            if (PrimitiveParser.TryParseDecimal(text, out decimal result, out int ignored, SymbolTable.InvariantUtf8))
                return (double)result;
            throw new SerializationException("Failed to parse Double");
        }
        public static decimal GetDecimal(this JsonReader reader) => GetDecimal(reader.Value);
        public static decimal GetDecimal(this ReadOnlySpan<byte> text)
        {
            if (PrimitiveParser.TryParseDecimal(text, out decimal result, out int ignored, SymbolTable.InvariantUtf8))
                return result;
            throw new SerializationException("Failed to parse Decimal");
        }

        public static DateTime GetDateTime(this JsonReader reader) => GetDateTime(reader.Value);
        public static DateTime GetDateTime(this ReadOnlySpan<byte> text)
        {
            string str = GetString(text);
            if (DateTime.TryParse(str, out var result)) //TODO: Improve perf
                return result;
            throw new SerializationException("Failed to parse DateTime");
        }
        public static DateTimeOffset GetDateTimeOffset(this JsonReader reader) => GetDateTimeOffset(reader.Value);
        public static DateTimeOffset GetDateTimeOffset(this ReadOnlySpan<byte> text)
        {
            string str = GetString(text);
            if (DateTimeOffset.TryParse(str, out var result)) //TODO: Improve perf
                return result;
            throw new SerializationException("Failed to parse DateTimeOffset");
        }

        public static void Skip(this JsonReader reader)
        {
            int initialDepth = reader._depth;
            while (reader.Read() && reader._depth > initialDepth) { }
        }
    }
}
