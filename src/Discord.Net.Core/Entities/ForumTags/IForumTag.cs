namespace Discord;

#nullable enable

/// <summary>
/// Represents a Discord forum tag
/// </summary>
public interface IForumTag
{
    /// <summary>
    ///     Gets the Id of the tag.
    ///</summary>
    /// <remarks>
    ///     This property may be <see langword="null"/> if the object is <see cref="ForumTagProperties"/>.
    /// </remarks>
    ulong? Id { get; }

    /// <summary>
    ///     Gets the name of the tag.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Gets the emoji of the tag or <see langword="null"/> if none is set.
    /// </summary>
    /// <remarks>
    ///     If the emoji is <see cref="Emote"/> only the <see cref="Emote.Id"/> will be populated.
    ///     Use <see cref="IGuild.GetEmoteAsync"/> to get the emoji.
    /// </remarks>
    IEmote? Emoji { get; }

    /// <summary>
    /// Gets whether this tag can only be added to or removed from threads by a member
    /// with the <see cref="GuildPermissions.ManageThreads"/> permission
    /// </summary>
    bool IsModerated { get; }
}
