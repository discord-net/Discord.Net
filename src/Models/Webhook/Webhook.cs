using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a webhook object.
    /// </summary>
    public record Webhook
    {
        /// <summary>
        ///     Creates a <see cref="Webhook"/> with the provided parameters.
        /// </summary>
        /// <param name="id">The id of the webhook.</param>
        /// <param name="type">The type of the webhook.</param>
        /// <param name="guildId">The guild id this webhook is for, if any.</param>
        /// <param name="channelId">The channel id this webhook is for, if any.</param>
        /// <param name="user">The user this webhook was created by (not returned when getting a webhook with its token).</param>
        /// <param name="name">The default name of the webhook.</param>
        /// <param name="avatar">The default user avatar hash of the webhook.</param>
        /// <param name="token">The secure token of the webhook (returned for Incoming Webhooks).</param>
        /// <param name="applicationId">The bot/OAuth2 application that created this webhook.</param>
        /// <param name="sourceGuild">The guild of the channel that this webhook is following (returned for Channel Follower Webhooks).</param>
        /// <param name="sourceChannel">The channel that this webhook is following (returned for Channel Follower Webhooks).</param>
        /// <param name="url">The url used for executing the webhook (returned by the webhooks OAuth2 flow).</param>
        [JsonConstructor]
        public Webhook(Snowflake id, WebhookType type, Optional<Snowflake?> guildId, Snowflake? channelId, Optional<User> user, string? name, string? avatar, Optional<string> token, Snowflake? applicationId, Optional<Guild> sourceGuild, Optional<Channel> sourceChannel, Optional<string> url)
        {
            Id = id;
            Type = type;
            GuildId = guildId;
            ChannelId = channelId;
            User = user;
            Name = name;
            Avatar = avatar;
            Token = token;
            ApplicationId = applicationId;
            SourceGuild = sourceGuild;
            SourceChannel = sourceChannel;
            Url = url;
        }

        /// <summary>
        ///     The id of the webhook.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake Id { get; }

        /// <summary>
        ///     The type of the webhook.
        /// </summary>
        [JsonPropertyName("type")]
        public WebhookType Type { get; }

        /// <summary>
        ///     The guild id this webhook is for, if any.
        /// </summary>
        [JsonPropertyName("guild_id")]
        public Optional<Snowflake?> GuildId { get; }

        /// <summary>
        ///     The channel id this webhook is for, if any.
        /// </summary>
        [JsonPropertyName("channel_id")]
        public Snowflake? ChannelId { get; }

        /// <summary>
        ///     The user this webhook was created by (not returned when getting a webhook with its token).
        /// </summary>
        [JsonPropertyName("user")]
        public Optional<User> User { get; }

        /// <summary>
        ///     The default name of the webhook.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; }

        /// <summary>
        ///     The default user avatar hash of the webhook.
        /// </summary>
        [JsonPropertyName("avatar")]
        public string? Avatar { get; }

        /// <summary>
        ///     The secure token of the webhook (returned for Incoming Webhooks).
        /// </summary>
        [JsonPropertyName("token")]
        public Optional<string> Token { get; }

        /// <summary>
        ///     The bot/OAuth2 application that created this webhook.
        /// </summary>
        [JsonPropertyName("application_id")]
        public Snowflake? ApplicationId { get; }

        /// <summary>
        ///     The guild of the channel that this webhook is following (returned for Channel Follower Webhooks).
        /// </summary>
        [JsonPropertyName("source_guild")]
        public Optional<Guild> SourceGuild { get; }

        /// <summary>
        ///     The channel that this webhook is following (returned for Channel Follower Webhooks).
        /// </summary>
        [JsonPropertyName("source_channel")]
        public Optional<Channel> SourceChannel { get; }

        /// <summary>
        ///     The url used for executing the webhook (returned by the webhooks OAuth2 flow).
        /// </summary>
        [JsonPropertyName("url")]
        public Optional<string> Url { get; }
    }
}
