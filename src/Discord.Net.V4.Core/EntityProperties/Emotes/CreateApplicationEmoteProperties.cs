using Discord.Models.Json;

namespace Discord;

public sealed class CreateApplicationEmoteProperties : IEntityProperties<CreateApplicationEmojiParams>
{
    public required string Name { get; set; }
    public required Image Image { get; set; }
    
    public CreateApplicationEmojiParams ToApiModel(CreateApplicationEmojiParams? existing = default)
    {
        return new CreateApplicationEmojiParams()
        {
            Name = Name,
            Image = Image.ToImageData()
        };
    }
}