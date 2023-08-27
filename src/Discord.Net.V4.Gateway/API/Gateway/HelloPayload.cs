using System;
using System.Text.Json.Serialization;

namespace Discord.API.Gateway
{
    public sealed class HelloPayload
    {
        [JsonPropertyName("heartbeat_interval")]
        public int HeartbeatInterval { get; set; }
    }
}

