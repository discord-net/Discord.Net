namespace Discord;

[Trait]
public interface IChannelFollowerIntegrationChannelTrait :
    IIntegrationChannelTrait
{
    [return: TypeHeuristic(nameof(ChannelFollowerWebhooks))]
    IChannelFollowerWebhookActor FollowerWebhook(ulong id) => ChannelFollowerWebhooks[id];
    ChannelFollowerWebhookLink.Enumerable.Indexable ChannelFollowerWebhooks { get; }
}
