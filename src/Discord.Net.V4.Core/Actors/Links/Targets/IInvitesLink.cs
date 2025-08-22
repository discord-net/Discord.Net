using Discord.Models.Models;

namespace Discord.Models;

public interface IInvitesLink<out TInviteActor> :
    IIndexableLink<InviteId, TInviteActor>
    where TInviteActor : IInviteActor;

public interface IChannelInvitesLink : 
    IInvitesLink<IChannelInviteActor>,
    ICreatable<ICreateChannelInviteParams, IInvitableChannelTrait>;
    
public interface IGuildChannelInvitesLink : 
    IChannelInvitesLink,
    IInvitesLink<IGuildChannelInviteActor>,
    ICreatable<ICreateChannelInviteParams, IInvitableGuildChannelTrait>;