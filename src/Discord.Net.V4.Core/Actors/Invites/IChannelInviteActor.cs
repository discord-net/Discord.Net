using Discord.Models;

namespace Discord;

public interface IChannelInviteActor :
    IInviteActor,
    IChannelRelationship,
    IEntityProvider<IChannelInvite, IInviteModel>,
    IActor<string, IChannelInvite>;
