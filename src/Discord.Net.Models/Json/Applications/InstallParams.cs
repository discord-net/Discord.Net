using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class InstallParams : IApplicationInstallParamsModel
{
    [JsonPropertyName("scopes")]
    public required string[] Scopes { get; set; }

    [JsonPropertyName("permissions")]
    public required string Permissions { get; set; }
}
