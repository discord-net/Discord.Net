using Discord.Models.Json;

namespace Discord;

public abstract class CreateGuildChannelBaseProperties : IEntityProperties<CreateGuildChannelParams>
{
    public required string Name { get; set; }
    public Optional<int?> Position { get; set; }
    public Optional<Overwrite[]?> PermissionOverwrites { get; set; }
    
    protected abstract Optional<ChannelType> ChannelType { get; }
    
    public virtual CreateGuildChannelParams ToApiModel(CreateGuildChannelParams? existing = default)
    {
        existing ??= new CreateGuildChannelParams {Name = Name};

        existing.Type = ChannelType.MapToInt();
        existing.Position = Position;
        existing.PermissionOverwrites = PermissionOverwrites.Map(v => v?.Select(v => v.ToApiModel()).ToArray());

        return existing;
    }
}