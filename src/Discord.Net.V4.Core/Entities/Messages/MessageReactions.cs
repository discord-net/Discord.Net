using Discord.Models;
using System.Collections.Immutable;

namespace Discord;

public sealed class MessageReactions : IConstructable<MessageReactions, IEnumerable<IReactionModel>>
{
    private MessageReactions(IDiscordClient client, IEnumerable<IReactionModel> reactionModels)
    {
        var emojiReactions = new Dictionary<string, ReactionMetadata>();
        var emoteReactions = new Dictionary<ulong, ReactionMetadata>();

        foreach (var model in reactionModels)
        {
            var entity = ReactionMetadata.Construct(client, model);

            if (model.EmojiId.HasValue)
                emoteReactions.Add(model.EmojiId.Value, entity);
            else if (model.EmojiName is not null)
                emojiReactions.Add(model.EmojiName, entity);
            else
                throw new DiscordException("No reaction emote/emoji information found");
        }

        EmojiReactions = emojiReactions.ToImmutableDictionary();
        EmoteReactions = emoteReactions.ToImmutableDictionary();
    }

    public ReactionMetadata? this[EntityOrId<string, Emoji> emoji]
        => EmojiReactions.GetValueOrDefault(emoji.Id);

    public ReactionMetadata? this[EntityOrId<ulong, IGuildEmote> emote]
        => EmoteReactions.GetValueOrDefault(emote.Id);

    public IReadOnlyDictionary<string, ReactionMetadata> EmojiReactions { get; }
    public IReadOnlyDictionary<ulong, ReactionMetadata> EmoteReactions { get; }

    public static MessageReactions Construct(IDiscordClient client, IEnumerable<IReactionModel> model)
        => new(client, model);
}
