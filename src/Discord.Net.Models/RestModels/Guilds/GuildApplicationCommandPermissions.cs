using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class GuildApplicationCommandPermission : IGuildApplicationCommandPermissionsModel
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("application_id")]
    public ulong ApplicationId { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("permissions")]
    public required ApplicationCommandPermissions[] Permissions { get; set; }

    IEnumerable<IApplicationCommandPermission> IGuildApplicationCommandPermissionsModel.Permissions => Permissions;
}
