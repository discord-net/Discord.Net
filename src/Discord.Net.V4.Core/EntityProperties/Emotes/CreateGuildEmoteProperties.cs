using Discord.Models.Json;

namespace Discord;

public sealed class CreateGuildEmoteProperties : IEntityProperties<CreateEmojiParams>
{
    public required string Name { get; set; }
    public required Image Image { get; set; }
    public Optional<IEnumerable<EntityOrId<ulong, IRole>>> Roles { get; set; }
    public CreateEmojiParams ToApiModel(CreateEmojiParams? existing = default)
    {
        return new CreateEmojiParams()
        {
            Name = Name,
            Image = Image.ToImageData(),
            Roles = Roles.Map(v => v.Select(x => x.Id).ToArray())
        };
    }
}