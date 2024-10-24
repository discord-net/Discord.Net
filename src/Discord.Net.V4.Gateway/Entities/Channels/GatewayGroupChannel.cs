using Discord.Gateway.State;
using Discord.Models;
using static Discord.Template;

namespace Discord.Gateway;

using MessageChannelTrait =
    GatewayMessageChannelTrait<GatewayGroupChannelActor, GatewayGroupChannel, GroupChannelIdentity>;

[ExtendInterfaceDefaults]
public sealed partial class GatewayGroupChannelActor :
    GatewayChannelActor,
    IGroupChannelActor,
    IGatewayCachedActor<ulong, GatewayGroupChannel, GroupChannelIdentity, IGroupDMChannelModel>
{
    [SourceOfTruth] internal override GroupChannelIdentity Identity { get; }

    [ProxyInterface(typeof(IMessageChannelTrait))]
    internal MessageChannelTrait MessageChannelTrait { get; }

    public GatewayGroupChannelActor(
        DiscordGatewayClient client,
        GroupChannelIdentity channel
    ) : base(client, channel)
    {
        Identity = channel | this;
        MessageChannelTrait = new(client, this, channel);
    }

    [SourceOfTruth]
    internal GatewayGroupChannel CreateEntity(IGroupDMChannelModel model)
        => Client.StateController.CreateLatent(this, model, CachePath);

    [SourceOfTruth]
    internal GatewayInvite CreateEntity(IInviteModel model)
        => Client.StateController.CreateLatent(Client.Invites[model.Id], model, CachePath);
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
        GatewayGroupChannelActor? actor = null
    ) : base(client, model, actor)
    {
        _model = model;
        Actor = actor ?? new(client, GroupChannelIdentity.Of(this));
    }

    public static GatewayGroupChannel Construct(
        DiscordGatewayClient client,
        IGatewayConstructionContext context,
        IGroupDMChannelModel model
    ) => new(
        client,
        model,
        context.TryGetActor<GatewayGroupChannelActor>()
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