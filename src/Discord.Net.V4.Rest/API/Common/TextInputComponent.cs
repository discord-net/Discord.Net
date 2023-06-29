using Newtonsoft.Json;

namespace Discord.API
{
    internal class TextInputComponent : IMessageComponent
    {
        [JsonPropertyName("type")]
        public ComponentType Type { get; set; }

        [JsonPropertyName("style")]
        public TextInputStyle Style { get; set; }

        [JsonPropertyName("custom_id")]
        public string CustomId { get; set; }

        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("placeholder")]
        public Optional<string> Placeholder { get; set; }

        [JsonPropertyName("min_length")]
        public Optional<int> MinLength { get; set; }

        [JsonPropertyName("max_length")]
        public Optional<int> MaxLength { get; set; }

        [JsonPropertyName("value")]
        public Optional<string> Value { get; set; }

        [JsonPropertyName("required")]
        public Optional<bool> Required { get; set; }

        public TextInputComponent() { }

        public TextInputComponent(Discord.TextInputComponent component)
        {
            Type = component.Type;
            Style = component.Style;
            CustomId = component.CustomId;
            Label = component.Label;
            Placeholder = component.Placeholder;
            MinLength = component.MinLength ?? Optional<int>.Unspecified;
            MaxLength = component.MaxLength ?? Optional<int>.Unspecified;
            Required = component.Required ?? Optional<bool>.Unspecified;
            Value = component.Value ?? Optional<string>.Unspecified;
        }
    }
}
