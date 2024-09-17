using Discord.Models.Json;

namespace Discord;

public sealed class CreateGuildTemplateProperties :
    IEntityProperties<CreateTemplateParams>
{
    public required string Name { get; set; }
    public Optional<string?> Description { get; set; }

    public CreateTemplateParams ToApiModel(CreateTemplateParams? existing = default)
    {
        return new CreateTemplateParams()
        {
            Name = Name,
            Description = Description
        };
    }
}