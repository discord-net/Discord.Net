using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ModifyGuildStickerParams
{
    [JsonPropertyName("name")]
    public Optional<string> Name { get; set; }

    [JsonPropertyName("description")]
    public Optional<string?> Description { get; set; }

    [JsonPropertyName("tags")]
    public Optional<string> Tags { get; set; }
}
