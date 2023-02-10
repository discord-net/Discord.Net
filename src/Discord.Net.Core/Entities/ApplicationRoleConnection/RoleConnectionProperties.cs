using System;
using System.Collections.Generic;

namespace Discord;

/// <summary>
///     Represents the properties used to modify user's <see cref="RoleConnection"/>.
/// </summary>
public class RoleConnectionProperties
{
    private const int MaxPlatformNameLength = 50;
    private const int MaxPlatformUsernameLength = 100;
    private const int MaxMetadataRecords = 100;

    private string _platformName;
    private string _platformUsername;
    private Dictionary<string, string> _metadata;

    /// <summary>
    ///     Gets or sets the vanity name of the platform a bot has connected. Max 50 characters.
    /// </summary>
    public string PlatformName
    {
        get => _platformName;
        set
        {
            if (value is not null)
                Preconditions.AtMost(value.Length, MaxPlatformNameLength, nameof(PlatformName), $"Platform name length must be less or equal to {MaxPlatformNameLength}");
            _platformName = value;
        }
    }

    /// <summary>
    ///     Gets or sets the username on the platform a bot has connected. Max 100 characters.
    /// </summary>
    public string PlatformUsername
    {
        get => _platformUsername;
        set
        {
            if (value is not null)
                Preconditions.AtMost(value.Length, MaxPlatformUsernameLength, nameof(PlatformUsername), $"Platform username length must be less or equal to {MaxPlatformUsernameLength}");
            _platformUsername = value;
        }
    }

    /// <summary>
    ///     Gets or sets object mapping <see cref="RoleConnectionMetadata"/> keys to their string-ified values.
    /// </summary>
    public Dictionary<string, string> Metadata
    {
        get => _metadata;
        set
        {
            if (value is not null)
                Preconditions.AtMost(value.Count, MaxPlatformUsernameLength, nameof(Metadata), $"Metadata records count must be less or equal to {MaxMetadataRecords}");
            _metadata = value;
        }
    }

    /// <summary>
    ///     Adds a metadata record with the provided key and value.
    /// </summary>
    /// <returns>The current <see cref="RoleConnectionProperties"/>.</returns>
    public RoleConnectionProperties WithDate(string key, DateTimeOffset value)
        => AddMetadataRecord(key, value.ToString("O"));

    /// <summary>
    ///     Adds a metadata record with the provided key and value.
    /// </summary>
    /// <returns>The current <see cref="RoleConnectionProperties"/>.</returns>
    public RoleConnectionProperties WithBool(string key, bool value)
        => AddMetadataRecord(key, value ? "1" : "0");

    /// <summary>
    ///     Adds a metadata record with the provided key and value.
    /// </summary>
    /// <returns>The current <see cref="RoleConnectionProperties"/>.</returns>
    public RoleConnectionProperties WithNumber(string key, int value)
        => AddMetadataRecord(key, value.ToString());

    /// <summary>
    ///     Adds a metadata record with the provided key and value.
    /// </summary>
    /// <returns>The current <see cref="RoleConnectionProperties"/>.</returns>
    public RoleConnectionProperties WithNumber(string key, uint value)
        => AddMetadataRecord(key, value.ToString());

    /// <summary>
    ///     Adds a metadata record with the provided key and value.
    /// </summary>
    /// <returns>The current <see cref="RoleConnectionProperties"/>.</returns>
    public RoleConnectionProperties WithNumber(string key, long value)
        => AddMetadataRecord(key, value.ToString());

    /// <summary>
    ///     Adds a metadata record with the provided key and value.
    /// </summary>
    /// <returns>The current <see cref="RoleConnectionProperties"/>.</returns>
    public RoleConnectionProperties WithNumber(string key, ulong value)
        => AddMetadataRecord(key, value.ToString());

    internal RoleConnectionProperties AddMetadataRecord(string key, string value)
    {
        Metadata ??= new Dictionary<string, string>();
        if (!Metadata.ContainsKey(key))
            Preconditions.AtMost(Metadata.Count + 1, MaxPlatformUsernameLength, nameof(Metadata), $"Metadata records count must be less or equal to {MaxMetadataRecords}");

        _metadata[key] = value;
        return this;
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="RoleConnectionProperties"/>.
    /// </summary>
    /// <param name="platformName">The name of the platform a bot has connected.</param>s
    /// <param name="platformUsername">Gets the username on the platform a bot has connected.</param>
    /// <param name="metadata">Object mapping <see cref="RoleConnectionMetadata"/> keys to their values.</param>
    public RoleConnectionProperties(string platformName, string platformUsername, IDictionary<string, string> metadata = null)
    {
        PlatformName = platformName;
        PlatformUsername = platformUsername;
        Metadata = metadata.ToDictionary();
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="RoleConnectionProperties"/>.
    /// </summary>
    public RoleConnectionProperties() { }

    /// <summary>
    ///     Initializes a new <see cref="RoleConnectionProperties"/> with the data from provided <see cref="RoleConnection"/>.
    /// </summary>
    public static RoleConnectionProperties FromRoleConnection(RoleConnection roleConnection)
        => new()
        {
            PlatformName = roleConnection.PlatformName,
            PlatformUsername = roleConnection.PlatformUsername,
            Metadata = roleConnection.Metadata.ToDictionary()
        };
}
