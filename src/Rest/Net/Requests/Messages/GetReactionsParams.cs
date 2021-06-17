using Discord.Net.Models;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record GetReactionsParams
    {
        /// <summary>
        /// Get <see cref="User"/>s after this <see cref="User"/> ID.
        /// </summary>
        public Optional<Snowflake> After { get; set; }

        /// <summary>
        /// Maximum number of <see cref="User"/>s to return.
        /// </summary>
        /// <remarks>
        /// Default: 25. Acceptable range: 1-100, inclusive.
        /// </remarks>
        public Optional<int> Limit { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
            Preconditions.NotZero(After, nameof(After));
            Preconditions.AtLeast(Limit, Reaction.MinGetReactionsAmount, nameof(Limit));
            Preconditions.AtMost(Limit, Reaction.MaxGetReactionsAmount, nameof(Limit));
        }
    }
}
