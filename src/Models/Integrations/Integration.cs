using System;
using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord integration object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/guild#integration-object-integration-structure"/>
    /// </remarks>
    public record Integration
    {
        /// <summary>
        /// <see cref="Integration"/> id.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake Id { get; init; }

        /// <summary>
        /// <see cref="Integration"/> name.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; init; } // Required property candidate

        /// <summary>
        /// <see cref="Integration"/> type (twitch, youtube, or discord).
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; init; } // Required property candidate

        /// <summary>
        /// Returns if this <see cref="Integration"/> is enabled.
        /// </summary>
        [JsonPropertyName("enabled")]
        public bool Enabled { get; init; }

        /// <summary>
        /// Returns if this <see cref="Integration"/> is syncing.
        /// </summary>
        [JsonPropertyName("syncing")]
        public Optional<bool> Syncing { get; init; }

        /// <summary>
        /// Id that this <see cref="Integration"/> uses for "subscribers".
        /// </summary>
        [JsonPropertyName("role_id")]
        public Optional<Snowflake> RoleId { get; init; }

        /// <summary>
        /// Whether emoticons should be synced for this <see cref="Integration"/> (twitch only currently).
        /// </summary>
        [JsonPropertyName("enable_emoticons")]
        public Optional<bool> EnableEmoticons { get; init; }

        /// <summary>
        /// The behavior of expiring subscribers.
        /// </summary>
        [JsonPropertyName("expire_behavior")]
        public Optional<IntegrationExpireBehavior> ExpireBehavior { get; init; }

        /// <summary>
        /// The grace period (in days) before expiring subscribers.
        /// </summary>
        [JsonPropertyName("expire_grace_period")]
        public Optional<int> ExpireGracePeriod { get; init; }

        /// <summary>
        /// <see cref="User"/> for this <see cref="Integration"/>.
        /// </summary>
        [JsonPropertyName("user")]
        public Optional<User> User { get; init; }

        /// <summary>
        /// <see cref="IntegrationAccount"/> information.
        /// </summary>
        [JsonPropertyName("account")]
        public IntegrationAccount? Account { get; init; } // Required property candidate

        /// <summary>
        /// When this <see cref="Integration"/> was last synced.
        /// </summary>
        [JsonPropertyName("synced_at")]
        public Optional<DateTimeOffset> SyncedAt { get; init; }

        /// <summary>
        /// How many subscribers this <see cref="Integration"/> has.
        /// </summary>
        [JsonPropertyName("subscriber_count")]
        public Optional<int> SubscriberCount { get; init; }

        /// <summary>
        /// Has this <see cref="Integration"/> been revoked.
        /// </summary>
        [JsonPropertyName("revoked")]
        public Optional<bool> Revoked { get; init; }

        /// <summary>
        /// The bot/OAuth2 <see cref="Models.Application"/> for discord integrations.
        /// </summary>
        [JsonPropertyName("application")]
        public Optional<Application> Application { get; init; }
    }
}
