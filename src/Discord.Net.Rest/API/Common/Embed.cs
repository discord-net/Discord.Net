using Discord.Net.Converters;
using Newtonsoft.Json;
using System;

namespace Discord.API
{
    internal class Embed
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("color")]
        public uint? Color { get; set; }
        [JsonProperty("type"), JsonConverter(typeof(EmbedTypeConverter))]
        public EmbedType Type { get; set; }
        [JsonProperty("timestamp")]
        public DateTimeOffset? Timestamp { get; set; }
        [JsonProperty("author")]
        public Optional<EmbedAuthor> Author { get; set; }
        [JsonProperty("footer")]
        public Optional<EmbedFooter> Footer { get; set; }
        [JsonProperty("video")]
        public Optional<EmbedVideo> Video { get; set; }
        [JsonProperty("thumbnail")]
        public Optional<EmbedThumbnail> Thumbnail { get; set; }
        [JsonProperty("image")]
        public Optional<EmbedImage> Image { get; set; }
        [JsonProperty("provider")]
        public Optional<EmbedProvider> Provider { get; set; }
        [JsonProperty("fields")]
        public Optional<EmbedField[]> Fields { get; set; }
    }
}
