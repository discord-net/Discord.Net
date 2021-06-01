using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents an audit log entry object.
    /// </summary>
    public record AuditLogEntry
    {
        /// <summary>
        ///     Creates a <see cref="AuditLogEntry"/> with the provided parameters.
        /// </summary>
        /// <param name="targetId">Id of the affected entity (webhook, user, role, etc.).</param>
        /// <param name="changes">Changes made to the target_id.</param>
        /// <param name="userId">The user who made the changes.</param>
        /// <param name="id">Id of the entry.</param>
        /// <param name="actionType">Type of action that occurred.</param>
        /// <param name="options">Additional info for certain action types.</param>
        /// <param name="reason">The reason for the change (0-512 characters).</param>
        [JsonConstructor]
        public AuditLogEntry(string? targetId, Optional<AuditLogChange[]> changes, Snowflake? userId, Snowflake id, AuditLogEvent actionType, Optional<AuditEntryInfo> options, Optional<string> reason)
        {
            TargetId = targetId;
            Changes = changes;
            UserId = userId;
            Id = id;
            ActionType = actionType;
            Options = options;
            Reason = reason;
        }

        /// <summary>
        ///     Id of the affected entity (webhook, user, role, etc.).
        /// </summary>
        [JsonPropertyName("target_id")]
        public string? TargetId { get; }

        /// <summary>
        ///     Changes made to the target_id.
        /// </summary>
        [JsonPropertyName("changes")]
        public Optional<AuditLogChange[]> Changes { get; }

        /// <summary>
        ///     The user who made the changes.
        /// </summary>
        [JsonPropertyName("user_id")]
        public Snowflake? UserId { get; }

        /// <summary>
        ///     Id of the entry.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake Id { get; }

        /// <summary>
        ///     Type of action that occurred.
        /// </summary>
        [JsonPropertyName("action_type")]
        public AuditLogEvent ActionType { get; }

        /// <summary>
        ///     Additional info for certain action types.
        /// </summary>
        [JsonPropertyName("options")]
        public Optional<AuditEntryInfo> Options { get; }

        /// <summary>
        ///     The reason for the change (0-512 characters).
        /// </summary>
        [JsonPropertyName("reason")]
        public Optional<string> Reason { get; }
    }
}
