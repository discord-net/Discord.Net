using Newtonsoft.Json;
using System.Collections.Generic;

namespace Discord.API
{
    internal class ApplicationCommandInteractionData : IDiscordInteractionData
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("options")]
        public Optional<List<ApplicationCommandInteractionDataOption>> Options { get; set; }

        [JsonProperty("resolved")]
        public Optional<ApplicationCommandInteractionDataResolved> Resolved { get; set; }

    }
}
