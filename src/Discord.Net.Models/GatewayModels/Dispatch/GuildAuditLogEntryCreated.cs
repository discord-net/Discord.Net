using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed partial class GuildAuditLogEntryCreated : IAuditLogEntryPayloadData
{
    [JsonIgnore, JsonExtend]
    public required AuditLogEntry AuditLogEntry { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }
}
