using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class IdentityPayloadData : IGatewayPayloadData
{
    [JsonPropertyName("token")]
    public required string Token { get; set; }

    [JsonPropertyName("properties")]
    public required IdentityConnectionProperties Properties { get; set; }

    [JsonPropertyName("intents")]
    public ulong Intents { get; set; }

    [JsonPropertyName("compress")]
    public Optional<bool> Compress { get; set; }

    [JsonPropertyName("large_threshold")]
    public Optional<int> LargeThreshold { get; set; }

    [JsonPropertyName("shard")]
    public Optional<int[]> Shard { get; set; }

    [JsonPropertyName("presence")]
    public Optional<PresenceUpdatePayloadData> Presence { get; set; }

}

public sealed class IdentityConnectionProperties
{
    [JsonPropertyName("os")]
    public required string OS { get; init; }

    [JsonPropertyName("browser")]
    public required string Browser { get; init; }

    [JsonPropertyName("device")]
    public required string Device { get; init; }
}
