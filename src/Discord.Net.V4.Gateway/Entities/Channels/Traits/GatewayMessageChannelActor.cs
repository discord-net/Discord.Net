using Discord.Models;

namespace Discord.Gateway;

public sealed partial class GatewayMessageChannelTrait<TChannelActor, TChannel, TIdentity>(
    DiscordGatewayClient client,
    TChannelActor channel,
    TIdentity identity
) :
    GatewayMessageChannelTrait(client, channel, identity),
    IGatewayTrait<ulong, TChannel, TIdentity>
    where TChannelActor : GatewayChannelActor
    where TChannel : GatewayChannel, IMessageChannel
    where TIdentity : class, MessageChannelIdentity
{
    internal override GatewayChannelActor Channel { get; } = channel;

    [SourceOfTruth] internal override TIdentity Identity { get; } = identity;
}

public partial class GatewayMessageChannelTrait :
    GatewayTrait<ulong, GatewayChannel, MessageChannelIdentity>,
    IMessageChannelTrait,
    IGatewayCachedActor<ulong, GatewayChannel, MessageChannelIdentity, IChannelModel>
{
    public IPagedIndexableLink<IMessageActor, ulong, IMessage, PageChannelMessagesParams> Messages =>
        throw new NotImplementedException();

    [ProxyInterface]
    internal virtual GatewayChannelActor Channel { get; }

    internal override MessageChannelIdentity Identity { get; }

    public GatewayMessageChannelTrait(
        DiscordGatewayClient client,
        GatewayChannelActor channel,
        MessageChannelIdentity identity
    ) : base(client, identity)
    {
        Identity = identity | this;
        Channel = channel;
    }

    [SourceOfTruth]
    internal IMessageChannel CreateEntity(IChannelModel model)
        => (IMessageChannel)Channel.CreateEntity(model);
}

// [ExtendInterfaceDefaults]
// public sealed partial class GatewayMessageChannelTrait(
//     DiscordGatewayClient client,
//     MessageChannelIdentity channel,
//     GuildIdentity? guild = null
// ) :
//     GatewayChannelActor(client, channel.Cast<GatewayChannel, GatewayChannelActor, IChannelModel>()),
//     IMessageChannelTrait
// {
//     internal new MessageChannelIdentity Identity { get; } = channel;
//
//     public IIndexableActor<IMessageActor, ulong, IMessage> Messages => throw new NotImplementedException();
//
//     IMessageChannel IEntityProvider<IMessageChannel, IChannelModel>.CreateEntity(IChannelModel model)
//         => (IMessageChannel)CreateEntity(model);
//
//     IMessageChannel IMessageChannelTrait.CreateEntity(IChannelModel model)
//         => (IMessageChannel)CreateEntity(model);
// }
