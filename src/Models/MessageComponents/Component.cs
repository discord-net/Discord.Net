using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a component object.
    /// </summary>
    public record Component
    {
        /// <summary>
        ///     Creates a <see cref="Component"/> with the provided parameters.
        /// </summary>
        /// <param name="type">Component type.</param>
        [JsonConstructor]
        public Component(ComponentType type)
        {
            Type = type;
        }

        /// <summary>
        ///     Component type.
        /// </summary>
        [JsonPropertyName("type")]
        public ComponentType Type { get; }
    }
}
