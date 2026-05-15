namespace Discord;

/// <summary>
///     Represents properties used to modify the current user's guild-specific information.
/// </summary>
public class SelfGuildUserProperties : GuildUserProperties
{
    /// <summary>
    ///     Gets or sets the banner image of the current user.
    /// </summary>
    public Optional<Image?> Banner { get; set; }

    /// <summary>
    ///     Gets or sets the avatar image of the current user.
    /// </summary>
    public Optional<Image?> Avatar { get; set; }

    /// <summary>
    ///     Gets or sets the user's biography text.
    /// </summary>
    public Optional<string> Bio { get; set; }
}
