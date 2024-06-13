using Discord.Models.Json;

namespace Discord;

public sealed class ModifyGuildScheduledEventProperties : IEntityProperties<ModifyGuildScheduledEventParams>
{
    public Optional<EntityOrId<ulong, IChannel>?> Channel { get; set; }
    public Optional<string?> Location { get; set; }
    public Optional<string> Name { get; set; }
    public Optional<GuildScheduledEventPrivacyLevel> PrivacyLevel { get; set; }
    public Optional<DateTimeOffset> StartTime { get; set; }
    public Optional<DateTimeOffset> EndTime { get; set; }
    public Optional<string?> Description { get; set; }
    public Optional<GuildScheduledEntityType> EntityType { get; set; }
    public Optional<GuildScheduledEventStatus> Status { get; set; }
    public Optional<Image> Image { get; set; }

    public ModifyGuildScheduledEventParams ToApiModel(ModifyGuildScheduledEventParams? existing = default)
    {
        existing ??= new();

        existing.ChannelId = Channel.Map(v => v?.Id);
        existing.EntityMetadata = Location
            .Map<GuildScheduledEventEntityMetadata?>(v => new()
            {
                Location = Optional.FromNullable(v)
            });
        existing.Name = Name;
        existing.PrivacyLevel = PrivacyLevel.Map(v => (int)v);
        existing.ScheduledStartTime = StartTime;
        existing.ScheduledEndTime = EndTime;
        existing.Description = Description;
        existing.EntityType = EntityType.Map(v => (int)v);
        existing.Status = Status.Map(v => (int)v);
        existing.Image = Image.Map(v => v.ToImageData());

        return existing;
    }
}
