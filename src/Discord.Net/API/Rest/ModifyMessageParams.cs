using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class ModifyMessageParams
    {
        [JsonProperty("content")]
        public string Content { get; set; } = "";
    }
}
