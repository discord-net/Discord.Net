using Discord.Models.Json;

namespace Discord;

public class CreateGuildChannelProperties : IEntityProperties<CreateGuildChannelParams>
{
    public required string Name { get; set; }
    public Optional<ChannelType> Type { get; set; }
    public Optional<int?> Position { get; set; }
    public Optional<Overwrite[]?> PermissionOverwrites { get; set; }

    public virtual CreateGuildChannelParams ToApiModel(CreateGuildChannelParams? existing = default)
    {
        existing ??= new CreateGuildChannelParams {Name = Name};

        existing.Type = Type.MapToInt();
        existing.Position = Position;
        existing.PermissionOverwrites = PermissionOverwrites.Map(v => v?.Select(v => v.ToApiModel()).ToArray());

        return existing;
    }
}
