namespace Discord;

public interface IInvite :
    IInviteActor
{
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
