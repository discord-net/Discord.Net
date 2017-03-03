using Newtonsoft.Json;
using System;

namespace Discord.Net.Converters
{
    internal class NullableConverter<T> : JsonConverter
        where T : struct
    {
        private readonly JsonConverter _innerConverter;

        public override bool CanConvert(Type objectType) => true;
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public NullableConverter(JsonConverter innerConverter)
        {
            _innerConverter = innerConverter;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object value = reader.Value;
            if (value == null)
                return null;
            else
            {
                T obj;
                if (_innerConverter != null)
                    obj = (T)_innerConverter.ReadJson(reader, typeof(T), null, serializer);
                else
                    obj = serializer.Deserialize<T>(reader);
                return obj;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else
            {
                var nullable = (T?)value;
                if (_innerConverter != null)
                    _innerConverter.WriteJson(writer, nullable.Value, serializer);
                else
                    serializer.Serialize(writer, nullable.Value, typeof(T));
            }
        }
    }
}
