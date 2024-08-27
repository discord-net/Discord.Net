namespace Discord.Models;

public interface IApplicationRoleConnectionMetadataModel : IEntityModel<string>
{
    int Type { get; }
    string Key { get; }
    string Name { get; }
    IDictionary<string, string>? NameLocalizations { get; }
    string Description { get; }
    IDictionary<string, string>? DescriptionLocalization { get; }

    string IEntityModel<string>.Id => Key;
}