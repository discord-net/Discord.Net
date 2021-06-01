using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a guild widget object.
    /// </summary>
    public record GuildWidget
    {
        /// <summary>
        ///     Creates a <see cref="GuildWidget"/> with the provided parameters.
        /// </summary>
        /// <param name="enabled">Whether the widget is enabled.</param>
        /// <param name="channelId">The widget channel id.</param>
        [JsonConstructor]
        public GuildWidget(bool enabled, Snowflake? channelId)
        {
            Enabled = enabled;
            ChannelId = channelId;
        }

        /// <summary>
        ///     Whether the widget is enabled.
        /// </summary>
        [JsonPropertyName("enabled")]
        public bool Enabled { get; }

        /// <summary>
        ///     The widget channel id.
        /// </summary>
        [JsonPropertyName("channel_id")]
        public Snowflake? ChannelId { get; }
    }
}
