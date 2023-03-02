using System;

namespace Discord.Rest;

public class RestGuildOnboardingPromptOption : RestEntity<ulong>, IGuildOnboardingPromptOption
{
    /// <inheritdoc />
    public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

    /// <inheritdoc />
    public ulong[] ChannelIds { get; private set; }

    /// <inheritdoc cref="IGuildOnboardingPromptOption.Channels" />
    public RestGuildChannel[] Channels { get; private set; }

    /// <inheritdoc />
    public ulong[] RoleIds { get; private set; }

    /// <inheritdoc cref="IGuildOnboardingPromptOption.Roles" />
    public RestRole[] Roles { get; private set; }

    /// <inheritdoc />
    public IEmote Emoji { get; private set; }

    internal RestGuildOnboardingPromptOption(BaseDiscordClient discord, ulong id) : base(discord, id)
    {
    }

    #region IGuildOnboardingPromptOption

    /// <inheritdoc />
    IGuildChannel[] IGuildOnboardingPromptOption.Channels => Channels;

    /// <inheritdoc />
    IRole[] IGuildOnboardingPromptOption.Roles => Roles;

    #endregion
}
