using Discord.Rest;

namespace Discord.Gateway;

public sealed partial class GatewayIncomingIntegrationChannelTrait<TChannelActor, TChannel, TIdentity>(
    DiscordGatewayClient client,
    TChannelActor channel,
    TIdentity identity
) :
    GatewayIncomingIntegrationChannelTrait(client, channel, identity),
    IGatewayTrait<ulong, TChannel, TIdentity>
    where TChannelActor : GatewayGuildChannelActor, IIntegrationChannelTrait
    where TChannel : GatewayGuildChannel, IIntegrationChannel
    where TIdentity : class, IncomingIntegrationChannelIdentity
{
    internal override TChannelActor Channel { get; } = channel;

    [SourceOfTruth]
    internal override TIdentity Identity { get; } = identity;
}

public partial class GatewayIncomingIntegrationChannelTrait :
    GatewayIntegrationChannelTrait,
    IIncomingIntegrationChannelTrait,
    IGatewayTrait<ulong, GatewayGuildChannel, IncomingIntegrationChannelIdentity>
{
    [SourceOfTruth] public IncomingWebhooks IncomingWebhooks { get; }

    [SourceOfTruth] internal override IncomingIntegrationChannelIdentity Identity { get; }

    public GatewayIncomingIntegrationChannelTrait(
        DiscordGatewayClient client,
        GatewayGuildChannelActor channel,
        IncomingIntegrationChannelIdentity identity
    ) : base(client, channel, identity)
    {
        Identity = identity | this;

        IncomingWebhooks = new(
            client,
            id => new GatewayIncomingWebhookActor(client, channel.Guild.Identity, identity,
                IncomingWebhookIdentity.Of(id)),
            model => RestIncomingWebhook.Construct(
                client.Rest,
                new RestIncomingWebhook.Context(
                    RestGuildIdentity.Of(channel.Guild.Id),
                    identity
                ),
                model
            ),
            channel.CachePath,
            IWebhook.GetChannelWebhooksRoute(this)
        );
    }
}
