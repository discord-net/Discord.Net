using System.Collections.Generic;

namespace Discord;

/// <summary>
///     Represents the guild onboarding prompt.
/// </summary>
public interface IGuildOnboardingPrompt : ISnowflakeEntity
{
    /// <summary>
    ///     Gets options available within the prompt.
    /// </summary>
    IReadOnlyCollection<IGuildOnboardingPromptOption> Options { get; }

    /// <summary>
    ///     Gets the title of the prompt.
    /// </summary>
    string Title { get; }

    /// <summary>
    ///     Indicates whether users are limited to selecting one option for the prompt.
    /// </summary>
    bool IsSingleSelect { get; }

    /// <summary>
    ///     Indicates whether the prompt is required before a user completes the onboarding flow.
    /// </summary>
    bool IsRequired { get; }

    /// <summary>
    ///     Indicates whether the prompt is present in the onboarding flow.
    ///     If <see langword="false"/>, the prompt will only appear in the Channels and Roles tab.
    /// </summary>
    bool IsInOnboarding { get; }

    /// <summary>
    ///     Gets the type of the prompt.
    /// </summary>
    GuildOnboardingPromptType Type { get; }
}
