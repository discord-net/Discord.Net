using Newtonsoft.Json;

namespace Discord.API
{
    internal class InteractionResponse
    {
        [JsonPropertyName("type")]
        public InteractionResponseType Type { get; set; }

        [JsonPropertyName("data")]
        public Optional<InteractionCallbackData> Data { get; set; }
    }
}
