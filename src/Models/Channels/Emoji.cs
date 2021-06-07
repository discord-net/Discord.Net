using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord emoji object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/emoji#emoji-object-emoji-structure"/>
    /// </remarks>
    public record Emoji
    {
        /// <summary>
        /// Emoji id.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake? Id { get; init; }

        /// <summary>
        /// Emoji name.
        /// </summary>
        /// <remarks>
        /// Can be null only in reaction emoji objects.
        /// </remarks>
        [JsonPropertyName("name")]
        public string? Name { get; init; }

        /// <summary>
        /// <see cref="Role"/>s allowed to use this emoji.
        /// </summary>
        [JsonPropertyName("roles")]
        public Optional<Snowflake[]> Roles { get; init; }

        /// <summary>
        /// <see cref="User"/> that created this emoji.
        /// </summary>
        [JsonPropertyName("user")]
        public Optional<User> User { get; init; }

        /// <summary>
        /// Whether this emoji must be wrapped in colons.
        /// </summary>
        [JsonPropertyName("require_colons")]
        public Optional<bool> RequireColons { get; init; }

        /// <summary>
        /// Whether this emoji is managed.
        /// </summary>
        [JsonPropertyName("managed")]
        public Optional<bool> Managed { get; init; }

        /// <summary>
        /// Whether this emoji is animated.
        /// </summary>
        [JsonPropertyName("animated")]
        public Optional<bool> Animated { get; init; }

        /// <summary>
        /// Whether this emoji can be used, may be false due to loss of Server Boosts.
        /// </summary>
        [JsonPropertyName("available")]
        public Optional<bool> Available { get; init; }
    }
}
