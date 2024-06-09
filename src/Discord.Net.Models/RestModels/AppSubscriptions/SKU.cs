
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public class SKU
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("application_id")]
    public ulong ApplicationId { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("slug")]
    public required string Slug { get; set; }
}
