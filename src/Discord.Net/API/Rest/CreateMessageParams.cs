using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class CreateMessageParams
    {
        [JsonProperty("content")]
        public string Content { get; set; } = "";
        [JsonProperty("nonce", NullValueHandling = NullValueHandling.Ignore)]
        public string Nonce { get; set; } = null;
        [JsonProperty("tts", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool IsTTS { get; set; } = false;
    }
}
