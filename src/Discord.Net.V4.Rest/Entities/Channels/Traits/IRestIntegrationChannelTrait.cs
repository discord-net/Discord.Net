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


    [Containerized, Trait]
    public partial interface WithIncoming : IRestIntegrationChannelTrait, IIntegrationChannelTrait.WithIncoming
    {
        new RestGuildChannelWebhookActor.Enumerable.Indexable.WithIncoming Webhooks
            => GetOrCreateTraitData(nameof(Webhooks), channel =>
                new RestGuildChannelWebhookActor.Enumerable.Indexable.WithIncoming(
                    channel.Client,
                    Webhooks,
                    Webhooks.EnumerableLink.EnumerableProvider,
                    new(
                        channel.Client,
                        RestActorProvider.GetOrCreate(
                            channel.Client,
                            Template.Of<IncomingWebhookIdentity>(),
                            channel.Guild.Identity,
                            (IncomingIntegrationChannelIdentity) channel.Identity
                        ),
                        Routes.GetChannelWebhooks(channel.Id)
                            .AsRequiredProvider()
                            .Map(v => v.OfType<IIncomingWebhookModel>())
                    )
                ));

        RestGuildChannelWebhookActor.Enumerable.Indexable IRestIntegrationChannelTrait.
            Webhooks => Webhooks;

        IGuildChannelWebhookActor.Enumerable.Indexable IIntegrationChannelTrait.Webhooks => Webhooks;
        IGuildChannelWebhookActor.Enumerable.Indexable.WithIncoming IIntegrationChannelTrait.WithIncoming.Webhooks => Webhooks;

        [Containerized, Trait]
        public partial interface WithChannelFollower : WithIncoming, IIntegrationChannelTrait.WithIncoming.WithChannelFollower
        {
            new RestGuildChannelWebhookActor.Enumerable.Indexable.WithIncoming.WithChannelFollower Webhooks
                => GetOrCreateTraitData(nameof(Webhooks), channel =>
                    new RestGuildChannelWebhookActor.Enumerable.Indexable.WithIncoming.WithChannelFollower(
                        channel.Client,
                        Webhooks,
                        Webhooks.EnumerableLink.EnumerableProvider,
                        new(
                            channel.Client,
                            RestActorProvider.GetOrCreate(
                                channel.Client,
                                Template.Of<IncomingWebhookIdentity>(),
                                channel.Guild.Identity,
                                (IncomingIntegrationChannelIdentity) channel.Identity
                            ),
                            Routes.GetChannelWebhooks(channel.Id)
                                .AsRequiredProvider()
                                .Map(v => v.OfType<IIncomingWebhookModel>())
                        ),
                        new(
                            channel.Client,
                            RestActorProvider.GetOrCreate(
                                channel.Client,
                                Template.Of<ChannelFollowerWebhookIdentity>(),
                                channel.Guild.Identity,
                                (ChannelFollowerIntegrationChannelIdentity) channel.Identity
                            ),
                            Routes.GetChannelWebhooks(channel.Id)
                                .AsRequiredProvider()
                                .Map(v => v.OfType<IChannelFollowerWebhookModel>())
                        )
                    )
                );

            RestGuildChannelWebhookActor.Enumerable.Indexable.WithIncoming WithIncoming.Webhooks => Webhooks;

            IGuildChannelWebhookActor.Enumerable.Indexable IIntegrationChannelTrait.Webhooks => Webhooks;
            IGuildChannelWebhookActor.Enumerable.Indexable.WithIncoming.WithChannelFollower 
                IIntegrationChannelTrait.WithIncoming.WithChannelFollower.Webhooks => Webhooks;
            IGuildChannelWebhookActor.Enumerable.Indexable.WithIncoming 
                IIntegrationChannelTrait.WithIncoming.Webhooks => Webhooks;
        }
    }

    [Containerized, Trait]
    public partial interface WithChannelFollower : IRestIntegrationChannelTrait, IIntegrationChannelTrait.WithChannelFollower
    {
        new RestGuildChannelWebhookActor.Enumerable.Indexable.WithChannelFollower Webhooks
            => GetOrCreateTraitData(nameof(Webhooks), channel =>
                new RestGuildChannelWebhookActor.Enumerable.Indexable.WithChannelFollower(
                    channel.Client,
                    Webhooks,
                    Webhooks.EnumerableLink.EnumerableProvider,
                    new(
                        channel.Client,
                        RestActorProvider.GetOrCreate(
                            channel.Client,
                            Template.Of<ChannelFollowerWebhookIdentity>(),
                            channel.Guild.Identity,
                            (ChannelFollowerIntegrationChannelIdentity) channel.Identity
                        ),
                        Routes.GetChannelWebhooks(channel.Id)
                            .AsRequiredProvider()
                            .Map(v => v.OfType<IChannelFollowerWebhookModel>())
                    )
                )
            );

        RestGuildChannelWebhookActor.Enumerable.Indexable IRestIntegrationChannelTrait.
            Webhooks => Webhooks;

        IGuildChannelWebhookActor.Enumerable.Indexable IIntegrationChannelTrait.Webhooks => Webhooks;
        IGuildChannelWebhookActor.Enumerable.Indexable.WithChannelFollower 
            IIntegrationChannelTrait.WithChannelFollower.Webhooks => Webhooks;
    }
}