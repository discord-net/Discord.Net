using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class Emoji : IEmojiModel
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }
}
