using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class CreateApplicationEmojiParams
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("image")]
    public required string Image { get; set; }
}