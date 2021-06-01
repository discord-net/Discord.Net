using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a followed channel object.
    /// </summary>
    public record FollowedChannel
    {
        /// <summary>
        ///     Creates a <see cref="FollowedChannel"/> with the provided parameters.
        /// </summary>
        /// <param name="channelId">Source channel id.</param>
        /// <param name="webhookId">Created target webhook id.</param>
        [JsonConstructor]
        public FollowedChannel(Snowflake channelId, Snowflake webhookId)
        {
            ChannelId = channelId;
            WebhookId = webhookId;
        }

        /// <summary>
        ///     Source channel id.
        /// </summary>
        [JsonPropertyName("channel_id")]
        public Snowflake ChannelId { get; }

        /// <summary>
        ///     Created target webhook id.
        /// </summary>
        [JsonPropertyName("webhook_id")]
        public Snowflake WebhookId { get; }
    }
}
