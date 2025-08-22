namespace Discord;

public interface IInvitableChannelTrait :
    IChannelActor,
    IInvitableTrait
{
    IChannelInvitesLink Invites { get; }
}