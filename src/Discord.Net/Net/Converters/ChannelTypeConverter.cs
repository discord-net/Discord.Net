using Newtonsoft.Json;
using System;

namespace Discord.Net.Converters
{
    public class ChannelTypeConverter : JsonConverter
    {
        public static readonly ChannelTypeConverter Instance = new ChannelTypeConverter();

        public override bool CanConvert(Type objectType) => true;
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return (ChannelType)((long)reader.Value);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue((int)value);
        }
    }
}
