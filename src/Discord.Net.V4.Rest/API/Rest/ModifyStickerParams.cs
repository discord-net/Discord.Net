using System.Text.Json.Serialization;

namespace Discord.API.Rest;

internal class ModifyStickerParams
{
    [JsonPropertyName("name")]
    public Optional<string> Name { get; set; }

    [JsonPropertyName("description")]
    public Optional<string> Description { get; set; }

    [JsonPropertyName("tags")]
    public Optional<string> Tags { get; set; }
}
