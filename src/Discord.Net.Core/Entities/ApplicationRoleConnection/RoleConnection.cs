using System.Collections.Generic;

namespace Discord;

public class RoleConnection
{
    /// <summary>
    ///     Gets the vanity name of the platform a bot has connected.
    /// </summary>
    public string PlatformName { get; }

    /// <summary>
    ///     Gets the username on the platform a bot has connected.
    /// </summary>
    public string PlatformUsername { get; }

    /// <summary>
    ///     
    /// </summary>
    public IReadOnlyDictionary<string, object> Metadata { get; }

    internal RoleConnection(string platformName, string platformUsername, IReadOnlyDictionary<string, object> metadata)
    {
        PlatformName = platformName;
        PlatformUsername = platformUsername;
        Metadata = metadata;
    }
}
