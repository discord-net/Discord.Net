using Discord.Models.Json;

namespace Discord;

public sealed class CreateRoleProperties : IEntityProperties<CreateGuildRoleParams>
{
    public Optional<string> Name { get; set; }
    public Optional<PermissionSet> Permissions { get; set; }
    public Optional<Color> Color { get; set; }
    public Optional<bool> Hoist { get; set; }
    public Optional<Image> Icon { get; set; }
    public Optional<Emoji> Emoji { get; set; }
    public Optional<bool> Mentionable { get; set; }
    
    public CreateGuildRoleParams ToApiModel(CreateGuildRoleParams? existing = default)
    {
        return new CreateGuildRoleParams()
        {
            Name = Name,
            Color = Color.Map(v => v.RawValue),
            Icon = Icon.Map<string?>(v => v.ToImageData()),
            Permissions = Permissions.Map(v => v.ToString()),
            IsHoisted = Hoist,
            IsMentionable = Mentionable,
            UnicodeEmoji = Emoji.Map(v => v.Name)
        };
    }
}