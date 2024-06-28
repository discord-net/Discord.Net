namespace Discord;

public interface IWebhookRelationship : IRelationship<ulong, IWebhook, ILoadableWebhookActor>
{
    ILoadableWebhookActor Webhook { get; }

    ILoadableWebhookActor IRelationship<ulong, IWebhook, ILoadableWebhookActor>.RelationshipLoadable => Webhook;
}
