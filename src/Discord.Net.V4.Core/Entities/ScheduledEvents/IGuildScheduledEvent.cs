namespace Discord;

public interface IGuildScheduledEvent : ISnowflakeEntity
{
    ILoadableEntity<ulong, IChannel>? Channel { get; }
    ILoadableEntity<ulong, IUser> Creator { get; }
    string Name { get; }
    string? Description { get; }
    string? CoverImageId { get; }
    DateTimeOffset ScheduledStartTime { get; }
    DateTimeOffset? ScheduledEndTime { get; }
    GuildScheduledEventPrivacyLevel PrivacyLevel { get; }
    GuildScheduledEventStatus Status { get; }
    GuildScheduledEntityType Type { get; }
    ulong? EntityId { get; }
    string? Location { get; }
    int? UserCount { get; }
}
