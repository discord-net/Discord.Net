using Discord.Net.Models;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record CreateGuildBanParams
    {
        /// <summary>
        /// Number of days to delete messages for (0-7).
        /// </summary>
        public Optional<int> DeleteMessageDays { get; set; }

        /// <summary>
        /// Reason for the ban.
        /// </summary>
        public Optional<string> Reason { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
            Preconditions.AtLeast(DeleteMessageDays, Ban.MinDaysToDeleteMessages, nameof(DeleteMessageDays));
            Preconditions.AtMost(DeleteMessageDays, Ban.MaxDaysToDeleteMessages, nameof(DeleteMessageDays));
        }
    }
}
