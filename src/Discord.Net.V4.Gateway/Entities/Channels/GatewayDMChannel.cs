using Discord.Gateway;
using Discord.Gateway.State;
using Discord.Models;
using static Discord.Template;

namespace Discord.Gateway;

using MessageChannelTrait = GatewayMessageChannelTrait<GatewayDMChannelActor, GatewayDMChannel, DMChannelIdentity>;

[ExtendInterfaceDefaults]
public sealed partial class GatewayDMChannelActor :
    GatewayChannelActor,
    IDMChannelActor,
    IGatewayCachedActor<ulong, GatewayDMChannel, DMChannelIdentity, IDMChannelModel>
{
    [SourceOfTruth] public GatewayUserActor Recipient { get; }

    [SourceOfTruth] internal override DMChannelIdentity Identity { get; }

    [ProxyInterface(typeof(IMessageChannelTrait))]
    internal MessageChannelTrait MessageChannelTrait { get; }

    public GatewayDMChannelActor(
        DiscordGatewayClient client,
        DMChannelIdentity channel,
        UserIdentity recipient
    ) : base(client, channel)
    {
        Identity = channel | this;
        Recipient = client.Users >> recipient;
        MessageChannelTrait = new(client, this, channel);
    }

    [SourceOfTruth]
    internal GatewayDMChannel CreateEntity(IDMChannelModel model)
        => Client.StateController.CreateLatent(this, model, CachePath);
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
        GatewayDMChannelActor? actor = null
    ) : base(client, model, actor)
    {
        _model = model;

        Recipient = recipient.Actor ?? new(client, recipient);

        Actor = actor ?? new(client, DMChannelIdentity.Of(this), UserIdentity.Of(Recipient));
    }

    public static GatewayDMChannel Construct(
        DiscordGatewayClient client,
        IGatewayConstructionContext context,
        IDMChannelModel model
    ) => new(
        client,
        model,
        context.Path.GetIdentity(T<UserIdentity>(), model.RecipientId),
        context.TryGetActor<GatewayDMChannelActor>()
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