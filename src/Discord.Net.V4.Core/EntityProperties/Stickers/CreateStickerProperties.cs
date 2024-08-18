using Discord.Models.Json;

namespace Discord;

public sealed class CreateGuildStickerProperties : IEntityProperties<CreateGuildStickerParams>
{
    public required string Name { get; set; }
    
    public required FileContents File { get; set; }
    
    public Optional<string> Description { get; set; }
    public Optional<string> Tags { get; set; }
    
    
    public CreateGuildStickerParams ToApiModel(CreateGuildStickerParams? existing = default)
    {
        return new CreateGuildStickerParams()
        {
            Name = Name,
            Description = Description,
            File = File.ToApiModel(),
            Tags = Tags
        };
    }
}
