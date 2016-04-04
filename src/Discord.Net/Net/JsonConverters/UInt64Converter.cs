using Newtonsoft.Json;
using System;
using System.Globalization;

namespace Discord.Net.JsonConverters
{
    public class UInt64Converter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(ulong);
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
