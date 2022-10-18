using System.Text.Json.Serialization;

namespace Discord.API
{
    internal class Guild
    {
        [JsonPropertyName("id")]
        public ulong Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("icon")]
        public string Icon { get; set; }
        [JsonPropertyName("splash")]
        public string Splash { get; set; }
        [JsonPropertyName("discovery_splash")]
        public string DiscoverySplash { get; set; }
        [JsonPropertyName("owner_id")]
        public ulong OwnerId { get; set; }
        [JsonPropertyName("region")]
        public string Region { get; set; }
        [JsonPropertyName("afk_channel_id")]
        public ulong? AFKChannelId { get; set; }
        [JsonPropertyName("afk_timeout")]
        public int AFKTimeout { get; set; }
        [JsonPropertyName("verification_level")]
        public VerificationLevel VerificationLevel { get; set; }
        [JsonPropertyName("default_message_notifications")]
        public DefaultMessageNotifications DefaultMessageNotifications { get; set; }
        [JsonPropertyName("explicit_content_filter")]
        public ExplicitContentFilterLevel ExplicitContentFilter { get; set; }
        [JsonPropertyName("voice_states")]
        public VoiceState[] VoiceStates { get; set; }
        [JsonPropertyName("roles")]
        public Role[] Roles { get; set; }
        [JsonPropertyName("emojis")]
        public Emoji[] Emojis { get; set; }
        [JsonPropertyName("features")]
        public GuildFeatures Features { get; set; }
        [JsonPropertyName("mfa_level")]
        public MfaLevel MfaLevel { get; set; }
        [JsonPropertyName("application_id")]
        public ulong? ApplicationId { get; set; }
        [JsonPropertyName("widget_enabled")]
        public Optional<bool> WidgetEnabled { get; set; }
        [JsonPropertyName("widget_channel_id")]
        public Optional<ulong?> WidgetChannelId { get; set; }
        [JsonPropertyName("system_channel_id")]
        public ulong? SystemChannelId { get; set; }
        [JsonPropertyName("premium_tier")]
        public PremiumTier PremiumTier { get; set; }
        [JsonPropertyName("vanity_url_code")]
        public string VanityURLCode { get; set; }
        [JsonPropertyName("banner")]
        public string Banner { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        // this value is inverted, flags set will turn OFF features
        [JsonPropertyName("system_channel_flags")]
        public SystemChannelMessageDeny SystemChannelFlags { get; set; }
        [JsonPropertyName("rules_channel_id")]
        public ulong? RulesChannelId { get; set; }
        [JsonPropertyName("max_presences")]
        public Optional<int?> MaxPresences { get; set; }
        [JsonPropertyName("max_members")]
        public Optional<int> MaxMembers { get; set; }
        [JsonPropertyName("premium_subscription_count")]
        public int? PremiumSubscriptionCount { get; set; }
        [JsonPropertyName("preferred_locale")]
        public string PreferredLocale { get; set; }
        [JsonPropertyName("public_updates_channel_id")]
        public ulong? PublicUpdatesChannelId { get; set; }
        [JsonPropertyName("max_video_channel_users")]
        public Optional<int> MaxVideoChannelUsers { get; set; }
        [JsonPropertyName("approximate_member_count")]
        public Optional<int> ApproximateMemberCount { get; set; }
        [JsonPropertyName("approximate_presence_count")]
        public Optional<int> ApproximatePresenceCount { get; set; }
        [JsonPropertyName("threads")]
        public Optional<Channel[]> Threads { get; set; }
        [JsonPropertyName("nsfw_level")]
        public NsfwLevel NsfwLevel { get; set; }
        [JsonPropertyName("stickers")]
        public Sticker[] Stickers { get; set; }
        [JsonPropertyName("premium_progress_bar_enabled")]
        public Optional<bool> IsBoostProgressBarEnabled { get; set; }
    }
}
