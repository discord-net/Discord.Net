using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord button component object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/interactions/message-components#buttons"/>
    /// </remarks>
    public record ButtonComponent : Component
    {
        /// <summary>
        /// Creates a <see cref="Component"/> with a type of <see cref="ComponentType.Button"/>.
        /// </summary>
        public ButtonComponent()
        {
            Type = ComponentType.Button;
        }

        /// <summary>
        /// One of button styles.
        /// </summary>
        [JsonPropertyName("style")]
        public Optional<ButtonStyle> Style { get; init; }

        /// <summary>
        /// Text that appears on the button, max 80 characters.
        /// </summary>
        [JsonPropertyName("label")]
        public Optional<string> Label { get; init; }

        /// <summary>
        /// Name, id, and animated.
        /// </summary>
        [JsonPropertyName("emoji")]
        public Optional<Emoji> Emoji { get; init; }

        /// <summary>
        /// A developer-defined identifier for the button, max 100 characters.
        /// </summary>
        [JsonPropertyName("custom_id")]
        public Optional<string> CustomId { get; init; }

        /// <summary>
        /// A url for link-style buttons.
        /// </summary>
        [JsonPropertyName("url")]
        public Optional<string> Url { get; init; }

        /// <summary>
        /// Whether the button is disabled, default false.
        /// </summary>
        [JsonPropertyName("disabled")]
        public Optional<bool> Disabled { get; init; }
    }
}
