namespace Discord
{
    /// <summary> Represents a Webhook Discord user. </summary>
    public interface IWebhookUser : IGuildUser
    {
        /// <summary> Gets the ID of a webhook. </summary>
        ulong WebhookId { get; }
    }
}
