using Discord.Gateway.State;
using Discord.Models;
using static Discord.Template;

namespace Discord.Gateway;

public sealed partial class GatewayGroupChannelActor(
    DiscordGatewayClient client,
    GroupChannelIdentity channel
) :
    GatewayChannelActor(client, channel),
    IGroupChannelActor,
    IGatewayCachedActor<ulong, GatewayGroupChannel, GroupChannelIdentity, IGroupDMChannelModel>
{
    [SourceOfTruth] internal override GroupChannelIdentity Identity { get; } = channel;

    [ProxyInterface] internal GatewayMessageChannelActor MessageChannelActor { get; } = new(client, channel);

    [SourceOfTruth]
    internal GatewayGroupChannel CreateEntity(IGroupDMChannelModel model)
        => Client.StateController.CreateLatent(this, model);
}

public sealed partial class GatewayGroupChannel :
    GatewayChannel,
    IGroupChannel,
    ICacheableEntity<GatewayGroupChannel, ulong, IGroupDMChannelModel>
{
    public IDefinedLoadableEntityEnumerable<ulong, IUser> Recipients => throw new NotImplementedException();

    [ProxyInterface] internal override GatewayGroupChannelActor Actor { get; }

    internal override IGroupDMChannelModel Model => _model;

    private IGroupDMChannelModel _model;

    public GatewayGroupChannel(
        DiscordGatewayClient client,
        IGroupDMChannelModel model,
        GatewayGroupChannelActor? actor = null,
        IEntityHandle<ulong, GatewayGroupChannel>? implicitHandle = null
    ) : base(client, model, actor, implicitHandle)
    {
        _model = model;
        Actor = actor ?? new(client, GroupChannelIdentity.Of(this));
    }

    public static GatewayGroupChannel Construct(
        DiscordGatewayClient client,
        ICacheConstructionContext<ulong, GatewayGroupChannel> context,
        IGroupDMChannelModel model
    ) => new(
        client,
        model,
        context.TryGetActor(T<GatewayGroupChannelActor>()),
        context.ImplicitHandle
    );

    [CovariantOverride]
    public ValueTask UpdateAsync(IGroupDMChannelModel model, bool updateCache = true, CancellationToken token = default)
    {
        if (updateCache) return UpdateCacheAsync(this, model, token);

        _model = model;

        return base.UpdateAsync(model, false, token);
    }

    public override IGroupDMChannelModel GetModel() => Model;
}
