using Discord.Models;

namespace Discord;

public interface IMessageActor :
    IActor<Snowflake, IMessage>
{
    IMessageChannelTrait Channel { get; }
}