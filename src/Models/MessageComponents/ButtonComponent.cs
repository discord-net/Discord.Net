using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a button component object.
    /// </summary>
    public record ButtonComponent : Component
    {
        /// <summary>
        ///     Creates a <see cref="ButtonComponent"/> with the provided parameters.
        /// </summary>
        /// <param name="type">Component type.</param>
        /// <param name="style">One of button styles.</param>
        /// <param name="label">Text that appears on the button, max 80 characters.</param>
        /// <param name="emoji">Name, id, and animated.</param>
        /// <param name="customId">A developer-defined identifier for the button, max 100 characters.</param>
        /// <param name="url">A url for link-style buttons.</param>
        /// <param name="disabled">Whether the button is disabled, default false.</param>
        [JsonConstructor]
        public ButtonComponent(ComponentType type, Optional<ButtonStyle> style, Optional<string> label, Optional<Emoji> emoji, Optional<string> customId, Optional<string> url, Optional<bool> disabled)
            : base(type)
        {
            Style = style;
            Label = label;
            Emoji = emoji;
            CustomId = customId;
            Url = url;
            Disabled = disabled;
        }

        /// <summary>
        ///     One of button styles.
        /// </summary>
        [JsonPropertyName("style")]
        public Optional<ButtonStyle> Style { get; }

        /// <summary>
        ///     Text that appears on the button, max 80 characters.
        /// </summary>
        [JsonPropertyName("label")]
        public Optional<string> Label { get; }

        /// <summary>
        ///     Name, id, and animated.
        /// </summary>
        [JsonPropertyName("emoji")]
        public Optional<Emoji> Emoji { get; }

        /// <summary>
        ///     A developer-defined identifier for the button, max 100 characters.
        /// </summary>
        [JsonPropertyName("custom_id")]
        public Optional<string> CustomId { get; }

        /// <summary>
        ///     A url for link-style buttons.
        /// </summary>
        [JsonPropertyName("url")]
        public Optional<string> Url { get; }

        /// <summary>
        ///     Whether the button is disabled, default false.
        /// </summary>
        [JsonPropertyName("disabled")]
        public Optional<bool> Disabled { get; }
    }
}
