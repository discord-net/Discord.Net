namespace Discord;

/// <summary>
///     Defines the criteria used to satisfy Onboarding constraints that are required for enabling.
/// </summary>
public enum GuildOnboardingMode
{
    /// <summary>
    ///     Counts only Default Channels towards constraints.
    /// </summary>
    Default = 0,

    /// <summary>
    ///     Counts Default Channels and Questions towards constraints.
    /// </summary>
    Advanced = 1,
}
