using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class InstallParams
{
    [JsonPropertyName("scopes")]
    public required string[] Scopes { get; set; }

    [JsonPropertyName("permissions")]
    public ulong Permission { get; set; }
}
