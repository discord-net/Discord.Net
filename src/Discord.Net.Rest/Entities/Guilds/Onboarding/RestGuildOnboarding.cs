using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.GuildOnboarding;

namespace Discord.Rest;

/// <inheritdoc />
public class RestGuildOnboarding : IGuildOnboarding
{
    internal BaseDiscordClient Discord;

    /// <inheritdoc />
    public ulong GuildId { get; private set; }

    /// <inheritdoc cref="IGuildOnboarding.Guild" />
    public RestGuild Guild { get; private set; }

    /// <inheritdoc />
    public IReadOnlyCollection<ulong> DefaultChannelIds { get; private set; }

    /// <inheritdoc />
    public bool IsEnabled { get; private set; }

    /// <inheritdoc />
    public GuildOnboardingMode Mode { get; private set; }

    /// <inheritdoc />
    public bool IsBelowRequirements { get; private set; }

    /// <inheritdoc cref="IGuildOnboarding.Prompts"/>
    public IReadOnlyCollection<RestGuildOnboardingPrompt> Prompts { get; private set; }

    internal RestGuildOnboarding(BaseDiscordClient discord, Model model, RestGuild guild = null)
    {
        Discord = discord;
        Guild = guild;
        Update(model);
    }

    internal void Update(Model model)
    {
        GuildId = model.GuildId;
        DefaultChannelIds = model.DefaultChannelIds.ToImmutableArray();
        IsEnabled = model.Enabled;
        Mode = model.Mode;
        IsBelowRequirements = model.IsBelowRequirements;
        Prompts = model.Prompts.Select(prompt => new RestGuildOnboardingPrompt(Discord, prompt.Id, prompt)).ToImmutableArray();
    }

    ///<inheritdoc/>
    public async Task ModifyAsync(Action<GuildOnboardingProperties> props, RequestOptions options = null)
    {
        var model = await GuildHelper.ModifyGuildOnboardingAsync(Guild, props, Discord, options);

        Update(model);
    }

    #region IGuildOnboarding

    /// <inheritdoc />
    IReadOnlyCollection<IGuildOnboardingPrompt> IGuildOnboarding.Prompts => Prompts;

    /// <inheritdoc />
    IGuild IGuildOnboarding.Guild => Guild;

    #endregion
}
