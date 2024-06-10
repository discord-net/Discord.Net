using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ModifyEmojiParams
{
    [JsonPropertyName("name")]
    public Optional<string> Name { get; set; }

    [JsonPropertyName("roles")]
    public Optional<ulong[]?> Roles { get; set; }
}
