using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord audit log object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/audit-log#audit-log-object-audit-log-structure"/>
    /// </remarks>
    public record AuditLog
    {
        /// <summary>
        /// Minimum amount of <see cref="AuditLogEntry"/> in <see cref="AuditLogEntries"/>.
        /// </summary>
        public const int MinimumGetEntryAmount = 1;

        /// <summary>
        /// Default amount of <see cref="AuditLogEntry"/> in <see cref="AuditLogEntries"/>.
        /// </summary>
        public const int DefaultGetEntryAmount = 50;

        /// <summary>
        /// Maximum amount of <see cref="AuditLogEntry"/> in <see cref="AuditLogEntries"/>.
        /// </summary>
        public const int MaximumGetEntryAmount = 100;

        /// <summary>
        /// List of <see cref="Webhook"/>s found in the <see cref="AuditLog"/>.
        /// </summary>
        [JsonPropertyName("webhooks")]
        public Webhook[]? Webhooks { get; init; } // Required property candidate

        /// <summary>
        /// List of <see cref="User"/>s found in the <see cref="AuditLog"/>.
        /// </summary>
        [JsonPropertyName("users")]
        public User[]? Users { get; init; } // Required property candidate

        /// <summary>
        /// List of <see cref="AuditLogEntry"/>.
        /// </summary>
        [JsonPropertyName("audit_log_entries")]
        public AuditLogEntry[]? AuditLogEntries { get; init; } // Required property candidate

        /// <summary>
        /// List of partial <see cref="Integration"/>s.
        /// </summary>
        [JsonPropertyName("integrations")]
        public Integration[]? Integrations { get; init; } // Required property candidate
    }
}
