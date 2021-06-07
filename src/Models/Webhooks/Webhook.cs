using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord webhook object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/webhook#webhook-object-webhook-structure"/>
    /// </remarks>
    public record Webhook
    {
        /// <summary>
        /// The id of the <see cref="Webhook"/>.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake Id { get; }

        /// <summary>
        /// The type of the <see cref="Webhook"/>.
        /// </summary>
        [JsonPropertyName("type")]
        public WebhookType Type { get; }

        /// <summary>
        /// The <see cref="Guild"/> id this <see cref="Webhook"/> is for, if any.
        /// </summary>
        [JsonPropertyName("guild_id")]
        public Optional<Snowflake?> GuildId { get; }

        /// <summary>
        /// The <see cref="Channel"/> id this <see cref="Webhook"/> is for, if any.
        /// </summary>
        [JsonPropertyName("channel_id")]
        public Snowflake? ChannelId { get; }

        /// <summary>
        /// The <see cref="User"/> this <see cref="Webhook"/> was created by (not
        /// returned when getting a <see cref="Webhook"/> with its token).
        /// </summary>
        [JsonPropertyName("user")]
        public Optional<User> User { get; }

        /// <summary>
        /// The default name of the <see cref="Webhook"/>.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; }

        /// <summary>
        /// The default user avatar hash of the <see cref="Webhook"/>.
        /// </summary>
        [JsonPropertyName("avatar")]
        public string? Avatar { get; }

        /// <summary>
        /// The secure token of the <see cref="Webhook"/> (returned for Incoming Webhooks).
        /// </summary>
        [JsonPropertyName("token")]
        public Optional<string> Token { get; }

        /// <summary>
        /// The bot/OAuth2 <see cref="Application"/> that created this <see cref="Webhook"/>.
        /// </summary>
        [JsonPropertyName("application_id")]
        public Snowflake? ApplicationId { get; }

        /// <summary>
        /// The <see cref="Guild"/> of the <see cref="Channel"/> that this <see cref="Webhook"/>
        /// is following (returned for Channel Follower Webhooks).
        /// </summary>
        [JsonPropertyName("source_guild")]
        public Optional<Guild> SourceGuild { get; }

        /// <summary>
        /// The <see cref="Channel"/> that this <see cref="Webhook"/> is following
        /// (returned for Channel Follower Webhooks).
        /// </summary>
        [JsonPropertyName("source_channel")]
        public Optional<Channel> SourceChannel { get; }

        /// <summary>
        /// The url used for executing the <see cref="Webhook"/> (returned by the webhooks OAuth2 flow).
        /// </summary>
        [JsonPropertyName("url")]
        public Optional<string> Url { get; }
    }
}
