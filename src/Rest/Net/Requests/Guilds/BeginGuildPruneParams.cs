using Discord.Net.Models;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record BeginGuildPruneParams
    {
        /// <summary>
        /// Number of days to prune (1-30).
        /// </summary>
        public Optional<int> Days { get; set; }

        /// <summary>
        /// Whether 'pruned' is returned, discouraged for large guilds.
        /// </summary>
        public Optional<bool> ComputePruneCount { get; set; }

        /// <summary>
        /// Role(s) to include, by id.
        /// </summary>
        public Optional<Snowflake[]> IncludeRoles { get; set; }

        /// <summary>
        /// Reason for the prune.
        /// </summary>
        public Optional<string> Reason { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
            Preconditions.AtLeast(Days, Guild.MinAmountOfDaysToPruneFor, nameof(Days));
            Preconditions.AtMost(Days, Guild.MaxAmountOfDaysToPruneFor, nameof(Days));
        }
    }
}
