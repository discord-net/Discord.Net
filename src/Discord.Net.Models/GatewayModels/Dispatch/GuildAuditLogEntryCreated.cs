using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed partial class GuildAuditLogEntryCreated : IAuditLogEntryPayloadData
{
    [JsonIgnore, JsonExtend] public AuditLogEntry AuditLogEntry { get; set; } = null!;

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }
}
