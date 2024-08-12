using Discord.Rest;

namespace Discord.Gateway;

public partial class GatewayChannelFollowerIntegrationChannelTrait<TChannelActor, TChannel, TIdentity>(
    DiscordGatewayClient client,
    TChannelActor channel,
    TIdentity identity
) :
    GatewayChannelFollowerIntegrationChannelTrait(client, channel, identity),
    IGatewayTrait<ulong, TChannel, TIdentity>
    where TChannelActor : GatewayGuildChannelActor, IIntegrationChannelTrait
    where TChannel : GatewayGuildChannel, IIntegrationChannel
    where TIdentity : class, ChannelFollowerIntegrationChannelIdentity
{
    internal override TChannelActor Channel { get; } = channel;

    [SourceOfTruth]
    internal override TIdentity Identity { get; } = identity;
}

public partial class GatewayChannelFollowerIntegrationChannelTrait :
    GatewayIntegrationChannelTrait,
    IChannelFollowerIntegrationChannelTrait
{
    [SourceOfTruth] public ChannelFollowerWebhooks ChannelFollowerWebhooks { get; }

    public GatewayChannelFollowerIntegrationChannelTrait(
        DiscordGatewayClient client,
        GatewayGuildChannelActor channel,
        ChannelFollowerIntegrationChannelIdentity identity
    ) : base(client, channel, identity)
    {
        ChannelFollowerWebhooks = new(
            client,
            id => new GatewayChannelFollowerWebhookActor(
                client,
                channel.Guild.Identity,
                identity,
                ChannelFollowerWebhookIdentity.Of(id)
            ),
            model => RestChannelFollowerWebhook.Construct(
                client.Rest,
                new RestChannelFollowerWebhook.Context(
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
