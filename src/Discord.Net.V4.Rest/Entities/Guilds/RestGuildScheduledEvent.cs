using Discord.Models;
using Discord.Models.Json;

namespace Discord.Rest.Guilds;


public sealed partial class RestLoadableGuildScheduledEventActor(DiscordRestClient client, ulong guildId, ulong id) :
    RestGuildScheduledEventActor(client, guildId, id),
    ILoadableGuildScheduledEventActor
{
    [ProxyInterface(typeof(ILoadableEntity<IGuildScheduledEvent>))]
    internal RestLoadable<ulong, RestGuildScheduledEvent, IGuildScheduledEvent, GuildScheduledEvent> Loadable { get; } =
        RestLoadable<ulong, RestGuildScheduledEvent, IGuildScheduledEvent, GuildScheduledEvent>
            .FromContextConstructable<RestGuildScheduledEvent, ulong>(
                client,
                id,
                Routes.GetGuildScheduledEvent,
                guildId
            );
}

[ExtendInterfaceDefaults(
    typeof(IGuildScheduledEventActor),
    typeof(IModifiable<ulong, IGuildScheduledEventActor, ModifyGuildScheduledEventProperties, ModifyGuildScheduledEventParams>),
    typeof(IDeletable<ulong, IGuildScheduledEventActor>)
)]
public partial class RestGuildScheduledEventActor(DiscordRestClient client, ulong guildId, ulong id) :
    RestActor<ulong, RestGuildScheduledEvent>(client, id),
    IGuildScheduledEventActor
{
    public RestLoadableGuildActor Guild { get; } = new(client, guildId);

    public IEnumerableIndexableActor<ILoadableGuildScheduledEventUserActor<IGuildScheduledEventUser>, ulong, IGuildScheduledEventUser> RSVPs => throw new NotImplementedException();

    ILoadableGuildActor IGuildRelationship.Guild => Guild;
}

public sealed partial class RestGuildScheduledEvent(
    DiscordRestClient client,
    ulong guildId, IGuildScheduledEventModel model,
    RestGuildScheduledEventActor? actor = null
):
    RestEntity<ulong>(client, model.Id),
    IGuildScheduledEvent,
    IContextConstructable<RestGuildScheduledEvent, IGuildScheduledEventModel, ulong, DiscordRestClient>
{
    [ProxyInterface(typeof(IGuildScheduledEventActor), typeof(IGuildRelationship))]
    internal RestGuildScheduledEventActor Actor { get; } = actor ?? new(client, guildId, model.Id);

    internal IGuildScheduledEventModel Model { get; } = model;

    public static RestGuildScheduledEvent Construct(DiscordRestClient client, IGuildScheduledEventModel model,
        ulong guildId)
        => new RestGuildScheduledEvent(client, guildId, model);

    #region Loadables

    public ILoadableEntity<ulong, IChannel>? Channel => throw new NotImplementedException();

    public ILoadableEntity<ulong, IUser> Creator => throw new NotImplementedException();

    #endregion

    #region Properties

    public string Name => Model.Name;

    public string? Description => Model.Description;

    public string? CoverImageId => Model.Image;

    public DateTimeOffset ScheduledStartTime => Model.ScheduledStartTime;

    public DateTimeOffset? ScheduledEndTime => Model.ScheduledEndTime;

    public GuildScheduledEventPrivacyLevel PrivacyLevel => (GuildScheduledEventPrivacyLevel)Model.PrivacyLevel;

    public GuildScheduledEventStatus Status => (GuildScheduledEventStatus)Model.Status;

    public GuildScheduledEntityType Type => (GuildScheduledEntityType)Model.EntityType;

    public ulong? EntityId => Model.EntityId;

    public string? Location => Model.Location;

    public int? UserCount => Model.UserCount;

    #endregion
}
