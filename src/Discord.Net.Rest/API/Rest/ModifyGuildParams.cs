using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ModifyGuildParams
    {
        [JsonProperty("username")]
        public Optional<string> Username { get; set; }
        [JsonProperty("name")]
        public Optional<string> Name { get; set; }
        [JsonProperty("region")]
        public Optional<string> RegionId { get; set; }
        [JsonProperty("verification_level")]
        public Optional<VerificationLevel> VerificationLevel { get; set; }
        [JsonProperty("default_message_notifications")]
        public Optional<DefaultMessageNotifications> DefaultMessageNotifications { get; set; }
        [JsonProperty("afk_timeout")]
        public Optional<int> AfkTimeout { get; set; }
        [JsonProperty("system_channel_id")]
        public Optional<ulong?> SystemChannelId { get; set; }
        [JsonProperty("icon")]
        public Optional<Image?> Icon { get; set; }
        [JsonProperty("banner")]
        public Optional<Image?> Banner { get; set; }
        [JsonProperty("splash")]
        public Optional<Image?> Splash { get; set; }
        [JsonProperty("afk_channel_id")]
        public Optional<ulong?> AfkChannelId { get; set; }
        [JsonProperty("owner_id")]
        public Optional<ulong> OwnerId { get; set; }
        [JsonProperty("explicit_content_filter")]
        public Optional<ExplicitContentFilterLevel> ExplicitContentFilter { get; set; }
        [JsonProperty("system_channel_flags")]
        public Optional<SystemChannelMessageDeny> SystemChannelFlags { get; set; }
        [JsonProperty("preferred_locale")]
        public string PreferredLocale { get; set; }
        [JsonProperty("premium_progress_bar_enabled")]
        public Optional<bool> IsBoostProgressBarEnabled { get; set; }
        [JsonProperty("features")]
        public Optional<GuildFeatures> GuildFeatures { get; set; }
    }
}
