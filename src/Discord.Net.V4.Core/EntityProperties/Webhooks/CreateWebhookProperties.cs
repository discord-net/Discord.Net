using Discord.Models.Json;

namespace Discord;

public sealed class CreateWebhookProperties : IEntityProperties<CreateWebhookParams>
{
    public required string Name { get; set; }
    public Optional<Image> Avatar { get; set; }
    
    public CreateWebhookParams ToApiModel(CreateWebhookParams? existing = default)
    {
        return new CreateWebhookParams()
        {
            Name = Name,
            Avatar = Avatar.Map<string?>(v => v.ToImageData())
        };
    }
}