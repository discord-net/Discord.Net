using Discord.Gateway.State;
using Discord.Models;
using static Discord.Template;

namespace Discord.Gateway;

public sealed partial class GatewayBanActor :
    GatewayCachedActor<ulong, GatewayBan, BanIdentity, IBanModel>,
    IBanActor
{
    [SourceOfTruth, StoreRoot] public GatewayGuildActor Guild { get; }

    [SourceOfTruth] public GatewayUserActor User { get; }

    internal override BanIdentity Identity { get; }

    public GatewayBanActor(
        DiscordGatewayClient client,
        GuildIdentity guild,
        BanIdentity ban,
        UserIdentity? user = null
    ) : base(client, ban)
    {
        Identity = ban | this;

        Guild = client.Guilds >> guild;
        User = client.Users >> (user | ban);
    }


    [SourceOfTruth]
    internal GatewayBan CreateEntity(IBanModel model)
        => Client.StateController.CreateLatent(this, model, CachePath);
}

public sealed partial class GatewayBan :
    GatewayCacheableEntity<GatewayBan, ulong, IBanModel>,
    IBan
{
    public string? Reason => Model.Reason;

    [ProxyInterface] internal GatewayBanActor Actor { get; }

    internal IBanModel Model { get; private set; }

    internal GatewayBan(
        DiscordGatewayClient client,
        GuildIdentity guild,
        IBanModel model,
        UserIdentity? user = null,
        GatewayBanActor? actor = null
    ) : base(client, model.Id)
    {
        Model = model;
        Actor = actor ?? new(client, guild, BanIdentity.Of(this), user);
    }

    public static GatewayBan Construct(
        DiscordGatewayClient client,
        IGatewayConstructionContext context,
        IBanModel model
    ) => new(
        client,
        context.Path.RequireIdentity(T<GuildIdentity>()),
        model,
        context.Path.GetIdentity(T<UserIdentity>()),
        context.TryGetActor<GatewayBanActor>()
    );

    public override ValueTask UpdateAsync(
        IBanModel model,
        bool updateCache = true,
        CancellationToken token = default)
    {
        if (updateCache) return UpdateCacheAsync(this, model, token);

        Model = model;

        return ValueTask.CompletedTask;
    }

    public override IBanModel GetModel() => Model;
}