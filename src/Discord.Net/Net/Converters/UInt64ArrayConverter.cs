using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Discord.Net.Converters
{
    public class UInt64ArrayConverter : JsonConverter
    {
        public static readonly UInt64ArrayConverter Instance = new UInt64ArrayConverter();

        public override bool CanConvert(Type objectType) => true;
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var result = new List<ulong>();
            if (reader.TokenType == JsonToken.StartArray)
            {
                reader.Read();
                while (reader.TokenType != JsonToken.EndArray)
                {
                    ulong id = ulong.Parse((string)reader.Value, NumberStyles.None, CultureInfo.InvariantCulture);
                    result.Add(id);
                    reader.Read();
                }
            }
            return result.ToArray();
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value != null)
            {
                writer.WriteStartArray();
                var a = (ulong[])value;
                for (int i = 0; i < a.Length; i++)
                    writer.WriteValue(a[i].ToString(CultureInfo.InvariantCulture));
                writer.WriteEndArray();
            }
            else
                writer.WriteNull();
        }
    }
}
