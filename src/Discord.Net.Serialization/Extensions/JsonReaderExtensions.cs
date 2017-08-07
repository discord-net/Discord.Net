using System;
using System.Text.Json;

namespace Discord.Serialization
{
    public static class JsonReaderExtensions
    {
        public static bool ParseBool(this JsonReader reader) => reader.Value.ParseBool();

        public static sbyte ParseInt8(this JsonReader reader) => reader.Value.ParseInt8();
        public static short ParseInt16(this JsonReader reader) => reader.Value.ParseInt16();
        public static int ParseInt32(this JsonReader reader) => reader.Value.ParseInt32();
        public static long ParseInt64(this JsonReader reader) => reader.Value.ParseInt64();

        public static byte ParseUInt8(this JsonReader reader) => reader.Value.ParseUInt8();
        public static ushort ParseUInt16(this JsonReader reader) => reader.Value.ParseUInt16();
        public static uint ParseUInt32(this JsonReader reader) => reader.Value.ParseUInt32();
        public static ulong ParseUInt64(this JsonReader reader) => reader.Value.ParseUInt64();

        public static char ParseChar(this JsonReader reader) => reader.Value.ParseChar();
        public static string ParseString(this JsonReader reader) => reader.Value.ParseString();

        public static float ParseSingle(this JsonReader reader) => reader.Value.ParseSingle();
        public static double ParseDouble(this JsonReader reader) => reader.Value.ParseDouble();
        public static decimal ParseDecimal(this JsonReader reader) => reader.Value.ParseDecimal();

        public static DateTime ParseDateTime(this JsonReader reader) => reader.Value.ParseDateTime();
        public static DateTimeOffset ParseDateTimeOffset(this JsonReader reader) => reader.Value.ParseDateTimeOffset();

        public static void Skip(this JsonReader reader)
        {
            int initialDepth = reader._depth;
            while (reader.Read() && reader._depth > initialDepth) { }
        }
    }
}
