using Discord.Models;

namespace Discord;

public interface IChannelActor :
    IActor<Snowflake, IChannel>,
    ILoadable<IChannel>
{
    
}