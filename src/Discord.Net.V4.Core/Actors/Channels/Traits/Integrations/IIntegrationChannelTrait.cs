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
    GuildChannelWebhookLink.Enumerable.Indexable Webhooks { get; }

    [SourceOfTruth]
    internal new IIntegrationChannel CreateEntity(IGuildChannelModel model);
}
