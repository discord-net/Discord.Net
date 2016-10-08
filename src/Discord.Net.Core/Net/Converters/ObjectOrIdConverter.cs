using Discord.API;
using Newtonsoft.Json;
using System;

namespace Discord.Net.Converters
{
    public class ObjectOrIdConverter<T> : JsonConverter
    {
        internal static ObjectOrIdConverter<T> Instance;

        private readonly JsonConverter _innerConverter;

        public override bool CanConvert(Type objectType) => true;
        public override bool CanRead => true;
        public override bool CanWrite => false;

        public ObjectOrIdConverter(JsonConverter innerConverter)
        {
            _innerConverter = innerConverter;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.String:
                case JsonToken.Integer:
                    return new ObjectOrId<T>(ulong.Parse(reader.ReadAsString()));
                default:
                    return new ObjectOrId<T>(serializer.Deserialize<T>(reader));
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new InvalidOperationException();
        }
    }
}
