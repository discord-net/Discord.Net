using System;
using System.Collections.Generic;

namespace Discord;

/// <summary>
///     Represents the connection object that the user has attached.
/// </summary>
public class RoleConnection
{
    /// <summary>
    ///     Gets the vanity name of the platform a bot has connected to.
    /// </summary>
    public string PlatformName { get; }

    /// <summary>
    ///     Gets the username on the platform a bot has connected to.
    /// </summary>
    public string PlatformUsername { get; }

    /// <summary>
    ///     Gets the object mapping <see cref="RoleConnectionMetadata"/> keys to their string-ified values.
    /// </summary>
    public IReadOnlyDictionary<string, string> Metadata { get; }

    internal RoleConnection(string platformName, string platformUsername, IReadOnlyDictionary<string, string> metadata)
    {
        PlatformName = platformName;
        PlatformUsername = platformUsername;
        Metadata = metadata;
    }

    /// <summary>
    ///     Initializes a new <see cref="RoleConnectionProperties"/> with the data from this object.
    /// </summary>
    public RoleConnectionProperties ToRoleConnectionProperties()
        => new()
        {
            PlatformName = PlatformName,
            PlatformUsername = PlatformUsername,
            Metadata = Metadata.ToDictionary()
        };
}
