using Discord.Models;

namespace Discord.Rest;

[Containerized, Trait]
public partial interface IRestIntegrationChannelTrait :
    IRestTraitProvider<RestGuildChannelActor>,
    IIntegrationChannelTrait
{
    [SourceOfTruth]
    new RestGuildChannelWebhookActor.Enumerable.Indexable Webhooks
        => GetOrCreateTraitData(nameof(Webhooks), channel =>
            new RestGuildChannelWebhookActor.Enumerable.Indexable(
                channel.Client,
                RestActorProvider.GetOrCreate(
                    channel.Client,
                    Template.Of<GuildChannelWebhookIdentity>(),
                    channel.Guild.Identity,
                    (IntegrationChannelIdentity) channel.Identity
                ),
                Routes.GetChannelWebhooks(channel.Id)
                    .AsRequiredProvider()
            )
        );

    private RestGuildChannelWebhookActor.Enumerable.Indexable.WithIncoming Incoming
        => GetOrCreateTraitData(nameof(Webhooks), channel =>
            new RestGuildChannelWebhookActor.Enumerable.Indexable.WithIncoming(
                channel.Client,
                RestActorProvider.GetOrCreate(
                    channel.Client,
                    Template.Of<GuildChannelWebhookIdentity>(),
                    channel.Guild.Identity,
                    (IntegrationChannelIdentity) channel.Identity
                ),
                Routes.GetChannelWebhooks(channel.Id)
                    .AsRequiredProvider()
            )
        );

    [
        TraitComponent,
        TraitLinkExtends(
            nameof(Webhooks),
            typeof(IGuildChannelWebhookActor.WithIncoming),
            Getter = nameof(Incoming)
        )
    ]
    private interface WithIncomingComponent : IIntegrationChannelTrait;
}