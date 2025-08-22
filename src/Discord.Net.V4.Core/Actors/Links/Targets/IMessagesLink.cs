using Discord.Models.Models;

namespace Discord.Models;

public interface IMessagesLink :
    IIndexableLink<Snowflake, IMessage>,
    IPagedLink<IPageMessagesParams, IMessage>
{
    
}