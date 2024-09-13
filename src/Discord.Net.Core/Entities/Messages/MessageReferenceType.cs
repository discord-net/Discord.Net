namespace Discord;

/// <summary>
///     Determines how associated data is populated.
/// </summary>
public enum MessageReferenceType
{
    /// <summary>
    ///     A standard reference used by replies.
    /// </summary>
    Default = 0,

    /// <summary>
    ///     Reference used to point to a message at a point in time.
    /// </summary>
    Forward = 1,
}
