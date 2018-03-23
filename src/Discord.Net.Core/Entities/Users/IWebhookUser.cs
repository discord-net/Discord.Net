namespace Discord
{
    /// <summary> Represents a Webhook Discord user. </summary>
    public interface IWebhookUser : IGuildUser
    {
        ulong WebhookId { get; }
    }
}
