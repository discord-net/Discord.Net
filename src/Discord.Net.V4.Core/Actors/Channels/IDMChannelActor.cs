using Discord.Models;

namespace Discord;

public interface IDMChannelActor :
    IActor<Snowflake, IDMChannel>,
    IChannelActor,
    IMessageChannelTrait
{
    
}