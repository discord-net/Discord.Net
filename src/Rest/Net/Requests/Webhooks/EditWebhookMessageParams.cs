using Discord.Net.Models;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record EditWebhookMessageParams
    {
        /// <summary>
        /// The message contents.
        /// </summary>
        public Optional<string?> Content { get; set; }

        /// <summary>
        /// Embedded rich content.
        /// </summary>
        public Optional<Embed[]?> Embeds { get; set; }

        /// <summary>
        /// The contents of the file being sent/edited.
        /// </summary>
        public Optional<MultipartFile?> File { get; set; }

        /// <summary>
        /// JSON encoded body of non-file params (multipart/form-data only).
        /// </summary>
        public Optional<EditWebhookMessageParams?> PayloadJson { get; set; } // TODO: Should change this to an easy way to convert to form data.

        /// <summary>
        /// Allowed mentions for the message.
        /// </summary>
        public Optional<AllowedMentions?> AllowedMentions { get; set; }

        /// <summary>
        /// Attached files to keep.
        /// </summary>
        public Optional<Attachment[]?> Attachments { get; set; }

        /// <summary>
        /// The components to include with the message.
        /// </summary>
        public Optional<Component[]?> Components { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
            Preconditions.LengthAtMost(Content, Message.MaxContentLength, nameof(Content));
            Preconditions.LengthAtMost(Embeds, Message.MaxEmbeds, nameof(Embeds));
        }
    }
}
