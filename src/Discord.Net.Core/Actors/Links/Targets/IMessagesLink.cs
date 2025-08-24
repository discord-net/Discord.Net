using Discord.Models;

namespace Discord;

public interface IMessagesLink :
    IIndexableLink<Snowflake, IMessageActor>,
    IPagedLink<IPageMessagesParams, IMessage>
{
    IChannelPinsLink Pins { get; }
}

public interface IMessagesLink<out TMessageActor> :
    IMessagesLink,
    IIndexableLink<Snowflake, TMessageActor>
    where TMessageActor : IMessageActor
{
    new TMessageActor this[Snowflake id] { get; }

    IMessageActor IIndexableLink<Snowflake, IMessageActor>.this[Snowflake id] => this[id];
    TMessageActor IIndexableLink<Snowflake, TMessageActor>.this[Snowflake id] => this[id];
}