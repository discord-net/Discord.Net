using Discord.Models;
using Discord.Rest;

namespace Discord.Gateway;

public partial class GatewayIntegrationChannelTrait<TChannelActor, TChannel, TIdentity>(
    DiscordGatewayClient client,
    TChannelActor channel,
    TIdentity identity
) :
    GatewayIntegrationChannelTrait(client, channel, identity),
    IGatewayTrait<ulong, TChannel, TIdentity>
    where TChannelActor : GatewayGuildChannelActor, IIntegrationChannelTrait
    where TChannel : GatewayGuildChannel, IIntegrationChannel
    where TIdentity : class, IntegrationChannelIdentity
{
    internal override TChannelActor Channel { get; } = channel;
    [SourceOfTruth] internal override TIdentity Identity { get; } = identity;
}

public partial class GatewayIntegrationChannelTrait :
    GatewayTrait<ulong, GatewayGuildChannel, IntegrationChannelIdentity>,
    IIntegrationChannelTrait,
    IGatewayCachedActor<ulong, GatewayGuildChannel, IntegrationChannelIdentity, IGuildChannelModel>,
    IActorTrait<ulong, IIntegrationChannel>
{
    [SourceOfTruth] public Webhooks Webhooks { get; }

    [ProxyInterface] internal virtual GatewayGuildChannelActor Channel { get; }

    internal override IntegrationChannelIdentity Identity { get; }

    [TypeFactory]
    public GatewayIntegrationChannelTrait(
        DiscordGatewayClient client,
        GatewayGuildChannelActor channel,
        IntegrationChannelIdentity identity
    ) : base(client, identity)
    {
        Identity = identity | this;
        Channel = channel;

        Webhooks = new(
            client,
            client.Webhooks,
            model => RestWebhook.Construct(client.Rest, model),
            channel.CachePath,
            IWebhook.GetChannelWebhooksRoute(this)
        );
    }
}

// public sealed partial class GatewayIntegrationChannelTrait :
//     GatewayGuildChannelActor,
//     IIntegrationChannelTrait
// {
//     public IEnumerableIndexableActor<IWebhookActor, ulong, IWebhook> Webhooks => throw new NotImplementedException();
//
//     public IEnumerableIndexableActor<IIncomingWebhookActor, ulong, IIncomingWebhook> IncomingWebhooks => throw new NotImplementedException();
//
//     public IEnumerableIndexableActor<IChannelFollowerWebhookActor, ulong, IChannelFollowerWebhook> ChannelFollowerWebhooks => throw new NotImplementedException();
//
//     internal new IntegrationChannelIdentity Identity { get; }
//
//     public GatewayIntegrationChannelTrait(
//         DiscordGatewayClient client,
//         GuildIdentity guild,
//         IntegrationChannelIdentity channel
//     ) : base(client, guild, channel.Cast<GatewayGuildChannel, GatewayGuildChannelActor, IGuildChannelModel>())
//     {
//         Identity = channel;
//
//     }
// }
