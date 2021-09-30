using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API
{
    internal class AutocompleteInteractionDataOption
    {
        [JsonProperty("type")]
        public ApplicationCommandOptionType Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("options")]
        public Optional<AutocompleteInteractionDataOption[]> Options { get; set; }

        [JsonProperty("value")]
        public Optional<object> Value { get; set; }

        [JsonProperty("focused")]
        public Optional<bool> Focused { get; set; }
    }
}
