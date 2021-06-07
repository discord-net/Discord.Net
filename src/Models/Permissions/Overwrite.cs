using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord overwrite object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/channel#overwrite-object-overwrite-structure"/>
    /// </remarks>
    public record Overwrite
    {
        /// <summary>
        /// <see cref="Role"/> or <see cref="User"/> id.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake Id { get; init; }

        /// <summary>
        /// Type of entity this <see cref="Overwrite"/> belongs to.
        /// </summary>
        [JsonPropertyName("type")]
        public OverwriteType Type { get; init; }

        /// <summary>
        /// <see cref="Permissions"/> bit set.
        /// </summary>
        [JsonPropertyName("allow")]
        public Permissions Allow { get; init; }

        /// <summary>
        /// <see cref="Permissions"/> bit set.
        /// </summary>
        [JsonPropertyName("deny")]
        public Permissions Deny { get; init; }
    }
}
