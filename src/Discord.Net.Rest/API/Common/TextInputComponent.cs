using Newtonsoft.Json;

namespace Discord.API
{
    internal class TextInputComponent : IInteractableComponent
    {
        [JsonProperty("type")]
        public ComponentType Type { get; set; }

        [JsonProperty("id")]
        public Optional<int> Id { get; set; }

        [JsonProperty("style")]
        public TextInputStyle Style { get; set; }

        [JsonProperty("custom_id")]
        public string CustomId { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("placeholder")]
        public Optional<string> Placeholder { get; set; }

        [JsonProperty("min_length")]
        public Optional<int> MinLength { get; set; }

        [JsonProperty("max_length")]
        public Optional<int> MaxLength { get; set; }

        [JsonProperty("value")]
        public Optional<string> Value { get; set; }

        [JsonProperty("required")]
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
            Id = component.Id ?? Optional<int>.Unspecified;
        }

        [JsonIgnore]
        int? IMessageComponent.Id => Id.ToNullable();
    }
}
