using Discord.Gateway;
using Discord.Gateway.State;
using Discord.Models;

namespace Discord.Gateway;

[ExtendInterfaceDefaults]
public partial class GatewayChannelActor(
    DiscordGatewayClient client,
    ChannelIdentity channel
) :
    GatewayCachedActor<ulong, GatewayChannel, ChannelIdentity, IChannelModel>(client, channel),
    IChannelActor
{
    [SourceOfTruth]
    internal GatewayChannel CreateEntity(IChannelModel model)
        => Client.StateController.CreateLatent(this, model, CachePath);
}

public partial class GatewayChannel :
    GatewayCacheableEntity<GatewayChannel, ulong, IChannelModel, ChannelIdentity>,
    IChannel
{
    public ChannelType Type => (ChannelType)Model.Type;

    [ProxyInterface] internal virtual GatewayChannelActor Actor { get; }

    internal virtual IChannelModel Model => _model;

    private IChannelModel _model;

    public GatewayChannel(
        DiscordGatewayClient client,
        IChannelModel model,
        GatewayChannelActor? actor = null,
        IEntityHandle<ulong, GatewayChannel>? implicitHandle = null
    ) : base(client, model.Id, implicitHandle)
    {
        _model = model;
        Actor = actor ?? new(client, ChannelIdentity.Of(this));
    }

    public static GatewayChannel Construct(
        DiscordGatewayClient client,
        ICacheConstructionContext<ulong, GatewayChannel> context,
        IChannelModel model)
    {
        // TODO: switch channel model type
        switch (model)
        {
            case IDMChannelModel dmChannelModel
                when context is ICacheConstructionContext<ulong, GatewayDMChannel> dmChannelContext:
                return GatewayDMChannel.Construct(client, dmChannelContext, dmChannelModel);
            case IGroupDMChannelModel groupDMChannelModel:
                throw new NotImplementedException();
            case IGuildChannelModel guildChannelModel
                when context is ICacheConstructionContext<ulong, GatewayGuildChannel> guildChannelContext:
                return GatewayGuildChannel.Construct(client, guildChannelContext, guildChannelModel);
            default:
                return new GatewayChannel(
                    client,
                    model,
                    context.TryGetActor(Template.T<GatewayChannelActor>()),
                    implicitHandle: context.ImplicitHandle);
        }
    }

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
