using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ModifyApplicationEmojiParams
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }
}