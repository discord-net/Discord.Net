namespace Discord.Net.Models
{
    /// <summary>
    /// Declares an enum which represents the webhook type for a <see cref="Webhook"/>.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/webhook#webhook-object-webhook-types"/>
    /// </remarks>
    public enum WebhookType
    {
        /// <summary>
        /// Incoming <see cref="Webhook"/>s can post <see cref="Message"/>s to
        /// <see cref="Channel"/>s with a generated token.
        /// </summary>
        Incoming = 1,

        /// <summary>
        /// Channel Follower <see cref="Webhook"/>s are internal webhooks used with
        /// Channel Following to post new <see cref="Message"/>s into <see cref="Channel"/>s.
        /// </summary>
        ChannelFollower = 2,

        /// <summary>
        /// <see cref="Application"/> <see cref="Webhook"/>s are webhooks used with Interactions.
        /// </summary>
        Application = 3,
    }
}
