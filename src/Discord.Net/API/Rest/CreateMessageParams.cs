using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class CreateMessageParams
    {
        [JsonProperty("content")]
        public string Content { get; set; } = "";

        [JsonProperty("nonce", NullValueHandling = NullValueHandling.Ignore)]
        public Optional<string> Nonce { get; set; }
        [JsonProperty("tts", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Optional<bool> IsTTS { get; set; }
    }
}
