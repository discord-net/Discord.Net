using Discord.Models;

namespace Discord.Gateway;

[ExtendInterfaceDefaults]
public sealed partial class GatewayMessageChannelActor(
    DiscordGatewayClient client,
    MessageChannelIdentity channel,
    GuildIdentity? guild = null
) :
    GatewayChannelActor(client, channel.Cast<GatewayChannel, GatewayChannelActor, IChannelModel>()),
    IMessageChannelActor
{
    internal MessageChannelIdentity MessageChannelIdentity { get; } = channel;

    public IIndexableActor<IMessageActor, ulong, IMessage> Messages => throw new NotImplementedException();

    IMessageChannel IEntityProvider<IMessageChannel, IChannelModel>.CreateEntity(IChannelModel model)
        => (IMessageChannel)CreateEntity(model);

    IMessageChannel IMessageChannelActor.CreateEntity(IChannelModel model)
        => (IMessageChannel)CreateEntity(model);
}
