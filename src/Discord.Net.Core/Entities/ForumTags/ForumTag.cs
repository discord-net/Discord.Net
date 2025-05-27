using System;
#nullable enable

namespace Discord;

/// <summary>
///     A struct representing a forum channel tag.
/// </summary>
public readonly record struct ForumTag : ISnowflakeEntity, IForumTag
{
    /// <inheritdoc/>
    public ulong Id { get; }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public IEmote? Emoji { get; }

    /// <inheritdoc/>
    public bool IsModerated { get; }

    /// <inheritdoc/>
    public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

    internal ForumTag(ulong id, string name, ulong? emojiId = null, string? emojiName = null, bool moderated = false)
    {
        if (emojiId.HasValue && emojiId.Value != 0)
            Emoji = new Emote(emojiId.Value, null, false);
        else if (emojiName != null)
            Emoji = new Emoji(emojiName);
        else
            Emoji = null;

        Id = id;
        Name = name;
        IsModerated = moderated;
    }

    public override int GetHashCode() => (Id, Name, Emoji, IsModerated).GetHashCode();

    /// <summary>
    /// Gets whether supplied tag is equals to the current one.
    /// </summary>
    public bool Equals(ForumTag tag)
        => Id == tag.Id &&
           Name == tag.Name &&
           (Emoji is Emoji emoji && tag.Emoji is Emoji otherEmoji && emoji.Equals(otherEmoji) ||
            Emoji is Emote emote && tag.Emoji is Emote otherEmote && emote.Equals(otherEmote)) &&
           IsModerated == tag.IsModerated;

    /// <inheritdoc/>
    ulong? IForumTag.Id => Id;
}
