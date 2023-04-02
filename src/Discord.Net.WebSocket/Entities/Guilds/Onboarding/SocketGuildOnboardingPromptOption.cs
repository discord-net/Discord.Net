using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Model = Discord.API.GuildOnboardingPromptOption;

namespace Discord.WebSocket;

/// <inheritdoc cref="IGuildOnboardingPromptOption"/>
public class SocketGuildOnboardingPromptOption : SocketEntity<ulong>, IGuildOnboardingPromptOption
{
    /// <inheritdoc />
    public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

    /// <inheritdoc />
    public IReadOnlyCollection<ulong> ChannelIds { get; private set; }

    /// <summary>
    ///     Gets channels a member is added to when the option is selected.
    /// </summary>
    public IReadOnlyCollection<SocketGuildChannel> Channels { get; private set; }

    /// <inheritdoc />
    public IReadOnlyCollection<ulong> RoleIds { get; private set; }

    /// <summary>
    ///     Gets roles assigned to a member when the option is selected.
    /// </summary>
    public IReadOnlyCollection<SocketRole> Roles { get; private set; }

    /// <inheritdoc />
    public IEmote Emoji { get; private set; }

    /// <inheritdoc />
    public string Title { get; private set; }

    /// <inheritdoc />
    public string Description { get; private set; }

    internal SocketGuildOnboardingPromptOption(DiscordSocketClient discord, ulong id, Model model, SocketGuild guild) : base(discord, id)
    {
        ChannelIds = model.ChannelIds.ToImmutableArray();
        RoleIds = model.RoleIds.ToImmutableArray();
        Title = model.Title;
        Description = model.Description.IsSpecified ? model.Description.Value : null;

        if (model.Emoji.Id.HasValue)
        {
            Emoji = new Emote(model.Emoji.Id.Value, model.Emoji.Name, model.Emoji.Animated ?? false);
        }
        else if (!string.IsNullOrWhiteSpace(model.Emoji.Name))
        {
            Emoji = new Emoji(model.Emoji.Name);
        }
        else
        {
            Emoji = null;
        }
        
        Roles = model.RoleIds.Select(guild.GetRole).ToImmutableArray();
        Channels = model.ChannelIds.Select(guild.GetChannel).ToImmutableArray();
    }
}
