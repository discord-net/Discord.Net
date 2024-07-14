using Discord.Models;
using Discord.Rest;

namespace Discord;

[Refreshable(nameof(Routes.GetInvite))]
public partial interface IInvite :
    IInviteActor,
    IEntityOf<IInviteModel>
{
    // static IApiOutRoute<IInviteModel> IRefreshable<IInvite, string, IInviteModel>.RefreshRoute(
    //     IPathable path,
    //     string id
    // ) => Routes.GetInvite(id, true, true, eventId: path.Optionally<IGuildScheduledEvent>());

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
