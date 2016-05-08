using Newtonsoft.Json;
using System;

namespace Discord.Net.Converters
{
    public class UInt64EntityConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(IEntity<ulong>);
        public override bool CanRead => false;
        public override bool CanWrite => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new InvalidOperationException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value != null)
                writer.WriteValue((value as IEntity<ulong>).Id);
            else
                writer.WriteNull();
        }
    }
}
