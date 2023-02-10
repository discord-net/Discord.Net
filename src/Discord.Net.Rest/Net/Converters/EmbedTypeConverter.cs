using Newtonsoft.Json;
using System;

namespace Discord.Net.Converters
{
    internal class EmbedTypeConverter : JsonConverter
    {
        public static readonly EmbedTypeConverter Instance = new EmbedTypeConverter();

        public override bool CanConvert(Type objectType) => true;
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return (string)reader.Value switch
            {
                "rich" => EmbedType.Rich,
                "link" => EmbedType.Link,
                "video" => EmbedType.Video,
                "image" => EmbedType.Image,
                "gifv" => EmbedType.Gifv,
                "article" => EmbedType.Article,
                "tweet" => EmbedType.Tweet,
                "html" => EmbedType.Html,
                // TODO 2.2 EmbedType.News
                _ => EmbedType.Unknown,
            };
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            switch ((EmbedType)value)
            {
                case EmbedType.Rich:
                    writer.WriteValue("rich");
                    break;
                case EmbedType.Link:
                    writer.WriteValue("link");
                    break;
                case EmbedType.Video:
                    writer.WriteValue("video");
                    break;
                case EmbedType.Image:
                    writer.WriteValue("image");
                    break;
                case EmbedType.Gifv:
                    writer.WriteValue("gifv");
                    break;
                case EmbedType.Article:
                    writer.WriteValue("article");
                    break;
                case EmbedType.Tweet:
                    writer.WriteValue("tweet");
                    break;
                case EmbedType.Html:
                    writer.WriteValue("html");
                    break;
                default:
                    throw new JsonSerializationException("Invalid embed type");
            }
        }
    }
}
