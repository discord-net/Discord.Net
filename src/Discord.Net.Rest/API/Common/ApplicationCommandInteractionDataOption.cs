using Newtonsoft.Json;
using System.Collections.Generic;

namespace Discord.API
{
    internal class ApplicationCommandInteractionDataOption
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public Optional<object> Value { get; set; }

        [JsonProperty("options")]
        public List<ApplicationCommandInteractionDataOption> Options { get; set; } = new();
    }
}
