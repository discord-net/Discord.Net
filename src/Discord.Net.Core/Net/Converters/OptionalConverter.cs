using Newtonsoft.Json;
using System;

namespace Discord.Net.Converters
{
    internal class OptionalConverter<T> : JsonConverter
    {
        private readonly JsonConverter _innerConverter;

        public override bool CanConvert(Type objectType) => true;
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public OptionalConverter(JsonConverter innerConverter)
        {
            _innerConverter = innerConverter;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            T obj;
            if (_innerConverter != null)
                obj = (T)_innerConverter.ReadJson(reader, typeof(T), null, serializer);
            else
                obj = serializer.Deserialize<T>(reader);
            return new Optional<T>(obj);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            value = ((Optional<T>)value).Value;
            if (_innerConverter != null)
                _innerConverter.WriteJson(writer, value, serializer);
            else
                serializer.Serialize(writer, value, typeof(T));
        }
    }
}
