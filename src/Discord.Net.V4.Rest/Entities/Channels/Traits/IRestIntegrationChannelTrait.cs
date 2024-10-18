global using IntegrationChannelIdentity = Discord.IIdentifiable<
    ulong,
    Discord.IIntegrationChannel,
    Discord.Rest.IRestIntegrationChannelTrait,
    Discord.Models.IChannelModel
>;

global using ChannelFollowerIntegrationChannelIdentity = Discord.IIdentifiable<
    ulong,
    Discord.IIntegrationChannel,
    Discord.Rest.IRestIntegrationChannelTrait.WithChannelFollower,
    Discord.Models.IChannelModel
>;

global using IncomingIntegrationChannelIdentity = Discord.IIdentifiable<
    ulong,
    Discord.IIntegrationChannel,
    Discord.Rest.IRestIntegrationChannelTrait.WithIncoming,
    Discord.Models.IChannelModel
>;

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
            RestGuildChannelWebhookActor.Enumerable.Indexable.Create(
                RestGuildChannelWebhookActor.DefaultEnumerableProvider,
                channel.Client,
                RestGuildChannelWebhookActor.GetProvider(channel.Client, channel.Guild.Identity,
                    (IntegrationChannelIdentity) channel.Identity)
            ));

    [Containerized, Trait]
    public new partial interface WithIncoming : IRestIntegrationChannelTrait, IIntegrationChannelTrait.WithIncoming
    {
        [SourceOfTruth]
        new RestGuildChannelWebhookActor.Enumerable.Indexable.WithIncoming Webhooks
            => GetOrCreateTraitData(nameof(Webhooks), channel =>
                RestGuildChannelWebhookActor.Enumerable.Indexable.WithIncoming.Create(
                    RestGuildChannelWebhookActor.DefaultEnumerableProvider,
                    channel.Client,
                    RestGuildChannelWebhookActor.GetProvider(channel.Client, channel.Guild.Identity,
                        (IntegrationChannelIdentity) channel.Identity)
                ));

        [Containerized, Trait]
        public new partial interface WithChannelFollower : WithIncoming,
            IIntegrationChannelTrait.WithIncoming.WithChannelFollower
        {
            [SourceOfTruth]
            new RestGuildChannelWebhookActor.Enumerable.Indexable.WithIncoming.WithChannelFollower Webhooks
                => GetOrCreateTraitData(nameof(Webhooks), channel =>
                    RestGuildChannelWebhookActor.Enumerable.Indexable.WithIncoming.Create(
                        RestGuildChannelWebhookActor.DefaultEnumerableProvider,
                        channel.Client,
                        RestGuildChannelWebhookActor.GetProvider(channel.Client, channel.Guild.Identity,
                            (IntegrationChannelIdentity) channel.Identity)
                    ));
        }
    }

    [Containerized, Trait]
    public new partial interface WithChannelFollower : IRestIntegrationChannelTrait,
        IIntegrationChannelTrait.WithChannelFollower
    {
        [SourceOfTruth]
        new RestGuildChannelWebhookActor.Enumerable.Indexable.WithChannelFollower Webhooks
            => GetOrCreateTraitData(nameof(Webhooks), channel =>
                RestGuildChannelWebhookActor.Enumerable.Indexable.WithIncoming.Create(
                    RestGuildChannelWebhookActor.DefaultEnumerableProvider,
                    channel.Client,
                    RestGuildChannelWebhookActor.GetProvider(channel.Client, channel.Guild.Identity,
                        (IntegrationChannelIdentity) channel.Identity)
                ));
    }

    // [SourceOfTruth]
    // new RestGuildChannelWebhookActor.Enumerable.Indexable Webhooks
    //     => GetOrCreateTraitData(nameof(Webhooks), channel =>
    //         RestGuildChannelWebhookActor.Enumerable.Indexable.Create(
    //             channel.Client,
    //             RestActorProvider.GetOrCreate(
    //                 channel.Client,
    //                 Template.Of<GuildChannelWebhookIdentity>(),
    //                 channel.Guild.Identity,
    //                 (IntegrationChannelIdentity) channel.Identity
    //             ),
    //             Routes.GetChannelWebhooks(channel.Id)
    //                 .AsRequiredProvider()
    //         )
    //     );
    //
    //
    // [Containerized, Trait]
    // public partial interface WithIncoming : IRestIntegrationChannelTrait, IIntegrationChannelTrait.WithIncoming
    // {
    //     new RestGuildChannelWebhookActor.Enumerable.Indexable.WithIncoming Webhooks
    //         => GetOrCreateTraitData(nameof(Webhooks), channel =>
    //             new RestGuildChannelWebhookActor.Enumerable.Indexable.WithIncoming(
    //                 channel.Client,
    //                 Webhooks,
    //                 Webhooks.EnumerableLink.EnumerableProvider,
    //                 new(
    //                     channel.Client,
    //                     RestActorProvider.GetOrCreate(
    //                         channel.Client,
    //                         Template.Of<IncomingWebhookIdentity>(),
    //                         channel.Guild.Identity,
    //                         (IncomingIntegrationChannelIdentity) channel.Identity
    //                     ),
    //                     Routes.GetChannelWebhooks(channel.Id)
    //                         .AsRequiredProvider()
    //                         .Map(v => v.OfType<IIncomingWebhookModel>())
    //                 )
    //             ));
    //
    //     RestGuildChannelWebhookActor.Enumerable.Indexable IRestIntegrationChannelTrait.
    //         Webhooks => Webhooks;
    //
    //     IGuildChannelWebhookActor.Enumerable.Indexable IIntegrationChannelTrait.Webhooks => Webhooks;
    //     IGuildChannelWebhookActor.Enumerable.Indexable.WithIncoming IIntegrationChannelTrait.WithIncoming.Webhooks => Webhooks;
    //
    //     [Containerized, Trait]
    //     public partial interface WithChannelFollower : WithIncoming, IIntegrationChannelTrait.WithIncoming.WithChannelFollower
    //     {
    //         new RestGuildChannelWebhookActor.Enumerable.Indexable.WithIncoming.WithChannelFollower Webhooks
    //             => GetOrCreateTraitData(nameof(Webhooks), channel =>
    //                 new RestGuildChannelWebhookActor.Enumerable.Indexable.WithIncoming.WithChannelFollower(
    //                     channel.Client,
    //                     Webhooks,
    //                     Webhooks.EnumerableLink.EnumerableProvider,
    //                     new(
    //                         channel.Client,
    //                         RestActorProvider.GetOrCreate(
    //                             channel.Client,
    //                             Template.Of<IncomingWebhookIdentity>(),
    //                             channel.Guild.Identity,
    //                             (IncomingIntegrationChannelIdentity) channel.Identity
    //                         ),
    //                         Routes.GetChannelWebhooks(channel.Id)
    //                             .AsRequiredProvider()
    //                             .Map(v => v.OfType<IIncomingWebhookModel>())
    //                     ),
    //                     new(
    //                         channel.Client,
    //                         RestActorProvider.GetOrCreate(
    //                             channel.Client,
    //                             Template.Of<ChannelFollowerWebhookIdentity>(),
    //                             channel.Guild.Identity,
    //                             (ChannelFollowerIntegrationChannelIdentity) channel.Identity
    //                         ),
    //                         Routes.GetChannelWebhooks(channel.Id)
    //                             .AsRequiredProvider()
    //                             .Map(v => v.OfType<IChannelFollowerWebhookModel>())
    //                     )
    //                 )
    //             );
    //
    //         RestGuildChannelWebhookActor.Enumerable.Indexable.WithIncoming WithIncoming.Webhooks => Webhooks;
    //
    //         IGuildChannelWebhookActor.Enumerable.Indexable IIntegrationChannelTrait.Webhooks => Webhooks;
    //         IGuildChannelWebhookActor.Enumerable.Indexable.WithIncoming.WithChannelFollower 
    //             IIntegrationChannelTrait.WithIncoming.WithChannelFollower.Webhooks => Webhooks;
    //         IGuildChannelWebhookActor.Enumerable.Indexable.WithIncoming 
    //             IIntegrationChannelTrait.WithIncoming.Webhooks => Webhooks;
    //     }
    // }
    //
    // [Containerized, Trait]
    // public partial interface WithChannelFollower : IRestIntegrationChannelTrait, IIntegrationChannelTrait.WithChannelFollower
    // {
    //     new RestGuildChannelWebhookActor.Enumerable.Indexable.WithChannelFollower Webhooks
    //         => GetOrCreateTraitData(nameof(Webhooks), channel =>
    //             new RestGuildChannelWebhookActor.Enumerable.Indexable.WithChannelFollower(
    //                 channel.Client,
    //                 Webhooks,
    //                 Webhooks.EnumerableLink.EnumerableProvider,
    //                 new(
    //                     channel.Client,
    //                     RestActorProvider.GetOrCreate(
    //                         channel.Client,
    //                         Template.Of<ChannelFollowerWebhookIdentity>(),
    //                         channel.Guild.Identity,
    //                         (ChannelFollowerIntegrationChannelIdentity) channel.Identity
    //                     ),
    //                     Routes.GetChannelWebhooks(channel.Id)
    //                         .AsRequiredProvider()
    //                         .Map(v => v.OfType<IChannelFollowerWebhookModel>())
    //                 )
    //             )
    //         );
    //
    //     RestGuildChannelWebhookActor.Enumerable.Indexable IRestIntegrationChannelTrait.
    //         Webhooks => Webhooks;
    //
    //     IGuildChannelWebhookActor.Enumerable.Indexable IIntegrationChannelTrait.Webhooks => Webhooks;
    //     IGuildChannelWebhookActor.Enumerable.Indexable.WithChannelFollower 
    //         IIntegrationChannelTrait.WithChannelFollower.Webhooks => Webhooks;
    // }
}