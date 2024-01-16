using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Model = Discord.API.GuildOnboardingPromptOption;

namespace Discord.Rest;

/// <inheritdoc cref="IGuildOnboardingPromptOption"/>
public class RestGuildOnboardingPromptOption : RestEntity<ulong>, IGuildOnboardingPromptOption
{
    /// <inheritdoc />
    public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

    /// <inheritdoc />
    public IReadOnlyCollection<ulong> ChannelIds { get; private set; }

    /// <inheritdoc />
    public IReadOnlyCollection<ulong> RoleIds { get; private set; }
    
    /// <inheritdoc />
    public IEmote Emoji { get; private set; }

    /// <inheritdoc />
    public string Title { get; private set; }

    /// <inheritdoc />
    public string Description { get; private set; }

    internal RestGuildOnboardingPromptOption(BaseDiscordClient discord, ulong id, Model model) : base(discord, id)
    {
        ChannelIds = model.ChannelIds.ToImmutableArray();
        RoleIds = model.RoleIds.ToImmutableArray();
        Title = model.Title;
        Description = model.Description;
        
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
    }
}
