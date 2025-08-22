namespace Discord.Models;

public interface IMessageActor :
    IActor<Snowflake, IMessage>
{
    IMessageChannelTrait Channel { get; }
}