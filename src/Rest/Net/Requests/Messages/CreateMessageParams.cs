using Discord.Net.Models;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to include in a request to create a <see cref="Message"/>.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/channel#create-message-jsonform-params"/>
    /// </remarks>
    public record CreateMessageParams
    {
        /// <summary>
        /// The <see cref="Message"/> content.
        /// </summary>
        /// <remarks>
        /// Up to 2000 characters.
        /// </remarks>
        public Optional<string> Content { get; set; }

        /// <summary>
        /// Set the <see cref="Message"/> as a TTS one.
        /// </summary>
        public Optional<bool> Tts { get; set; }

        /// <summary>
        /// The contents of the file being sent.
        /// </summary>
        public Optional<MultipartFile> File { get; set; }

        /// <summary>
        /// Array of <see cref="Embed"/>s to include with the <see cref="Message"/>.
        /// </summary>
        /// <remarks>
        /// Up to 10 <see cref="Embed"/>s.
        /// </remarks>
        public Optional<Embed[]> Embeds { get; set; }

        /// <summary>
        /// JSON encoded body of non-file params.
        /// </summary>
        /// <remarks>
        /// Multipart/form-data only.
        /// </remarks>
        public Optional<CreateMessageParams> PayloadJson { get; set; } // TODO: Should change this to an easy way to convert to form data.

        /// <summary>
        /// Allowed mentions for the <see cref="Message"/>.
        /// </summary>
        public Optional<AllowedMentions> AlloweMentions { get; set; }

        /// <summary>
        /// Include to make your <see cref="Message"/> a reply.
        /// </summary>
        public Optional<MessageReference> MessageReference { get; set; }

        /// <summary>
        /// The <see cref="Component"/>s to include with the <see cref="Message"/>.
        /// </summary>
        public Optional<Component[]> Components { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
            Preconditions.LengthAtMost(Content!, Message.MaxContentLength, nameof(Content));
            Preconditions.LengthAtMost(Embeds!, Message.MaxEmbeds, nameof(Embeds));
        }
    }
}
