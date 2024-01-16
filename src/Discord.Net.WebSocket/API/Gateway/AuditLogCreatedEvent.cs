using Newtonsoft.Json;

namespace Discord.API.Gateway;

internal class AuditLogCreatedEvent : AuditLogEntry
{
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }
}
