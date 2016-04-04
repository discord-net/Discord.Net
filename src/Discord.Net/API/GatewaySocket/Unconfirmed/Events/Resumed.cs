using Newtonsoft.Json;

namespace Discord.API.GatewaySocket
{
    public class ResumedEvent 
    { 
        [JsonProperty("heartbeat_interval")]
        public int HeartbeatInterval { get; set; }
    }
}
