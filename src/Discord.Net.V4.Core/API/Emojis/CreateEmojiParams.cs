using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class CreateEmojiParams
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("image")]
    public Image Image { get; set; }

    [JsonPropertyName("roles")]
    public ulong[]? Roles { get; set; }
}
