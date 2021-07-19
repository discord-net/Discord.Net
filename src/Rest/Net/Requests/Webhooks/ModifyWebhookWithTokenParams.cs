using Discord.Net.Models;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record ModifyWebhookWithTokenParams
    {
        /// <summary>
        /// The default name of the webhook.
        /// </summary>
        public Optional<string> Name { get; set; }

        /// <summary>
        /// Image for the default webhook avatar.
        /// </summary>
        public Optional<Image?> Avatar { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
            Preconditions.NotNull(Name!, nameof(Name));
            Preconditions.LengthAtLeast(Name!, Webhook.MinNameLength, nameof(Name));
            Preconditions.LengthAtMost(Name!, Webhook.MaxNameLength, nameof(Name));
        }
    }
}
