using Discord.Models;

namespace Discord;

public interface IChannelInviteActor :
    IChannelRelationship<IInvitableChannelTrait, IInvitableChannel>,
    IEntityProvider<IChannelInvite, IInviteModel>,
    IActor<string, IChannelInvite>;
