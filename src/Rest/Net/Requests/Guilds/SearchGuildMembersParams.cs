using Discord.Net.Models;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record SearchGuildMembersParams
    {
        /// <summary>
        /// Query string to match username(s) and nickname(s) against.
        /// </summary>
        public string? Query { get; set; } // Required property candidate

        /// <summary>
        /// Max number of members to return (1-1000).
        /// </summary>
        public Optional<int> Limit { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
            Preconditions.NotNull(Query, nameof(Query));
            Preconditions.AtLeast(Limit, Guild.MinUserLimitToList, nameof(Limit));
            Preconditions.AtMost(Limit, Guild.MaxUserLimitToList, nameof(Limit));
        }
    }
}
