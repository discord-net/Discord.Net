using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ApplicationRoleConnection : IApplicationRoleConnectionModel
{
    [JsonPropertyName("platform_name")]
    public Optional<string> PlatformName { get; set; }

    [JsonPropertyName("platform_username")]
    public Optional<string> PlatformUsername { get; set; }

    [JsonPropertyName("metadata")]
    public required Dictionary<string, string> Metadata { get; set; }

    string? IApplicationRoleConnectionModel.PlatformName => ~PlatformName;
    string? IApplicationRoleConnectionModel.PlatformUsername => ~PlatformUsername;
    IDictionary<string, string> IApplicationRoleConnectionModel.Metadata => Metadata;
}
