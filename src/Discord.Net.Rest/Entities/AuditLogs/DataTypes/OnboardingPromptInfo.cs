using Discord.API.AuditLogs;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Discord.Rest;

public class OnboardingPromptInfo
{
    internal OnboardingPromptInfo(OnboardingPromptAuditLogModel model, BaseDiscordClient discord)
    {
        Title = model.Title;
        IsSingleSelect = model.IsSingleSelect;
        IsRequired = model.IsRequired;
        IsInOnboarding = model.IsInOnboarding;
        Type = model.Type;
        Options = model.Options?.Select(x => new RestGuildOnboardingPromptOption(discord, x.Id, x)).ToImmutableArray();
    }

    /// <inheritdoc cref="IGuildOnboardingPrompt.Title"/>
    /// <remarks>
    ///     <see langword="null"/> if this property is not mentioned in this entry.
    /// </remarks>
    string Title { get; }

    /// <inheritdoc cref="IGuildOnboardingPrompt.IsSingleSelect"/>
    /// <remarks>
    ///     <see langword="null"/> if this property is not mentioned in this entry.
    /// </remarks>
    bool? IsSingleSelect { get; }

    /// <inheritdoc cref="IGuildOnboardingPrompt.IsRequired"/>
    /// <remarks>
    ///     <see langword="null"/> if this property is not mentioned in this entry.
    /// </remarks>
    bool? IsRequired { get; }

    /// <inheritdoc cref="IGuildOnboardingPrompt.IsInOnboarding"/>
    /// <remarks>
    ///     <see langword="null"/> if this property is not mentioned in this entry.
    /// </remarks>
    bool? IsInOnboarding { get; }

    /// <inheritdoc cref="IGuildOnboardingPrompt.Type"/>
    /// <remarks>
    ///     <see langword="null"/> if this property is not mentioned in this entry.
    /// </remarks>
    GuildOnboardingPromptType? Type { get; }

    /// <inheritdoc cref="IGuildOnboardingPrompt.Options"/>
    /// <remarks>
    ///     <see langword="null"/> if this property is not mentioned in this entry.
    /// </remarks>
    IReadOnlyCollection<IGuildOnboardingPromptOption> Options { get; }
}
