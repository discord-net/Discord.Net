using Discord.Models;

namespace Discord;

public interface IMessagesLink :
    IIndexableLink<Snowflake, IMessage>,
    IPagedLink<IPageMessagesParams, IMessage>
{
    
}