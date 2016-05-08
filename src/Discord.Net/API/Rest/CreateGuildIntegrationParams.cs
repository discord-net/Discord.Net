using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class CreateGuildIntegrationParams
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
