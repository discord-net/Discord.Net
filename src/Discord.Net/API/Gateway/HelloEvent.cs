using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    public class HelloEvent
    {
        [JsonProperty("heartbeat_interval")]
        public int HeartbeatInterval { get; set; }
    }
}
