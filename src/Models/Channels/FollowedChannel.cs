using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord followed channel object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/channel#followed-channel-object-followed-channel-structure"/>
    /// </remarks>
    public record FollowedChannel
    {
        /// <summary>
        /// Source <see cref="Channel"/> id.
        /// </summary>
        [JsonPropertyName("channel_id")]
        public Snowflake ChannelId { get; init; }

        /// <summary>
        /// Created target <see cref="Webhook"/> id.
        /// </summary>
        [JsonPropertyName("webhook_id")]
        public Snowflake WebhookId { get; init; }
    }
}
