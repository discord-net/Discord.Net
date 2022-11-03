using Newtonsoft.Json;
using System.Linq;

namespace Discord.API
{
    internal class SelectMenuComponent : IMessageComponent
    {
        [JsonProperty("type")]
        public ComponentType Type { get; set; }

        [JsonProperty("custom_id")]
        public string CustomId { get; set; }

        [JsonProperty("options")]
        public SelectMenuOption[] Options { get; set; }

        [JsonProperty("placeholder")]
        public Optional<string> Placeholder { get; set; }

        [JsonProperty("min_values")]
        public int MinValues { get; set; }

        [JsonProperty("max_values")]
        public int MaxValues { get; set; }

        [JsonProperty("disabled")]
        public bool Disabled { get; set; }

        [JsonProperty("channel_types")]
        public Optional<ChannelType[]> ChannelTypes { get; set; }

        [JsonProperty("resolved")]
        public Optional<MessageComponentInteractionDataResolved> Resolved { get; set; }

        [JsonProperty("values")]
        public Optional<string[]> Values { get; set; }
        public SelectMenuComponent() { }

        public SelectMenuComponent(Discord.SelectMenuComponent component)
        {
            Type = component.Type;
            CustomId = component.CustomId;
            Options = component.Options?.Select(x => new SelectMenuOption(x)).ToArray();
            Placeholder = component.Placeholder;
            MinValues = component.MinValues;
            MaxValues = component.MaxValues;
            Disabled = component.IsDisabled;
            ChannelTypes = component.ChannelTypes.ToArray();
        }
    }
}
