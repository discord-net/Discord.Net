using Discord.Models;

namespace Discord.Gateway;

[ExtendInterfaceDefaults]
public sealed partial class GatewayMessageChannelTrait(
    DiscordGatewayClient client,
    MessageChannelIdentity channel,
    GuildIdentity? guild = null
) :
    GatewayChannelActor(client, channel.Cast<GatewayChannel, GatewayChannelActor, IChannelModel>()),
    IMessageChannelTrait
{
    internal new MessageChannelIdentity Identity { get; } = channel;

    public IIndexableActor<IMessageActor, ulong, IMessage> Messages => throw new NotImplementedException();

    IMessageChannel IEntityProvider<IMessageChannel, IChannelModel>.CreateEntity(IChannelModel model)
        => (IMessageChannel)CreateEntity(model);

    IMessageChannel IMessageChannelTrait.CreateEntity(IChannelModel model)
        => (IMessageChannel)CreateEntity(model);
}
