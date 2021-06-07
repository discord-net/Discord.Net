using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord message reference object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/channel#message-reference-object-message-reference-structure"/>
    /// </remarks>
    public record MessageReference
    {
        /// <summary>
        /// Id of the originating <see cref="Message"/>.
        /// </summary>
        [JsonPropertyName("message_id")]
        public Optional<Snowflake> MessageId { get; init; }

        /// <summary>
        /// Id of the originating <see cref="Message"/>'s <see cref="Channel"/>.
        /// </summary>
        [JsonPropertyName("channel_id")]
        public Optional<Snowflake> ChannelId { get; init; }

        /// <summary>
        /// Id of the originating <see cref="Message"/>'s <see cref="Guild"/>.
        /// </summary>
        [JsonPropertyName("guild_id")]
        public Optional<Snowflake> GuildId { get; init; }

        /// <summary>
        /// When sending, whether to error if the referenced <see cref="Message"/> doesn't
        /// exist instead of sending as a normal (non-reply) <see cref="Message"/>, default true.
        /// </summary>
        [JsonPropertyName("fail_if_not_exists")]
        public Optional<bool> FailIfNotExists { get; init; }
    }
}
