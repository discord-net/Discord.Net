using Discord.Net.Models;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record ListGuildMembersParams
    {
        /// <summary>
        /// Max number of members to return (1-1000).
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// The highest user id in the previous page.
        /// </summary>
        public Snowflake After { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
            Preconditions.AtLeast(Limit, Guild.MinUserLimitToList, nameof(Limit));
            Preconditions.AtMost(Limit, Guild.MaxUserLimitToList, nameof(Limit));
        }
    }
}
