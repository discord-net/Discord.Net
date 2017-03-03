using Newtonsoft.Json;
using System;

namespace Discord.Net.Converters
{
    internal class StringEntityConverter : JsonConverter
    {
        public static readonly StringEntityConverter Instance = new StringEntityConverter();

        public override bool CanConvert(Type objectType) => true;
        public override bool CanRead => false;
        public override bool CanWrite => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new InvalidOperationException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value != null)
                writer.WriteValue((value as IEntity<string>).Id);
            else
                writer.WriteNull();
        }
    }
}
