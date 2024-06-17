namespace Discord;

public interface IInvite :
    IInviteActor
{
    InviteType Type { get; }
    ILoadableGuildActor<IGuild>? Guild { get; }
    ILoadableChannelActor<IChannel>? Channel { get; }
    ILoadableUserActor<IUser>? Inviter { get; }
    InviteTargetType TargetType { get; }
    ILoadableUserActor<IUser>? TargetUser { get; }
    // TODO: application
    int? ApproximatePresenceCount { get; }
    int? ApproximateMemberCount { get; }
    DateTimeOffset? ExpiresAt { get; }
    ILoadableGuildScheduledEventActor<IGuildScheduledEvent>? GuildScheduledEvent { get; }
}
