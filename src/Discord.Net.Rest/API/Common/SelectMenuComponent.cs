using System.Text.Json.Serialization;
using System.Linq;

namespace Discord.API
{
    internal class SelectMenuComponent : IMessageComponent
    {
        [JsonPropertyName("type")]
        public ComponentType Type { get; set; }

        [JsonPropertyName("custom_id")]
        public string CustomId { get; set; }

        [JsonPropertyName("options")]
        public SelectMenuOption[] Options { get; set; }

        [JsonPropertyName("placeholder")]
        public Optional<string> Placeholder { get; set; }

        [JsonPropertyName("min_values")]
        public int MinValues { get; set; }

        [JsonPropertyName("max_values")]
        public int MaxValues { get; set; }

        [JsonPropertyName("disabled")]
        public bool Disabled { get; set; }

        [JsonPropertyName("values")]
        public Optional<string[]> Values { get; set; }
        public SelectMenuComponent() { }

        public SelectMenuComponent(Discord.SelectMenuComponent component)
        {
            Type = component.Type;
            CustomId = component.CustomId;
            Options = component.Options.Select(x => new SelectMenuOption(x)).ToArray();
            Placeholder = component.Placeholder;
            MinValues = component.MinValues;
            MaxValues = component.MaxValues;
            Disabled = component.IsDisabled;
        }
    }
}
