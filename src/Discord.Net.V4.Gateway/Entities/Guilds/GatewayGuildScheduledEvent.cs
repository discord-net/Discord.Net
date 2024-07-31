using Discord.Gateway.State;
using Discord.Gateway;
using Discord.Models;
using static Discord.Template;

namespace Discord.Gateway;

public sealed partial class GatewayGuildScheduledEventActor(
    DiscordGatewayClient client,
    GuildIdentity guild,
    GuildScheduledEventIdentity scheduledEvent
) :
    GatewayCachedActor<ulong, GatewayGuildScheduledEvent, GuildScheduledEventIdentity, IGuildScheduledEventModel>(client, scheduledEvent),
    IGuildScheduledEventActor
{
    [StoreRoot] [SourceOfTruth] public GatewayGuildActor Guild { get; } = guild.Actor ?? new(client, guild);

    public IEnumerableIndexableActor<IGuildScheduledEventUserActor, ulong, IGuildScheduledEventUser> RSVPs =>
        throw new NotImplementedException();

    [SourceOfTruth]
    internal GatewayGuildScheduledEvent CreateEntity(IGuildScheduledEventModel model)
        => Client.StateController.CreateLatent(this, model);
}

public sealed partial class GatewayGuildScheduledEvent :
    GatewayCacheableEntity<GatewayGuildScheduledEvent, ulong, IGuildScheduledEventModel, GuildScheduledEventIdentity>,
    IGuildScheduledEvent
{
    [SourceOfTruth]
    public GatewayGuildChannelActor? Channel { get; private set; }

    [SourceOfTruth]
    public GatewayUserActor Creator { get; }

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

    [ProxyInterface]
    internal GatewayGuildScheduledEventActor Actor { get; }

    internal IGuildScheduledEventModel Model { get; private set; }

    public GatewayGuildScheduledEvent(
        DiscordGatewayClient client,
        GuildIdentity guild,
        IGuildScheduledEventModel model,
        GatewayGuildScheduledEventActor? actor = null,
        IEntityHandle<ulong, GatewayGuildScheduledEvent>? implicitHandle = null
    ) : base(client, model.Id, implicitHandle)
    {
        Model = model;
        Actor = actor ?? new(client, guild, GuildScheduledEventIdentity.Of(this));

        Channel = model.ChannelId.Map(
            (id, client, guild) => new GatewayGuildChannelActor(client, guild, GuildChannelIdentity.Of(id)),
            client,
            guild
        );

        Creator = new GatewayUserActor(client, UserIdentity.Of(model.CreatorId));
    }

    public static GatewayGuildScheduledEvent Construct(
        DiscordGatewayClient client,
        ICacheConstructionContext<ulong, GatewayGuildScheduledEvent> context,
        IGuildScheduledEventModel model
    ) => new(
        client,
        context.Path.GetIdentity(T<GuildIdentity>(), model.GuildId),
        model,
        context.TryGetActor(T<GatewayGuildScheduledEventActor>()),
        context.ImplicitHandle
    );

    public override ValueTask UpdateAsync(
        IGuildScheduledEventModel model,
        bool updateCache = true,
        CancellationToken token = default)
    {
        if (updateCache) return UpdateCacheAsync(this, model, token);

        Channel = Channel.UpdateFrom(
            model.ChannelId,
            GatewayGuildChannelActor.Factory,
            Client,
            Guild.Identity
        );

        Model = model;

        return ValueTask.CompletedTask;
    }

    public override IGuildScheduledEventModel GetModel() => Model;
}
