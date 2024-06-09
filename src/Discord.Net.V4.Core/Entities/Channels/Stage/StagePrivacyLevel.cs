namespace Discord;

/// <summary>
///     Represents the privacy level of a stage.
/// </summary>
public enum StagePrivacyLevel
{
    /// <summary>
    ///     The Stage instance is visible publicly, such as on Stage Discovery.
    /// </summary>
    [Obsolete("Deprecated in the Discord API", true)]
    Public = 1,

    /// <summary>
    ///     The Stage instance is visible to only guild members.
    /// </summary>
    GuildOnly = 2
}
