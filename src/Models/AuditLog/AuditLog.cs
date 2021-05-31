using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a discord audit log object.
    /// </summary>
    public record AuditLog
    {
        /*
        /// <summary>
        ///     Gets an array of <see cref="Webhook"/>s.
        /// </summary>
        [JsonPropertyName("webhooks")]
        public Optional<Webhook[]> Webhooks { get; init; }*/ //TODO Add Webhook

        /*
        /// <summary>
        ///     Gets an array of <see cref="User"/>s.
        /// </summary>
        [JsonPropertyName("users")]
        public Optional<User[]> Users { get; init; }*/ //TODO Add User

        /// <summary>
        ///     Gets an array of <see cref="AuditLogEntry"/>s.
        /// </summary>
        [JsonPropertyName("audit_log_entries")]
        public Optional<AuditLogEntry[]> AuditLogEntries { get; init; }

        /*
        /// <summary>
        ///     Gets an array of <see cref="Integration"/>s.
        /// </summary>
        [JsonPropertyName("integrations")]
        public Optional<Integration[]> Integrations { get; init; }*/ //TODO Add Integration
    }
}
