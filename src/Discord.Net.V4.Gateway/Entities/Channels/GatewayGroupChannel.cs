using Discord.Gateway.State;
using Discord.Models;

namespace Discord.Gateway;

public sealed partial class GatewayGroupChannelActor(
    DiscordGatewayClient client,
    GroupChannelIdentity channel
):
    GatewayChannelActor(client, channel),
    IGroupChannelActor,
    IGatewayCachedActor<ulong, GatewayGroupChannel, GroupChannelIdentity, IGroupDMChannelModel>
{

}


public sealed partial class GatewayGroupChannel :
    GatewayChannel,
    IGroupChannel,
    ICacheableEntity<GatewayGroupChannel, ulong, IGroupDMChannelModel>
{
    [ProxyInterface]
    internal override GatewayGroupChannelActor Actor { get; }

    internal override IGroupDMChannelModel Model => _model;

    private IGroupDMChannelModel _model;

    public GatewayGroupChannel(
        DiscordGatewayClient client,
        IChannelModel model,
        GatewayChannelActor? actor = null,
        IEntityHandle<ulong, GatewayChannel>? implicitHandle = null
        ) : base(client, model, actor, implicitHandle)
    {
    }

    public IDefinedLoadableEntityEnumerable<ulong, IUser> Recipients => throw new NotImplementedException();

    public IGroupDMChannelModel GetModel() => throw new NotImplementedException();

    public ValueTask UpdateAsync(IGroupDMChannelModel model, bool updateCache = true, CancellationToken token = default) => throw new NotImplementedException();

    public static GatewayGroupChannel Construct(DiscordGatewayClient client,
        ICacheConstructionContext<ulong, GatewayGroupChannel> context, IGroupDMChannelModel model) =>
        throw new NotImplementedException();
}
