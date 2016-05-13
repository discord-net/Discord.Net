using Discord.API;
using Newtonsoft.Json;
using System;

namespace Discord.Net.Converters
{
    public class OptionalConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Optional<>);
        public override bool CanRead => false;
        public override bool CanWrite => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new InvalidOperationException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, (value as IOptional).Value);
        }
    }
}
