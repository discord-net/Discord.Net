using Discord.Models.Json;

namespace Discord;

public sealed class CreateGuildFromTemplateProperties :
    IEntityProperties<CreateGuildFromTemplateParams>
{
    public required string Name { get; set; }
    public Optional<Image> Icon { get; set; }
    
    public CreateGuildFromTemplateParams ToApiModel(CreateGuildFromTemplateParams? existing = default)
    {
        return new CreateGuildFromTemplateParams()
        {
            Name = Name,
            Icon = Icon.Map(v => v.ToImageData())
        };
    }
}