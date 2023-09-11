using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class GuildApplicationCommandPermission
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("application_id")]
    public ulong ApplicationId { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("permissions")]
    public ApplicationCommandPermissions[] Permissions { get; set; }
}
