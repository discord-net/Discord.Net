using Discord.Models;

namespace Discord;

public interface IReaction :
    IEntity<DiscordEmojiId, IReactionModel>
{
    int NormalCount { get; }
    int SuperReactionsCount { get; }
    bool HasReacted { get; }
    bool HasSuperReacted { get; }
    IReadOnlyCollection<Color> SuperReactionColors { get; }
}