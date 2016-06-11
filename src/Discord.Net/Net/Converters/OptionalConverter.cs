using Newtonsoft.Json;
using System;

namespace Discord.Net.Converters
{
    public class OptionalConverter<T> : JsonConverter
    {
        public static readonly OptionalConverter<T> Instance = new OptionalConverter<T>();

        public override bool CanConvert(Type objectType) => true;
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return new Optional<T>(serializer.Deserialize<T>(reader));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, ((Optional<T>)value).Value);
        }
    }
}
