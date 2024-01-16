using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Model = Discord.API.GuildOnboardingPrompt;

namespace Discord.WebSocket;


/// <inheritdoc cref="IGuildOnboardingPrompt"/>
public class SocketGuildOnboardingPrompt : SocketEntity<ulong>, IGuildOnboardingPrompt
{
    /// <inheritdoc />
    public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

    /// <inheritdoc cref="IGuildOnboardingPrompt.Options"/>
    public IReadOnlyCollection<SocketGuildOnboardingPromptOption> Options { get; private set; }

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

    internal SocketGuildOnboardingPrompt(DiscordSocketClient discord, ulong id, Model model, SocketGuild guild) : base(discord, id)
    {
        Title = model.Title;
        IsSingleSelect = model.IsSingleSelect;
        IsInOnboarding = model.IsInOnboarding;
        IsRequired = model.IsRequired;
        Type = model.Type;

        Options = model.Options.Select(option => new SocketGuildOnboardingPromptOption(discord, option.Id, option, guild)).ToImmutableArray();
    }

    #region IGuildOnboardingPrompt

    /// <inheritdoc />
    IReadOnlyCollection<IGuildOnboardingPromptOption> IGuildOnboardingPrompt.Options => Options;

    #endregion
}
