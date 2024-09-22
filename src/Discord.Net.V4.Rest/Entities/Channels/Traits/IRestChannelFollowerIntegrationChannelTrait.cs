using Discord.Models;

namespace Discord.Rest;

[Containerized]
public partial interface IRestChannelFollowerIntegrationChannelTrait :
    IChannelFollowerIntegrationChannelTrait,
    IRestIntegrationChannelTrait
{
    [SourceOfTruth]
    new RestChannelFollowerWebhookActor.Enumerable.Indexable ChannelFollowerWebhooks
        => GetOrCreateTraitData(nameof(ChannelFollowerWebhooks), channel =>
            new RestChannelFollowerWebhookActor.Enumerable.Indexable(
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
        );
}