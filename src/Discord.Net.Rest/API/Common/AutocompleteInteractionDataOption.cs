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

        [JsonProperty("value")]
        public object Value { get; set; }

        [JsonProperty("focused")]
        public bool Focused { get; set; }
    }
}
