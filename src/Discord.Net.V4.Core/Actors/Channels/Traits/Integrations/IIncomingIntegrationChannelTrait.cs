namespace Discord;

[Trait]
public interface IIncomingIntegrationChannelTrait :
    IIntegrationChannelTrait
{
    [return: TypeHeuristic(nameof(IncomingWebhooks))]
    IIncomingWebhookActor IncomingWebhook(ulong id) => IncomingWebhooks[id];
    IEnumerableIndexableActor<IIncomingWebhookActor, ulong, IIncomingWebhook> IncomingWebhooks { get; }
}
