namespace Discord;

public interface IChannelInviteActor :
    IActor<InviteId, IChannelInvite>,
    IInviteActor
{
    IInvitableChannelTrait Channel { get; }
}