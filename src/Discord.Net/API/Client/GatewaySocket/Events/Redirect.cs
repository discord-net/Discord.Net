using Newtonsoft.Json;

namespace Discord.API.Client.GatewaySocket
{
    public sealed class RedirectEvent
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
