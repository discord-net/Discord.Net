using System.Collections.Generic;

namespace Discord;

/// <summary>
///     Represents properties used to create or modify guild onboarding.
/// </summary>
public class GuildOnboardingProperties
{
    /// <summary>
    ///     Gets or sets prompts shown during onboarding and in customize community.
    /// </summary>
    public Optional<GuildOnboardingPromptProperties[]> Prompts { get; set; }

    /// <summary>
    ///     Gets or sets channel IDs that members get opted into automatically.
    /// </summary>
    public Optional<ulong[]> ChannelIds { get; set; }

    /// <summary>
    ///     Gets or sets whether onboarding is enabled in the guild.
    /// </summary>
    public Optional<bool> IsEnabled { get; set; }

    /// <summary>
    ///     Gets or sets current mode of onboarding.
    /// </summary>
    public Optional<GuildOnboardingMode> Mode { get; set;}
}
