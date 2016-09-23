using Newtonsoft.Json;
using System;
using System.Globalization;

namespace Discord.Net.Converters
{
    public class NullableUInt64Converter : JsonConverter
    {
        public static readonly NullableUInt64Converter Instance = new NullableUInt64Converter();

        public override bool CanConvert(Type objectType) => true;
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object value = reader.Value;
            if (value != null)
                return ulong.Parse((string)value, NumberStyles.None, CultureInfo.InvariantCulture);
            else
                return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value != null)
                writer.WriteValue(((ulong?)value).Value.ToString(CultureInfo.InvariantCulture));
            else
                writer.WriteNull();
        }
    }
}
