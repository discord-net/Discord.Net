using Discord.Models;
using Discord.Rest;

namespace Discord;

public interface IInvite :
    IInviteActor,
    IRefreshable<IInvite, string, IInviteModel>
{
    static IApiOutRoute<IInviteModel> IRefreshable<IInvite, string, IInviteModel>.RefreshRoute(
        IInvite self,
        string id
    ) => Routes.GetInvite(id, true, true, eventId: self.GuildScheduledEvent?.Id);

    InviteType Type { get; }
    ILoadableGuildActor? Guild { get; }
    ILoadableChannelActor? Channel { get; }
    ILoadableUserActor? Inviter { get; }
    InviteTargetType TargetType { get; }

    ILoadableUserActor? TargetUser { get; }

    // TODO: application
    int? ApproximatePresenceCount { get; }
    int? ApproximateMemberCount { get; }
    DateTimeOffset? ExpiresAt { get; }
    ILoadableGuildScheduledEventActor? GuildScheduledEvent { get; }
}
