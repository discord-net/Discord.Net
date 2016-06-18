using Newtonsoft.Json;
using System;

namespace Discord.Net.Converters
{
    public class DirectionConverter : JsonConverter
    {
        public static readonly DirectionConverter Instance = new DirectionConverter();

        public override bool CanConvert(Type objectType) => true;
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            switch ((string)reader.Value)
            {
                case "before":
                    return Direction.Before;
                case "after":
                    return Direction.After;
                case "around":
                    return Direction.Around;
                default:
                    throw new JsonSerializationException("Unknown direction");
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            switch ((Direction)value)
            {
                case Direction.Before:
                    writer.WriteValue("before");
                    break;
                case Direction.After:
                    writer.WriteValue("after");
                    break;
                case Direction.Around:
                    writer.WriteValue("around");
                    break;
                default:
                    throw new JsonSerializationException("Invalid direction");
            }
        }
    }
}
