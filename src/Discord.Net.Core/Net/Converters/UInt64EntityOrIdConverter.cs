using Discord.API;
using Newtonsoft.Json;
using System;

namespace Discord.Net.Converters
{
    internal class UInt64EntityOrIdConverter<T> : JsonConverter
    {
        private readonly JsonConverter _innerConverter;

        public override bool CanConvert(Type objectType) => true;
        public override bool CanRead => true;
        public override bool CanWrite => false;

        public UInt64EntityOrIdConverter(JsonConverter innerConverter)
        {
            _innerConverter = innerConverter;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.String:
                case JsonToken.Integer:
                    return new EntityOrId<T>(ulong.Parse(reader.ReadAsString()));
                default:
                    T obj;
                    if (_innerConverter != null)
                        obj = (T)_innerConverter.ReadJson(reader, typeof(T), null, serializer);
                    else
                        obj = serializer.Deserialize<T>(reader);
                    return new EntityOrId<T>(obj);
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new InvalidOperationException();
        }
    }
}
