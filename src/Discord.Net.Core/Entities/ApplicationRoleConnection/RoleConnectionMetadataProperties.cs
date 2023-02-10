using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord;

/// <summary>
///     Properties object used to create or modify <see cref="RoleConnectionMetadata"/> object.
/// </summary>
public class RoleConnectionMetadataProperties
{
    private const int MaxKeyLength = 50;
    private const int MaxNameLength = 100;
    private const int MaxDescriptionLength = 200;

    private string _key;
    private string _name;
    private string _description;

    private IReadOnlyDictionary<string, string> _nameLocalizations;
    private IReadOnlyDictionary<string, string> _descriptionLocalizations;

    /// <summary>
    ///     Gets or sets the of metadata value.
    /// </summary>
    public RoleConnectionMetadataType Type { get; set; }

    /// <summary>
    ///     Gets or sets the dictionary key for the metadata field.
    /// </summary>
    public string Key
    {
        get => _key;
        set
        {
            Preconditions.AtMost(value.Length, MaxKeyLength, nameof(Key), $"Key length must be less than or equal to {MaxKeyLength}");
            _key = value;
        }
    }

    /// <summary>
    ///     Gets or sets the name of the metadata field.
    /// </summary>
    public string Name
    {
        get => _name;
        set
        {
            Preconditions.AtMost(value.Length, MaxNameLength, nameof(Name), $"Name length must be less than or equal to {MaxNameLength}");
            _name = value;
        }
    }

    /// <summary>
    ///     Gets or sets the description of the metadata field.
    /// </summary>
    public string Description
    {
        get => _description;
        set
        {
            Preconditions.AtMost(value.Length, MaxDescriptionLength, nameof(Description), $"Description length must be less than or equal to {MaxDescriptionLength}");
            _description = value;
        }
    }

    /// <summary>
    ///     Gets or sets translations of the name. <see langword="null"/> if not set.
    /// </summary>
    public IReadOnlyDictionary<string, string> NameLocalizations
    {
        get => _nameLocalizations;
        set
        {
            if (value is not null)
                foreach (var localization in value)
                    if (localization.Value.Length > MaxNameLength)
                        throw new ArgumentException($"Name localization length must be less than or equal to {MaxNameLength}. Locale '{localization}'");
            _nameLocalizations = value;
        }
    }

    /// <summary>
    ///     Gets or sets translations of the description. <see langword="null"/> if not set.
    /// </summary>
    public IReadOnlyDictionary<string, string> DescriptionLocalizations
    {
        get => _descriptionLocalizations;
        set
        {
            if (value is not null)
                foreach (var localization in value)
                    if (localization.Value.Length > MaxDescriptionLength)
                        throw new ArgumentException($"Description localization length must be less than or equal to {MaxDescriptionLength}. Locale '{localization}'");
            _descriptionLocalizations = value;
        }
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="RoleConnectionMetadataProperties"/>.
    /// </summary>
    /// <param name="type">The type of the metadata value.</param>
    /// <param name="key">The dictionary key for the metadata field. Max 50 characters.</param>
    /// <param name="name">The name of the metadata visible in user profile. Max 100 characters.</param>
    /// <param name="description">The description of the metadata visible in user profile. Max 200 characters.</param>
    /// <param name="nameLocalizations">Translations for the name.</param>
    /// <param name="descriptionLocalizations">Translations for the description.</param>
    public RoleConnectionMetadataProperties(RoleConnectionMetadataType type, string key, string name, string description,
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
    ///     Initializes a new instance of <see cref="RoleConnectionMetadataProperties"/>.
    /// </summary>
    public RoleConnectionMetadataProperties() { }

    /// <summary>
    ///     Initializes a new <see cref="RoleConnectionMetadataProperties"/> with the data from provided <see cref="RoleConnectionMetadata"/>.
    /// </summary>
    public static RoleConnectionMetadataProperties FromRoleConnectionMetadata(RoleConnectionMetadata metadata)
        => new()
        {
            Name = metadata.Name,
            Description = metadata.Description,
            Type = metadata.Type,
            Key = metadata.Key,
            NameLocalizations = metadata.NameLocalizations,
            DescriptionLocalizations = metadata.DescriptionLocalizations
        };
}

