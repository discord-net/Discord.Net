using Discord.API.AuditLogs;
using Discord.Rest;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Discord.WebSocket;

public class SocketOnboardingInfo
{
    internal SocketOnboardingInfo(OnboardingAuditLogModel model, DiscordSocketClient discord)
    {
        Prompts = model.Prompts?.Select(x => new RestGuildOnboardingPrompt(discord, x.Id, x)).ToImmutableArray();
        DefaultChannelIds = model.DefaultChannelIds;
        IsEnabled = model.Enabled;
    }

    /// <inheritdoc cref="IGuildOnboarding.Prompts"/>
    /// <remarks>
    ///     <see langword="null"/> if this property is not mentioned in this entry.
    /// </remarks>
    IReadOnlyCollection<IGuildOnboardingPrompt> Prompts { get; }

    /// <inheritdoc cref="IGuildOnboarding.DefaultChannelIds"/>
    /// <remarks>
    ///     <see langword="null"/> if this property is not mentioned in this entry.
    /// </remarks>
    IReadOnlyCollection<ulong> DefaultChannelIds { get; }

    /// <inheritdoc cref="IGuildOnboarding.IsEnabled"/>
    /// <remarks>
    ///     <see langword="null"/> if this property is not mentioned in this entry.
    /// </remarks>
    bool? IsEnabled { get; }
}
