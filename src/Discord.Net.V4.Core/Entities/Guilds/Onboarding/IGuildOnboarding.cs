namespace Discord;

/// <summary>
///     Represents the guild onboarding flow.
/// </summary>
public interface IGuildOnboarding : ISnowflakeEntity
{
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

    /// <summary>
    ///     Gets the current mode of onboarding.
    /// </summary>
    GuildOnboardingMode Mode { get; }

    /// <summary>
    ///     Gets whether the server does not meet requirements to enable guild onboarding.
    /// </summary>
    bool IsBelowRequirements { get; }
}
