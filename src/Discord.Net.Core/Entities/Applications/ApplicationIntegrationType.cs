namespace Discord;

/// <summary>
///     Defines where an application can be installed.
/// </summary>
public enum ApplicationIntegrationType
{
    /// <summary>
    ///     The application can be installed to a guild.
    /// </summary>
    GuildInstall = 0,

    /// <summary>
    ///     The application can be installed to a user.
    /// </summary>
    UserInstall = 1,
}
