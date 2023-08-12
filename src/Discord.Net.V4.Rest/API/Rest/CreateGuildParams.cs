using System.Text.Json.Serialization;

namespace Discord.API.Rest;

internal class CreateGuildParams
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("region")]
    public string RegionId { get; set; }

    [JsonPropertyName("icon")]
    public Optional<Image?> Icon { get; set; }
}
