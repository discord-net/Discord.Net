using System;
using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a embed object.
    /// </summary>
    public record Embed
    {
        /// <summary>
        ///     Creates a <see cref="Embed"/> with the provided parameters.
        /// </summary>
        /// <param name="title">Title of embed.</param>
        /// <param name="type">Type of embed (always "rich" for webhook embeds).</param>
        /// <param name="description">Description of embed.</param>
        /// <param name="url">Url of embed.</param>
        /// <param name="timestamp">Timestamp of embed content.</param>
        /// <param name="color">Color code of the embed.</param>
        /// <param name="footer">Footer information.</param>
        /// <param name="image">Image information.</param>
        /// <param name="thumbnail">Thumbnail information.</param>
        /// <param name="video">Video information.</param>
        /// <param name="provider">Provider information.</param>
        /// <param name="author">Author information.</param>
        /// <param name="fields">Fields information.</param>
        [JsonConstructor]
        public Embed(Optional<string> title, Optional<EmbedType> type, Optional<string> description, Optional<string> url, Optional<DateTimeOffset> timestamp, Optional<int> color, Optional<EmbedFooter> footer, Optional<EmbedImage> image, Optional<EmbedThumbnail> thumbnail, Optional<EmbedVideo> video, Optional<EmbedProvider> provider, Optional<EmbedAuthor> author, Optional<EmbedField[]> fields)
        {
            Title = title;
            Type = type;
            Description = description;
            Url = url;
            Timestamp = timestamp;
            Color = color;
            Footer = footer;
            Image = image;
            Thumbnail = thumbnail;
            Video = video;
            Provider = provider;
            Author = author;
            Fields = fields;
        }

        /// <summary>
        ///     Title of embed.
        /// </summary>
        [JsonPropertyName("title")]
        public Optional<string> Title { get; }

        /// <summary>
        ///     Type of embed (always "rich" for webhook embeds).
        /// </summary>
        [JsonPropertyName("type")]
        public Optional<EmbedType> Type { get; }

        /// <summary>
        ///     Description of embed.
        /// </summary>
        [JsonPropertyName("description")]
        public Optional<string> Description { get; }

        /// <summary>
        ///     Url of embed.
        /// </summary>
        [JsonPropertyName("url")]
        public Optional<string> Url { get; }

        /// <summary>
        ///     Timestamp of embed content.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public Optional<DateTimeOffset> Timestamp { get; }

        /// <summary>
        ///     Color code of the embed.
        /// </summary>
        [JsonPropertyName("color")]
        public Optional<int> Color { get; }

        /// <summary>
        ///     Footer information.
        /// </summary>
        [JsonPropertyName("footer")]
        public Optional<EmbedFooter> Footer { get; }

        /// <summary>
        ///     Image information.
        /// </summary>
        [JsonPropertyName("image")]
        public Optional<EmbedImage> Image { get; }

        /// <summary>
        ///     Thumbnail information.
        /// </summary>
        [JsonPropertyName("thumbnail")]
        public Optional<EmbedThumbnail> Thumbnail { get; }

        /// <summary>
        ///     Video information.
        /// </summary>
        [JsonPropertyName("video")]
        public Optional<EmbedVideo> Video { get; }

        /// <summary>
        ///     Provider information.
        /// </summary>
        [JsonPropertyName("provider")]
        public Optional<EmbedProvider> Provider { get; }

        /// <summary>
        ///     Author information.
        /// </summary>
        [JsonPropertyName("author")]
        public Optional<EmbedAuthor> Author { get; }

        /// <summary>
        ///     Fields information.
        /// </summary>
        [JsonPropertyName("fields")]
        public Optional<EmbedField[]> Fields { get; }
    }
}
