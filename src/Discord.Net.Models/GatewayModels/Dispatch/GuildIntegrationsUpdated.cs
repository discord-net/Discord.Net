using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class GuildIntegrationsUpdated : IGuildIntegrationsUpdatedPayloadData
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }
}
