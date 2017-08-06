#pragma warning disable CS1591
using System;
using Discord.Serialization;

namespace Discord.API
{
    internal class Embed
    {
        [ModelProperty("title")]
        public string Title { get; set; }
        [ModelProperty("description")]
        public string Description { get; set; }
        [ModelProperty("url")]
        public string Url { get; set; }
        [ModelProperty("color")]
        public uint? Color { get; set; }
        [ModelProperty("type")]
        public EmbedType Type { get; set; }
        [ModelProperty("timestamp")]
        public DateTimeOffset? Timestamp { get; set; }
        [ModelProperty("author")]
        public Optional<EmbedAuthor> Author { get; set; }
        [ModelProperty("footer")]
        public Optional<EmbedFooter> Footer { get; set; }
        [ModelProperty("video")]
        public Optional<EmbedVideo> Video { get; set; }
        [ModelProperty("thumbnail")]
        public Optional<EmbedThumbnail> Thumbnail { get; set; }
        [ModelProperty("image")]
        public Optional<EmbedImage> Image { get; set; }
        [ModelProperty("provider")]
        public Optional<EmbedProvider> Provider { get; set; }
        [ModelProperty("fields")]
        public Optional<EmbedField[]> Fields { get; set; }
    }
}
