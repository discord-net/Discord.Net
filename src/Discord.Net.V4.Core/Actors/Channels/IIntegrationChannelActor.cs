namespace Discord;

public interface IIntegrationChannelActor :
    IGuildChannelActor,
    IActor<ulong, IIntegrationChannel>
{
    [return: TypeHeuristic(nameof(Webhooks))]
    IWebhookActor Webhook(ulong id) => Webhooks[id];
    IEnumerableIndexableActor<IWebhookActor, ulong, IWebhook> Webhooks { get; }

    [return: TypeHeuristic(nameof(IncomingWebhooks))]
    IIncomingWebhookActor IncomingWebhook(ulong id) => IncomingWebhooks[id];
    IEnumerableIndexableActor<IIncomingWebhookActor, ulong, IIncomingWebhook> IncomingWebhooks { get; }

    [return: TypeHeuristic(nameof(ChannelFollowerWebhooks))]
    IChannelFollowerWebhookActor FollowerWebhook(ulong id) => ChannelFollowerWebhooks[id];
    IEnumerableIndexableActor<IChannelFollowerWebhookActor, ulong, IChannelFollowerWebhook> ChannelFollowerWebhooks { get; }
}
