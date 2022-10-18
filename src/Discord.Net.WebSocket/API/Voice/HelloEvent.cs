using System.Text.Json.Serialization;

namespace Discord.API.Voice
{
    internal class HelloEvent
    {
        [JsonPropertyName("heartbeat_interval")]
        public int HeartbeatInterval { get; set; }
    }
}
