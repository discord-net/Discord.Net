using Discord.Models;

namespace Discord;

public interface IChannelInviteActor :
    IInviteActor,
    IChannelRelationship<IInvitableChannelTrait, IInvitableChannel>,
    IEntityProvider<IChannelInvite, IInviteModel>,
    IActor<string, IChannelInvite>;
