using Discord.Net.Models;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record ModifyStageInstanceParams
    {
        /// <summary>
        /// The topic of the Stage instance (1-120 characters).
        /// </summary>
        public Optional<string> Topic { get; set; }

        /// <summary>
        /// The privacy level of the Stage instance.
        /// </summary>
        public Optional<int> PrivacyLevel { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
            Preconditions.NotNull(Topic!, nameof(Topic));
            Preconditions.LengthAtLeast(Topic!, StageInstance.MinStageChannelTopicLength, nameof(Topic));
            Preconditions.LengthAtMost(Topic!, StageInstance.MaxStageChannelTopicLength, nameof(Topic));
        }
    }
}
