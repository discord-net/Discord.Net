using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents an audit log change object.
    /// </summary>
    public record AuditLogChange
    {
        /// <summary>
        ///     New value of the key.
        /// </summary>
        [JsonPropertyName("new_value")]
        public Optional<object> NewValue { get; init; }

        /// <summary>
        ///     Old value of the key.
        /// </summary>
        [JsonPropertyName("old_value")]
        public Optional<object> OldValue { get; init; }

        /// <summary>
        ///     Name of the audit log change key.
        /// </summary>
        [JsonPropertyName("key")]
        public string? Key { get; init; }
    }
}
