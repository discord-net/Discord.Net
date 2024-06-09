using Discord.Models;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class Guild : IGuildModel
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

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
    public string? Region { get; set; }

    [JsonPropertyName("afk_channel_id")]
    public ulong? AFKChannelId { get; set; }

    [JsonPropertyName("afk_timeout")]
    public int AFKTimeout { get; set; }

    [JsonPropertyName("widget_enabled")]
    public Optional<bool> WidgetEnabled { get; set; }

    [JsonPropertyName("widget_channel_id")]
    public Optional<ulong?> WidgetChannelId { get; set; }

    [JsonPropertyName("verification_level")]
    public int VerificationLevel { get; set; }

    [JsonPropertyName("default_message_notifications")]
    public int DefaultMessageNotifications { get; set; }

    [JsonPropertyName("explicit_content_filter")]
    public int ExplicitContentFilter { get; set; }

    [JsonPropertyName("roles")]
    public required Role[] Roles { get; set; }

    [JsonPropertyName("emojis")]
    public required GuildEmote[] Emojis { get; set; }

    [JsonPropertyName("features")]
    public required string[] Features { get; set; }

    [JsonPropertyName("mfa_level")]
    public int MFALevel { get; set; }

    [JsonPropertyName("application_id")]
    public ulong? ApplicationId { get; set; }

    [JsonPropertyName("system_channel_id")]
    public ulong? SystemChannelId { get; set; }

    // this value is inverted, flags set will turn OFF features
    [JsonPropertyName("system_channel_flags")]
    public int SystemChannelFlags { get; set; }

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
    public int PremiumTier { get; set; }

    [JsonPropertyName("premium_subscription_count")]
    public Optional<int> PremiumSubscriptionCount { get; set; }

    [JsonPropertyName("preferred_locale")]
    public required string PreferredLocale { get; set; }

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
    public int NsfwLevel { get; set; }

    [JsonPropertyName("stickers")]
    public Optional<Sticker[]> Stickers { get; set; }

    [JsonPropertyName("premium_progress_bar_enabled")]
    public Optional<bool> PremiumProgressBarEnabled { get; set; }

    [JsonPropertyName("safety_alerts_channel_id")]
    public Optional<ulong?> SafetyAlertsChannelId { get; set; }

    int? IGuildModel.MaxPresence => MaxPresences;
    int? IGuildModel.MaxMembers => MaxMembers;
    bool? IGuildModel.PremiumProgressBarEnabled => PremiumProgressBarEnabled;
    ulong? IGuildModel.SafetyAlertsChannelId => SafetyAlertsChannelId;
    int? IGuildModel.MaxVideoChannelUsers => MaxVideoChannelUsers;
    int? IGuildModel.MaxStageVideoChannelUsers => MaxStageVideoChannelUsers;
    int? IGuildModel.ApproximateMemberCount => ApproximateMemberCount;
    IWelcomeScreenModel? IGuildModel.WelcomeScreen => ~WelcomeScreen;
    int? IGuildModel.PremiumSubscriptionCount => PremiumSubscriptionCount;
    string? IGuildModel.Vanity => VanityUrlCode;
    bool IGuildModel.WidgetEnabled => WidgetEnabled;
    ulong? IGuildModel.WidgetChannelId => WidgetChannelId;

}
