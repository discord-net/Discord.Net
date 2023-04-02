using System.Collections.Generic;

namespace Discord;

/// <summary>
///     Represents the guild onboarding flow.
/// </summary>
public interface IGuildOnboarding
{
    /// <summary>
    ///     Gets the ID of the guild this onboarding is part of.
    /// </summary>
    ulong GuildId { get; }

    /// <summary>
    ///     Gets the guild this onboarding is part of.
    /// </summary>
    IGuild Guild { get; }

    /// <summary>
    ///     Gets prompts shown during onboarding and in customize community.
    /// </summary>
    IReadOnlyCollection<IGuildOnboardingPrompt> Prompts { get; }

    /// <summary>
    ///     Gets IDs of channels that members get opted into automatically.
    /// </summary>
    IReadOnlyCollection<ulong> DefaultChannelIds { get; }

    /// <summary>
    ///     Gets whether onboarding is enabled in the guild.
    /// </summary>
    bool IsEnabled { get; }
}
