using Newtonsoft.Json;

namespace Discord.API.GatewaySocket
{
    public class RedirectEvent
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
