namespace Discord;

/// <summary>
///     Represents a Discord forum tag
/// </summary>
public interface IForumTag : ISnowflakeEntity
{
    /// <summary>
    ///     Gets the name of the tag.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Gets the emoji of the tag or <see langword="null" /> if none is set.
    /// </summary>
    ILoadableEntity<IEmote>? Emoji { get; }

    /// <summary>
    ///     Gets whether this tag can only be added to or removed from threads by a member
    ///     with the <see cref="GuildPermissions.ManageThreads" /> permission
    /// </summary>
    bool IsModerated { get; }
}
