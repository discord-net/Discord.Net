using Discord.Net.Models;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record CreateStageInstanceParams
    {
        /// <summary>
        /// The id of the Stage channel.
        /// </summary>
        public Snowflake ChannelId { get; set; }

        /// <summary>
        /// The topic of the Stage instance (1-120 characters).
        /// </summary>
        public string? Topic { get; set; } // Required property candidate

        /// <summary>
        /// The privacy level of the Stage instance (default <see cref="PrivacyLevel.GuildOnly"/>).
        /// </summary>
        public Optional<PrivacyLevel> PrivacyLevel { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
            Preconditions.NotZero(ChannelId, nameof(ChannelId));
            Preconditions.NotNull(Topic, nameof(Topic));
            Preconditions.LengthAtLeast(Topic, StageInstance.MinStageChannelTopicLength, nameof(Topic));
            Preconditions.LengthAtMost(Topic, StageInstance.MaxStageChannelTopicLength, nameof(Topic));
        }
    }
}
