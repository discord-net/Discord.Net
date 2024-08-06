using Discord.Gateway.State;
using Discord.Models;
using static Discord.Template;

namespace Discord.Gateway;

[ExtendInterfaceDefaults]
public sealed partial class GatewayNewsChannelActor :
    GatewayTextChannelActor,
    INewsChannelActor,
    IGatewayCachedActor<ulong, GatewayNewsChannel, NewsChannelIdentity, IGuildNewsChannelModel>
{
    [SourceOfTruth] internal override NewsChannelIdentity Identity { get; }

    [method: TypeFactory]
    public GatewayNewsChannelActor(
        DiscordGatewayClient client,
        GuildIdentity guild,
        NewsChannelIdentity channel
    ) : base(client, guild, channel)
    {
        Identity = channel | this;
    }

    [SourceOfTruth]
    [CovariantOverride]
    internal GatewayNewsChannel CreateEntity(IGuildNewsChannelModel model)
        => Client.StateController.CreateLatent(this, model, CachePath);
}

[ExtendInterfaceDefaults]
public sealed partial class GatewayNewsChannel :
    GatewayTextChannel,
    INewsChannel,
    ICacheableEntity<GatewayNewsChannel, ulong, IGuildNewsChannelModel>
{
    [ProxyInterface] internal override GatewayNewsChannelActor Actor { get; }
    internal override IGuildNewsChannelModel Model => _model;

    private IGuildNewsChannelModel _model;

    public GatewayNewsChannel(
        DiscordGatewayClient client,
        GuildIdentity guild,
        IGuildNewsChannelModel model,
        GatewayNewsChannelActor? actor = null
    ) : base(client, guild, model, actor)
    {
        _model = model;
        Actor = actor ?? new(client, guild, NewsChannelIdentity.Of(this));
    }

    public static GatewayNewsChannel Construct(
        DiscordGatewayClient client,
        IGatewayConstructionContext context,
        IGuildNewsChannelModel model
    ) => new(
        client,
        context.Path.GetIdentity(T<GuildIdentity>(), model.GuildId),
        model,
        context.TryGetActor<GatewayNewsChannelActor>()
    );

    [CovariantOverride]
    public ValueTask UpdateAsync(
        IGuildNewsChannelModel model,
        bool updateCache = true,
        CancellationToken token = default)
    {
        if (updateCache) return UpdateCacheAsync(this, model, token);

        _model = model;

        return base.UpdateAsync(model, false, token);
    }


    public override IGuildNewsChannelModel GetModel() => Model;
}
