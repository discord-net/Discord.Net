using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord guild widget object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/guild#guild-widget-object-guild-widget-structure"/>
    /// </remarks>
    public record GuildWidget
    {

        /// <summary>
        /// Whether the widget is enabled.
        /// </summary>
        [JsonPropertyName("enabled")]
        public bool Enabled { get; init; }

        /// <summary>
        /// The widget <see cref="Channel"/> id.
        /// </summary>
        [JsonPropertyName("channel_id")]
        public Snowflake? ChannelId { get; init; }
    }
}
