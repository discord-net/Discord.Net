using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Channels;

namespace Discord.Rest.Guilds;

public sealed partial class RestLoadableGuildScheduledEventActor(
    DiscordRestClient client,
    GuildIdentity guild,
    GuildScheduledEventIdentity scheduledEvent
) :
    RestGuildScheduledEventActor(client, guild, scheduledEvent),
    ILoadableGuildScheduledEventActor
{
    [ProxyInterface(typeof(ILoadableEntity<IGuildScheduledEvent>))]
    internal RestLoadable<ulong, RestGuildScheduledEvent, IGuildScheduledEvent, IGuildScheduledEventModel> Loadable
    {
        get;
    } =
        RestLoadable<ulong, RestGuildScheduledEvent, IGuildScheduledEvent, IGuildScheduledEventModel>
            .FromContextConstructable<RestGuildScheduledEvent, GuildIdentity>(
                client,
                scheduledEvent,
                (guild, id) => Routes.GetGuildScheduledEvent(guild.Id, id),
                guild
            );
}

[ExtendInterfaceDefaults(
    typeof(IGuildScheduledEventActor),
    typeof(IModifiable<ulong, IGuildScheduledEventActor, ModifyGuildScheduledEventProperties,
        ModifyGuildScheduledEventParams>),
    typeof(IDeletable<ulong, IGuildScheduledEventActor>)
)]
public partial class RestGuildScheduledEventActor(
    DiscordRestClient client,
    GuildIdentity guild,
    GuildScheduledEventIdentity scheduledEvent
) :
    RestActor<ulong, RestGuildScheduledEvent, GuildScheduledEventIdentity>(client, scheduledEvent),
    IGuildScheduledEventActor
{
    public RestLoadableGuildActor Guild { get; } = new(client, guild);

    public IEnumerableIndexableActor<ILoadableGuildScheduledEventUserActor<IGuildScheduledEventUser>, ulong,
        IGuildScheduledEventUser> RSVPs => throw new NotImplementedException();

    ILoadableGuildActor IGuildRelationship.Guild => Guild;

    IGuildScheduledEvent IEntityProvider<IGuildScheduledEvent, IGuildScheduledEventModel>.CreateEntity(
        IGuildScheduledEventModel model
    ) => RestGuildScheduledEvent.Construct(Client, model, Guild.Loadable.Identity);
}

public sealed partial class RestGuildScheduledEvent :
    RestEntity<ulong>,
    IGuildScheduledEvent,
    IContextConstructable<RestGuildScheduledEvent, IGuildScheduledEventModel, GuildIdentity, DiscordRestClient>
{
    #region Properties

    public RestLoadableGuildChannelActor? Channel { get; private set; }

    public RestLoadableUserActor Creator { get; }

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

    [ProxyInterface(
        typeof(IGuildScheduledEventActor),
        typeof(IGuildRelationship),
        typeof(IEntityProvider<IGuildScheduledEvent, IGuildScheduledEventModel>)
    )]
    internal RestGuildScheduledEventActor Actor { get; }

    internal IGuildScheduledEventModel Model { get; private set; }

    internal RestGuildScheduledEvent(
        DiscordRestClient client,
        GuildIdentity guild,
        IGuildScheduledEventModel model,
        RestGuildScheduledEventActor? actor = null
    ) : base(client, model.Id)
    {
        Actor = actor ?? new(client, guild, GuildScheduledEventIdentity.Of(this));
        Model = model;

        Creator = new RestLoadableUserActor(
            client,
            UserIdentity.FromReferenced(model, model.CreatorId, model => RestUser.Construct(client, model))
        );

        Channel = model.ChannelId.Map(
            static (id, client, guild)
                => new RestLoadableGuildChannelActor(client, guild, GuildChannelIdentity.Of(id)),
            client,
            guild
        );
    }

    public static RestGuildScheduledEvent Construct(DiscordRestClient client, IGuildScheduledEventModel model,
        GuildIdentity guild) =>
        new(client, guild, model);

    public ValueTask UpdateAsync(IGuildScheduledEventModel model, CancellationToken token = default)
    {
        if (!Model.ChannelId?.Equals(model.ChannelId) ?? model.ChannelId is not null)
        {
            if (model.ChannelId.HasValue)
            {
                Channel ??= new RestLoadableGuildChannelActor(Client, Actor.Guild.Identity,
                    GuildChannelIdentity.Of(model.ChannelId.Value));

                Channel.Loadable.Id = model.ChannelId.Value;
            }
            else
            {
                Channel = null;
            }
        }

        Model = model;

        return ValueTask.CompletedTask;
    }

    public IGuildScheduledEventModel GetModel() => Model;

    ILoadableEntity<ulong, IUser> IGuildScheduledEvent.Creator => Creator;

    ILoadableEntity<ulong, IGuildChannel>? IGuildScheduledEvent.Channel => Channel;
}
