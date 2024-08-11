using Discord.Models;

namespace Discord.Gateway;

public sealed partial class GatewayIncomingWebhookActor :
    GatewayWebhookActor,
    IIncomingWebhookActor,
    IGatewayCachedActor<ulong, GatewayIncomingWebhook, IncomingWebhookIdentity, IWebhookModel>
{
    [SourceOfTruth]
    public GatewayGuildActor Guild { get; }

    public GatewayIntegrationChannelTrait Channel { get; }

    [SourceOfTruth] internal override IncomingWebhookIdentity Identity { get; }

    public GatewayIncomingWebhookActor(
        DiscordGatewayClient client,
        GuildIdentity guild,
        IntegrationChannelIdentity channel,
        IncomingWebhookIdentity webhook
    ) : base(client, webhook)
    {
        Identity = webhook | this;

        Guild = client.Guilds[guild];
        Channel = Guild.
    }

    public IIncomingWebhook CreateEntity(IWebhookModel model) => throw new NotImplementedException();
}

public sealed partial class GatewayIncomingWebhook :
    GatewayWebhook,
    IIncomingWebhook,
    ICacheableEntity<GatewayIncomingWebhook, ulong, IWebhookModel>
{
}
