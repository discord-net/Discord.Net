using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class Sku : ISkuModel
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
    
    [JsonPropertyName("flags")]
    public int Flags { get; set; }
}