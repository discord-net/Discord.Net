using Discord.Models.Json;

namespace Discord;

public sealed class CreateGuildEmoteProperties : IEntityProperties<CreateGuildEmojiParams>
{
    public required string Name { get; set; }
    public required Image Image { get; set; }
    public Optional<IEnumerable<EntityOrId<ulong, IRole>>> Roles { get; set; }
    public CreateGuildEmojiParams ToApiModel(CreateGuildEmojiParams? existing = default)
    {
        return new CreateGuildEmojiParams()
        {
            Name = Name,
            Image = Image.ToImageData(),
            Roles = Roles.Map(v => v.Select(x => x.Id).ToArray())
        };
    }
}