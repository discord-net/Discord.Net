using Discord.Gateway;
using Discord.Gateway.State;
using Discord.Models;

namespace Discord.Gateway;

[ExtendInterfaceDefaults]
public partial class GatewayChannelActor :
    GatewayCachedActor<ulong, GatewayChannel, ChannelIdentity, IChannelModel>,
    IChannelActor
{
    internal override ChannelIdentity Identity { get; }

    public GatewayChannelActor(
        DiscordGatewayClient client,
        ChannelIdentity channel
    ) : base(client, channel)
    {
        Identity = channel | this;
    }

    [SourceOfTruth]
    internal GatewayChannel CreateEntity(IChannelModel model)
        => Client.StateController.CreateLatent(this, model, CachePath);
}

public partial class GatewayChannel :
    GatewayCacheableEntity<GatewayChannel, ulong, IChannelModel>,
    IChannel
{
    public ChannelType Type => (ChannelType)Model.Type;

    [ProxyInterface] internal virtual GatewayChannelActor Actor { get; }

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

    public static GatewayChannel Construct(
        DiscordGatewayClient client,
        IGatewayConstructionContext context,
        IChannelModel model)
    {
        return model switch
        {
            IDMChannelModel dmChannelModel
                => GatewayDMChannel.Construct(client, context, dmChannelModel),
            IGroupDMChannelModel groupDMChannelModel
                => GatewayGroupChannel.Construct(client, context, groupDMChannelModel),
            IGuildChannelModel guildChannelModel
                => GatewayGuildChannel.Construct(client, context, guildChannelModel),
            _ => new GatewayChannel(client, model, context.TryGetActor<GatewayChannelActor>())
        };
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
