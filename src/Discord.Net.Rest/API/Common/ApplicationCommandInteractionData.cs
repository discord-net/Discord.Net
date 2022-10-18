using System.Text.Json.Serialization;

namespace Discord.API
{
    internal class ApplicationCommandInteractionData : IResolvable, IDiscordInteractionData
    {
        [JsonPropertyName("id")]
        public ulong Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("options")]
        public Optional<ApplicationCommandInteractionDataOption[]> Options { get; set; }

        [JsonPropertyName("resolved")]
        public Optional<ApplicationCommandInteractionDataResolved> Resolved { get; set; }

        [JsonPropertyName("type")]
        public ApplicationCommandType Type { get; set; }
    }
}
