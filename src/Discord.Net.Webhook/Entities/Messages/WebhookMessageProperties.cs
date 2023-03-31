using System.Collections.Generic;

namespace Discord.Webhook
{
    /// <summary>
    ///     Properties that are used to modify an Webhook message with the specified changes.
    /// </summary>
    public class WebhookMessageProperties
    {
        /// <summary>
        ///     Gets or sets the content of the message.
        /// </summary>
        /// <remarks>
        ///     This must be less than the constant defined by <see cref="DiscordConfig.MaxMessageSize"/>.
        /// </remarks>
        public Optional<string> Content { get; set; }
        /// <summary>
        ///     Gets or sets the embed array that the message should display.
        /// </summary>
        public Optional<IEnumerable<Embed>> Embeds { get; set; }
        /// <summary>
        ///     Gets or sets the allowed mentions of the message.
        /// </summary>
        public Optional<AllowedMentions> AllowedMentions { get; set; }
        /// <summary>
        ///     Gets or sets the components that the message should display.
        /// </summary>
        public Optional<MessageComponent> Components { get; set; }
        /// <summary>
        ///     Gets or sets the attachments for the message.
        /// </summary>
        public Optional<IEnumerable<FileAttachment>> Attachments { get; set; }
    }
}
