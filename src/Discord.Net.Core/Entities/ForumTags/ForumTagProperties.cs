namespace Discord;

#nullable enable

public class ForumTagProperties : IForumTag
{
    /// <summary>
    ///     Gets the Id of the tag.
    /// </summary>
    public ulong? Id { get; }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public IEmote? Emoji { get; }

    /// <inheritdoc/>
    public bool IsModerated { get; }

    internal ForumTagProperties(ulong? id, string name, IEmote? emoji = null, bool isModerated = false)
    {
        Id = id;
        Name = name;
        Emoji = emoji;
        IsModerated = isModerated;
    }

    public override int GetHashCode() => (Id, Name, Emoji, IsModerated).GetHashCode();

    public override bool Equals(object? obj)
        => obj is ForumTagProperties tag && Equals(tag);

    /// <summary>
    /// Gets whether supplied tag is equals to the current one.
    /// </summary>
    public bool Equals(ForumTagProperties? tag)
        => tag is not null &&
           Id == tag.Id &&
           Name == tag.Name &&
           (Emoji is Emoji emoji && tag.Emoji is Emoji otherEmoji && emoji.Equals(otherEmoji) ||
            Emoji is Emote emote && tag.Emoji is Emote otherEmote && emote.Equals(otherEmote)) &&
           IsModerated == tag.IsModerated;

    public static bool operator ==(ForumTagProperties? left, ForumTagProperties? right)
        => left?.Equals(right) ?? right is null;

    public static bool operator !=(ForumTagProperties? left, ForumTagProperties? right) => !(left == right);
}
