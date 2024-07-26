using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class GetGatewayResponse
{
    [JsonPropertyName("url")]
    public required string Url { get; set; }
}
