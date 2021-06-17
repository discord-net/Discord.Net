using Discord.Net.Models;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record GetChannelMessagesParams
    {
        /// <summary>
        /// Get <see cref="Message"/>s around this <see cref="Message"/> ID.
        /// </summary>
        public Optional<Snowflake> Around { get; set; }

        /// <summary>
        /// Get <see cref="Message"/>s before this <see cref="Message"/> ID.
        /// </summary>
        public Optional<Snowflake> Before { get; set; }

        /// <summary>
        /// Get <see cref="Message"/>s after this <see cref="Message"/> ID.
        /// </summary>
        public Optional<Snowflake> After { get; set; }

        /// <summary>
        /// Maximum number of <see cref="Message"/>s to return.
        /// </summary>
        /// <remarks>
        /// Default: 50. Acceptable range: 1-100, inclusive.
        /// </remarks>
        public Optional<int> Limit { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
            Preconditions.NotZero(Before, nameof(Before));
            Preconditions.Exclusive(new[] { Around, Before, After }, new[] { nameof(Around), nameof(Before), nameof(After) });
            Preconditions.AtLeast(Limit, Channel.MinGetMessagesAmount, nameof(Limit));
            Preconditions.AtMost(Limit, Channel.MaxGetMessagesAmount, nameof(Limit));
        }
    }
}
