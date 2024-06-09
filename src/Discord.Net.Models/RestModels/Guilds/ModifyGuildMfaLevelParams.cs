using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ModifyGuildMfaLevelParams
{
    [JsonPropertyName("level")]
    public int Level { get; set; }
}
