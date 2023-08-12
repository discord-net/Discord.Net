using System.Collections.Generic;

namespace Discord;

/// <summary>
///     Represents properties used to create or modify guild onboarding prompt option.
/// </summary>
public class GuildOnboardingPromptOptionProperties
{
    /// <summary>
    ///     Gets or sets the Id of the prompt option. If the value is <see langword="null" /> a new prompt will be created.
    ///     The existing one will be updated otherwise.
    /// </summary>
    public ulong? Id { get; set; }

    /// <summary>
    ///     Gets or set IDs for channels a member is added to when the option is selected.
    /// </summary>
    public ulong[] ChannelIds { get; set; }

    /// <summary>
    ///     Gets or sets IDs for roles assigned to a member when the option is selected.
    /// </summary>
    public ulong[] RoleIds { get; set; }

    /// <summary>
    ///     Gets or sets the emoji of the option.
    /// </summary>
    public Optional<IEmote> Emoji { get; set; }

    /// <summary>
    ///     Gets or sets the title of the option.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    ///     Gets or sets the description of the option.
    /// </summary>
    public string Description { get; set; }
}
