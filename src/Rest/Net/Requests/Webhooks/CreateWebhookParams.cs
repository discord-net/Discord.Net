using Discord.Net.Models;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record CreateWebhookParams
    {
        /// <summary>
        /// Name of the webhook (1-80 characters).
        /// </summary>
        public string? Name { get; set; } // Required property candidate

        /// <summary>
        /// Image for the default webhook avatar.
        /// </summary>
        public Optional<Image?> Avatar { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
            Preconditions.NotNull(Name, nameof(Name));
            Preconditions.LengthAtLeast(Name, Webhook.MinNameLength, nameof(Name));
            Preconditions.LengthAtMost(Name, Webhook.MaxNameLength, nameof(Name));
        }
    }
}
