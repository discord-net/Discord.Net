using Newtonsoft.Json;

namespace Discord.API.Client.GatewaySocket
{
    public class ResumedEvent 
    { 
        [JsonProperty("heartbeat_interval")]
        public int HeartbeatInterval { get; set; }
    }
}
