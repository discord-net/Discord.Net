using System.Text.Json.Serialization;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class ModifyGuildParams
    {
        [JsonPropertyName("username")]
        public Optional<string> Username { get; set; }
        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }
        [JsonPropertyName("region")]
        public Optional<string> RegionId { get; set; }
        [JsonPropertyName("verification_level")]
        public Optional<VerificationLevel> VerificationLevel { get; set; }
        [JsonPropertyName("default_message_notifications")]
        public Optional<DefaultMessageNotifications> DefaultMessageNotifications { get; set; }
        [JsonPropertyName("afk_timeout")]
        public Optional<int> AfkTimeout { get; set; }
        [JsonPropertyName("system_channel_id")]
        public Optional<ulong?> SystemChannelId { get; set; }
        [JsonPropertyName("icon")]
        public Optional<Image?> Icon { get; set; }
        [JsonPropertyName("banner")]
        public Optional<Image?> Banner { get; set; }
        [JsonPropertyName("splash")]
        public Optional<Image?> Splash { get; set; }
        [JsonPropertyName("afk_channel_id")]
        public Optional<ulong?> AfkChannelId { get; set; }
        [JsonPropertyName("owner_id")]
        public Optional<ulong> OwnerId { get; set; }
        [JsonPropertyName("explicit_content_filter")]
        public Optional<ExplicitContentFilterLevel> ExplicitContentFilter { get; set; }
        [JsonPropertyName("system_channel_flags")]
        public Optional<SystemChannelMessageDeny> SystemChannelFlags { get; set; }
        [JsonPropertyName("preferred_locale")]
        public string PreferredLocale { get; set; }
        [JsonPropertyName("premium_progress_bar_enabled")]
        public Optional<bool> IsBoostProgressBarEnabled { get; set; }
    }
}
