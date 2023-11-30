namespace Discord;

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
