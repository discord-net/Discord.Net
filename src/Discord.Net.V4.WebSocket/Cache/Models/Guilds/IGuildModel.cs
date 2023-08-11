using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.Cache
{
    public interface IGuildModel : IEntityModel<ulong>
    {
        string Name { get; }
        string? Icon { get; }
        string? Splash { get; }
        string? DiscoverySplash { get; }
        ulong OwnerId { get; }
        ulong? AFKChannel { get; }
        int AFKTimeout { get; }
        bool WidgetEnabled { get; }
        ulong WidgetChannel { get; }
        VerificationLevel VerificationLevel { get; }
        DefaultMessageNotifications DefaultMessageNotification { get; }
        ExplicitContentFilterLevel ExplicitContentFilter { get; }
        GuildFeature Features { get; }
        string[] ExperimentalFeatures { get; }
        MfaLevel MFALevel { get; }
        ulong? ApplicationId { get; }
        ulong? SystemChannel { get; }
        SystemChannelMessageDeny SystemChannelFlags { get; }
        ulong? RulesChannel { get; }
        int? MaxPresense { get; }
        int MaxMembers { get; }
        string? Vanity { get; }
        string? Description { get; }
        string? Banner { get; }
        PremiumTier PremiumTier { get; }
        int PremiumSubscriptionCount { get; }
        string PreferredLocale { get; }
        ulong? PublicUpdatesChannel { get; }
        int MaxVideoChannelUsers { get; }
        int MaxStageVideoChannelUsers { get; }
        int ApproximateMemberCount { get; }
        IWelcomeScreenModel? WelcomeScreen { get; }
        NsfwLevel NsfwLevel { get; }
        bool? PremiumProgressBarEnabled { get; }
        ulong? SafetyAlertsChannel { get; }
    }

    public interface IWelcomeScreenModel
    {
        string? Description { get; }
        IWelcomeScreenChannelModel[] WelcomeChannels { get; }
    }

    public interface IWelcomeScreenChannelModel
    {
        ulong? ChannelId { get; }
        string? Description { get; }
        ulong? EmojiId { get; }
        string? EmojiName { get; }
    }
}
