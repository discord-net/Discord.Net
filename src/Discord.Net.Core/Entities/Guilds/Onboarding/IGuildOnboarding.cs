using System.Collections.Generic;

namespace Discord;

/// <summary>
///     
/// </summary>
public interface IGuildOnboarding
{
    /// <summary>
    /// 
    /// </summary>
    ulong GuildId { get; }

    /// <summary>
    /// 
    /// </summary>
    IGuild Guild { get; }

    /// <summary>
    /// 
    /// </summary>
    IReadOnlyCollection<IGuildOnboardingPrompt> Prompts { get; }

    /// <summary>
    /// 
    /// </summary>
    IReadOnlyCollection<ulong> DefaultChannelIds { get; }

    /// <summary>
    /// 
    /// </summary>
    bool IsEnabled { get; }
}
