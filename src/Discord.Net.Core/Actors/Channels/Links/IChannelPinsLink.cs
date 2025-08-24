using Discord.Models;

namespace Discord;

public interface IChannelPinsLink : 
    IIndexableLink<Snowflake, IPinnedMessageActor>,
    IPagedLink<IListChannelPinsParams, IPinnedMessage>
{
    Task AddAsync(Snowflake messageId, RequestOptions options = default);
}