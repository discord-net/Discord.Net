using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class ModifyChannelPermissionsParams
{
    [JsonPropertyName("allow")]
    public Optional<ChannelPermission?> Allow { get; set; }

    [JsonPropertyName("deny")]
    public Optional<ChannelPermission?> Deny { get; set; }

    [JsonPropertyName("type")]
    public Optional<PermissionTarget?> Type { get; set; }
}
