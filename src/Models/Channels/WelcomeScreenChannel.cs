using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a welcome screen channel object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/guild#welcome-screen-object-welcome-screen-channel-structure"/>
    /// </remarks>
    public record WelcomeScreenChannel
    {
        /// <summary>
        /// The <see cref="WelcomeScreenChannel"/>'s id.
        /// </summary>
        [JsonPropertyName("channel_id")]
        public Snowflake ChannelId { get; init; }

        /// <summary>
        /// The description shown for the <see cref="WelcomeScreenChannel"/>.
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; init; } // Required property candidate

        /// <summary>
        /// The <see cref="Emoji.Id"/>, if the <see cref="Emoji"/> is custom.
        /// </summary>
        [JsonPropertyName("emoji_id")]
        public Snowflake? EmojiId { get; init; }

        /// <summary>
        /// The <see cref="Emoji.Name"/> if custom, the unicode character if standard, or null if no <see cref="Emoji"/> is set.
        /// </summary>
        [JsonPropertyName("emoji_name")]
        public string? EmojiName { get; init; }
    }
}
