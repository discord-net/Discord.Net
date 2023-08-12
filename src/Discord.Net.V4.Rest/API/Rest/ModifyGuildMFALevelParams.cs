using System.Text.Json.Serialization;

namespace Discord.API.Rest;

internal class ModifyGuildMFALevelParams
{
    [JsonPropertyName("level")]
    public MfaLevel MfaLevel { get; set; }
}
