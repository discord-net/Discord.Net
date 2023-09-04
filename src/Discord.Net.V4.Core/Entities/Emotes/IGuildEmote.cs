namespace Discord;

/// <summary>
///     An image-based emote that is attached to a guild.
/// </summary>
public interface IGuildEmote : IEmote, ISnowflakeEntity, IModifyable<EmoteProperties>
{
    /// <summary>
    ///     Gets whether this emoji is managed by an integration.
    /// </summary>
    /// <returns>
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
    ///     Gets whether this emoji is available for use, may be <see langword="false"/> due to loss of Server Boosts
    /// </summary>
    bool IsAvailable { get; }

    IEntityEnumerableSource<ulong, IRole> Roles { get; }

    IEntitySource<ulong, IUser>? Creator { get; }

    IEntitySource<ulong, IGuild> Guild { get; }
}
