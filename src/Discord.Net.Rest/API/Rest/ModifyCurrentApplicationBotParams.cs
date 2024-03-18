using Newtonsoft.Json;
using System.Collections.Generic;

namespace Discord.API.Rest;

internal class ModifyCurrentApplicationBotParams
{
    [JsonProperty("custom_install_url")]
    public Optional<string> CustomInstallUrl { get; set; }

    [JsonProperty("description")]
    public Optional<string> Description { get; set; }

    [JsonProperty("role_connections_verification_url")]
    public Optional<string> RoleConnectionsEndpointUrl { get; set; }

    [JsonProperty("install_params")]
    public Optional<InstallParams> InstallParams { get; set; }

    [JsonProperty("flags")]
    public Optional<ApplicationFlags> Flags { get; set; }

    [JsonProperty("icon")]
    public Optional<Image?> Icon { get; set; }

    [JsonProperty("cover_image")]
    public Optional<Image?> CoverImage { get; set; }

    [JsonProperty("interactions_endpoint_url")]
    public Optional<string> InteractionsEndpointUrl { get; set; }

    [JsonProperty("tags")]
    public Optional<string[]> Tags { get; set; }

    [JsonProperty("integration_types_config")]
    public Optional<Dictionary<ApplicationIntegrationType, InstallParams>> IntegrationTypesConfig { get; set; }
} 
