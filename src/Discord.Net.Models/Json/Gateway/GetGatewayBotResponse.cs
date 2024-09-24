using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class GetGatewayBotResponse
{
    [JsonPropertyName("url")]
    public required string Url { get; set; }

    [JsonPropertyName("shards")]
    public required int Shards { get; set; }

    [JsonPropertyName("session_start_limit")]
    public required SessionStartLimit SessionStartLimit { get; set; }
}
