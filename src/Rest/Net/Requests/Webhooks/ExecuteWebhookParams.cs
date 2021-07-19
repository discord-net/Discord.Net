using Discord.Net.Models;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record ExecuteWebhookParams
    {
        /// <summary>
        /// Waits for server confirmation of message send before response, and returns the created message.
        /// </summary>
        public Optional<bool> Wait { get; set; }

        /// <summary>
        /// Send a message to the specified thread within a webhook's channel. The thread will automatically be unarchived.
        /// </summary>
        public Optional<Snowflake> ThreadId { get; set; }

        /// <summary>
        /// The message contents.
        /// </summary>
        public Optional<string> Content { get; set; }

        /// <summary>
        /// Override the default username of the webhook.
        /// </summary>
        public Optional<string> Username { get; set; }

        /// <summary>
        /// Override the default avatar of the webhook.
        /// </summary>
        public Optional<string> AvatarUrl { get; set; }

        /// <summary>
        /// True if this is a TTS message.
        /// </summary>
        public Optional<bool> Tts { get; set; }

        /// <summary>
        /// The contents of the file being sent.
        /// </summary>
        public Optional<MultipartFile> File { get; set; }

        /// <summary>
        /// Embedded rich content.
        /// </summary>
        public Optional<Embed[]> Embeds { get; set; }

        /// <summary>
        /// JSON encoded body of non-file params.
        /// </summary>
        public Optional<ExecuteWebhookParams> PayloadJson { get; set; }

        /// <summary>
        /// Allowed mentions for the message.
        /// </summary>
        public Optional<AllowedMentions> AllowedMentions { get; set; }

        /// <summary>
        /// The components to include with the message.
        /// </summary>
        public Optional<Component[]> Components { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
            Preconditions.NotZero(ThreadId, nameof(ThreadId));
            Preconditions.LengthAtMost(Content!, Message.MaxContentLength, nameof(Content));
            Preconditions.NotNull(Username!, nameof(Username));
            Preconditions.LengthAtLeast(Username!, Webhook.MinNameLength, nameof(Username));
            Preconditions.LengthAtMost(Username!, Webhook.MaxNameLength, nameof(Username));
            Preconditions.LengthAtMost(Embeds!, Message.MaxEmbeds, nameof(Embeds));
        }
    }
}
