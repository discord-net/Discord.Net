using Newtonsoft.Json;
using System;

namespace Discord.Net.Converters
{
    public class ChannelTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(ChannelType);
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            switch ((string)reader.Value)
            {
                case "text":
                    return ChannelType.Text;
                case "voice":
                    return ChannelType.Voice;
                default:
                    throw new JsonSerializationException("Unknown channel type");
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            switch ((ChannelType)value)
            {
                case ChannelType.Text:
                    writer.WriteValue("text");
                    break;
                case ChannelType.Voice:
                    writer.WriteValue("voice");
                    break;
                default:
                    throw new JsonSerializationException("Invalid channel type");
            }
        }
    }
}
