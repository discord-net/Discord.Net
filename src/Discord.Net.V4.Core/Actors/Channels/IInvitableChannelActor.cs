using Discord.Models;
using Discord.Rest;

namespace Discord;

public interface IInvitableChannelActor :
    IChannelActor,
    IActor<ulong, IInvitableChannel>,
    IInvitable<CreateChannelInviteProperties>;
