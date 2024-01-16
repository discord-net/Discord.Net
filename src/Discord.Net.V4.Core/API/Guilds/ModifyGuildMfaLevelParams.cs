using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class ModifyGuildMfaLevelParams
{
    [JsonPropertyName("level")]
    public MfaLevel Level { get; set; }
}
