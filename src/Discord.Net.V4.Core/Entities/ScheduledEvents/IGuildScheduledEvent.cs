using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

using IRefreshable = IRefreshable<IGuildScheduledEvent, ulong, IGuildScheduledEventModel>;
using IModifiable =
    IModifiable<ulong, IGuildScheduledEvent, ModifyGuildScheduledEventProperties, ModifyGuildScheduledEventParams,
        IGuildScheduledEventModel>;

public interface IGuildScheduledEvent :
    ISnowflakeEntity,
    IGuildScheduledEventActor,
    IRefreshable,
    IModifiable
{
    static IApiInOutRoute<ModifyGuildScheduledEventParams, IEntityModel> IModifiable.ModifyRoute(
        IPathable path,
        ulong id,
        ModifyGuildScheduledEventParams args
    ) => Routes.ModifyGuildScheduledEvent(path.Require<IGuild>(), id, args);

    static IApiOutRoute<IGuildScheduledEventModel> IRefreshable.RefreshRoute(IGuildScheduledEvent self,
        ulong id) => Routes.GetGuildScheduledEvent(self.Require<IGuild>(), id);

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
