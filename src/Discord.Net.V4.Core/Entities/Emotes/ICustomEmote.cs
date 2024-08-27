using Discord.Models;

namespace Discord;

public interface ICustomEmote :
    IEmote,
    ISnowflakeEntity<ICustomEmoteModel>
{
    new ulong Id { get; }

    /// <summary>
    ///     Gets whether this emoji is managed by an integration.
    /// </summary>
    bool IsManaged { get; }

    /// <summary>
    ///     Gets whether this emoji must be wrapped in colons.
    /// </summary>
    bool RequireColons { get; }

    /// <summary>
    ///     Gets whether this emoji is animated.
    /// </summary>
    bool IsAnimated { get; }

    IUserActor? Creator { get; }

    DiscordEmojiId IIdentifiable<DiscordEmojiId>.Id => new(
        (this as ISnowflakeEntity<ICustomEmoteModel>).Id,
        Name,
        IsAnimated
    );

    ulong IIdentifiable<ulong>.Id => Id;
}