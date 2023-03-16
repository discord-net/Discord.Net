using Discord.Rest;

namespace Discord.API.AuditLogs;

public class GuildInfoAuditLogModel : IAuditLogInfoModel
{
    [JsonField("name")]
    public string Name { get; set; }

    [JsonField("afk_timeout")]
    public int? AfkTimeout { get; set; }

    [JsonField("widget_enabled")]
    public bool? IsEmbeddable { get; set; }

    [JsonField("default_message_notifications")]
    public DefaultMessageNotifications? DefaultMessageNotifications { get; set; }

    [JsonField("mfa_level")]
    public MfaLevel? MfaLevel { get; set; }

    [JsonField("verification_level")]
    public VerificationLevel? VerificationLevel { get; set; }

    [JsonField("explicit_content_filter")]
    public ExplicitContentFilterLevel? ExplicitContentFilterLevel { get; set; }

    [JsonField("icon_hash")]
    public string IconHash { get; set; }

    [JsonField("discovery_splash")]
    public string DiscoverySplash { get; set; }

    [JsonField("splash")]
    public string Splash { get; set; }

    [JsonField("afk_channel_id")]
    public ulong? AfkChannelId { get; set; }

    [JsonField("widget_channel_id")]
    public ulong? EmbeddedChannelId { get; set; }

    [JsonField("system_channel_id")]
    public ulong? SystemChannelId { get; set; }

    [JsonField("rules_channel_id")]
    public ulong? RulesChannelId { get; set; }

    [JsonField("public_updates_channel_id")]
    public ulong? PublicUpdatesChannelId { get; set; }

    [JsonField("owner_id")]
    public ulong? OwnerId { get; set; }

    [JsonField("application_id")]
    public ulong? ApplicationId { get; set; }

    [JsonField("region")]
    public string RegionId { get; set; }

    [JsonField("banner")]
    public string Banner { get; set; }

    [JsonField("vanity_url_code")]
    public string VanityUrl { get; set; }

    [JsonField("system_channel_flags")]
    public SystemChannelMessageDeny? SystemChannelFlags { get; set; }

    [JsonField("description")]
    public string Description { get; set; }

    [JsonField("preferred_locale")]
    public string PreferredLocale { get; set; }

    [JsonField("nsfw_level")]
    public NsfwLevel? NsfwLevel { get; set; }

    [JsonField("premium_progress_bar_enabled")]
    public bool? ProgressBarEnabled { get; set; }

}
