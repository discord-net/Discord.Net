using System;
using System.Diagnostics;

namespace Discord;

/// <summary>
///     Represents a soundboard sound.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class SoundboardSound : ISnowflakeEntity
{
    /// <inheritdoc />
    public ulong Id => SoundId;

    /// <summary>
    ///     Gets the Id of the sound.
    /// </summary>
    public ulong SoundId { get; internal set; }

    /// <inheritdoc />
    /// <remarks>
    ///     May be inaccurate for default sounds.
    /// </remarks>
    public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

    /// <summary>
    ///     Gets the name of the sound.
    /// </summary>
    public string Name { get; internal set; }

    /// <summary>
    ///     Gets the Id of the author of the sound.
    /// </summary>
    public ulong AuthorId { get; internal set; }

    /// <summary>
    ///     Gets the author of the sound.
    /// </summary>
    public IUser Author { get; internal set; }

    /// <summary>
    ///     Gets the icon of the sound.
    /// </summary>
    /// <remarks>
    ///     Custom emojis will only have Id property filled due to limited data returned by discord.
    /// </remarks>
    public IEmote Emoji { get; internal set; }

    /// <summary>
    ///     Gets the volume of the sound.
    /// </summary>
    public double Volume { get; internal set; }

    /// <summary>
    ///     Gets whether the sound is available or not.
    /// </summary>
    public bool? IsAvailable { get; internal set; }

    /// <summary>
    ///     Gets the Id of the guild this sound belongs to. <see langword="null"/> if not available.
    /// </summary>
    public ulong? GuildId { get; internal set; }

    internal SoundboardSound(ulong soundId, string name, ulong authorId, double volume, ulong? guildId = null,
        string emojiName = null, ulong? emojiId = null, IUser author = null, bool? isAvailable = null)
    {
        GuildId = guildId;
        SoundId = soundId; 
        Name = name;
        AuthorId = authorId;
        Author = author;
        IsAvailable = isAvailable;
        Volume = volume;

        if (emojiId is not null)
            Emoji = new Emote(emojiId.Value, emojiName, false);
        else if (!string.IsNullOrWhiteSpace(emojiName))
            Emoji = new Emoji(emojiName);
        else
            Emoji = null;
    }

    private string DebuggerDisplay => $"{Name} ({SoundId})";

    internal SoundboardSound Clone() => (SoundboardSound)MemberwiseClone();

    /// <summary>
    ///     Gets the url for the sound.
    /// </summary>
    public string GetUrl()
        => CDN.GetSoundboardSoundUrl(SoundId);
}
