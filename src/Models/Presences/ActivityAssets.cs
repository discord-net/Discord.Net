using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord activity assets object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/topics/gateway#activity-object-activity-assets"/>
    /// </remarks>
    public record ActivityAssets
    {
        /// <summary>
        /// The id for a large asset of the <see cref="Activity"/>, usually a snowflake.
        /// </summary>
        [JsonPropertyName("large_image")]
        public Optional<string> LargeImage { get; init; }

        /// <summary>
        /// Text displayed when hovering over the large image of the <see cref="Activity"/>.
        /// </summary>
        [JsonPropertyName("large_text")]
        public Optional<string> LargeText { get; init; }

        /// <summary>
        /// The id for a small asset of the <see cref="Activity"/>, usually a snowflake.
        /// </summary>
        [JsonPropertyName("small_image")]
        public Optional<string> SmallImage { get; init; }

        /// <summary>
        /// Text displayed when hovering over the small image of the <see cref="Activity"/>.
        /// </summary>
        [JsonPropertyName("small_text")]
        public Optional<string> SmallText { get; init; }
    }
}
