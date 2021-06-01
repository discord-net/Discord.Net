using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a embed field object.
    /// </summary>
    public record EmbedField
    {
        /// <summary>
        ///     Creates a <see cref="EmbedField"/> with the provided parameters.
        /// </summary>
        /// <param name="name">Name of the field.</param>
        /// <param name="value">Value of the field.</param>
        /// <param name="inline">Whether or not this field should display inline.</param>
        [JsonConstructor]
        public EmbedField(string name, string value, Optional<bool> inline)
        {
            Name = name;
            Value = value;
            Inline = inline;
        }

        /// <summary>
        ///     Name of the field.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; }

        /// <summary>
        ///     Value of the field.
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; }

        /// <summary>
        ///     Whether or not this field should display inline.
        /// </summary>
        [JsonPropertyName("inline")]
        public Optional<bool> Inline { get; }
    }
}
