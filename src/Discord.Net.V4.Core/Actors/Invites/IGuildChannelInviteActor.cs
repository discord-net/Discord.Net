namespace Discord.Models;

public interface IGuildChannelInviteActor :
    IActor<InviteId, IGuildChannelInvite>,
    IChannelInviteActor,
    IGuildInviteActor
{
    new IInvitableGuildChannelTrait Channel { get; }

    IInvitableChannelTrait IChannelInviteActor.Channel => Channel;
}