using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents an audit log entry object.
    /// </summary>
    public record AuditLogEntry
    {
        /// <summary>
        ///     Id of the affected entity (webhook, user, role, etc.).
        /// </summary>
        [JsonPropertyName("target_id")]
        public Snowflake? TargetId { get; init; }

        /// <summary>
        ///     Changes made to the <see cref="TargetId"/>.
        /// </summary>
        [JsonPropertyName("changes")]
        public Optional<AuditLogChange[]> Changes { get; init; }

        /// <summary>
        ///     The user who made the changes.
        /// </summary>
        [JsonPropertyName("user_id")]
        public Snowflake? UserId { get; init; }

        /// <summary>
        ///     Id of the entry.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake Id { get; init; }

        /// <summary>
        ///     Type of action that occurred.
        /// </summary>
        [JsonPropertyName("action_type")]
        public AuditLogEvent ActionType { get; init; }

        /// <summary>
        ///     Additional info for certain action types.
        /// </summary>
        [JsonPropertyName("options")]
        public Optional<AuditEntryInfo> Options { get; init; }

        /// <summary>
        ///     The reason for the change (0-512 characters).
        /// </summary>
        [JsonPropertyName("reason")]
        public Optional<string> Reason { get; init; }
    }
}
