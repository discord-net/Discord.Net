using Discord.Models;

namespace Discord;

public interface IApplicationRoleConnectionMetadata :
    IEntity<string, IApplicationRoleConnectionMetadataModel>
{
    RoleConnectionMetadataType Type { get; }
    string Key { get; }
    string Name { get; }
    IReadOnlyDictionary<string, string> NameLocalization { get; }
    string Description { get; }
    IReadOnlyDictionary<string, string> DescriptionLocalization { get; }

    string IIdentifiable<string>.Id => Key;
}