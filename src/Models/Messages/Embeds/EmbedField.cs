using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord embed field object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/channel#embed-object-embed-field-structure"/>
    /// </remarks>
    public record EmbedField
    {
        /// <summary>
        /// Name of the field.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; init; } // Required property candidate

        /// <summary>
        /// Value of the field.
        /// </summary>
        [JsonPropertyName("value")]
        public string? Value { get; init; } // Required property candidate

        /// <summary>
        /// Whether or not this field should display inline.
        /// </summary>
        [JsonPropertyName("inline")]
        public Optional<bool> Inline { get; init; }
    }
}
