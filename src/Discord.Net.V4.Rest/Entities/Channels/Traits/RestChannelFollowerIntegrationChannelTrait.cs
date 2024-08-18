using Discord.Models;

namespace Discord.Rest;

using EnumerableChannelFollowerWebhookActor =
    RestEnumerableIndexableLink<RestChannelFollowerWebhookActor, ulong, RestChannelFollowerWebhook,
        IChannelFollowerWebhook, IEnumerable<IWebhookModel>>;

public sealed partial class RestChannelFollowerIntegrationChannelTrait<TChannelActor, TChannel, TIdentity>(
    DiscordRestClient client,
    TChannelActor channel,
    TIdentity identity
) :
    RestChannelFollowerIntegrationChannelTrait(client, channel, identity)
    where TChannelActor :
    RestGuildChannelActor,
    IRestActor<ulong, TChannel, TIdentity>,
    IIntegrationChannelTrait
    where TChannel : RestGuildChannel, IIntegrationChannel
    where TIdentity : class, ChannelFollowerIntegrationChannelIdentity
{
    [ProxyInterface] internal override TChannelActor Channel { get; } = channel;

    internal override TIdentity Identity { get; } = identity;
}

public partial class RestChannelFollowerIntegrationChannelTrait :
    RestIntegrationChannelTrait,
    IChannelFollowerIntegrationChannelTrait
{
    [SourceOfTruth] public EnumerableChannelFollowerWebhookActor ChannelFollowerWebhooks { get; }

    internal override ChannelFollowerIntegrationChannelIdentity Identity { get; }

    public RestChannelFollowerIntegrationChannelTrait(
        DiscordRestClient client,
        RestGuildChannelActor channel,
        ChannelFollowerIntegrationChannelIdentity identity
    ) : base(client, channel, identity)
    {
        Identity = identity | this;

        ChannelFollowerWebhooks = RestActors.Fetchable(
            Template.T<RestChannelFollowerWebhookActor>(),
            client,
            RestChannelFollowerWebhookActor.Factory,
            channel.Guild.Identity,
            identity,
            entityFactory: RestChannelFollowerWebhook.Construct,
            new RestChannelFollowerWebhook.Context(channel.Guild.Identity, identity),
            IWebhook.GetChannelWebhooksRoute(this)
        );
    }
}
