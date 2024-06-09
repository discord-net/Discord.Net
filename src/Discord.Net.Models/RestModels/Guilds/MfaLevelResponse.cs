using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class MfaLevelResponse
{
    [JsonPropertyName("level")]
    public int Level { get; set; }
}
