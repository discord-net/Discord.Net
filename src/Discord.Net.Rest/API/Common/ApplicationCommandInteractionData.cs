using Newtonsoft.Json;

namespace Discord.API
{
    internal class ApplicationCommandInteractionData : IResolvable, IDiscordInteractionData
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("options")]
        public Optional<ApplicationCommandInteractionDataOption[]> Options { get; set; }

        [JsonProperty("resolved")]
        public Optional<ApplicationCommandInteractionDataResolved> Resolved { get; set; }

        [JsonProperty("type")]
        public ApplicationCommandType Type { get; set; }
    }
}
