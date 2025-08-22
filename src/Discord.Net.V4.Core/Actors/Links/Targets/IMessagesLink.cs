using Discord.Models;
using Discord.Models.Models;

namespace Discord;

public interface IMessagesLink :
    IIndexableLink<Snowflake, IMessage>,
    IPagedLink<IPageMessagesParams, IMessage>
{
    
}