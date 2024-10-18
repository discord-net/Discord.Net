using Discord.Models;

namespace Discord;

[Trait]
public partial interface IIntegrationChannelTrait :
    IGuildChannelActor,
    IEntityProvider<IIntegrationChannel, IGuildChannelModel>,
    IActorTrait<ulong, IIntegrationChannel>
{
    [return: TypeHeuristic(nameof(Webhooks))]
    IGuildChannelWebhookActor Webhook(ulong id) => Webhooks[id];

    IGuildChannelWebhookActor.Enumerable.Indexable Webhooks { get; }

    [SourceOfTruth]
    internal new IIntegrationChannel CreateEntity(IGuildChannelModel model);
    
    [
        TraitComponent,
        TraitLinkExtends(nameof(Webhooks), typeof(IGuildChannelWebhookActor.WithIncoming))
    ]
    private interface WithIncomingComponent : IIntegrationChannelTrait;

    [
        TraitComponent(Parent = typeof(WithIncomingComponent)),
        TraitLinkExtends(nameof(Webhooks), typeof(IGuildChannelWebhookActor.WithChannelFollower))
    ]
    private interface WithChannelFollowerComponent : IIntegrationChannelTrait;
}