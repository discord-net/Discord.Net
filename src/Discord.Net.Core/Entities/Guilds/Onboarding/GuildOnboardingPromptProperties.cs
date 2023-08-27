using System.Collections.Generic;

namespace Discord;

/// <summary>
///     Represents properties used to create or modify guild onboarding prompt.
/// </summary>
public class GuildOnboardingPromptProperties
{
    /// <summary>
    ///     Gets or sets the Id of the prompt. If the value is <see langword="null" /> a new prompt will be created.
    ///     The existing one will be updated otherwise.
    /// </summary>
    public ulong? Id { get; set; }

    /// <summary>
    ///     Gets or sets options available within the prompt.
    /// </summary>
    public GuildOnboardingPromptOptionProperties[] Options { get; set; }

    /// <summary>
    ///     Gets or sets the title of the prompt.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    ///     Gets or sets whether users are limited to selecting one option for the prompt.
    /// </summary>
    public bool IsSingleSelect { get; set; }

    /// <summary>
    ///     Gets or sets whether the prompt is required before a user completes the onboarding flow.
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    ///     Gets or sets whether the prompt is present in the onboarding flow. 
    /// </summary>
    public bool IsInOnboarding { get; set; }

    /// <summary>
    ///     Gets or set the type of the prompt.
    /// </summary>
    public GuildOnboardingPromptType Type { get; set; }
}
