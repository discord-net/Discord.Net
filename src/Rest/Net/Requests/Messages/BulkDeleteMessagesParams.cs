using Discord.Net.Models;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record BulkDeleteMessagesParams
    {
        /// <summary>
        /// An array of <see cref="Message"/> ids to delete.
        /// </summary>
        /// <remarks>
        /// Length must be between 2 and 100, inclusive.
        /// </remarks>
        public Snowflake[]? Messages { get; set; } // Required property candidate

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
            Preconditions.LengthAtLeast(Messages!, Message.MinBulkDeleteAmount, nameof(Messages));
            Preconditions.LengthAtMost(Messages!, Message.MaxBulkDeleteAmount, nameof(Messages));
        }
    }
}
