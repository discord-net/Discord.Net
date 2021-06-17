using System;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record ListPrivateArchivedThreadsParams
    {
        /// <summary>
        /// Returns threads before this timestamp.
        /// </summary>
        public Optional<DateTimeOffset> Before { get; set; }

        /// <summary>
        /// Maximum number of threads to return.
        /// </summary>
        public Optional<int> Limit { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
            Preconditions.NotZero(Limit, nameof(Limit));
        }
    }
}
