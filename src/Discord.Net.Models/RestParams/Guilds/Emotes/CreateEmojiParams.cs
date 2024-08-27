using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class CreateGuildEmojiParams
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("image")]
    public required string Image { get; set; }

    [JsonPropertyName("roles")]
    public Optional<ulong[]> Roles { get; set; }
}
