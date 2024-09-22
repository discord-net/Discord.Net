// namespace Discord;
//
// [Trait]
// public partial interface IChannelFollowerIntegrationChannelTrait :
//     IIntegrationChannelTrait
// {
//     [return: TypeHeuristic(nameof(ChannelFollowerWebhooks))]
//     IChannelFollowerWebhookActor ChannelFollowerWebhook(ulong id) => ChannelFollowerWebhooks[id];
//     IChannelFollowerWebhookActor.Enumerable.Indexable ChannelFollowerWebhooks { get; }
//     
//     IGuildChannelWebhookActor.Enumerable.Indexable.WithIncoming.WithChannelFollower TestWebhooks { get; }
//
//     IGuildChannelWebhookActor.Enumerable.Indexable IIntegrationChannelTrait.Webhooks => TestWebhooks;
// }
