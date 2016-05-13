using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class ModifyMessageParams
    {
        [JsonProperty("content")]
        public Optional<string> Content { get; set; } = "";
    }
}
