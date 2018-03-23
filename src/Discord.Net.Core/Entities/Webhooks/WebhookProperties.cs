namespace Discord
{
    /// <summary>
    /// Properties used to modify an <see cref="IWebhook"/> with the specified changes.
    /// </summary>
    /// <example>
    /// <code language="c#">
    /// await webhook.ModifyAsync(x =>
    /// {
    ///     x.Name = "Bob";
    ///     x.Avatar = new Image("avatar.jpg");
    /// });
    /// </code>
    /// </example>
    /// <seealso cref="IWebhook"/>
    public class WebhookProperties
    {
        /// <summary>
        /// Gets or sets the default name of the webhook.
        /// </summary>
        public Optional<string> Name { get; set; }
        /// <summary>
        /// Gets or sets the default avatar of the webhook.
        /// </summary>
        public Optional<Image?> Image { get; set; }
        /// <summary>
        /// Gets or sets the channel for this webhook.
        /// </summary>
        /// <remarks>
        /// This field is not used when authenticated with <see cref="TokenType.Webhook"/>.
        /// </remarks>
        public Optional<ITextChannel> Channel { get; set; }
        /// <summary>
        /// Gets or sets the channel ID for this webhook.
        /// </summary>
        /// <remarks>
        /// This field is not used when authenticated with <see cref="TokenType.Webhook"/>.
        /// </remarks>
        public Optional<ulong> ChannelId { get; set; }
    }
}
