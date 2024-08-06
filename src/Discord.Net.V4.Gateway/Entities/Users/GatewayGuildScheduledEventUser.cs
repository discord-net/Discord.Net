using Discord.Gateway.State;
using Discord.Models;
using static Discord.Template;

namespace Discord.Gateway;

using BaseActorType =
    GatewayCachedActor<ulong, GatewayGuildScheduledEventUser, GuildScheduledEventUserIdentity,
        IGuildScheduledEventUserModel>;
using BaseEntityType =
    GatewayCacheableEntity<GatewayGuildScheduledEventUser, ulong, IGuildScheduledEventUserModel>;

public sealed partial class GatewayGuildScheduledEventUserActor :
    BaseActorType,
    IGuildScheduledEventUserActor
{
    [SourceOfTruth] public GatewayUserActor User { get; }

    [SourceOfTruth] public GatewayMemberActor Member { get; }

    [SourceOfTruth, StoreRoot] public GatewayGuildScheduledEventActor GuildScheduledEvent { get; }

    [SourceOfTruth] public GatewayGuildActor Guild { get; }

    internal override GuildScheduledEventUserIdentity Identity { get; }

    public GatewayGuildScheduledEventUserActor(
        DiscordGatewayClient client,
        GuildIdentity guild,
        GuildScheduledEventIdentity scheduledEvent,
        GuildScheduledEventUserIdentity eventUser,
        UserIdentity? user = null,
        MemberIdentity? member = null
    ) : base(client, eventUser)
    {
        Identity = eventUser | this;

        Guild = client.Guilds >> guild;
        User = client.Users >> (user | eventUser);
        Member = Guild.Members >> (member | User | eventUser);
        GuildScheduledEvent = scheduledEvent.Actor ?? new(client, guild | Guild, scheduledEvent);
    }
}

public sealed partial class GatewayGuildScheduledEventUser :
    BaseEntityType,
    IGuildScheduledEventUser
{
    [ProxyInterface] internal GatewayGuildScheduledEventUserActor Actor { get; }

    internal IGuildScheduledEventUserModel Model { get; private set; }

    public GatewayGuildScheduledEventUser(
        DiscordGatewayClient client,
        GuildIdentity guild,
        GuildScheduledEventIdentity scheduledEvent,
        IGuildScheduledEventUserModel model,
        UserIdentity? user = null,
        MemberIdentity? member = null,
        GatewayGuildScheduledEventUserActor? actor = null
    ) : base(client, model.Id)
    {
        Model = model;
        Actor = actor ?? new(client, guild, scheduledEvent, GuildScheduledEventUserIdentity.Of(this), user, member);
    }

    public static GatewayGuildScheduledEventUser Construct(
        DiscordGatewayClient client,
        IGatewayConstructionContext context,
        IGuildScheduledEventUserModel model
    ) => new(
        client,
        context.Path.RequireIdentity(T<GuildIdentity>()),
        context.Path.GetIdentity(T<GuildScheduledEventIdentity>(), model.GuildScheduledEventId),
        model,
        context.Path.GetIdentity(T<UserIdentity>()),
        context.Path.GetIdentity(T<MemberIdentity>()),
        context.TryGetActor<GatewayGuildScheduledEventUserActor>()
    );

    public override ValueTask UpdateAsync(
        IGuildScheduledEventUserModel model,
        bool updateCache = true,
        CancellationToken token = default)
    {
        if (updateCache) return UpdateCacheAsync(this, model, token);

        Model = model;

        return ValueTask.CompletedTask;
    }

    public override IGuildScheduledEventUserModel GetModel() => Model;
}
