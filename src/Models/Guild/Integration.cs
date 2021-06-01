using System;
using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a integration object.
    /// </summary>
    public record Integration
    {
        /// <summary>
        ///     Creates a <see cref="Integration"/> with the provided parameters.
        /// </summary>
        /// <param name="id">Integration id.</param>
        /// <param name="name">Integration name.</param>
        /// <param name="type">Integration type (twitch, youtube, or discord).</param>
        /// <param name="enabled">Is this integration enabled.</param>
        /// <param name="syncing">Is this integration syncing.</param>
        /// <param name="roleId">Id that this integration uses for "subscribers".</param>
        /// <param name="enableEmoticons">Whether emoticons should be synced for this integration (twitch only currently).</param>
        /// <param name="expireBehavior">The behavior of expiring subscribers.</param>
        /// <param name="expireGracePeriod">The grace period (in days) before expiring subscribers.</param>
        /// <param name="user">User for this integration.</param>
        /// <param name="account">Integration account information.</param>
        /// <param name="syncedAt">When this integration was last synced.</param>
        /// <param name="subscriberCount">How many subscribers this integration has.</param>
        /// <param name="revoked">Has this integration been revoked.</param>
        /// <param name="application">The bot/OAuth2 application for discord integrations.</param>
        [JsonConstructor]
        public Integration(Snowflake id, string name, string type, bool enabled, Optional<bool> syncing, Optional<Snowflake> roleId, Optional<bool> enableEmoticons, Optional<IntegrationExpireBehavior> expireBehavior, Optional<int> expireGracePeriod, Optional<User> user, IntegrationAccount account, Optional<DateTimeOffset> syncedAt, Optional<int> subscriberCount, Optional<bool> revoked, Optional<Application> application)
        {
            Id = id;
            Name = name;
            Type = type;
            Enabled = enabled;
            Syncing = syncing;
            RoleId = roleId;
            EnableEmoticons = enableEmoticons;
            ExpireBehavior = expireBehavior;
            ExpireGracePeriod = expireGracePeriod;
            User = user;
            Account = account;
            SyncedAt = syncedAt;
            SubscriberCount = subscriberCount;
            Revoked = revoked;
            Application = application;
        }

        /// <summary>
        ///     Integration id.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake Id { get; }

        /// <summary>
        ///     Integration name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; }

        /// <summary>
        ///     Integration type (twitch, youtube, or discord).
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; }

        /// <summary>
        ///     Is this integration enabled.
        /// </summary>
        [JsonPropertyName("enabled")]
        public bool Enabled { get; }

        /// <summary>
        ///     Is this integration syncing.
        /// </summary>
        [JsonPropertyName("syncing")]
        public Optional<bool> Syncing { get; }

        /// <summary>
        ///     Id that this integration uses for "subscribers".
        /// </summary>
        [JsonPropertyName("role_id")]
        public Optional<Snowflake> RoleId { get; }

        /// <summary>
        ///     Whether emoticons should be synced for this integration (twitch only currently).
        /// </summary>
        [JsonPropertyName("enable_emoticons")]
        public Optional<bool> EnableEmoticons { get; }

        /// <summary>
        ///     The behavior of expiring subscribers.
        /// </summary>
        [JsonPropertyName("expire_behavior")]
        public Optional<IntegrationExpireBehavior> ExpireBehavior { get; }

        /// <summary>
        ///     The grace period (in days) before expiring subscribers.
        /// </summary>
        [JsonPropertyName("expire_grace_period")]
        public Optional<int> ExpireGracePeriod { get; }

        /// <summary>
        ///     User for this integration.
        /// </summary>
        [JsonPropertyName("user")]
        public Optional<User> User { get; }

        /// <summary>
        ///     Integration account information.
        /// </summary>
        [JsonPropertyName("account")]
        public IntegrationAccount Account { get; }

        /// <summary>
        ///     When this integration was last synced.
        /// </summary>
        [JsonPropertyName("synced_at")]
        public Optional<DateTimeOffset> SyncedAt { get; }

        /// <summary>
        ///     How many subscribers this integration has.
        /// </summary>
        [JsonPropertyName("subscriber_count")]
        public Optional<int> SubscriberCount { get; }

        /// <summary>
        ///     Has this integration been revoked.
        /// </summary>
        [JsonPropertyName("revoked")]
        public Optional<bool> Revoked { get; }

        /// <summary>
        ///     The bot/OAuth2 application for discord integrations.
        /// </summary>
        [JsonPropertyName("application")]
        public Optional<Application> Application { get; }
    }
}
