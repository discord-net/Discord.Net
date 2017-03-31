namespace Discord
{
    //TODO: Add webhook endpoints
    public interface IWebhookUser : IGuildUser
    {
        ulong WebhookId { get; }
    }
}
