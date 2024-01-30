using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class MfaLevelResponse
{
    [JsonPropertyName("level")]
    public MfaLevel Level { get; set; }
}
