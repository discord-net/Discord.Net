namespace Discord;

/// <summary>
///     An enum indicating in what event context a rule should be checked.
/// </summary>
public enum AutoModEventType
{
    /// <summary>
    ///     When a member sends or edits a message in the guild.
    /// </summary>
    MessageSend = 1,

    /// <summary>
    ///     When a member edits their profile.
    /// </summary>
    MemberUpdate = 2,
}
