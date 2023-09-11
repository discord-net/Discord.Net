using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class ApplicationCommandPermissions
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("type")]
    public ApplicationCommandPermissionTarget Type { get; set; }

    [JsonPropertyName("permission")]
    public bool Permission { get; set; }
}
