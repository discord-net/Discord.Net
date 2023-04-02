using Newtonsoft.Json;

namespace Discord.API.Rest;

internal class ModifyCurrentApplicationBotParams
{
    [JsonProperty("interactions_endpoint_url")]
    public Optional<string> InteractionsEndpointUrl { get; set; }

    [JsonProperty("role_connections_verification_url")]
    public Optional<string> RoleConnectionsEndpointUrl { get; set; }

    [JsonProperty("description")]
    public Optional<string> Description { get; set; }

    [JsonProperty("tags")]
    public Optional<string[]> Tags { get; set; }

    [JsonProperty("icon")]
    public Optional<Image?> Icon { get; set; }
}
