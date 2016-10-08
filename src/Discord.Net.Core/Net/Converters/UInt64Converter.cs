using Newtonsoft.Json;
using System;
using System.Globalization;

namespace Discord.Net.Converters
{
    internal class UInt64Converter : JsonConverter
    {
        public static readonly UInt64Converter Instance = new UInt64Converter();

        public override bool CanConvert(Type objectType) => true;
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return ulong.Parse((string)reader.Value, NumberStyles.None, CultureInfo.InvariantCulture);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((ulong)value).ToString(CultureInfo.InvariantCulture));
        }
    }
}
