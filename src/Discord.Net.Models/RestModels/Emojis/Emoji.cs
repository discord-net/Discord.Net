using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class Emoji : IEmote
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }
}
