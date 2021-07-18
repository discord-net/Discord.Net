using System.Collections.Generic;
using Discord.Net.Models;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record GetGuildPruneCountParams
    {
        /// <summary>
        /// Number of days to count prune for (1-30).
        /// </summary>
        public Optional<int> Days { get; set; }

        /// <summary>
        /// Role(s) to include, by id.
        /// </summary>
        public Optional<IEnumerable<Snowflake>> IncludeRoles { get; set; }

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
