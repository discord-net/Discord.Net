using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class VoiceRegion : IVoiceRegionModel
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("optimal")]
    public bool IsOptimal { get; set; }

    [JsonPropertyName("deprecated")]
    public bool IsDeprecated { get; set; }

    [JsonPropertyName("custom")]
    public bool IsCustom { get; set; }
}
