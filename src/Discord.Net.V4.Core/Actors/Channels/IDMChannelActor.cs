namespace Discord.Models;

public interface IDMChannelActor :
    IActor<Snowflake, IDMChannel>,
    IChannelActor,
    IMessageChannelTrait
{
    
}