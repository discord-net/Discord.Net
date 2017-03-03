#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    internal class HelloEvent
    {
        [JsonProperty("heartbeat_interval")]
        public int HeartbeatInterval { get; set; }
    }
}
