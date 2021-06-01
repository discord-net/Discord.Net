using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents an audit log change object.
    /// </summary>
    public record AuditLogChange
    {
        /// <summary>
        ///     Creates a <see cref="AuditLogChange"/> with the provided parameters.
        /// </summary>
        /// <param name="newValue">New value of the key.</param>
        /// <param name="oldValue">Old value of the key.</param>
        /// <param name="key">Name of audit log change key.</param>
        [JsonConstructor]
        public AuditLogChange(Optional<object> newValue, Optional<object> oldValue, string key)
        {
            NewValue = newValue;
            OldValue = oldValue;
            Key = key;
        }

        /// <summary>
        ///     New value of the key.
        /// </summary>
        [JsonPropertyName("new_value")]
        public Optional<object> NewValue { get; }

        /// <summary>
        ///     Old value of the key.
        /// </summary>
        [JsonPropertyName("old_value")]
        public Optional<object> OldValue { get; }

        /// <summary>
        ///     Name of audit log change key.
        /// </summary>
        [JsonPropertyName("key")]
        public string Key { get; }
    }
}
