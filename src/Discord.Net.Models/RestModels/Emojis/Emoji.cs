using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Discord.Models.Json;

public sealed class Emoji : IEmojiModel
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }
}
