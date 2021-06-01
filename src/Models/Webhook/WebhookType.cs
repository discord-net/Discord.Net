namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents the webhook type.
    /// </summary>
    public enum WebhookType
    {
        /// <summary>
        ///     Incoming Webhooks can post messages to channels with a generated token.
        /// </summary>
        Incoming = 1,

        /// <summary>
        ///     Channel Follower Webhooks are internal webhooks used with Channel Following to post new messages into channels.
        /// </summary>
        ChannelFollower = 2,

        /// <summary>
        ///     Application webhooks are webhooks used with Interactions.
        /// </summary>
        Application = 3,
    }
}
