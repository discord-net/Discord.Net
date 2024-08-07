using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class GuildRoleCreatedUpdated : IGuildRoleCreateUpdatePayloadData
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("role")]
    public required Role Role { get; set; }

    IRoleModel IGuildRoleCreateUpdatePayloadData.Role => Role;
}
