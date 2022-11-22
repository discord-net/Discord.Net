namespace Discord;

/// <summary>
///     Represents public flags for a channel.
/// </summary>
public enum ChannelFlags
{
    /// <summary> 
    ///     Default value for flags, when none are given to a channel.
    /// </summary>
    None = 0,

    /// <summary>
    ///      Flag given to a thread channel pinned on top of parent forum channel.
    /// </summary>
    Pinned = 1 << 1,

    /// <summary>
    ///     Flag given to a forum channel that requires people to select tags when posting.
    /// </summary>
    RequireTag = 1 << 4
}
