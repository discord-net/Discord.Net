namespace Discord;

public enum ApplicationMonetizationState
{
    /// <summary>
    ///     Application has no monetization set up.
    /// </summary>
    None = 1,

    /// <summary>
    ///     Application has monetization set up.
    /// </summary>
    Enabled = 2,

    /// <summary>
    ///     Application has been blocked from monetizing.
    /// </summary>
    Blocked = 3,
}
