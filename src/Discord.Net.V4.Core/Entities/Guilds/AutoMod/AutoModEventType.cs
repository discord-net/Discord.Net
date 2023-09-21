namespace Discord;

/// <summary>
///     An enum indecating in what event context a rule should be checked.
/// </summary>
public enum AutoModEventType
{
    /// <summary>
    ///     When a member sends or edits a message in the guild.
    /// </summary>
    MessageSend = 1
}
