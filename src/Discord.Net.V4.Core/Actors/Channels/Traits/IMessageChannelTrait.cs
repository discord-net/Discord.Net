using Discord.Models;

namespace Discord;

public interface IMessageChannelTrait :
    IActor<Snowflake, IMessageChannel>,
    IChannelActor
{
    IMessagesLink Messages { get; }
}