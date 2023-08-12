using System.Text.Json.Serialization;

namespace Discord.API.Rest;

internal class ModifyGuildApplicationCommandPermissionsParams
{
    [JsonPropertyName("permissions")]
    public ApplicationCommandPermissions[] Permissions { get; set; }
}
