using System.Collections.Generic;

namespace Discord;

/// <summary>
///     Represents the guild onboarding prompt option.
/// </summary>
public interface IGuildOnboardingPromptOption : ISnowflakeEntity
{
    /// <summary>
    ///     Gets IDs of channels a member is added to when the option is selected.
    /// </summary>
    IReadOnlyCollection<ulong> ChannelIds { get; }

    /// <summary>
    ///     Gets IDs of roles assigned to a member when the option is selected.
    /// </summary>
    IReadOnlyCollection<ulong> RoleIds { get; }

    /// <summary>
    ///     Gets the emoji of the option. <see langword="null"/> if none is set.
    /// </summary>
    IEmote Emoji { get; }

    /// <summary>
    ///     Gets the title of the option.
    /// </summary>
    string Title { get; }

    /// <summary>
    ///     Gets the description of the option. <see langword="null"/> if none is set.
    /// </summary>
    string Description { get; }
}
