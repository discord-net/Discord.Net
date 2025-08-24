using Discord.Models;

namespace Discord;

public interface IVoiceChannelActor :
    IActor<Snowflake, IVoiceChannel>,
    IChannel,
    IGuildChannelTrait, 
    IGuildMessageChannelTrait,
    INestedChannelTrait,
    IInvitableGuildChannelTrait
{
}