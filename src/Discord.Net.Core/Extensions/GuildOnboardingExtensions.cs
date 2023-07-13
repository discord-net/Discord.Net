using System.Linq;

namespace Discord;

public static class GuildOnboardingExtensions
{
    public static GuildOnboardingProperties ToProperties(this IGuildOnboarding onboarding)
        => new ()
        {
            ChannelIds = onboarding.DefaultChannelIds.ToArray(),
            IsEnabled = onboarding.IsEnabled,
            Mode = onboarding.Mode,
            Prompts = onboarding.Prompts.Select(x => x.ToProperties()).ToArray(),
        };

    public static GuildOnboardingPromptProperties ToProperties(this IGuildOnboardingPrompt prompt)
        => new()
        {
            Id = prompt.Id,
            Type = prompt.Type,
            IsInOnboarding = prompt.IsInOnboarding,
            IsRequired = prompt.IsRequired,
            IsSingleSelect = prompt.IsSingleSelect,
            Title = prompt.Title,
            Options = prompt.Options.Select(x => x.ToProperties()).ToArray()
        };

    public static GuildOnboardingPromptOptionProperties ToProperties(this IGuildOnboardingPromptOption option)
        => new()
        {
            Title = option.Title,
            ChannelIds = option.ChannelIds.ToArray(),
            Description = option.Description,
            Emoji = Optional.Create(option.Emoji),
            Id = option.Id,
            RoleIds = option.RoleIds.ToArray(),
        };

}
