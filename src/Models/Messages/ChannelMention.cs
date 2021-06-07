using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord channel mention object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/channel#channel-mention-object-channel-mention-structure"/>
    /// </remarks>
    public record ChannelMention
    {
        /// <summary>
        /// Id of the <see cref="Channel"/>.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake Id { get; init; }

        /// <summary>
        /// Id of the <see cref="Guild"/> containing the <see cref="Channel"/>.
        /// </summary>
        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; init; }

        /// <summary>
        /// The type of <see cref="Channel"/>.
        /// </summary>
        [JsonPropertyName("type")]
        public ChannelType Type { get; init; }

        /// <summary>
        /// The name of the <see cref="Channel"/>.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; init; } // Required property candidate
    }
}
