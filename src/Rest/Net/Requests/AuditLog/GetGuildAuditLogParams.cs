using Discord.Net.Models;

namespace Discord.Net.Rest
{
    /// <summary>
    /// Parameters to add to the request.
    /// </summary>
    public record GetGuildAuditLogParams
    {
        /// <summary>
        /// Filter the log for actions made by a user.
        /// </summary>
        public Optional<Snowflake> UserId { get; set; }

        /// <summary>
        /// Filter the log by type of audit log event.
        /// </summary>
        public Optional<AuditLogEvent> ActionType { get; set; }

        /// <summary>
        /// Filter the log before a certain entry id.
        /// </summary>
        public Optional<Snowflake> Before { get; set; }

        /// <summary>
        /// How many entries are returned.
        /// </summary>
        /// <remarks>
        /// Default: 50. Acceptable range: 1-100, inclusive.
        /// </remarks>
        public Optional<int> Limit { get; set; }

        /// <summary>
        /// Validates the data.
        /// </summary>
        public void Validate()
        {
            Preconditions.NotZero(UserId, nameof(UserId));
            Preconditions.AtLeast(Limit, AuditLog.MinimumGetEntryAmount, nameof(Limit));
            Preconditions.AtMost(Limit, AuditLog.MaximumGetEntryAmount, nameof(Limit));
        }
    }
}
