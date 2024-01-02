using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class ModifyEmojiParams
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("roles")]
    public ulong[]? Roles { get; set; }
}
