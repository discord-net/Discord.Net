using Discord.API;
using Newtonsoft.Json;
using System;
using System.Reflection;

namespace Discord.Net.Converters
{
    public class OptionalConverter : JsonConverter
    {
        public static readonly OptionalConverter Instance = new OptionalConverter();
        internal static readonly PropertyInfo IsSpecifiedProperty = typeof(IOptional).GetProperty(nameof(IOptional.IsSpecified));

        public override bool CanConvert(Type objectType) => true;
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
