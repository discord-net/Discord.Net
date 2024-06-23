using Discord.Models;

namespace Discord;

/// <summary>
///     Represents a general container for any type of emote in a message.
/// </summary>
public interface IEmote : IEntityProperties<Models.Json.IEmote>, IConstructable<IEmote, IEmoteModel>
{
    /// <summary>
    ///     Gets the display name or Unicode representation of this emote.
    /// </summary>
    /// <returns>
    ///     A string representing the display name or the Unicode representation (e.g. <c>🤔</c>) of this emote.
    /// </returns>
    string? Name { get; }

    static IEmote IConstructable<IEmote, IEmoteModel, IDiscordClient>.Construct(IDiscordClient client,
        IEmoteModel model)
        => Construct(client, model);

    new static IEmote Construct(IDiscordClient client, IEmoteModel model) =>
        model switch
        {
            IEmojiModel emojiModel => Emoji.Construct(client, emojiModel),
            IGuildEmoteModel guildEmoteModel => GuildEmote.Construct(client, guildEmoteModel),
            _ => throw new ArgumentOutOfRangeException(nameof(model))
        };
}
