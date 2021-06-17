namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record ListJoinedPrivateArchivedThreadsParams
    {
        /// <summary>
        /// Returns threads before this id.
        /// </summary>
        public Optional<Snowflake> Before { get; set; }

        /// <summary>
        /// Maximum number of threads to return.
        /// </summary>
        public Optional<int> Limit { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
            Preconditions.NotZero(Before, nameof(Before));
            Preconditions.NotZero(Limit, nameof(Limit));
        }
    }
}
