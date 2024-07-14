using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

[Refreshable(nameof(Routes.GetGuildScheduledEvent))]
public partial interface IGuildScheduledEvent :
    ISnowflakeEntity,
    IGuildScheduledEventActor,
    IEntityOf<IGuildScheduledEventModel>
{
    ILoadableEntity<ulong, IGuildChannel>? Channel { get; }
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
