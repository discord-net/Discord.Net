namespace Discord;

/// <summary>
///     Properties used to create a poll question.
/// </summary>
public class PollMediaProperties
{
    /// <summary>
    ///     Gets or sets the text of the question for the poll.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    ///     Gets or sets the emoji of the question for the poll.
    /// </summary>
    public IEmote Emoji { get; set; }
}
