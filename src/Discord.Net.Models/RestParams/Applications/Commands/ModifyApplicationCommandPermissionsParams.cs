using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ModifyApplicationCommandPermissionsParams
{
    [JsonPropertyName("permissions")]
    public required ApplicationCommandPermissions[] Permissions { get; set; }
}