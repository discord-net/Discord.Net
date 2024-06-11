using Discord.EntityRelationships;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

using Modifiable = IModifiable<ulong, IGuildEmote, EmoteProperties, ModifyEmojiParams>;
using Deletable = IDeletable<ulong, IGuildEmote>;

/// <summary>
///     An image-based emote that is attached to a guild.
/// </summary>
public interface IGuildEmote :
    IEmote,
    ISnowflakeEntity,
    Modifiable,
    Deletable,
    IGuildRelationship
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

    ILoadableEntityEnumerable<ulong, IRole> Roles { get; }

    ILoadableEntity<ulong, IUser>? Creator { get; }

    static ApiBodyRoute<ModifyEmojiParams> Modifiable.ModifyRoute(IPathable path, ulong id, ModifyEmojiParams args)
        => Routes.ModifyGuildEmoji(path.Require<IGuild>(), id, args);

    static BasicApiRoute Deletable.DeleteRoute(IPathable path, ulong id)
        => Routes.DeleteGuildEmoji(path.Require<IGuild>(), id);
}
