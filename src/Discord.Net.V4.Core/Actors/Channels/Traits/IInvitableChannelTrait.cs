namespace Discord.Models;

public interface IInvitableChannelTrait :
    IChannelActor,
    IInvitableTrait
{
    IChannelInvitesLink Invites { get; }
}