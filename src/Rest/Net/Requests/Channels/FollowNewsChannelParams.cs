using Discord.Net.Models;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record FollowNewsChannelParams
    {
        /// <summary>
        /// Id of target <see cref="Channel"/>.
        /// </summary>
        public Snowflake WebhookChannelId { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
            Preconditions.NotZero(WebhookChannelId, nameof(WebhookChannelId));
        }
    }
}
