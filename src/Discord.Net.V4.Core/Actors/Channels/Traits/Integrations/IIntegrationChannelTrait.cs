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
    EnumerableIndexableGuildChannelWebhookLink Webhooks { get; }

    [SourceOfTruth]
    internal new IIntegrationChannel CreateEntity(IGuildChannelModel model);
}
