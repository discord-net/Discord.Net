using Discord.Models;
using Discord.Rest;

namespace Discord;

[Refreshable(nameof(Routes.GetInvite))]
public partial interface IInvite :
    IInviteActor,
    IEntity<string, IInviteModel>
{
    InviteType Type { get; }
    IUserActor? Inviter { get; }
    InviteTargetType? TargetType { get; }

    IUserActor? TargetUser { get; }

    // TODO: application
    int? ApproximatePresenceCount { get; }
    int? ApproximateMemberCount { get; }
    DateTimeOffset? ExpiresAt { get; }
}
