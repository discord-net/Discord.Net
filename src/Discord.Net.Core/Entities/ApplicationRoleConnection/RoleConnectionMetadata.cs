using System.Collections.Generic;

namespace Discord;

public class RoleConnectionMetadata
{
    public RoleConnectionMetadataType Type { get; }

    public string Key { get; }

    public string Name{ get; }

    public Optional<Dictionary<string, string>> NameLocalizations { get; }

    public string Description { get; }

    public Optional<Dictionary<string, string>> DescriptionLocalizations { get; }

    internal RoleConnectionMetadata(RoleConnectionMetadataType type, string key, string name, string description,
        Dictionary<string, string> nameLocalizations = null, Dictionary<string, string> descriptionLocalizations = null)
    {
        Type = type;
        Key = key;
        Name = name;
        Description = description;
        NameLocalizations = nameLocalizations;
        DescriptionLocalizations = descriptionLocalizations;
    }
}
