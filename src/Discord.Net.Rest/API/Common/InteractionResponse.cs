using Newtonsoft.Json;

namespace Discord.API
{
    internal class InteractionResponse
    {
        [JsonProperty("type")]
        public InteractionResponseType Type { get; set; }

        [JsonProperty("data")]
        public Optional<InteractionCallbackData> Data { get; set; }
    }
}
