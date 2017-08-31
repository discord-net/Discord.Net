namespace Discord
{
    public interface IWebhookUser : IGuildUser
    {
        ulong WebhookId { get; }
    }
}
