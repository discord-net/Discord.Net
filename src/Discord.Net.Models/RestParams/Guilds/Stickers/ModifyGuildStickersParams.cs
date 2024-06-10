using System.Text.Json.Serialization;

namespace Discord.Models.Json.Stickers;

public sealed class ModifyGuildStickersParams
{
    [JsonPropertyName("name")]
    public Optional<string> Name { get; set; }

    [JsonPropertyName("description")]
    public Optional<string?> Description { get; set; }

    [JsonPropertyName("tags")]
    public Optional<string> Tags { get; set; }
}
