using Discord.Models.Json;

namespace Discord;

public sealed class ModifyApplicationRoleConnectionMetadataProperties : 
    IEntityProperties<ApplicationRoleConnectionMetadata>
{
    public required RoleConnectionMetadataType Type { get; set; }
    public required string Key { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    
    public Optional<IDictionary<string, string>> NameLocalizations { get; set; }
    public Optional<IDictionary<string, string>> DescriptionLocalizations { get; set; }
    
    public ApplicationRoleConnectionMetadata ToApiModel(ApplicationRoleConnectionMetadata? existing = default)
    {
        return new ApplicationRoleConnectionMetadata()
        {
            Type = (int)Type,
            Key = Key,
            Name = Name,
            Description = Description,
            NameLocalizations = NameLocalizations,
            DescriptionLocalizations = DescriptionLocalizations
        };
    }
}