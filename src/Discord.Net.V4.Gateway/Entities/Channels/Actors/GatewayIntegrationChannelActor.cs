using Discord.Models;

namespace Discord.Gateway;

public sealed partial class GatewayIntegrationChannelTrait :
    GatewayGuildChannelActor,
    IIntegrationChannelTrait
{
    public IEnumerableIndexableActor<IWebhookActor, ulong, IWebhook> Webhooks => throw new NotImplementedException();

    public IEnumerableIndexableActor<IIncomingWebhookActor, ulong, IIncomingWebhook> IncomingWebhooks => throw new NotImplementedException();

    public IEnumerableIndexableActor<IChannelFollowerWebhookActor, ulong, IChannelFollowerWebhook> ChannelFollowerWebhooks => throw new NotImplementedException();

    internal new IntegrationChannelIdentity Identity { get; }

    public GatewayIntegrationChannelTrait(
        DiscordGatewayClient client,
        GuildIdentity guild,
        IntegrationChannelIdentity channel
    ) : base(client, guild, channel.Cast<GatewayGuildChannel, GatewayGuildChannelActor, IGuildChannelModel>())
    {
        Identity = channel;

    }
}
