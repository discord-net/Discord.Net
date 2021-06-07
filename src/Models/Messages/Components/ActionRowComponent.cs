using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord action row component.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/interactions/message-components#actionrow"/>
    /// </remarks>
    public record ActionRowComponent : Component
    {
        /// <summary>
        /// Creates a <see cref="Component"/> with a type of <see cref="ComponentType.ActionRow"/>.
        /// </summary>
        public ActionRowComponent()
        {
            Type = ComponentType.ActionRow;
        }

        /// <summary>
        /// Components inside this action row, like <see cref="ButtonComponent"/>s
        /// or other interactive components.
        /// </summary>
        [JsonPropertyName("components")]
        public Optional<Component[]> Components { get; init; }
    }
}
