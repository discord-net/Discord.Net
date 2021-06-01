using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a welcome screen channel object.
    /// </summary>
    public record WelcomeScreenChannel
    {
        /// <summary>
        ///     Creates a <see cref="WelcomeScreenChannel"/> with the provided parameters.
        /// </summary>
        /// <param name="channelId">The channel's id.</param>
        /// <param name="description">The description shown for the channel.</param>
        /// <param name="emojiId">The emoji id, if the emoji is custom.</param>
        /// <param name="emojiName">The emoji name if custom, the unicode character if standard, or null if no emoji is set.</param>
        [JsonConstructor]
        public WelcomeScreenChannel(Snowflake channelId, string description, Snowflake? emojiId, string? emojiName)
        {
            ChannelId = channelId;
            Description = description;
            EmojiId = emojiId;
            EmojiName = emojiName;
        }

        /// <summary>
        ///     The channel's id.
        /// </summary>
        [JsonPropertyName("channel_id")]
        public Snowflake ChannelId { get; }

        /// <summary>
        ///     The description shown for the channel.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; }

        /// <summary>
        ///     The emoji id, if the emoji is custom.
        /// </summary>
        [JsonPropertyName("emoji_id")]
        public Snowflake? EmojiId { get; }

        /// <summary>
        ///     The emoji name if custom, the unicode character if standard, or null if no emoji is set.
        /// </summary>
        [JsonPropertyName("emoji_name")]
        public string? EmojiName { get; }
    }
}
