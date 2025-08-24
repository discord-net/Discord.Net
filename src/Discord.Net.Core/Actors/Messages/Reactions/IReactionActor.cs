using Discord.Models;

namespace Discord;

public interface IReactionActor :
    IActor<EmojiId, IReaction>,
    IDeletable
{
    IReactionUsersLink Users { get; }
    
    Task DeleteAllAsync(RequestOptions options = default);
}
