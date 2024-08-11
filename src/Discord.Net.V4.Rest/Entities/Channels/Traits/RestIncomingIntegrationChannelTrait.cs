using Discord.Models;

namespace Discord.Rest;

using EnumerableIncomingWebhookActor =
    RestEnumerableIndexableActor<RestIncomingWebhookActor, ulong, RestIncomingWebhook, IIncomingWebhook,
        IEnumerable<IWebhookModel>>;

public sealed partial class RestIncomingIntegrationChannelTrait<TChannelActor, TChannel, TIdentity>(
    DiscordRestClient client,
    TChannelActor channel,
    TIdentity identity
) :
    RestIncomingIntegrationChannelTrait(client, channel, identity)
    where TChannelActor :
    RestGuildChannelActor,
    IRestActor<ulong, TChannel, TIdentity>,
    IIntegrationChannelTrait
    where TChannel : RestGuildChannel, IIntegrationChannel
    where TIdentity : class, IncomingIntegrationChannelIdentity
{
    [ProxyInterface] internal override TChannelActor Channel { get; } = channel;

    internal override TIdentity Identity { get; } = identity;
}

public partial class RestIncomingIntegrationChannelTrait :
    RestIntegrationChannelTrait,
    IIncomingIntegrationChannelTrait
{
    [SourceOfTruth] public EnumerableIncomingWebhookActor IncomingWebhooks { get; }

    internal override IncomingIntegrationChannelIdentity Identity { get; }

    public RestIncomingIntegrationChannelTrait(
        DiscordRestClient client,
        RestGuildChannelActor channel,
        IncomingIntegrationChannelIdentity identity
    ) : base(client, channel, identity)
    {
        Identity = identity | this;

        IncomingWebhooks = RestActors.Fetchable(
            Template.T<RestIncomingWebhookActor>(),
            client,
            RestIncomingWebhookActor.Factory,
            channel.Guild.Identity,
            identity,
            entityFactory: RestIncomingWebhook.Construct,
            new RestIncomingWebhook.Context(channel.Guild.Identity, identity),
            IWebhook.GetChannelWebhooksRoute(this)
        );
    }
}
