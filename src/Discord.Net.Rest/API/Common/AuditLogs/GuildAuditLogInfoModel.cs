using Newtonsoft.Json;

namespace Discord.API.AuditLogs;

public class GuildAuditLogInfoModel : IAuditLogInfoModel
{
    [JsonProperty("name")]
    public Optional<string> Name { get; set; }

    [JsonProperty("afk_timeout")]
    public Optional<int?> AfkTimeout { get; set; }

    [JsonProperty("widget_enabled")]
    public Optional<bool> IsEmbeddable { get; set; }

    [JsonProperty("default_message_notifications")]
    public Optional<DefaultMessageNotifications> DefaultMessageNotifications { get; set; }

    [JsonProperty("mfa_level")]
    public Optional<MfaLevel> MfaLevel { get; set; }

    [JsonProperty("verification_level")]
    public Optional<VerificationLevel> VerificationLevel { get; set; }

    [JsonProperty("explicit_content_filter")]
    public Optional<ExplicitContentFilterLevel> ExplicitContentFilterLevel { get; set; }

    [JsonProperty("icon_hash")]
    public Optional<string> IconHash { get; set; }

    [JsonProperty("discovery_splash")]
    public Optional<string> DiscoverySplash { get; set; }

    [JsonProperty("splash")]
    public Optional<string> Splash { get; set; }

    [JsonProperty("afk_channel_id")]
    public Optional<ulong> AfkChannelId { get; set; }

    [JsonProperty("widget_channel_id")]
    public Optional<ulong> EmbeddedChannelId { get; set; }

    [JsonProperty("system_channel_id")]
    public Optional<ulong> SystemChannelId { get; set; }

    [JsonProperty("rules_channel_id")]
    public Optional<ulong> RulesChannelId { get; set; }

    [JsonProperty("public_updates_channel_id")]
    public Optional<ulong> PublicUpdatesChannelId { get; set; }

    [JsonProperty("owner_id")]
    public Optional<ulong> OwnerId { get; set; }

    [JsonProperty("application_id")]
    public Optional<ulong> ApplicationId { get; set; }

    [JsonProperty("region")]
    public Optional<string> RegionId { get; set; }

    [JsonProperty("banner")]
    public Optional<string> Banner { get; set; }

    [JsonProperty("vanity_url_code")]
    public Optional<string> VanityUrl { get; set; }

    [JsonProperty("system_channel_flags")]
    public Optional<SystemChannelMessageDeny> SystemChannelFlags { get; set; }

    [JsonProperty("description")]
    public Optional<string> Description { get; set; }

    [JsonProperty("preferred_locale")]
    public Optional<string> PreferredLocale { get; set; }

    [JsonProperty("nsfw_level")]
    public Optional<NsfwLevel> NsfwLevel { get; set; }

    [JsonProperty("premium_progress_bar_enabled")]
    public Optional<bool> ProgressBarEnabled { get; set; }

}
