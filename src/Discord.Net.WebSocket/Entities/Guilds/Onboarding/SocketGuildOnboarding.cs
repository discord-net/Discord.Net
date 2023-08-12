using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.GuildOnboarding;

namespace Discord.WebSocket;

/// <inheritdoc />
public class SocketGuildOnboarding : IGuildOnboarding
{
    internal DiscordSocketClient Discord;

    /// <inheritdoc />
    public ulong GuildId { get; private set; }

    /// <inheritdoc cref="IGuildOnboarding.Guild"/>
    public SocketGuild Guild { get; private set; }

    /// <inheritdoc cref="IGuildOnboarding.Prompts"/>
    public IReadOnlyCollection<SocketGuildOnboardingPrompt> Prompts { get; private set; }

    /// <inheritdoc />
    public IReadOnlyCollection<ulong> DefaultChannelIds { get; private set; }

    /// <summary>
    ///     Gets channels members get opted in automatically.
    /// </summary>
    public IReadOnlyCollection<SocketGuildChannel> DefaultChannels { get; private set; }

    /// <inheritdoc />
    public bool IsEnabled { get; private set; }

    /// <inheritdoc />
    public bool IsBelowRequirements { get; private set; }

    /// <inheritdoc />
    public GuildOnboardingMode Mode { get; private set; }

    internal SocketGuildOnboarding(DiscordSocketClient discord, Model model, SocketGuild guild)
    {
        Discord = discord;
        Guild = guild;
        Update(model);
    }

    internal void Update(Model model)
    {
        GuildId = model.GuildId;
        IsEnabled = model.Enabled;
        Mode = model.Mode;
        IsBelowRequirements = model.IsBelowRequirements;

        DefaultChannelIds = model.DefaultChannelIds;
        DefaultChannels = model.DefaultChannelIds.Select(Guild.GetChannel).ToImmutableArray();

        Prompts = model.Prompts.Select(x => new SocketGuildOnboardingPrompt(Discord, x.Id, x, Guild)).ToImmutableArray();
    }

    ///<inheritdoc/>
    public async Task ModifyAsync(Action<GuildOnboardingProperties> props, RequestOptions options = null)
    {
        var model = await GuildHelper.ModifyGuildOnboardingAsync(Guild, props, Discord, options);

        Update(model);
    }

    #region IGuildOnboarding

    /// <inheritdoc />
    IGuild IGuildOnboarding.Guild => Guild;

    /// <inheritdoc />
    IReadOnlyCollection<IGuildOnboardingPrompt> IGuildOnboarding.Prompts => Prompts;

    #endregion
}
