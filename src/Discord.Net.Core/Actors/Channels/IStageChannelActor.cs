using Discord.Models;

namespace Discord;

public interface IStageChannelActor :
    IActor<Snowflake, IStageChannel>,
    IGuildChannelTrait,
    IInvitableGuildChannelTrait,
    IGuildMessageChannelTrait
{
    
}