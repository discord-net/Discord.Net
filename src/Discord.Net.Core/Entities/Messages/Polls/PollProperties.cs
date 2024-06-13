using System.Collections.Generic;

namespace Discord;

/// <summary>
///     Properties used to create a poll.
/// </summary>
public class PollProperties
{
    /// <summary>
    ///     Gets or sets the question for the poll.
    /// </summary>
    public PollMediaProperties Question { get; set; }

    /// <summary>
    ///     Gets or sets the answers for the poll.
    /// </summary>
    public List<PollMediaProperties> Answers { get; set; }

    /// <summary>
    ///     Gets or sets the duration for the poll in hours. Max duration is 168 hours (7 days).
    /// </summary>
    public uint Duration { get; set; }

    /// <summary>
    ///     Gets or sets whether the poll allows multiple answers.
    /// </summary>
    public bool AllowMultiselect { get; set; }

    /// <summary>
    ///     Gets or sets the layout type for the poll.
    /// </summary>
    public PollLayout LayoutType { get; set; }
}
