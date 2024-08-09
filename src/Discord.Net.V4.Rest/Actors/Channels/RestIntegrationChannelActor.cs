using Discord.Models;
using Discord.Rest;

namespace Discord.Rest;

using EnumerableWebhookActor =
    RestEnumerableIndexableActor<RestWebhookActor, ulong, RestWebhook, IWebhook, IEnumerable<IWebhookModel>>;
using EnumerableIncomingWebhookActor =
    RestEnumerableIndexableActor<RestIncomingWebhookActor, ulong, RestIncomingWebhook, IIncomingWebhook,
        IEnumerable<IWebhookModel>>;
using EnumerableChannelFollowerWebhookActor =
    RestEnumerableIndexableActor<RestChannelFollowerWebhookActor, ulong, RestChannelFollowerWebhook,
        IChannelFollowerWebhook, IEnumerable<IWebhookModel>>;

public sealed partial class RestIntegrationChannelActor :
    RestGuildChannelActor,
    IIntegrationChannelActor
{
    internal new IntegrationChannelIdentity Identity { get; }

    public RestIntegrationChannelActor(
        DiscordRestClient client,
        GuildIdentity guild,
        IntegrationChannelIdentity channel
    ) : base(client, guild, channel.Cast<RestGuildChannel, RestGuildChannelActor, IGuildChannelModel>())
    {
        Identity = channel | this;

        Webhooks = RestActors.Fetchable(
            Template.T<RestWebhookActor>(),
            client,
            RestWebhookActor.Factory,
            RestWebhook.Construct,
            IWebhook.GetChannelWebhooksRoute(this)
        );

        IncomingWebhooks = RestActors.Fetchable(
            Template.T<RestIncomingWebhookActor>(),
            client,
            RestIncomingWebhookActor.Factory,
            guild,
            channel,
            entityFactory: RestIncomingWebhook.Construct,
            new RestIncomingWebhook.Context(guild, channel),
            IWebhook.GetChannelWebhooksRoute(this)
        );

        ChannelFollowerWebhooks = RestActors.Fetchable(
            Template.T<RestChannelFollowerWebhookActor>(),
            client,
            RestChannelFollowerWebhookActor.Factory,
            guild,
            channel,
            entityFactory: RestChannelFollowerWebhook.Construct,
            new RestChannelFollowerWebhook.Context(guild, channel),
            IWebhook.GetChannelWebhooksRoute(this)
        );
    }

    [SourceOfTruth] public EnumerableWebhookActor Webhooks { get; }

    [SourceOfTruth] public EnumerableIncomingWebhookActor IncomingWebhooks { get; }

    [SourceOfTruth] public EnumerableChannelFollowerWebhookActor ChannelFollowerWebhooks { get; }
}
