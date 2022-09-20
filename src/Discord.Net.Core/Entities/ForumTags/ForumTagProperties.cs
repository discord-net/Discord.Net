namespace Discord;

#nullable enable

public class ForumTagProperties
{
    /// <summary>
    ///     Gets the name of the tag.
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Gets the emoji of the tag or <see langword="null"/> if none is set.
    /// </summary>
    public IEmote? Emoji { get; }

    /// <summary>
    /// Gets whether this tag can only be added to or removed from threads by a member
    /// with the <see cref="GuildPermissions.ManageThreads"/> permission
    /// </summary>
    public bool IsModerated { get; }

    internal ForumTagProperties(string name, IEmote? emoji = null, bool isMmoderated = false)
    {
        Name = name;
        Emoji = emoji;
        IsModerated = isMmoderated;
    }
}
