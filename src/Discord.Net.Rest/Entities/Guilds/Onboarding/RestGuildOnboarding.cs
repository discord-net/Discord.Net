using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Model = Discord.API.GuildOnboarding;

namespace Discord.Rest;

/// <inheritdoc />
public class RestGuildOnboarding : IGuildOnboarding
{
    /// <inheritdoc />
    public ulong GuildId { get; private set; }

    /// <inheritdoc cref="IGuildOnboarding.Guild" />
    public RestGuild Guild { get; private set; }

    /// <inheritdoc />
    public IReadOnlyCollection<ulong> DefaultChannelIds { get; private set; }

    /// <inheritdoc />
    public bool IsEnabled { get; private set; }

    /// <inheritdoc cref="IGuildOnboarding.Prompts"/>
    public IReadOnlyCollection<RestGuildOnboardingPrompt> Prompts { get; private set; }

    internal RestGuildOnboarding(BaseDiscordClient discord, Model model, RestGuild guild = null)
    {
        GuildId = model.GuildId;
        DefaultChannelIds = model.DefaultChannelIds.ToImmutableArray();
        IsEnabled = model.Enabled;

        Guild = guild;
        Prompts = model.Prompts.Select(prompt => new RestGuildOnboardingPrompt(discord, prompt.Id, prompt)).ToImmutableArray();
    }

    #region IGuildOnboarding

    /// <inheritdoc />
    IReadOnlyCollection<IGuildOnboardingPrompt> IGuildOnboarding.Prompts => Prompts;

    /// <inheritdoc />
    IGuild IGuildOnboarding.Guild => Guild;

    #endregion
}
