using Discord.Models;
using Discord.Rest;

namespace Discord;

[Trait]
public interface IInvitableChannelTrait :
    IChannelActor,
    IActorTrait<ulong, IInvitableChannel>,
    IInvitable<CreateChannelInviteProperties>;
