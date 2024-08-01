using Discord.Models;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public class Guild :
    PartialGuild,
    IGuildModel,
    IModelSource,
    IModelSourceOfMultiple<IRoleModel>,
    IModelSourceOfMultiple<IGuildEmoteModel>,
    IModelSourceOfMultiple<IGuildStickerModel>
{
    [JsonPropertyName("icon_hash")]
    public Optional<string?> IconHash { get; set; }

    [JsonPropertyName("discovery_splash")]
    public string? DiscoverySplashId { get; set; }

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

    [JsonPropertyName("default_message_notifications")]
    public int DefaultMessageNotifications { get; set; }

    [JsonPropertyName("explicit_content_filter")]
    public int ExplicitContentFilter { get; set; }

    [JsonPropertyName("roles")]
    public required Role[] Roles { get; set; }

    [JsonPropertyName("emojis")]
    public required GuildEmote[] Emojis { get; set; }

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

    [JsonPropertyName("premium_tier")]
    public int PremiumTier { get; set; }

    [JsonPropertyName("preferred_locale")]
    public required string PreferredLocale { get; set; }

    [JsonPropertyName("public_updates_channel_id")]
    public ulong? PublicUpdatesChannelId { get; set; }

    [JsonPropertyName("max_video_channel_users")]
    public Optional<int> MaxVideoChannelUsers { get; set; }

    [JsonPropertyName("max_stage_video_channel_users")]
    public Optional<int> MaxStageVideoChannelUsers { get; set; }

    [JsonPropertyName("stickers")]
    public Optional<Sticker[]> Stickers { get; set; }

    [JsonPropertyName("premium_progress_bar_enabled")]
    public bool PremiumProgressBarEnabled { get; set; }

    [JsonPropertyName("safety_alerts_channel_id")]
    public Optional<ulong?> SafetyAlertsChannelId { get; set; }

    public IEnumerable<ulong> RoleIds => Roles.Select(x => x.Id);
    int? IGuildModel.MaxPresence => ~MaxPresences;
    int? IGuildModel.MaxMembers => ~MaxMembers;
    bool IGuildModel.PremiumProgressBarEnabled => PremiumProgressBarEnabled;
    ulong? IGuildModel.SafetyAlertsChannelId => ~SafetyAlertsChannelId;
    int? IGuildModel.MaxVideoChannelUsers => ~MaxVideoChannelUsers;
    int? IGuildModel.MaxStageVideoChannelUsers => ~MaxStageVideoChannelUsers;
    IWelcomeScreenModel? IGuildModel.WelcomeScreen => ~WelcomeScreen;
    bool IGuildModel.WidgetEnabled => ~WidgetEnabled;
    ulong? IGuildModel.WidgetChannelId => ~WidgetChannelId;

    IEnumerable<IRoleModel> IModelSourceOfMultiple<IRoleModel>.GetModels() => Roles;

    IEnumerable<IGuildEmoteModel> IModelSourceOfMultiple<IGuildEmoteModel>.GetModels() => Emojis;

    IEnumerable<IGuildStickerModel> IModelSourceOfMultiple<IGuildStickerModel>.GetModels() => Stickers | [];

    public virtual IEnumerable<IEntityModel> GetDefinedModels()
    {
        IEnumerable<IEntityModel> entities = [..Roles, ..Emojis];

        if (Stickers.IsSpecified)
            entities = entities.Concat(Stickers.Value);

        return entities;
    }
}
