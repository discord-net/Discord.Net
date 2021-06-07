using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a generic component object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/interactions/message-components#component-object"/>
    /// </remarks>
    public record Component
    {
        /// <summary>
        /// Component type.
        /// </summary>
        [JsonPropertyName("type")]
        public ComponentType Type { get; init; }
    }
}
