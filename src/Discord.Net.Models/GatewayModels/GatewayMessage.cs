using Discord.Converters;
using System;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class GatewayMessage : IGatewayMessage
{
    [JsonPropertyName("op")]
    public GatewayOpCode OpCode { get; set; }

    [JsonPropertyName("t")]
    public Optional<string?> EventName { get; set; }
    
    [JsonPropertyName("s")]
    public Optional<int?> Sequence { get; set; }
    
    // see ../Converters/GatewayPayloadConverter.cs
    [JsonIgnore]
    public IGatewayPayloadData? Payload { get; set; }

    string? IGatewayMessage.EventName => ~EventName;
    int? IGatewayMessage.Sequence => ~Sequence;
}
