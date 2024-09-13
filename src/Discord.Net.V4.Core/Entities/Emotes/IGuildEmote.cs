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
    ICustomEmote,
    IGuildEmoteActor
{
    /// <summary>
    ///     Gets whether this emoji is available for use, may be <see langword="false" /> due to loss of Server Boosts
    /// </summary>
    bool IsAvailable { get; }

    IRoleActor.Defined Roles { get; }
}
