using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a stage instance object.
    /// </summary>
    public record StageInstance
    {
        /// <summary>
        ///     Creates a <see cref="StageInstance"/> with the provided parameters.
        /// </summary>
        /// <param name="id">The id of this Stage instance.</param>
        /// <param name="guildId">The guild id of the associated Stage channel.</param>
        /// <param name="channelId">The id of the associated Stage channel.</param>
        /// <param name="topic">The topic of the Stage instance (1-120 characters).</param>
        /// <param name="privacyLevel">The privacy level of the Stage instance.</param>
        /// <param name="discoverableDisabled">Whether or not Stage discovery is disabled.</param>
        [JsonConstructor]
        public StageInstance(Snowflake id, Snowflake guildId, Snowflake channelId, string topic, PrivacyLevel privacyLevel, bool discoverableDisabled)
        {
            Id = id;
            GuildId = guildId;
            ChannelId = channelId;
            Topic = topic;
            PrivacyLevel = privacyLevel;
            DiscoverableDisabled = discoverableDisabled;
        }

        /// <summary>
        ///     The id of this Stage instance.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake Id { get; }

        /// <summary>
        ///     The guild id of the associated Stage channel.
        /// </summary>
        [JsonPropertyName("guild_id")]
        public Snowflake GuildId { get; }

        /// <summary>
        ///     The id of the associated Stage channel.
        /// </summary>
        [JsonPropertyName("channel_id")]
        public Snowflake ChannelId { get; }

        /// <summary>
        ///     The topic of the Stage instance (1-120 characters).
        /// </summary>
        [JsonPropertyName("topic")]
        public string Topic { get; }

        /// <summary>
        ///     The privacy level of the Stage instance.
        /// </summary>
        [JsonPropertyName("privacy_level")]
        public PrivacyLevel PrivacyLevel { get; }

        /// <summary>
        ///     Whether or not Stage discovery is disabled.
        /// </summary>
        [JsonPropertyName("discoverable_disabled")]
        public bool DiscoverableDisabled { get; }
    }
}
