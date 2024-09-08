namespace Discord;

[Trait]
public partial interface IChannelFollowerIntegrationChannelTrait :
    IIntegrationChannelTrait
{
    [return: TypeHeuristic(nameof(ChannelFollowerWebhooks))]
    IChannelFollowerWebhookActor ChannelFollowerWebhook(ulong id) => ChannelFollowerWebhooks[id];
    IChannelFollowerWebhookActor.Enumerable.Indexable ChannelFollowerWebhooks { get; }
}
