using Discord.Models;

namespace Discord.Rest;

public partial interface IRestIncomingIntegrationChannelTrait :
    IRestIntegrationChannelTrait,
    IIncomingIntegrationChannelTrait
{
    [SourceOfTruth]
    new RestIncomingWebhookActor.Enumerable.Indexable IncomingWebhooks
        => GetOrCreateTraitData(nameof(IncomingWebhooks), channel =>
            new RestIncomingWebhookActor.Enumerable.Indexable(
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
        );
}