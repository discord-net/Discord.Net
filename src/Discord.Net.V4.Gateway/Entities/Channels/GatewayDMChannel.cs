using Discord.Gateway;
using Discord.Gateway.State;
using Discord.Models;
using static Discord.Template;

namespace Discord.Gateway;

public sealed partial class GatewayDMChannelActor(
    DiscordGatewayClient client,
    DMChannelIdentity channel,
    UserIdentity recipient
) :
    GatewayChannelActor(client, channel),
    IDMChannelActor,
    IGatewayCachedActor<ulong, GatewayDMChannel, DMChannelIdentity, IDMChannelModel>
{
    [SourceOfTruth] public GatewayUserActor Recipient { get; } = new(client, recipient);

    [SourceOfTruth] internal override DMChannelIdentity Identity { get; } = channel;

    [ProxyInterface(typeof(IMessageChannelActor))]
    internal GatewayMessageChannelActor MessageChannelActor { get; } = new(client, channel);

    [SourceOfTruth]
    internal GatewayDMChannel CreateEntity(IDMChannelModel model)
        => Client.StateController.CreateLatent(this, model);
}

public sealed partial class GatewayDMChannel :
    GatewayChannel,
    IDMChannel,
    ICacheableEntity<GatewayDMChannel, ulong, IDMChannelModel>
{
    [SourceOfTruth] public GatewayUserActor Recipient { get; private set; }

    [ProxyInterface] internal override GatewayDMChannelActor Actor { get; }

    internal override IDMChannelModel Model => _model;

    private IDMChannelModel _model;

    internal GatewayDMChannel(
        DiscordGatewayClient client,
        IDMChannelModel model,
        UserIdentity recipient,
        GatewayDMChannelActor? actor = null,
        IEntityHandle<ulong, GatewayChannel>? implicitHandle = null
    ) : base(client, model, actor, implicitHandle)
    {
        _model = model;

        Recipient = recipient.Actor ?? new(client, recipient);

        Actor = actor ?? new(client, DMChannelIdentity.Of(this), UserIdentity.Of(Recipient));
    }

    public static GatewayDMChannel Construct(
        DiscordGatewayClient client,
        ICacheConstructionContext<ulong, GatewayDMChannel> context,
        IDMChannelModel model
    ) => new(
        client,
        model,
        context.Path.GetIdentity(T<UserIdentity>(), model.RecipientId),
        implicitHandle: context.ImplicitHandle
    );

    [CovariantOverride]
    public ValueTask UpdateAsync(IDMChannelModel model, bool updateCache = true, CancellationToken token = default)
    {
        if (updateCache) return UpdateCacheAsync(this, model, token);

        _model = model;

        return base.UpdateAsync(model, false, token);
    }


    public override IDMChannelModel GetModel() => Model;
}
