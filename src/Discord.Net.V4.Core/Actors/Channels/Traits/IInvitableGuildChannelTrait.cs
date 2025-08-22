namespace Discord.Models;

public interface IInvitableGuildChannelTrait :
    IInvitableChannelTrait,
    IGuildChannelTrait
{
    new IGuildChannelInvitesLink Invites { get; }

    IChannelInvitesLink IInvitableChannelTrait.Invites => Invites;
}