using Discord.Net.Models;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record StartThreadWithMessageParams
    {
        /// <summary>
        /// <see cref="ThreadChannel"/> name.
        /// </summary>
        /// <remarks>
        /// Must be between 1-100, inclusive.
        /// </remarks>
        public string? Name { get; set; } // Required property candidate

        /// <summary>
        /// Duration in minutes to automatically archive the thread after recent activity.
        /// </summary>
        /// <remarks>
        /// Can be set to: 60, 1440, 4320, 10080.
        /// </remarks>
        public int AutoArchiveDuration { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
            Preconditions.NotNullOrWhitespace(Name!, nameof(Name));
            Preconditions.LengthAtLeast(Name!, Channel.MinChannelNameLength, nameof(Name));
            Preconditions.LengthAtMost(Name!, Channel.MaxChannelNameLength, nameof(Name));
            Preconditions.MustBeOneOf(AutoArchiveDuration, new[] { 60, 1440, 4320, 10080 }, nameof(AutoArchiveDuration));
        }
    }
}
