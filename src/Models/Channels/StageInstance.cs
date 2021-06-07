using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord stage instance object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/stage-instance#stage-instance-object-stage-instance-structure"/>
    /// </remarks>
    public record StageInstance
    {
        /// <summary>
        /// The id of this <see cref="StageInstance"/>.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake Id { get; }

        /// <summary>
        /// The <see cref="Guild"/> id of the associated Stage <see cref="Channel"/>.
        /// </summary>
        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; }

        /// <summary>
        /// The id of the associated Stage <see cref="Channel"/>.
        /// </summary>
        [JsonPropertyName("channel_id")]
        public Snowflake ChannelId { get; }

        /// <summary>
        /// The topic of the <see cref="StageInstance"/>.
        /// </summary>
        [JsonPropertyName("topic")]
        public string? Topic { get; } // Required property candidate

        /// <summary>
        /// The <see cref="Models.PrivacyLevel"/> of the <see cref="StageInstance"/>.
        /// </summary>
        [JsonPropertyName("privacy_level")]
        public PrivacyLevel PrivacyLevel { get; }

        /// <summary>
        /// Whether or not Stage discovery is disabled.
        /// </summary>
        [JsonPropertyName("discoverable_disabled")]
        public bool DiscoverableDisabled { get; }
    }
}
