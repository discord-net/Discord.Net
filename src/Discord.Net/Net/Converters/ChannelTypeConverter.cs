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
            // TODO: This should probably just be a cast to an enum
            switch ((long)reader.Value)
            {
                case 0:
                    return ChannelType.Text;
                case 1:
                    return ChannelType.DM;
                case 2:
                    return ChannelType.Voice;
                case 3:
                    return ChannelType.Group;
                default:
                    throw new JsonSerializationException("Unknown channel type");
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            /*switch ((ChannelType)value)
            {
                case ChannelType.Text:
                    writer.WriteValue("text");
                    break;
                case ChannelType.Voice:
                    writer.WriteValue("voice");
                    break;
                default:
                    throw new JsonSerializationException("Invalid channel type");
            }*/
            writer.WriteValue((int)value);
        }
    }
}
