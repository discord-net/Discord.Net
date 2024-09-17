using Discord.Models.Json;

namespace Discord;

public sealed class ModifyGuildTemplateProperties :
    IEntityProperties<ModifyGuildTemplateParams>
{
    public Optional<string> Name { get; set; }
    public Optional<string?> Description { get; set; }
    
    public ModifyGuildTemplateParams ToApiModel(ModifyGuildTemplateParams? existing = default)
    {
        return new ModifyGuildTemplateParams()
        {
            Name = Name,
            Description = Description
        };
    }
}