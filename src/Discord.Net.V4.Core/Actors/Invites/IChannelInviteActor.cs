using Discord.Models;

namespace Discord;

public interface IChannelInviteActor :
    IInviteActor,
    IChannelActor.CanonicalRelationship,
    IEntityProvider<IChannelInvite, IInviteModel>,
    IActor<string, IChannelInvite>;
