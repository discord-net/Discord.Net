using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class ModifyCurrentApplicationParams
{
    [JsonPropertyName("custom_install_url")]
    public Optional<string> CustomInstallUrl { get; set; }

    [JsonPropertyName("description")]
    public Optional<string> Description { get; set; }

    [JsonPropertyName("role_connections_verification_url")]
    public Optional<string> RoleConnectionsVerificationUrl { get; set; }

    [JsonPropertyName("flags")]
    public Optional<ApplicationFlags> Flags { get; set; }

    [JsonPropertyName("icon")]
    public Optional<Image> Icon { get; set; }

    [JsonPropertyName("cover_image")]
    public Optional<Image> CoverImage { get; set; }

    [JsonPropertyName("interactions_endpoint_url")]
    public Optional<string> InteractionsEndpointUrl { get; set; }

    [JsonPropertyName("tags")]
    public Optional<string[]> Tags { get; set; }
}
