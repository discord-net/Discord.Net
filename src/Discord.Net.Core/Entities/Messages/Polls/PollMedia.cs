namespace Discord;

/// <summary>
///     Represents a poll media object.
/// </summary>
public readonly struct PollMedia
{
    /// <summary>
    ///     Gets the text of the field.
    /// </summary>
    public readonly string Text;

    /// <summary>
    ///     Gets the emoji of the field. <see langword="null"/> if no emoji is set.
    /// </summary>
    public readonly IEmote Emoji;

    internal PollMedia(string text, IEmote emoji)
    {
        Text = text;
        Emoji = emoji;
    }
}
