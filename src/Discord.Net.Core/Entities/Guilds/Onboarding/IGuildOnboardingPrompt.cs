using System.Collections.Generic;

namespace Discord;

/// <summary>
///     
/// </summary>
public interface IGuildOnboardingPrompt : ISnowflakeEntity
{
    /// <summary>
    /// 
    /// </summary>
    IReadOnlyCollection<IGuildOnboardingPromptOption> Options { get; }

    /// <summary>
    /// 
    /// </summary>
    string Title { get; }

    /// <summary>
    /// 
    /// </summary>
    bool IsSingleSelect { get; }

    /// <summary>
    /// 
    /// </summary>
    bool IsRequired { get; }

    /// <summary>
    /// 
    /// </summary>
    bool IsInOnboarding { get; }

    /// <summary>
    /// 
    /// </summary>
    GuildOnboardingPromptType Type { get; }
}
