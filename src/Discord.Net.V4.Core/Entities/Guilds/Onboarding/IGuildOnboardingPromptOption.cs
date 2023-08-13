using System.Collections.Generic;

namespace Discord;

/// <summary>
///     Represents the guild onboarding prompt option.
/// </summary>
public interface IGuildOnboardingPromptOption : ISnowflakeEntity
{
    IEntityEnumerableSource<IGuildChannel, ulong> Channels { get; }
    IEntityEnumerableSource<IRole, ulong> Roles { get; }

    /// <summary>
    ///     Gets the emoji of the option. <see langword="null" /> if none is set.
    /// </summary>
    IEmote Emoji { get; }

    /// <summary>
    ///     Gets the title of the option.
    /// </summary>
    string Title { get; }

    /// <summary>
    ///     Gets the description of the option. <see langword="null" /> if none is set.
    /// </summary>
    string Description { get; }
}
