using Discord.Models;
using Discord.Rest;

namespace Discord.Rest;

using EnumerableWebhookActor =
    RestEnumerableIndexableLink<RestWebhookActor, ulong, RestWebhook, IWebhook, IEnumerable<IWebhookModel>>;

public partial class RestIntegrationChannelTrait<TChannelActor, TChannel, TIdentity> :
    RestIntegrationChannelTrait
    where TChannelActor :
    RestGuildChannelActor,
    IRestActor<ulong, TChannel, TIdentity>,
    IIntegrationChannelTrait
    where TChannel : RestGuildChannel, IIntegrationChannel
    where TIdentity : class, IntegrationChannelIdentity
{
    [ProxyInterface] internal override TChannelActor Channel { get; }

    internal override TIdentity Identity { get; }

    public RestIntegrationChannelTrait(
        DiscordRestClient client,
        TChannelActor channel,
        TIdentity identity
    ) : base(client, channel, identity)
    {
        Identity = identity;
        Channel = channel;
    }
}

public partial class RestIntegrationChannelTrait :
    RestTrait<ulong, RestGuildChannel, IntegrationChannelIdentity, IGuildChannelModel>,
    IIntegrationChannelTrait
{
    [SourceOfTruth] public EnumerableWebhookActor Webhooks { get; }

    [ProxyInterface]
    internal virtual RestGuildChannelActor Channel { get; }

    internal override IntegrationChannelIdentity Identity { get; }

    [TypeFactory]
    public RestIntegrationChannelTrait(
        DiscordRestClient client,
        RestGuildChannelActor channel,
        IntegrationChannelIdentity identity
    ) : base(client, identity)
    {
        Channel = channel;
        Identity = identity | this;

        Webhooks = RestActors.Fetchable(
            Template.T<RestWebhookActor>(),
            client,
            RestWebhookActor.Factory,
            RestWebhook.Construct,
            IWebhook.GetChannelWebhooksRoute(this)
        );
    }
}
