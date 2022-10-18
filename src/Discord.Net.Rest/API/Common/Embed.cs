using System;
using System.Text.Json.Serialization;
using Discord.Net.Converters;

namespace Discord.API
{
    internal class Embed
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("url")]
        public string Url { get; set; }
        [JsonPropertyName("color")]
        public uint? Color { get; set; }
        [JsonPropertyName("type"), JsonConverter(typeof(EmbedTypeConverter))]
        public EmbedType Type { get; set; }
        [JsonPropertyName("timestamp")]
        public DateTimeOffset? Timestamp { get; set; }
        [JsonPropertyName("author")]
        public Optional<EmbedAuthor> Author { get; set; }
        [JsonPropertyName("footer")]
        public Optional<EmbedFooter> Footer { get; set; }
        [JsonPropertyName("video")]
        public Optional<EmbedVideo> Video { get; set; }
        [JsonPropertyName("thumbnail")]
        public Optional<EmbedThumbnail> Thumbnail { get; set; }
        [JsonPropertyName("image")]
        public Optional<EmbedImage> Image { get; set; }
        [JsonPropertyName("provider")]
        public Optional<EmbedProvider> Provider { get; set; }
        [JsonPropertyName("fields")]
        public Optional<EmbedField[]> Fields { get; set; }
    }
}
