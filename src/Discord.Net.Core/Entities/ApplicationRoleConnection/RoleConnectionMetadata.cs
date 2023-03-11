using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord;

/// <summary>
///     Represents the role connection metadata object.
/// </summary>
public class RoleConnectionMetadata
{
    /// <summary>
    ///     Gets the of metadata value.
    /// </summary>
    public RoleConnectionMetadataType Type { get; }

    /// <summary>
    ///     Gets the dictionary key for the metadata field.
    /// </summary>
    public string Key { get; }

    /// <summary>
    ///     Gets the name of the metadata field.
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Gets the description of the metadata field.
    /// </summary>
    public string Description { get; }

    /// <summary>
    ///     Gets translations of the name. <see langword="null"/> if not set.
    /// </summary>
    public IReadOnlyDictionary<string, string> NameLocalizations { get; }

    /// <summary>
    ///     Gets translations of the description. <see langword="null"/> if not set.
    /// </summary>
    public IReadOnlyDictionary<string, string> DescriptionLocalizations { get; }

    internal RoleConnectionMetadata(RoleConnectionMetadataType type, string key, string name, string description,
        IDictionary<string, string> nameLocalizations = null, IDictionary<string, string> descriptionLocalizations = null)
    {
        Type = type;
        Key = key;
        Name = name;
        Description = description;
        NameLocalizations = nameLocalizations?.ToImmutableDictionary();
        DescriptionLocalizations = descriptionLocalizations?.ToImmutableDictionary();
    }

    /// <summary>
    ///     Initializes a new <see cref="RoleConnectionMetadataProperties"/> with the data from this object.
    /// </summary>
    public RoleConnectionMetadataProperties ToRoleConnectionMetadataProperties()
        => new()
        {
            Name = Name,
            Description = Description,
            Type = Type,
            Key = Key,
            NameLocalizations = NameLocalizations,
            DescriptionLocalizations = DescriptionLocalizations
        };
}
