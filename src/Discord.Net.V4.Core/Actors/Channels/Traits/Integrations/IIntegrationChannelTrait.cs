namespace Discord;

[Trait]
public partial interface IIntegrationChannelTrait :
    IGuildChannelActor,
    IActorTrait<ulong, IIntegrationChannel>
{
    [return: TypeHeuristic(nameof(Webhooks))]
    IWebhookActor Webhook(ulong id) => Webhooks[id];
    IEnumerableIndexableActor<IWebhookActor, ulong, IWebhook> Webhooks { get; }
}
