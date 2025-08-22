using Discord.Models;
using Discord.Models.Models;

namespace Discord;

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