namespace Discord.Models;

public interface IChannelActor :
    IActor<Snowflake, IChannel>,
    ILoadable<IChannel>
{
    
}