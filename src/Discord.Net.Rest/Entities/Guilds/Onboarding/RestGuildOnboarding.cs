using System;

using Model = Discord.API.GuildOnboarding;

namespace Discord.Rest;

/// <summary>
/// 
/// </summary>
public class RestGuildOnboarding : IGuildOnboarding
{
    /// <inheritdoc />
    public ulong GuildId { get; private set; }

    /// <inheritdoc cref="IGuildOnboarding.Guild" />
    public RestGuild Guild { get; private set; }

    /// <inheritdoc />
    public ulong[] DefaultChannelIds { get; private set; }

    /// <inheritdoc cref="IGuildOnboarding.DefaultChannels" />
    public RestGuildChannel[] DefaultChannels { get; private set; }

    /// <inheritdoc />
    public bool IsEnabled { get; private set; }

    /// <inheritdoc cref="IGuildOnboarding.Prompts"/>
    public RestGuildOnboardingPrompt[] Prompts { get; private set; }

    #region IGuildOnboarding

    /// <inheritdoc />
    IGuildOnboardingPrompt[] IGuildOnboarding.Prompts => Prompts;

    /// <inheritdoc />
    IGuild IGuildOnboarding.Guild => Guild;

    /// <inheritdoc />
    IGuildChannel[] IGuildOnboarding.DefaultChannels => DefaultChannels;

    #endregion
}
