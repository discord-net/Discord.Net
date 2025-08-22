namespace Discord.Models;

public interface IMessageChannelTrait :
    IActor<Snowflake, IMessageChannel>,
    IChannelActor
{
    IMessagesLink Messages { get; }
}