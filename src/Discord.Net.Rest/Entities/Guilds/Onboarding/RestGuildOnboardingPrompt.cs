using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Model = Discord.API.GuildOnboardingPrompt;

namespace Discord.Rest;

/// <inheritdoc cref="IGuildOnboardingPrompt"/>
public class RestGuildOnboardingPrompt : RestEntity<ulong>, IGuildOnboardingPrompt
{
    /// <inheritdoc />
    public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);
    
    /// <inheritdoc cref="IGuildOnboardingPrompt.Options"/>
    public IReadOnlyCollection<RestGuildOnboardingPromptOption> Options { get; private set; }

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

    internal RestGuildOnboardingPrompt(BaseDiscordClient discord, ulong id, Model model) : base(discord, id)
    {
        Title = model.Title;
        IsSingleSelect = model.IsSingleSelect;
        IsInOnboarding = model.IsInOnboarding;
        IsRequired = model.IsRequired;
        Type = model.Type;

        Options = model.Options.Select(option => new RestGuildOnboardingPromptOption(discord, option.Id, option)).ToImmutableArray();
    }

    #region IGuildOnboardingPrompt

    /// <inheritdoc />
    IReadOnlyCollection<IGuildOnboardingPromptOption> IGuildOnboardingPrompt.Options => Options;

    #endregion
}
