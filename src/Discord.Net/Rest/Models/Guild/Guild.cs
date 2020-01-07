#pragma warning disable CS8618 // Uninitialized NRT expected in models
using System.Text.Json.Serialization;

namespace Discord.Models
{
    public class Guild
    {
        public const int MinGuildNameLength = 2;
        public const int MaxGuildNameLength = 100;

        public const int DefaultMaxPresences = 5000;
        public const string DefaultLocale = "en-US";

        [JsonPropertyName("id")]
        public Snowflake Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("icon")]
        public string? IconId { get; set; }
        [JsonPropertyName("splash")]
        public string? SplashId { get; set; }
        [JsonPropertyName("owner")]
        public Optional<bool> IsOwner { get; set; }
        [JsonPropertyName("owner_id")]
        public Snowflake OwnerId { get; set; }
        [JsonPropertyName("permissions")]
        public Optional<GuildPermissions> UserPermissions { get; set; }
        [JsonPropertyName("region")]
        public string VoiceRegionId { get; set; }
        [JsonPropertyName("afk_channel_id")]
        public Snowflake? AfkChannelId { get; set; }
        [JsonPropertyName("afk_timeout")]
        public int AfkTimeout { get; set; }
        [JsonPropertyName("embed_enabled")]
        public Optional<bool> IsEmbeddable { get; set; }
        [JsonPropertyName("embed_channel_id")]
        public Optional<Snowflake> EmbeddedChannelId { get; set; }
        [JsonPropertyName("verification_level")]
        public VerificationLevel VerificationLevel { get; set; }
        [JsonPropertyName("default_message_notifications")]
        public MessageNotificationLevel DefaultMessageNotificationLevel { get; set; }
        [JsonPropertyName("explicit_content_filter")]
        public ExplicitContentFilterLevel ExplicitContentFilterLevel { get; set; }
        // todo: roles
        // todo: emoji
        [JsonPropertyName("features")]
        public string[] Features { get; set; }
        [JsonPropertyName("mfa_level")]
        public MFALevel MFALevel { get; set; }
        [JsonPropertyName("application_id")]
        public Snowflake? OwnerApplicationId { get; set; }
        [JsonPropertyName("widget_enabled")]
        public Optional<bool> IsWidgetEnabled { get; set; }
        [JsonPropertyName("widget_channel_id")]
        public Optional<Snowflake> WidgetChannelId { get; set; }
        [JsonPropertyName("system_channel_id")]
        public Snowflake? SystemChannelId { get; set; }
        [JsonPropertyName("max_presences")]
        public Optional<int?> MaxPresences { get; set; }
        [JsonPropertyName("max_members")]
        public Optional<int> MaxMembers { get; set; }
        [JsonPropertyName("vanity_url_code")]
        public string? VanityUrlCode { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("banner")]
        public string? BannerId { get; set; }
        [JsonPropertyName("premium_tier")]
        public PremiumTier PremiumTier { get; set; }
        [JsonPropertyName("premium_subscription_count")]
        public Optional<int> PremiumSubscriptionCount { get; set; }
        [JsonPropertyName("preferred_locale")]
        public string PreferredLocale { get; set; }
    }
}
