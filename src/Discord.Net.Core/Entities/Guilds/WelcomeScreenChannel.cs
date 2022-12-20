using System;

namespace Discord;

public class WelcomeScreenChannel : ISnowflakeEntity
{
    /// <summary>
    /// 	Gets the channel's id.
    /// </summary>
    public ulong Id { get; }

    /// <summary>
    /// 	Gets the description shown for the channel.
    /// </summary>
    public string Description { get; }

    /// <summary>
    ///     Gets the emoji for this channel. <see cref="Emoji"/> if it is unicode emoji, <see cref="Emote"/> if it is a custom one and <see langword="null"/> if none is set.
    /// </summary>
    /// <remarks>
    ///     If the emoji is <see cref="Emote"/> only the <see cref="Emote.Id"/> will be populated.
    ///     Use <see cref="IGuild.GetEmoteAsync"/> to get the emoji.
    /// </remarks>
    public IEmote Emoji { get; }

    /// <inheritdoc/>
    public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

    internal WelcomeScreenChannel(ulong id, string description, string emojiName = null, ulong? emoteId = null)
    {
        Id = id;
        Description = description;

        if (emoteId.HasValue && emoteId.Value != 0)
            Emoji = new Emote(emoteId.Value, emojiName, false);
        else if (emojiName != null)
            Emoji = new Emoji(emojiName);
        else
            Emoji = null;
    }
}
