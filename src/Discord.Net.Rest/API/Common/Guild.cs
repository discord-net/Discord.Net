#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API
{
    internal class Guild
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("icon")]
        public string Icon { get; set; }
        [JsonProperty("splash")]
        public string Splash { get; set; }
        [JsonProperty("discovery_splash")]
        public string DiscoverySplash { get; set; }
        [JsonProperty("owner_id")]
        public ulong OwnerId { get; set; }
        [JsonProperty("region")]
        public string Region { get; set; }
        [JsonProperty("afk_channel_id")]
        public ulong? AFKChannelId { get; set; }
        [JsonProperty("afk_timeout")]
        public int AFKTimeout { get; set; }
        [JsonProperty("embed_enabled")]
        public Optional<bool> EmbedEnabled { get; set; }
        [JsonProperty("embed_channel_id")]
        public Optional<ulong?> EmbedChannelId { get; set; }
        [JsonProperty("verification_level")]
        public VerificationLevel VerificationLevel { get; set; }
        [JsonProperty("default_message_notifications")]
        public DefaultMessageNotifications DefaultMessageNotifications { get; set; }
        [JsonProperty("explicit_content_filter")]
        public ExplicitContentFilterLevel ExplicitContentFilter { get; set; }
        [JsonProperty("voice_states")]
        public VoiceState[] VoiceStates { get; set; }
        [JsonProperty("roles")]
        public Role[] Roles { get; set; }
        [JsonProperty("emojis")]
        public Emoji[] Emojis { get; set; }
        [JsonProperty("features")]
        public string[] Features { get; set; }
        [JsonProperty("mfa_level")]
        public MfaLevel MfaLevel { get; set; }
        [JsonProperty("application_id")]
        public ulong? ApplicationId { get; set; }
        [JsonProperty("widget_enabled")]
        public Optional<bool> WidgetEnabled { get; set; }
        [JsonProperty("widget_channel_id")]
        public Optional<ulong?> WidgetChannelId { get; set; }
        [JsonProperty("system_channel_id")]
        public ulong? SystemChannelId { get; set; }
        [JsonProperty("premium_tier")]
        public PremiumTier PremiumTier { get; set; }
        [JsonProperty("vanity_url_code")]
        public string VanityURLCode { get; set; }
        [JsonProperty("banner")]
        public string Banner { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        // this value is inverted, flags set will turn OFF features
        [JsonProperty("system_channel_flags")]
        public SystemChannelMessageDeny SystemChannelFlags { get; set; }
        [JsonProperty("rules_channel_id")]
        public ulong? RulesChannelId { get; set; }
        [JsonProperty("max_presences")]
        public Optional<int?> MaxPresences { get; set; }
        [JsonProperty("max_members")]
        public Optional<int> MaxMembers { get; set; }
        [JsonProperty("premium_subscription_count")]
        public Optional<int> PremiumSubscriptionCount { get; set; }
        [JsonProperty("preferred_locale")]
        public string PreferredLocale { get; set; }
        [JsonProperty("public_updates_channel_id")]
        public ulong? PublicUpdatesChannelId { get; set; }
        [JsonProperty("max_video_channel_users")]
        public Optional<int> MaxVideoChannelUsers { get; set; }
        [JsonProperty("approximate_member_count")]
        public Optional<int> ApproximateMemberCount { get; set; }
        [JsonProperty("approximate_presence_count")]
        public Optional<int> ApproximatePresenceCount { get; set; }
    }
}
