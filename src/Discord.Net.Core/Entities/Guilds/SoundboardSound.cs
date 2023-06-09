using System;

namespace Discord;

/// <summary>
///     Represents a soundboard sound.
/// </summary>
public class SoundboardSound : ISnowflakeEntity
{
    /// <inheritdoc />
    public ulong Id { get; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

    /// <summary>
    ///     
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     
    /// </summary>
    public ulong AuthorId { get; }

    /// <summary>
    ///     
    /// </summary>
    public IUser Author { get; }

    /// <summary>
    ///     
    /// </summary>
    /// <remarks>
    ///     Custom emojis will only have Id property filled due to limited data returned by discord.
    /// </remarks>
    public IEmote Emoji { get; }

    /// <summary>
    ///     
    /// </summary>
    public string OverridePath { get; }

    /// <summary>
    ///     
    /// </summary>
    public double Volume { get; }

    /// <summary>
    ///     
    /// </summary>
    public bool? Available { get; }

    internal SoundboardSound(ulong id, string name, ulong authorId, double volume, string overridePath = null, string emojiName = null, ulong? emojiId = null, IUser author = null, bool? available = null)
    {
        Id = id;
        Name = name;
        AuthorId = authorId;
        Author = author;
        Available = available;
        OverridePath = overridePath;
        Volume = volume;

        if (emojiId is not null)
            Emoji = new Emote(emojiId.Value, emojiName, false);
        else if (!string.IsNullOrWhiteSpace(emojiName))
            Emoji = new Emoji(emojiName);
        else
            Emoji = null;
    }
}
