using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ModifyCurrentApplicationParams
{
    [JsonPropertyName("custom_install_url")]
    public Optional<string> CustomInstallUrl { get; set; }

    [JsonPropertyName("description")]
    public Optional<string> Description { get; set; }

    [JsonPropertyName("role_connections_verification_url")]
    public Optional<string> RoleConnectionsVerificationUrl { get; set; }

    [JsonPropertyName("install_params")]
    public Optional<InstallParams> InstallParams { get; set; }
    
    [JsonPropertyName("integration_types_config")]
    public Optional<ApplicationIntegrationTypesConfig> IntegrationTypesConfig { get; set; }
    
    [JsonPropertyName("flags")]
    public Optional<int> Flags { get; set; }

    [JsonPropertyName("icon")]
    public Optional<string> Icon { get; set; }

    [JsonPropertyName("cover_image")]
    public Optional<string> CoverImage { get; set; }

    [JsonPropertyName("interactions_endpoint_url")]
    public Optional<string> InteractionsEndpointUrl { get; set; }

    [JsonPropertyName("tags")]
    public Optional<string[]> Tags { get; set; }
}
