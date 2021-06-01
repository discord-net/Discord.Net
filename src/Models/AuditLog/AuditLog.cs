using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents an audit log object.
    /// </summary>
    public record AuditLog
    {
        /// <summary>
        ///     Creates a <see cref="AuditLog"/> with the provided parameters.
        /// </summary>
        /// <param name="webhooks">List of webhooks found in the audit log.</param>
        /// <param name="users">List of users found in the audit log.</param>
        /// <param name="auditLogEntries">List of audit log entries.</param>
        /// <param name="integrations">List of partial integration objects.</param>
        [JsonConstructor]
        public AuditLog(Webhook[] webhooks, User[] users, AuditLogEntry[] auditLogEntries, Integration[] integrations)
        {
            Webhooks = webhooks;
            Users = users;
            AuditLogEntries = auditLogEntries;
            Integrations = integrations;
        }

        /// <summary>
        ///     List of webhooks found in the audit log.
        /// </summary>
        [JsonPropertyName("webhooks")]
        public Webhook[] Webhooks { get; }

        /// <summary>
        ///     List of users found in the audit log.
        /// </summary>
        [JsonPropertyName("users")]
        public User[] Users { get; }

        /// <summary>
        ///     List of audit log entries.
        /// </summary>
        [JsonPropertyName("audit_log_entries")]
        public AuditLogEntry[] AuditLogEntries { get; }

        /// <summary>
        ///     List of partial integration objects.
        /// </summary>
        [JsonPropertyName("integrations")]
        public Integration[] Integrations { get; }
    }
}
