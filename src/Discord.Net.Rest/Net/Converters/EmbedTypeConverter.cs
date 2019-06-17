using System;
using Newtonsoft.Json;

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
            switch ((string)reader.Value)
            {
                case "rich":
                    return EmbedType.Rich;
                case "link":
                    return EmbedType.Link;
                case "video":
                    return EmbedType.Video;
                case "image":
                    return EmbedType.Image;
                case "gifv":
                    return EmbedType.Gifv;
                case "article":
                    return EmbedType.Article;
                case "tweet":
                    return EmbedType.Tweet;
                case "html":
                    return EmbedType.Html;
                case "application_news": // TODO 2.2 EmbedType.News
                default:
                    return EmbedType.Unknown;
            }
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
