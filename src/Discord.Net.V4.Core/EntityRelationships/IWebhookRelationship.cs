namespace Discord;

public interface IWebhookRelationship :
    IRelationship<IWebhookActor, ulong, IWebhook>
{
    IWebhookActor Webhook { get; }

    IWebhookActor IRelationship<IWebhookActor, ulong, IWebhook>.RelationshipActor => Webhook;
}
