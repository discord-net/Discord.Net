using Discord.Models;

namespace Discord;

public interface IReactionsLink :
    IIndexableLink<EmojiId, IReactionActor>,
    IDeletable
{
    Task AddAsync(EmojiId emoji, RequestOptions options = default);
}