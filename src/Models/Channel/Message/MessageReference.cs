using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a message reference object.
    /// </summary>
    public record MessageReference
    {
        /// <summary>
        ///     Creates a <see cref="MessageReference"/> with the provided parameters.
        /// </summary>
        /// <param name="messageId">Id of the originating message.</param>
        /// <param name="channelId">Id of the originating message's channel.</param>
        /// <param name="guildId">Id of the originating message's guild.</param>
        /// <param name="failIfNotExists">When sending, whether to error if the referenced message doesn't exist instead of sending as a normal (non-reply) message, default true.</param>
        [JsonConstructor]
        public MessageReference(Optional<Snowflake> messageId, Optional<Snowflake> channelId, Optional<Snowflake> guildId, Optional<bool> failIfNotExists)
        {
            MessageId = messageId;
            ChannelId = channelId;
            GuildId = guildId;
            FailIfNotExists = failIfNotExists;
        }

        /// <summary>
        ///     Id of the originating message.
        /// </summary>
        [JsonPropertyName("message_id")]
        public Optional<Snowflake> MessageId { get; }

        /// <summary>
        ///     Id of the originating message's channel.
        /// </summary>
        [JsonPropertyName("channel_id")]
        public Optional<Snowflake> ChannelId { get; }

        /// <summary>
        ///     Id of the originating message's guild.
        /// </summary>
        [JsonPropertyName("guild_id")]
        public Optional<Snowflake> GuildId { get; }

        /// <summary>
        ///     When sending, whether to error if the referenced message doesn't exist instead of sending as a normal (non-reply) message, default true.
        /// </summary>
        [JsonPropertyName("fail_if_not_exists")]
        public Optional<bool> FailIfNotExists { get; }
    }
}
