using System;
using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord embed object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/channel#embed-object-embed-structure"/>
    /// </remarks>
    public record Embed
    {
        /// <summary>
        /// Title of <see cref="Embed"/>.
        /// </summary>
        [JsonPropertyName("title")]
        public Optional<string> Title { get; init; }

        /// <summary>
        /// Type of <see cref="Embed"/> (always <see cref="EmbedType.Rich"/> for webhook embeds).
        /// </summary>
        [JsonPropertyName("type")]
        public Optional<EmbedType> Type { get; init; }

        /// <summary>
        /// Description of <see cref="Embed"/>.
        /// </summary>
        [JsonPropertyName("description")]
        public Optional<string> Description { get; init; }

        /// <summary>
        /// Url of <see cref="Embed"/>.
        /// </summary>
        [JsonPropertyName("url")]
        public Optional<string> Url { get; init; }

        /// <summary>
        /// Timestamp of <see cref="Embed"/> content.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public Optional<DateTimeOffset> Timestamp { get; init; }

        /// <summary>
        /// <see cref="Color"/> of the <see cref="Embed"/>.
        /// </summary>
        [JsonPropertyName("color")]
        public Optional<Color> Color { get; init; }

        /// <summary>
        /// Footer information.
        /// </summary>
        [JsonPropertyName("footer")]
        public Optional<EmbedFooter> Footer { get; init; }

        /// <summary>
        /// Image information.
        /// </summary>
        [JsonPropertyName("image")]
        public Optional<EmbedImage> Image { get; init; }

        /// <summary>
        /// Thumbnail information.
        /// </summary>
        [JsonPropertyName("thumbnail")]
        public Optional<EmbedThumbnail> Thumbnail { get; init; }

        /// <summary>
        /// Video information.
        /// </summary>
        [JsonPropertyName("video")]
        public Optional<EmbedVideo> Video { get; init; }

        /// <summary>
        /// Provider information.
        /// </summary>
        [JsonPropertyName("provider")]
        public Optional<EmbedProvider> Provider { get; init; }

        /// <summary>
        /// Author information.
        /// </summary>
        [JsonPropertyName("author")]
        public Optional<EmbedAuthor> Author { get; init; }

        /// <summary>
        /// Fields information.
        /// </summary>
        [JsonPropertyName("fields")]
        public Optional<EmbedField[]> Fields { get; init; }
    }
}
