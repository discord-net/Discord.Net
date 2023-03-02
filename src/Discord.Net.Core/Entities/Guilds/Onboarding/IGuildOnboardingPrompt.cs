namespace Discord;

/// <summary>
///     
/// </summary>
public interface IGuildOnboardingPrompt : ISnowflakeEntity
{
    /// <summary>
    /// 
    /// </summary>
    IGuildOnboardingPromptOption[] Options { get; }

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
