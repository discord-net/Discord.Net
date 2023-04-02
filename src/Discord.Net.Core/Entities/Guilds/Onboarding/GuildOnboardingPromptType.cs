namespace Discord;

/// <summary>
///     Represents the guild onboarding option type.
/// </summary>
public enum GuildOnboardingPromptType
{
    /// <summary>
    ///     The prompt accepts multiple choices.
    /// </summary>
    MultipleChoice = 0,

    /// <summary>
    ///     The prompt uses a dropdown menu.
    /// </summary>
    Dropdown = 1,
}
