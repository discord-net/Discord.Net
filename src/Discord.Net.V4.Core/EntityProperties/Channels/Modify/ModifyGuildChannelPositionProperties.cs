using Discord.Models.Json;

namespace Discord;

public class ModifyGuildChannelPositionProperties : IEntityProperties<ModifyGuildChannelPositionsParams>
{
    public required EntityOrId<ulong, IGuildChannel> Channel { get; set; }
    public Optional<int?> Position { get; set; }
    public Optional<bool?> LockPermissions { get; set; }
    public Optional<EntityOrId<ulong, ICategoryChannel>?> Category { get; set; }

    public ModifyGuildChannelPositionsParams ToApiModel(ModifyGuildChannelPositionsParams? existing = default)
    {
        existing ??= new ModifyGuildChannelPositionsParams();

        existing.Id = Channel.Id;
        existing.Position = Position;
        existing.LockPermissions = LockPermissions;
        existing.ParentId = Category.Map(v => v?.Id);

        return existing;
    }
}