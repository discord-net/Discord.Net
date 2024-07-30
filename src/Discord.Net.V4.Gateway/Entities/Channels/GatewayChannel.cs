using Discord.Gateway.Cache;
using Discord.Gateway.State;
using Discord.Models;

namespace Discord.Gateway;

[ExtendInterfaceDefaults]
public partial class GatewayChannelActor(
    DiscordGatewayClient client,
    ChannelIdentity channel
):
    GatewayCachedActor<ulong, GatewayChannel, ChannelIdentity, IChannelModel>(client, channel),
    IChannelActor
{

    [SourceOfTruth]
    internal GatewayChannel CreateEntity(IChannelModel model)
        => Client.StateController.CreateLatent<ulong, GatewayChannel, IChannelModel>(this, model);

    internal override ValueTask<IEntityModelStore<ulong, IChannelModel>> GetStoreAsync(
        CancellationToken token = default
    ) => Client.StateController.GetStoreAsync(Template.Of<ChannelIdentity>(), token);
}

public partial class GatewayChannel :
    GatewayCacheableEntity<GatewayChannel, ulong, IChannelModel, ChannelIdentity>,
    IChannel,
    IStoreProvider<ulong, IChannelModel>,
    IBrokerProvider<ulong, GatewayChannel, IChannelModel>,
    IContextConstructable<GatewayChannel, IChannelModel, ICacheConstructionContext<ulong, GatewayChannel>, DiscordGatewayClient>
{
    public ChannelType Type => (ChannelType)Model.Type;

    [ProxyInterface]
    internal virtual GatewayChannelActor Actor { get; }

    internal virtual IChannelModel Model => _model;

    private IChannelModel _model;

    public GatewayChannel(
        DiscordGatewayClient client,
        IChannelModel model,
        GatewayChannelActor? actor = null
    ) : base(client, model.Id)
    {
        _model = model;
        Actor = actor ?? new(client, ChannelIdentity.Of(this));
    }

    public static GatewayChannel Construct(DiscordGatewayClient client, ICacheConstructionContext<ulong, GatewayChannel> context, IChannelModel model) => throw new NotImplementedException();

    public override ValueTask UpdateAsync(
        IChannelModel model,
        bool updateCache = true,
        CancellationToken token = default)
    {
        if (updateCache) return UpdateCacheAsync(this, model, token);

        _model = model;

        return ValueTask.CompletedTask;
    }

    public override IChannelModel GetModel() => Model;

}
