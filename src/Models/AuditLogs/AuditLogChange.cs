using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord audit log change object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/audit-log#audit-log-change-object-audit-log-change-structure"/>
    /// </remarks>
    public record AuditLogChange
    {
        /// <summary>
        /// New value of the key.
        /// </summary>
        [JsonPropertyName("new_value")]
        public Optional<object> NewValue { get; }

        /// <summary>
        /// Old value of the key.
        /// </summary>
        [JsonPropertyName("old_value")]
        public Optional<object> OldValue { get; }

        /// <summary>
        /// Audit log change key that will define the type of value.
        /// </summary>
        [JsonPropertyName("key")]
        public AuditLogChangeKey Key { get; }
    }
}
