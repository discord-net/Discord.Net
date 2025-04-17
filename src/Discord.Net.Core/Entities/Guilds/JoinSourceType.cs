namespace Discord;

/// <summary>
///     Represents the source of a user joining a guild.
/// </summary>
public enum JoinSourceType
{
    /// <summary>
    ///     Unknown source.
    /// </summary>
    Unknown = 0,

    /// <summary>
    ///     The user was invited by a bot.
    /// </summary>
    BotInvite = 1,

    /// <summary>
    ///     The user was invited by an integration.
    /// </summary>
    Integration = 2,

    /// <summary>
    ///     The user joined via server discovery.
    /// </summary>
    ServerDiscovery = 3,

    /// <summary>
    ///     The user joined via the student hub.
    /// </summary>
    StudentHub = 4,

    /// <summary>
    ///     The user joined via an invite code.
    /// </summary>
    InviteCode = 5,

    /// <summary>
    ///     The user joined via a vanity URL.
    /// </summary>
    VanityUrl = 6,

    /// <summary>
    ///     The user was manually verified
    /// </summary>
    ManualVerification = 7
}
