using System;

namespace Discord.Rest;

/// <summary>
/// 
/// </summary>
public class RestGuildOnboardingPrompt : RestEntity<ulong>, IGuildOnboardingPrompt
{
    /// <inheritdoc />
    public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);
    
    /// <inheritdoc cref="IGuildOnboardingPrompt.Options"/>
    public RestGuildOnboardingPromptOption[] Options { get; private set; }

    /// <inheritdoc />
    public string Title { get; private set; }

    /// <inheritdoc />
    public bool IsSingleSelect { get; private set; }

    /// <inheritdoc />
    public bool IsRequired { get; private set; }

    /// <inheritdoc />
    public bool IsInOnboarding { get; private set; }

    /// <inheritdoc />
    public GuildOnboardingPromptType Type { get; private set; }

    internal RestGuildOnboardingPrompt(BaseDiscordClient discord, ulong id) : base(discord, id)
    {
    }

    #region IGuildOnboardingPrompt

    /// <inheritdoc />
    IGuildOnboardingPromptOption[] IGuildOnboardingPrompt.Options => Options;

    #endregion
}
