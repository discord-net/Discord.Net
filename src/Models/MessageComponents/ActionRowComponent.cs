using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a component object.
    /// </summary>
    public record ActionRowComponent : Component
    {
        /// <summary>
        ///     Creates an <see cref="ActionRowComponent"/> with the provided parameters.
        /// </summary>
        /// <param name="type">Component type.</param>
        [JsonConstructor]
        public ActionRowComponent(ComponentType type)
            : base(type)
        {
        }

        /// <summary>
        ///     Components inside this action row, like buttons or other interactive components.
        /// </summary>
        [JsonPropertyName("components")]
        public Optional<Component[]> Components { get; }
    }
}
