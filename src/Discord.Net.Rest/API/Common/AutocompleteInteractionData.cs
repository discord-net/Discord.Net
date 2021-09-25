using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API
{
    internal class AutocompleteInteractionData : IDiscordInteractionData
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public ApplicationCommandType Type { get; set; }

        [JsonProperty("version")]
        public ulong Version { get; set; }

        [JsonProperty("options")]
        public AutocompleteInteractionDataOption[] Options { get; set; }
    }
}
