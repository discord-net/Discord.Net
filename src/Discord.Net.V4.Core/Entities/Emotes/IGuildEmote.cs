using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

/// <summary>
///     An image-based emote that is attached to a guild.
/// </summary>
[Refreshable(nameof(Routes.GetGuildEmoji))]
[FetchableOfMany(nameof(Routes.ListGuildEmojis))]
public partial interface IGuildEmote :
    ISnowflakeEntity<IGuildEmoteModel>,
    IEmote,
    IGuildEmoteActor
{
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

    /// <summary>
    ///     Gets whether this emoji is available for use, may be <see langword="false" /> due to loss of Server Boosts
    /// </summary>
    bool IsAvailable { get; }

    RoleLink.Defined Roles { get; }

    IUserActor? Creator { get; }

    DiscordEmojiId IIdentifiable<DiscordEmojiId>.Id => new(Name, (this as ISnowflakeEntity<IGuildEmoteModel>).Id);
}
