namespace Discord;

[Trait]
public interface IChannelFollowerIntegrationChannelTrait :
    IIntegrationChannelTrait
{
    [return: TypeHeuristic(nameof(ChannelFollowerWebhooks))]
    IChannelFollowerWebhookActor FollowerWebhook(ulong id) => ChannelFollowerWebhooks[id];
    IEnumerableIndexableActor<IChannelFollowerWebhookActor, ulong, IChannelFollowerWebhook> ChannelFollowerWebhooks { get; }
}
