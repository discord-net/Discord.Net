using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class GuildRoleDeleted : IGuildRoleDeletePayloadData
{
    [JsonPropertyName("guild_id")] public ulong GuildId { get; set; }

    [JsonPropertyName("role_id")] public ulong RoleId { get; set; }
}
