using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Discord.Net.Converters
{
    internal class ArrayConverter<T> : JsonConverter
    {
        private readonly JsonConverter _innerConverter;

        public override bool CanConvert(Type objectType) => true;
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public ArrayConverter(JsonConverter innerConverter)
        {
            _innerConverter = innerConverter;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var result = new List<T>();
            if (reader.TokenType == JsonToken.StartArray)
            {
                reader.Read();
                while (reader.TokenType != JsonToken.EndArray)
                {
                    T obj;
                    if (_innerConverter != null)
                        obj = (T)_innerConverter.ReadJson(reader, typeof(T), null, serializer);
                    else
                        obj = serializer.Deserialize<T>(reader);
                    result.Add(obj);
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
                var a = (T[])value;
                for (int i = 0; i < a.Length; i++)
                {
                    if (_innerConverter != null)
                        _innerConverter.WriteJson(writer, a[i], serializer);
                    else
                        serializer.Serialize(writer, a[i], typeof(T));
                }

                writer.WriteEndArray();
            }
            else
                writer.WriteNull();
        }
    }
}
