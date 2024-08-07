using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ApplicationCommandPermissions : IApplicationCommandPermission
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("permission")]
    public bool Permission { get; set; }
}
