using System;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class GatewayMessage : IGatewayMessage
{
    [JsonPropertyName("op")]
    public GatewayOpCode OpCode { get; set; }

    [JsonPropertyName("t")]
    public string? EventName { get; set; }

    [JsonPropertyName("s")]
    public int? Sequence { get; set; }

    [JsonPropertyName("d")]
    public IGatewayPayloadData? Payload { get; set; }
}
