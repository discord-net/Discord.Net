using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Model = Discord.API.GuildOnboarding;

namespace Discord.WebSocket;

/// <inheritdoc />
public class SocketGuildOnboarding : IGuildOnboarding
{
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

    internal SocketGuildOnboarding(DiscordSocketClient discord, Model model, SocketGuild guild)
    {
        GuildId = model.GuildId;
        Guild = guild;
        IsEnabled = model.Enabled;

        DefaultChannelIds = model.DefaultChannelIds;

        DefaultChannels = model.DefaultChannelIds.Select(guild.GetChannel).ToImmutableArray();
        Prompts = model.Prompts.Select(x => new SocketGuildOnboardingPrompt(discord, x.Id, x, guild)).ToImmutableArray();

    }

    #region IGuildOnboarding

    /// <inheritdoc />
    IGuild IGuildOnboarding.Guild => Guild;

    /// <inheritdoc />
    IReadOnlyCollection<IGuildOnboardingPrompt> IGuildOnboarding.Prompts => Prompts;

    #endregion
}
