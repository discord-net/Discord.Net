using Discord.Models.Json;

namespace Discord;

public sealed class CreateGuildScheduledEventProperties : IEntityProperties<CreateGuildScheduledEventParams>
{
    public Optional<EntityOrId<ulong, IGuildChannel>> Channel { get; set; }
    public Optional<string> Location { get; set; }
    public required string Name { get; set; }
    public StagePrivacyLevel PrivacyLevel { get; set; } = StagePrivacyLevel.GuildOnly;
    public required DateTimeOffset ScheduledStartTime { get; set; }
    public Optional<DateTimeOffset> ScheduledEndTime { get; set; }
    public Optional<string> Description { get; set; }
    public required GuildScheduledEventEntityType Type { get; set; }
    public Optional<Image> Image { get; set; }
    
    // TODO: recurrence
    
    public CreateGuildScheduledEventParams ToApiModel(CreateGuildScheduledEventParams? existing = default)
    {
        return new()
        {
            Name = Name,
            Image = Image.Map(v => v.ToImageData()),
            PrivacyLevel = (int) PrivacyLevel,
            Description = Description,
            ScheduledEndTime = ScheduledEndTime,
            ScheduledStartTime = ScheduledStartTime,
            ChannelId = Channel.MapToId(),
            EntityMetadata = Location.Map(v => new GuildScheduledEventEntityMetadata {Location = v}),
            EntityType = (int) Type
        };
    }
}