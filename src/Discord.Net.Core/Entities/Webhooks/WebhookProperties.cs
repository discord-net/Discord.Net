namespace Discord
{
    /// <summary>
    /// Modify an <see cref="IWebhook"/> with the specified parameters.
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
        /// The default name of the webhook.
        /// </summary>
        public Optional<string> Name { get; set; }
        /// <summary>
        /// The default avatar of the webhook.
        /// </summary>
        public Optional<Image?> Image { get; set; }
    }
}
