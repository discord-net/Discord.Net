namespace Discord;

/// <summary>
///     Specifies the type of mentions that will be notified from the message content.
/// </summary>
[Flags]
public enum AllowedMentionTypes : byte
{
    /// <summary>
    ///     No flag is set.
    /// </summary>
    /// <remarks>
    ///     This flag is not used to control mentions.
    ///     <note type="warning">
    ///         It will always be present and does not mean mentions will not be allowed.
    ///     </note>
    /// </remarks>
    None = 0,

    /// <summary>
    ///     Controls role mentions.
    /// </summary>
    Roles = 1 << 0,

    /// <summary>
    ///     Controls user mentions.
    /// </summary>
    Users = 1 << 1,

    /// <summary>
    ///     Controls <code>@everyone</code> and <code>@here</code> mentions.
    /// </summary>
    Everyone = 1 << 2
}
