using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ModifyUserRoleConnectionParams
{
    [JsonPropertyName("platform_name")]
    public Optional<string> PlatformName { get; set; }

    [JsonPropertyName("platform_username")]
    public Optional<string> PlatformUsername { get; set; }

    [JsonPropertyName("metadata")]
    public Optional<Dictionary<string, string>> Metadata { get; set; }
}
