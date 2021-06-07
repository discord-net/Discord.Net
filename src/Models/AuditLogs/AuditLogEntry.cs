using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord audit log entry object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/audit-log#audit-log-entry-object-audit-log-entry-structure"/>
    /// </remarks>
    public record AuditLogEntry
    {
        /// <summary>
        /// Id of the affected entity (webhook, user, role, etc.).
        /// </summary>
        [JsonPropertyName("target_id")]
        public string? TargetId { get; init; }

        /// <summary>
        /// Changes made to the <see cref="TargetId"/>.
        /// </summary>
        [JsonPropertyName("changes")]
        public Optional<AuditLogChange[]> Changes { get; init; }

        /// <summary>
        /// Id of the user who made the changes.
        /// </summary>
        [JsonPropertyName("user_id")]
        public Snowflake? UserId { get; init; }

        /// <summary>
        /// Id of the entry.
        /// </summary>
        [JsonPropertyName("id")]
        public Snowflake Id { get; init; }

        /// <summary>
        /// Type of action that occurred.
        /// </summary>
        [JsonPropertyName("action_type")]
        public AuditLogEvent ActionType { get; init; }

        /// <summary>
        /// Additional info for certain action types.
        /// </summary>
        [JsonPropertyName("options")]
        public Optional<OptionalAuditEntryInfo> Options { get; init; }

        /// <summary>
        /// The reason for the change.
        /// </summary>
        [JsonPropertyName("reason")]
        public Optional<string> Reason { get; init; }
    }
}
