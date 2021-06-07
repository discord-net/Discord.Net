using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord sticker object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/channel#message-object-message-sticker-structure"/>
    /// </remarks>
    public record Sticker
    {
        /// <summary>
        /// Id of the <see cref="Sticker"/>.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake Id { get; init; }

        /// <summary>
        /// Id of the pack the <see cref="Sticker"/> is from.
        /// </summary>
        [JsonPropertyName("pack_id")]
        public Snowflake PackId { get; init; }

        /// <summary>
        /// Name of the <see cref="Sticker"/>.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; init; } // Required property candidate

        /// <summary>
        /// Description of the <see cref="Sticker"/>.
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; init; } // Required property candidate

        /// <summary>
        /// A comma-separated list of tags for the <see cref="Sticker"/>.
        /// </summary>
        [JsonPropertyName("tags")]
        public Optional<string> Tags { get; init; }

        /// <summary>
        /// <see cref="Sticker"/> asset hash.
        /// </summary>
        [JsonPropertyName("asset")]
        public string? Asset { get; init; } // Required property candidate

        /// <summary>
        /// Type of <see cref="Sticker"/> format.
        /// </summary>
        [JsonPropertyName("format_type")]
        public StickerFormatType FormatType { get; init; }
    }
}
