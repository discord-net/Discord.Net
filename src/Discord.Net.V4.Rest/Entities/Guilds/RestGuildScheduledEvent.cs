using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using Discord.Rest.Extensions;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public partial class RestGuildScheduledEventActor :
    RestActor<RestGuildScheduledEventActor, ulong, RestGuildScheduledEvent, IGuildScheduledEventModel>,
    IGuildScheduledEventActor
{
    [SourceOfTruth]
    public RestGuildActor Guild { get; }

    [SourceOfTruth]
    public RestGuildScheduledEventUserActor.Paged<PageGuildScheduledEventUsersParams> RSVPs { get; }

    internal override GuildScheduledEventIdentity Identity { get; }

    [TypeFactory]
    public RestGuildScheduledEventActor(
        DiscordRestClient client,
        GuildIdentity guild,
        GuildScheduledEventIdentity scheduledEvent
        ) : base(client, scheduledEvent)
    {
        Identity = scheduledEvent | this;

        Guild = guild.Actor ?? new(client, guild);
        RSVPs = new(
            client,
            this,
            x => x,
            (model, _) => RestGuildScheduledEventUser.Construct(client, new(Guild.Identity, Identity), model)
        );
    }

    [SourceOfTruth]
    internal override RestGuildScheduledEvent CreateEntity(
        IGuildScheduledEventModel model
    ) => RestGuildScheduledEvent.Construct(Client, Guild.Identity, model);
}

public sealed partial class RestGuildScheduledEvent :
    RestEntity<ulong>,
    IGuildScheduledEvent,
    IRestConstructable<RestGuildScheduledEvent, RestGuildScheduledEventActor, IGuildScheduledEventModel>
{
    [SourceOfTruth]
    public RestGuildChannelActor? Channel { get; private set; }

    [SourceOfTruth]
    public RestUserActor Creator { get; }

    public string Name => Model.Name;

    public string? Description => Model.Description;

    public string? CoverImageId => Model.Image;

    public DateTimeOffset ScheduledStartTime => Model.ScheduledStartTime;

    public DateTimeOffset? ScheduledEndTime => Model.ScheduledEndTime;

    public GuildScheduledEventPrivacyLevel PrivacyLevel => (GuildScheduledEventPrivacyLevel)Model.PrivacyLevel;

    public GuildScheduledEventStatus Status => (GuildScheduledEventStatus)Model.Status;

    public GuildScheduledEventEntityType Type => (GuildScheduledEventEntityType)Model.EntityType;

    public ulong? EntityId => Model.EntityId;

    public string? Location => Model.Location;

    public int? UserCount => Model.UserCount;

    [ProxyInterface(
        typeof(IGuildScheduledEventActor),
        typeof(IGuildRelationship),
        typeof(IEntityProvider<IGuildScheduledEvent, IGuildScheduledEventModel>)
    )]
    internal RestGuildScheduledEventActor Actor { get; }

    internal IGuildScheduledEventModel Model { get; private set; }

    internal RestGuildScheduledEvent(
        DiscordRestClient client,
        IGuildScheduledEventModel model,
        RestGuildScheduledEventActor actor
    ) : base(client, model.Id)
    {
        Actor = actor;
        Model = model;

        Creator = new RestUserActor(
            client,
            UserIdentity.FromReferenced(model, model.CreatorId, model => RestUser.Construct(client, model))
        );

        Channel = model.ChannelId.Map(
            static (id, client, guild)
                => new RestGuildChannelActor(client, guild, GuildChannelIdentity.Of(id)),
            client,
            guild
        );
    }

    public static RestGuildScheduledEvent Construct(
        DiscordRestClient client,
        RestGuildScheduledEventActor actor,
        IGuildScheduledEventModel model
    ) => new(client, model, actor);

    public ValueTask UpdateAsync(IGuildScheduledEventModel model, CancellationToken token = default)
    {
        Channel = Channel.UpdateFrom(
            model.ChannelId,
            RestGuildChannelActor.Factory,
            Client,
            Actor.Guild.Identity
        );

        Model = model;

        return ValueTask.CompletedTask;
    }

    public IGuildScheduledEventModel GetModel() => Model;
}
