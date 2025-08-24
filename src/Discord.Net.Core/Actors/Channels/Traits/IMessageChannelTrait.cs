using Discord.Models;

namespace Discord;

public interface IMessageChannelTrait :
    IActor<Snowflake, IMessageChannel>,
    IChannelActor
{
    IMessagesLink Messages { get; }
}

public interface IGuildMessageChannelTrait : 
    IMessageChannelTrait,
    IGuildChannelTrait
{
    new IMessagesLink<IGuildMessageActor> Messages { get; }

    IMessagesLink IMessageChannelTrait.Messages => Messages;
}