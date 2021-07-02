using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public SelectMenuComponent() { }

        public SelectMenuComponent(Discord.SelectMenu component)
        {
            this.Type = component.Type;
            this.CustomId = component.CustomId;
            this.Options = component.Options.Select(x => new SelectMenuOption(x)).ToArray();
            this.Placeholder = component.Placeholder;
            this.MinValues = component.MinValues;
            this.MaxValues = component.MaxValues;
        }
    }
}
