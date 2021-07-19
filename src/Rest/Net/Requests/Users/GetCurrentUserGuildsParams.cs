using Discord.Net.Models;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record GetCurrentUserGuildsParams
    {
        /// <summary>
        /// Get guilds before this guild ID.
        /// </summary>
        public Snowflake Before { get; set; }

        /// <summary>
        /// Get guilds after this guild ID.
        /// </summary>
        public Snowflake After { get; set; }

        /// <summary>
        /// Max number of guilds to return (1-200).
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
            Preconditions.NotZero(Before, nameof(Before));
            Preconditions.AtLeast(Limit, Guild.MinGetGuildsAmount, nameof(Limit));
            Preconditions.AtMost(Limit, Guild.MaxGetGuildsAmount, nameof(Limit));
        }
    }
}
