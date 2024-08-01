namespace Discord.Models;

[ModelEquality]
public partial interface IGuildModel : IPartialGuildModel, IEntityModel<ulong>
{
    string? DiscoverySplashId { get; }
    ulong OwnerId { get; }
    ulong? AFKChannelId { get; }
    int AFKTimeout { get; }
    bool WidgetEnabled { get; }
    ulong? WidgetChannelId { get; }
    int DefaultMessageNotifications { get; }
    int ExplicitContentFilter { get; }
    int MFALevel { get; }
    ulong? ApplicationId { get; }
    ulong? SystemChannelId { get; }
    int SystemChannelFlags { get; }
    ulong? RulesChannelId { get; }
    int? MaxPresence { get; }
    int? MaxMembers { get; }
    int PremiumTier { get; }
    string PreferredLocale { get; }
    ulong? PublicUpdatesChannelId { get; }
    int? MaxVideoChannelUsers { get; }
    int? MaxStageVideoChannelUsers { get; }
    IWelcomeScreenModel? WelcomeScreen { get; }
    bool PremiumProgressBarEnabled { get; }
    ulong? SafetyAlertsChannelId { get; }
    IEnumerable<ulong> RoleIds { get; }
}

public interface IWelcomeScreenModel
{
    string? Description { get; }
    IEnumerable<IWelcomeScreenChannelModel> WelcomeChannels { get; }
}

public interface IWelcomeScreenChannelModel
{
    ulong ChannelId { get; }
    string? Description { get; }
    ulong? EmojiId { get; }
    string? EmojiName { get; }
}

