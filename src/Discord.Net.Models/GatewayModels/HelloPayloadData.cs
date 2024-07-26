using System;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class HelloPayloadData : IHelloPayloadData
{
    [JsonPropertyName("heartbeat_interval")]
    public int HeartbeatInterval { get; set; }
}
