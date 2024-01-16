namespace Discord;

public enum AutoModActionType
{
    /// <summary>
    ///     Blocks the content of a message according to the rule.
    /// </summary>
    BlockMessage = 1,

    /// <summary>
    ///     Logs user content to a specified channel.
    /// </summary>
    SendAlertMessage = 2,

    /// <summary>
    ///     Timeout user for a specified duration.
    /// </summary>
    Timeout = 3,

    /// <summary>
    ///     Prevents a member from using text, voice, or other interactions.
    /// </summary>
    BlockMemberInteraction = 4
}
