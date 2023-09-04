using Discord.Models;
using System.Text.Json.Serialization;

namespace Discord.API;

public class Guild : IGuildModel
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("icon")]
    public string? Icon { get; set; }

    [JsonPropertyName("icon_hash")]
    public string? IconHash { get; set; }

    [JsonPropertyName("splash")]
    public string? Splash { get; set; }

    [JsonPropertyName("discovery_splash")]
    public string? DiscoverySplash { get; set; }
    
    [JsonPropertyName("owner_id")]
    public ulong OwnerId { get; set; }
    
    [JsonPropertyName("region")]
    public string Region { get; set; }

    [JsonPropertyName("afk_channel_id")]
    public ulong? AFKChannelId { get; set; }

    [JsonPropertyName("afk_timeout")]
    public AfkTimeout AFKTimeout { get; set; }

    [JsonPropertyName("widget_enabled")]
    public Optional<bool> WidgetEnabled { get; set; }

    [JsonPropertyName("widget_channel_id")]
    public Optional<ulong?> WidgetChannelId { get; set; }

    [JsonPropertyName("verification_level")]
    public VerificationLevel VerificationLevel { get; set; }

    [JsonPropertyName("default_message_notifications")]
    public DefaultMessageNotifications DefaultMessageNotifications { get; set; }

    [JsonPropertyName("explicit_content_filter")]
    public ExplicitContentFilterLevel ExplicitContentFilter { get; set; }
    
    [JsonPropertyName("roles")]
    public Role[] Roles { get; set; }

    [JsonPropertyName("emojis")]
    public Emoji[] Emojis { get; set; }

    [JsonPropertyName("features")]
    public GuildFeatures Features { get; set; }

    [JsonPropertyName("mfa_level")]
    public MfaLevel MFALevel { get; set; }

    [JsonPropertyName("application_id")]
    public ulong? ApplicationId { get; set; }

    [JsonPropertyName("system_channel_id")]
    public ulong? SystemChannelId { get; set; }

    // this value is inverted, flags set will turn OFF features
    [JsonPropertyName("system_channel_flags")]
    public SystemChannelFlags SystemChannelFlags { get; set; }

    [JsonPropertyName("rules_channel_id")]
    public ulong? RulesChannelId { get; set; }

    [JsonPropertyName("max_presences")]
    public Optional<int?> MaxPresences { get; set; }

    [JsonPropertyName("max_members")]
    public Optional<int> MaxMembers { get; set; }

    [JsonPropertyName("vanity_url_code")]
    public string? VanityUrlCode { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("banner")]
    public string? Banner { get; set; }

    [JsonPropertyName("premium_tier")]
    public PremiumTier PremiumTier { get; set; }

    [JsonPropertyName("premium_subscription_count")]
    public Optional<int> PremiumSubscriptionCount { get; set; }

    [JsonPropertyName("preferred_locale")]
    public string PreferredLocale { get; set; }

    [JsonPropertyName("public_updates_channel_id")]
    public ulong? PublicUpdatesChannelId { get; set; }

    [JsonPropertyName("max_video_channel_users")]
    public Optional<int> MaxVideoChannelUsers { get; set; }

    [JsonPropertyName("max_stage_video_channel_users")]
    public Optional<int> MaxStageVideoChannelUsers { get; set; }

    [JsonPropertyName("approximate_member_count")]
    public Optional<int> ApproximateMemberCount { get; set; }

    [JsonPropertyName("approximate_presence_count")]
    public Optional<int> ApproximatePresenceCount { get; set; }

    [JsonPropertyName("welcome_screen")]
    public Optional<WelcomeScreen> WelcomeScreen { get; set; }

    [JsonPropertyName("nsfw_level")]
    public NsfwLevel NsfwLevel { get; set; }

    [JsonPropertyName("stickers")]
    public Optional<Sticker[]> Stickers { get; set; }

    [JsonPropertyName("premium_progress_bar_enabled")]
    public Optional<bool> IsBoostProgressBarEnabled { get; set; }

    [JsonPropertyName("safety_alerts_channel_id")]
    public Optional<ulong?> SafetyAlertsChannelId { get; set; }


    int IGuildModel.AFKTimeout => (int)AFKTimeout;

    bool IGuildModel.WidgetEnabled => WidgetEnabled.GetValueOrDefault();

    ulong? IGuildModel.WidgetChannelId => WidgetChannelId.GetValueOrDefault();

    GuildFeature IGuildModel.Features => Features.Value;

    string[] IGuildModel.ExperimentalFeatures => Features.Experimental.ToArray();

    MfaLevel IGuildModel.MFALevel => MFALevel;

    int? IGuildModel.MaxPresense => MaxPresences.GetValueOrDefault();

    int? IGuildModel.MaxMembers => MaxMembers.ToNullable();

    string? IGuildModel.Vanity => VanityUrlCode;

    int? IGuildModel.PremiumSubscriptionCount => PremiumSubscriptionCount.ToNullable();

    int? IGuildModel.MaxVideoChannelUsers => MaxVideoChannelUsers.ToNullable();

    int? IGuildModel.MaxStageVideoChannelUsers => MaxStageVideoChannelUsers.ToNullable();

    int? IGuildModel.ApproximateMemberCount => ApproximateMemberCount.ToNullable();

    IWelcomeScreenModel? IGuildModel.WelcomeScreen => WelcomeScreen.GetValueOrDefault();

    bool? IGuildModel.PremiumProgressBarEnabled => IsBoostProgressBarEnabled.ToNullable();

    ulong? IGuildModel.SafetyAlertsChannelId => SafetyAlertsChannelId.GetValueOrDefault();
}
